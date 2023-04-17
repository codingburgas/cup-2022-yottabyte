using Yottabyte.Shared.Models.Events;

namespace Yottabyte.Services.Contracts;

public interface IGeolocationService
{
    Task<GeolocationModel> GetGeolocationAsync(double Latitude, double Longitude);
}
