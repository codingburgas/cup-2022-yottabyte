using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yottabyte.Shared.Models.Events;

namespace Yottabyte.Services.Contracts;

public interface IEventService
{
    Task<List<EventVM>> GetAllEventsAsync();

    Task<EventVM?> GetEventById(string id);
 
    Task<bool> CheckForDuplicateEventAsync(double? longitude, double? latitude);
    
    Task CreateEventAsync(EventIM eventModel);
    
    Task DeleteEventWithIdAsync(string id);
}
