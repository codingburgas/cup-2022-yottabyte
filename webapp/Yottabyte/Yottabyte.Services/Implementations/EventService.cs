using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using SendGrid.Helpers.Mail;
using Yottabyte.Data;
using Yottabyte.Data.Models.Events;
using Yottabyte.Services.Contracts;
using Yottabyte.Shared.Models.Events;

namespace Yottabyte.Services.Implementations;

internal class EventService : IEventService
{
    private readonly ApplicationDbContext context;
    private readonly IMapper mapper;
    private readonly IGeolocationService geolocationService;
    private readonly ITimeZoneService timeZoneService;
    private readonly IDateService dateService;
    private readonly IFileService fileService;

    public EventService(
        ApplicationDbContext context,
        IMapper mapper,
        IGeolocationService geolocationService,
        ITimeZoneService timeZoneService,
        IDateService dateService,
        IFileService fileService)
    {
        this.context = context;
        this.mapper = mapper;
        this.geolocationService = geolocationService;
        this.timeZoneService = timeZoneService;
        this.dateService = dateService;
        this.fileService = fileService;
    }

    public async Task<bool> CheckForDuplicateEventAsync(double? longitude, double? latitude)
    {
        return await this.context.Events!
            .FirstOrDefaultAsync(e => e.Longitude == longitude && e.Latitude == latitude && e.StartTime.Date > DateTime.Now.Date) is not null;
    }

    public async Task CreateEventAsync(EventIM eventModel)
    {
        var geolocation = await this.geolocationService.GetGeolocationAsync((double)eventModel.Latitude!, (double)eventModel.Longitude!);
        var timeZone = this.timeZoneService.ConvertTimeZoneIANAtoWindows(geolocation.TimeZoneIANAId!);
        var startTime = this.dateService.GetStarTimeForEvent(timeZone);
        var imageUrl = await this.fileService.SaveImageAsync(eventModel.Image!, "yottabyteeventimagestest");

        var @event = this.mapper.Map<Event>(eventModel);

        @event.Id = Guid.NewGuid().ToString();
        @event.StartTime = startTime;
        @event.Location = geolocation.Location;
        @event.StartTime = startTime;
        @event.ImageURL = imageUrl;

        await this.context.AddAsync(@event);
        await this.context.SaveChangesAsync();
    }

    public async Task DeleteEventWithIdAsync(string id)
    {
        var @event = await this.context.Events!.FindAsync(id);
        this.context.Events!.Remove(@event!);
    }

    public async Task<List<EventVM>> GetAllEventsAsync()
    {
        return await this.context.Events
            .ProjectTo<EventVM>(this.mapper.ConfigurationProvider)
            .ToListAsync();
    }

    public async Task<EventVM?> GetEventById(string id)
    {
        return this.mapper.Map<EventVM?>(await this.context.Events!.FindAsync(id));
    }
}
