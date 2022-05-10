using System;
using System.IO;
using System.Text;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.IdentityModel.Tokens.Jwt;
using Azure.Storage.Blobs;
using AzureMapsToolkit.Search;
using AzureMapsToolkit.Timezone;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs.Specialized;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.Azure.CognitiveServices.Vision.CustomVision.Prediction;
using Yottabyte.Shared;
using TimeZoneConverter;
using Yottabyte.Server.Data;

namespace Yottabyte.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class EventsController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly IConfiguration _configuration;

        public readonly Dictionary<string, string> fileExtToConType = new()
        {
             { ".png", "image/x-png" },
             { ".jpg", "image/jpeg" },
             { ".svg", "image/svg+xml" },
             { ".gif", "image/gif" }
        };

        public EventsController(DataContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        // GET: api/events/auth
        [HttpGet("auth")]
        public async Task<ActionResult<IEnumerable<Event>>> GetAuthEvent()
        {
            return await _context.Event.ToListAsync();
        }

        // GET: api/events/auth/5
        [HttpGet("auth/{id}")]
        public async Task<ActionResult<Event>> GetAuthEvent(int id)
        {
            var @event = await _context.Event.FindAsync(id);

            if (@event == null)
            {
                return NotFound();
            }

            return @event;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<Event>>> GetEvent()
        {
            return await _context.Event.ToListAsync();
        }

        // GET: api/Events/5
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<Event>> GetEvent(int id)
        {
            var @event = await _context.Event.FindAsync(id);

            if (@event == null)
            {
                return NotFound();
            }

            return @event;
        }

        /*
        // PUT: api/Events/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutEvent(int id, Event @event)
        {
            if (id != @event.Id)
            {
                return BadRequest();
            }

            _context.Entry(@event).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EventExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }
         */
        // POST: api/events/createnewevent
        [HttpPost("createNewEvent")]
        public async Task<ActionResult<Response>> PostEvent([FromForm] EventIM eventIm)
        {
            var token = await HttpContext.GetTokenAsync("access_token");
            var claims = ExtractClaims(token);

            bool isReqUserAdmin = claims.ToArray()[1].Value == "True";
            
            if (!isReqUserAdmin)
            {
                return Unauthorized(new Response { Type = "event-create-failure", Data = "The user isn't an admin" });
            }

            Event @event = IMToEvent(eventIm);

            bool doesEventExist = await _context.Event
                .FirstOrDefaultAsync(e => e.Long == @event.Long && e.Lat == @event.Lat && e.StartTime.Date > DateTime.Now.Date) != null;

            if (doesEventExist)
            {
                return Conflict(new Response { Type = "event-create-failure", Data = "There is already event in this place and it is active" });
            }

            string[] permittedExtensions = { ".png", ".jpg" };

            var ext = Path.GetExtension(eventIm.Image.FileName).ToLowerInvariant();

            if (string.IsNullOrEmpty(ext) || !permittedExtensions.Contains(ext))
            {
                return BadRequest(new Response { Type = "user-update-failure", Data = "The file extension of the image is invalid" });
            }

            // Send the image to the Azure Custom Vision API Endpoint            
            CustomVisionPredictionClient predictionApi = AuthenticatePrediction(
                _configuration["AzureCustomVision:PredictionEndpoint"],
                _configuration["AzureCustomVision:PredictionKey"]);

            var result = await predictionApi.ClassifyImageAsync(
                Guid.Parse(_configuration["AzureCustomVision:PredictionModeId"]),
                _configuration["AzureCustomVision:PredictionModelPublishedName"],
                eventIm.Image.OpenReadStream());

            double unclearProb = 0;
            double clearProb = 0;

            foreach (var predicion in result.Predictions)
            {
                if (predicion.TagName == "Clear")
                {
                    clearProb = predicion.Probability;
                }
                else if (predicion.TagName == "Unclear")
                {
                    unclearProb = predicion.Probability;
                }
            }

            if (unclearProb <= clearProb)
            {
                return BadRequest(new Response { Type = "event-create-failure", Data = "The area is clear!" });
            }

            // Get the geolocation
            var am = new AzureMapsToolkit.AzureMapsServices(_configuration["AzureMaps:Key"]);

            var searchReverseRequest = new SearchNearbyRequest
            {
                Lat = Double.Parse(@event.Lat),
                Lon = Double.Parse(@event.Long),
                Language = "en_EN"
            };

            var resp = am.GetSearchNearby(searchReverseRequest).Result;
            
            if (resp.Error != null)
            {
                return StatusCode(500, new Response { Type = "event-create-failure", Data = "There was porblem with getting reverse geolocation! Please try again! " + resp.Error.Error.Message });
            }

            if (resp.Result.Results.Length == 0)
            {
                return BadRequest(new Response { Type = "event-create-failure", Data = "There is not a beach at this location!" });
            }

            int index = 0;

            for (int i = 0; i < resp.Result.Results.Length; i++)
            {
                if (resp.Result.Results[i].Poi.Categories.Contains("beach"))
                {
                    index = i;
                    break;
                }

                if (i == resp.Result.Results.Length + 1)
                {
                    return BadRequest(new Response { Type = "event-create-failure", Data = "There is not a beach at this location!" });
                }

            }

            var address = resp.Result.Results[index].Address;    
            var nameOfBeach = resp.Result.Results[index].Poi.Name;
            
            @event.Location = $"{address.Country}, {address.Municipality}, {nameOfBeach}";

            var timezonRequest = new TimeZoneRequest
            {
                Query = @event.Lat.ToString() + "," + @event.Long.ToString(),
                Options = TimezoneOptions.All
            };

            var tzResp = am.GetTimezoneByCoordinates(timezonRequest).Result;

            if (tzResp.Error != null)
            {
                return StatusCode(500, new Response { Type = "event-create-failure", Data = "There was porblem with getting timezone! Please try again! " + resp.Error.Error.Message });
            }

            string timezoneIANAId = tzResp.Result.TimeZones[0].Id;

            string timezone = TZConvert.IanaToWindows(timezoneIANAId); 

            DateTime timeNow = DateTime.UtcNow;

            var localDatetime = TimeZoneInfo.ConvertTimeFromUtc(timeNow,
                TimeZoneInfo.FindSystemTimeZoneById(timezone));

            if (localDatetime.DayOfWeek == DayOfWeek.Sunday)
            {
                // Skip to the next week
                localDatetime = localDatetime.AddDays(6);
            }
            else
            {
                // Skip to the next week
                localDatetime = localDatetime.AddDays(7);

                @event.StartTime = localDatetime.AddDays(6 - (int)localDatetime.DayOfWeek);
            }

            TimeSpan ts = new(9, 0, 0);

            @event.StartTime = @event.StartTime.Date + ts;

            @event.StartTime = TimeZoneInfo.ConvertTimeToUtc(@event.StartTime,
                TimeZoneInfo.FindSystemTimeZoneById(timezone));

            // Save the image to Azure Blob
            var connectionString = _configuration["AzureStorage:ConnectionString"];

            BlobServiceClient blobServiceClient = new(connectionString);

            string containerName = "yottabyteeventimagestest";

            BlobContainerClient containerClient;

            containerClient = blobServiceClient.GetBlobContainerClient(containerName);
            containerClient.CreateIfNotExists();

            BlockBlobClient blockBlobClient;

            blockBlobClient = containerClient.GetBlockBlobClient(
                Path.GetRandomFileName() + Guid.NewGuid().ToString() + Path.GetExtension(eventIm.Image.FileName).ToLowerInvariant()
            );

            var blobHttpHeader = new BlobHttpHeaders { ContentType = fileExtToConType[Path.GetExtension(eventIm.Image.FileName).ToLowerInvariant()] };

            await blockBlobClient.UploadAsync(
                eventIm.Image.OpenReadStream(),
                new BlobUploadOptions { HttpHeaders = blobHttpHeader }
            );

            @event.ImageURL = blockBlobClient.Uri.AbsoluteUri;

            _context.Event.Add(@event);
            await _context.SaveChangesAsync();

            return Ok(new Response { Type = "event-create-success" });
        }

        // DELETE: api/events/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Response>> DeleteEvent(int id)
        {
            var token = await HttpContext.GetTokenAsync("access_token");
            var claims = ExtractClaims(token);

            bool isReqUserAdmin = claims.ToArray()[1].Value == "True";

            if (!isReqUserAdmin)
            {
                return Unauthorized(new Response { Type = "event-deletion-failure", Data = "The user isn't an admin" });
            }

            var @event = await _context.Event.FindAsync(id);
            if (@event == null)
            {
                return NotFound(new Response { Type = "event-deletion-failure", Data = "There isn't an event with this id" });
            }

            var connectionString = (string)_configuration["AzureStorage:ConnectionString"];

            BlobServiceClient blobServiceClient = new BlobServiceClient(connectionString);

            string containerName = "yottabyteeventimagestest";

            BlobContainerClient containerClient;

            containerClient = blobServiceClient.GetBlobContainerClient(containerName);
            containerClient.CreateIfNotExists();

            // This should be dynamic calculated
            BlockBlobClient blockBlobClient = containerClient.GetBlockBlobClient(@event.ImageURL[77..]);
            await blockBlobClient.DeleteAsync();

            _context.Event.Remove(@event);
            await _context.SaveChangesAsync();

            return Ok(new Response { Type = "event-deletion-success" });
        }
        
        private bool EventExists(int id)
        {
            return _context.Event.Any(e => e.Id == id);
        }

        private static Event IMToEvent(EventIM eventIM) =>
          new()
          {
              Long = eventIM.Long,
              Lat = eventIM.Lat
          };

        private static CustomVisionPredictionClient AuthenticatePrediction(string endpoint, string predictionKey)
        {
            // Create a prediction endpoint, passing in the obtained prediction key
            CustomVisionPredictionClient predictionApi = new(new ApiKeyServiceClientCredentials(predictionKey))
            {
                Endpoint = endpoint
            };
            return predictionApi;
        }

        public IEnumerable<Claim> ExtractClaims(string jwtToken)
        {
            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            JwtSecurityToken securityToken = (JwtSecurityToken)tokenHandler.ReadToken(jwtToken);
            IEnumerable<Claim> claims = securityToken.Claims;
            return claims;
        }
    }
}
