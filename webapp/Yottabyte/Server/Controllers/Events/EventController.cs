using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Yottabyte.Data.Models.Auth;
using Yottabyte.Services.Contracts;
using Yottabyte.Shared.Models;
using Yottabyte.Shared.Models.Events;

namespace Yottabyte.Server.Controllers.Events;

/// <summary>
/// Controller for the events.
/// </summary>
[Route("api/[controller]")]
[ApiController]
[Authorize]
public class EventController : ControllerBase
{
    private readonly IEventService eventService;
    private readonly ICustomVisionService customVisionService;

    /// <summary>
    /// Constructor fot the event controller
    /// </summary>
    /// <param name="eventService">Event service</param>
    /// <param name="customVisionService">Custom vision service</param>
    public EventController(
        IEventService eventService,
        ICustomVisionService customVisionService)
    {
        this.eventService = eventService;
        this.customVisionService = customVisionService;
    }

    /// <summary>
    /// Async method that gets all of the events.
    /// </summary>
    /// <returns>All of the events, if any.</returns>
    [HttpGet("all")]
    [AllowAnonymous]
    public async Task<ActionResult<List<EventVM>>> GetAllEventsAsync()
    {
        var events = await this.eventService.GetAllEventsAsync();

        if (events.Count == 0)
        {
            return this.NotFound(events);
        }

        return this.Ok(events);
    }

    /// <summary>
    /// Async method that gets event by it's id
    /// </summary>
    /// <param name="id">Id of the event</param>
    /// <returns>The event, if it exists</returns>
    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<ActionResult<EventVM>> GetEventByIdAsync(string id)
    {
        var @event = await this.eventService.GetEventById(id);

        if (@event is null)
        {
            return this.NotFound();
        }

        return this.Ok(@event);
    }

    /// <summary>
    /// Async method that creates an event.
    /// </summary>
    /// <param name="eventModel">Event info</param>
    /// <returns>Response message</returns>
    [HttpPost("createNewEvent")]
    [Authorize(Roles = UserRoles.Admin)]
    public async Task<ActionResult<Response>> CreateNewEventAsync([FromForm] EventIM eventModel)
    {
        // check if event already exists
        if(await this.eventService.CheckForDuplicateEventAsync(eventModel.Longitude, eventModel.Latitude))
        {
            return this.Conflict(new Response
            {
                Status = "event-create-failure",
                Message = "There is already event in this place and it is active"
            });
        }

        // send image to azure custom vision
        if (!await this.customVisionService.IsBeachPollutedAsync(eventModel.Image))
        {
            return BadRequest(new Response { 
                Status = "event-create-failure",
                Message = "The area is clear!" 
            });
        }

        // create event
        try
        {
            await this.eventService.CreateEventAsync(eventModel);
        }
        catch (SystemException ex)
        {
            return this.BadRequest(new Response
            {
                Status = "event-create-failure",
                Message = ex.Message
            });
        }
        
        return this.Ok(new Response { Status = "event-create-success" });
    }

    /// <summary>
    /// Async method that deletes an event.
    /// </summary>
    /// <param name="id">Id of the event</param>
    /// <returns>Response message</returns>
    [Authorize(Roles = UserRoles.Admin )]
    [HttpDelete("{id}")]
    public async Task<ActionResult<Response>> DeleteEventAsync(string id)
    {
        if ((await this.eventService.GetEventById(id)) is null)
        {
            return this.BadRequest(new Response
            {
                Status = "event-deletion-failure",
                Message = "There isn't an event with this id"
            });
        }
        
        await this.eventService.DeleteEventWithIdAsync(id);

        return this.Ok(new Response 
        {
            Status = "event-deletion-success"
        });
    }
}
