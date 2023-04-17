using AutoMapper;
using Yottabyte.Data.Models.Auth;
using Yottabyte.Data.Models.Events;
using Yottabyte.Shared.Models.Auth;
using Yottabyte.Shared.Models.Events;

namespace Yottabyte.Server.Helpers;

/// <summary>
/// Mapping profile.
/// </summary>
public class MappingProfile : Profile
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MappingProfile"/> class.
    /// </summary>
    public MappingProfile()
    {
        this.CreateMap<UserIM, User>();
        this.CreateMap<User, UserVM>();
        this.CreateMap<EventIM, Event>();
        this.CreateMap<Event, EventVM>();
    }
}