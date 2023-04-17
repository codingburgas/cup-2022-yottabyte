using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeZoneConverter;
using Yottabyte.Services.Contracts;

namespace Yottabyte.Services.Implementations;

internal class TimeZoneService : ITimeZoneService
{
    public string ConvertTimeZoneIANAtoWindows(string timeZoneIANAId)
    {
        return TZConvert.IanaToWindows(timeZoneIANAId);
    }
}
