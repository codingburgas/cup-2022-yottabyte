using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yottabyte.Services.Contracts;

public interface ICustomVisionService
{
    Task<bool> IsBeachPollutedAsync(IFormFile file);
}
