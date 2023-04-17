using Microsoft.Extensions.Configuration;
using Yottabyte.Services.Contracts;
using Yottabyte.Shared.Models.Events;
using AzureMapsToolkit;
using AzureMapsToolkit.Search;
using AzureMapsToolkit.Timezone;

namespace Yottabyte.Services.Implementations;

internal class GeolocationService : IGeolocationService
{
    private readonly IConfiguration configuration;
    private AzureMapsServices azureMapsServices;

    public GeolocationService(IConfiguration configuration)
    {
        this.configuration = configuration;
        this.azureMapsServices = new AzureMapsServices(this.configuration["AzureMaps:Key"]);
    }
    
    public async Task<GeolocationModel> GetGeolocationAsync(double Latitude, double Longitude)
    {
        var searchReverseRequest = new SearchNearbyRequest
        {
            Lat = Latitude,
            Lon = Longitude,
            Language = "en_EN"
        };

        var resp = await this.azureMapsServices.GetSearchNearby(searchReverseRequest);

        if (resp.Error != null)
        {
            throw new("There was problem with getting reverse geolocation! Please try again! " + resp.Error.Error.Message);
        }

        if (resp.Result.Results.Length == 0)
        {
            throw new("There is not a beach at this location!");
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
               throw new("There is not a beach at this location!");
            }
        }

        var address = resp.Result.Results[index].Address;
        var nameOfBeach = resp.Result.Results[index].Poi.Name;

        var location =  $"{address.Country}, {address.Municipality}, {nameOfBeach}";

        var timeZoneRequest = new TimeZoneRequest
        {
            Query = Latitude.ToString() + "," + Longitude.ToString(),
            Options = TimezoneOptions.All
        };

        var tzResp = await azureMapsServices.GetTimezoneByCoordinates(timeZoneRequest);

        if (tzResp.Error != null)
        {
            throw new("There was problem with getting the time zone! Please try again! " + resp.Error!.Error.Message);
        }

        var timeZoneIANAId = tzResp.Result.TimeZones[0].Id;
        
        return new()
        {
            Location = location,
            NameOfBeach = nameOfBeach,
            TimeZoneIANAId = timeZoneIANAId,
        };
    }
}
