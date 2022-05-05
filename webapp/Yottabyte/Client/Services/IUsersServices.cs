using Yottabyte.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Yottabyte.Client.Services
{
    interface IUsersServices
    {
        event Action OnChange;
        Task<String> CreateUser(UserIM user);
    }
}
