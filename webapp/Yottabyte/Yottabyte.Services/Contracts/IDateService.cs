using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yottabyte.Services.Contracts;

public interface IDateService
{
    DateTime GetStarTimeForEvent(string timeZone);
}
