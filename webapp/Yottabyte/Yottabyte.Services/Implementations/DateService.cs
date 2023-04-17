using Yottabyte.Services.Contracts;

namespace Yottabyte.Services.Implementations;

internal class DateService : IDateService
{
    public DateTime GetStarTimeForEvent(string timeZone)
    {
        DateTime timeNow = DateTime.UtcNow;

        var localDatetime = TimeZoneInfo.ConvertTimeFromUtc(timeNow,
            TimeZoneInfo.FindSystemTimeZoneById(timeZone));
        
        DateTime startTime;

        if (localDatetime.DayOfWeek == DayOfWeek.Sunday)
        {
            // Skip to the next week
            startTime = localDatetime.AddDays(6);
        }
        else
        {
            // Skip to the next week
            localDatetime = localDatetime.AddDays(7);

            startTime = localDatetime.AddDays(6 - (int)localDatetime.DayOfWeek);
        }

        TimeSpan ts = new(9, 0, 0);
        startTime = startTime.Date + ts;
        startTime = TimeZoneInfo.ConvertTimeToUtc(startTime,
            TimeZoneInfo.FindSystemTimeZoneById(timeZone));

        return startTime;
    }
}
