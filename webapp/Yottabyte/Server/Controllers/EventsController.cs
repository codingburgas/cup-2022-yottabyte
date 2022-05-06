using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Azure.CognitiveServices.Vision.CustomVision.Prediction;
using Yottabyte.Shared;
using Yottabyte.Server.Data;
using Microsoft.Extensions.Configuration;
using AzureMapsToolkit.Search;
using AzureMapsToolkit.Timezone;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Specialized;
using Azure.Storage.Blobs.Models;
using System.Globalization;

namespace Yottabyte.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class EventsController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly IConfiguration _configuration;

        public readonly Dictionary<string, string> fileExtToConType = new Dictionary<string, string>
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
            Event @event = IMToEvent(eventIm);

            bool doesUserExist = await _context.Event
                .FirstOrDefaultAsync(e => e.Long == @event.Long && e.Lat == @event.Lat && e.StartTime.Date > DateTime.Now.Date) != null;

            if (doesUserExist)
            {
                return Conflict(new Response { Type = "event-create-failure", Data = "There is already event in this place and it is active" });
            }

            string[] permittedExtensions = { ".png", ".jpg" };

            var ext = Path.GetExtension(eventIm.Image.FileName).ToLowerInvariant();

            if (string.IsNullOrEmpty(ext) || !permittedExtensions.Contains(ext))
            {
                return BadRequest(new Response { Type = "user-update-failure", Data = "The file extension of avatar image is invalid" });
            }

            // Send the image to the Azure Custom Vision API Endpoint            
            CustomVisionPredictionClient predictionApi = AuthenticatePrediction(
                _configuration["AzureCustomVision:PredictionEndpoint"],
                _configuration["AzureCustomVision:PredictionKey"]);

            var result = await predictionApi.ClassifyImageAsync(
                Guid.Parse(_configuration["AzureCustomVision:PredictionModeId"]),
                _configuration["AzureCustomVision:PredictionModelPublishedName"],
                eventIm.Image.OpenReadStream());


            double unclearProb = result.Predictions[0].Probability;
            double clearProb = result.Predictions[1].Probability;

            if (unclearProb <= clearProb)
            {
                return BadRequest(new Response { Type = "event-create-failure", Data = "The area is clear!" });
            }

            // Get the geolocation
            var am = new AzureMapsToolkit.AzureMapsServices(_configuration["AzureMaps:Key"]);

            Console.WriteLine(@event.Lat.ToString());
            Console.WriteLine(@event.Long.ToString());

            var searchReverseRequest = new SearchAddressReverseRequest
            {
                Query = @event.Lat.ToString() + "," + @event.Long.ToString(),
                Language = "en_EN"
            };

            var resp = am.GetSearchAddressReverse(searchReverseRequest).Result;
            
            if (resp.Error != null)
            {
                return StatusCode(500, new Response { Type = "event-create-failure", Data = "There was porblem with getting reverse geolocation! Please try again! " + resp.Error.Error.Message });
            }

            var address = resp.Result.Addresses[0].Address;
            string location;

            if (address.StreetName != null)
            {
                location = $"{address.Country}, {address.Municipality}, {address.StreetName}";
            }
            else
            {
                location = $"{address.Country}, {address.Municipality}, Beach";
            }

            @event.Location = location;;

            var timezonRequest = new TimeZoneRequest
            {
                Query = @event.Lat.ToString() + "," + @event.Long.ToString()
            };

            var tzResp = am.GetTimezoneByCoordinates(timezonRequest).Result;

            if (tzResp.Error != null)
            {
                return StatusCode(500, new Response { Type = "event-create-failure", Data = "There was porblem with getting timezone! Please try again! " + resp.Error.Error.Message });
            }

            string timezone = tzResp.Result.TimeZones[0].Names.Standard;

            // TODO: Add the other timezones
            if (timezone == "Eastern European Standard Time")
            {
                timezone = "E. Europe Standard Time";
            }

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

        /*
        // DELETE: api/Events/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEvent(int id)
        {
            var @event = await _context.Event.FindAsync(id);
            if (@event == null)
            {
                return NotFound();
            }

            _context.Event.Remove(@event);
            await _context.SaveChangesAsync();

            return NoContent();
        }
         * */

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
    }
}
