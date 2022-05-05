using Yottabyte.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;

namespace Yottabyte.Client.Services
{
    public class UserServices : IUsersServices
    {
        private readonly HttpClient _httpClient;

        public UserServices(IHttpClientFactory clientFac)
        {
            _httpClient = clientFac.CreateClient("ServerAPI");
        }

        public List<UserIM> UsersIM { get; set; } = new List<UserIM>();

        public event Action OnChange;

        public async Task<List<UserIM>> CreateUser(UserIM user)
        {
            var result = await _httpClient.PostAsJsonAsync($"api/users/register", user);
            Response res;

            if(result.IsSuccessStatusCode)
            {
                res = await result.Content.ReadFromJsonAsync<Response>();
                UsersIM = await result.Content.ReadFromJsonAsync<List<UserIM>>();
            } 

            //res.Type = "fail";

            OnChange.Invoke();
            return UsersIM;

            //throw new NotImplementedException();
        }
    }
}
