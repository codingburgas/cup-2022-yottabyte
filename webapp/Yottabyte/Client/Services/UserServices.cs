using Yottabyte.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using System.Text.Json;
using System.Net.Http.Headers;
using System.IO;

namespace Yottabyte.Client.Services
{
    public class UserServices : IUsersServices
    {
        private readonly HttpClient _httpClient;

        public UserServices(IHttpClientFactory clientFac)
        {
            _httpClient = clientFac.CreateClient("ServerAPI");
        }

        public event Action OnChange;

        public async Task<String> CreateUser(UserIM user, Stream avatarStream, string filename)
        {
            var formContent = new MultipartFormDataContent
            {
                { new StringContent(user.FName), "\"FName\"" },
                { new StringContent(user.LName), "\"LName\"" },
                { new StringContent(user.Email), "\"Email\"" },
                { new StringContent(user.Password), "\"Password\"" },
            };

            if (avatarStream != null)
            {
                Console.WriteLine("Qsha4");
                formContent.Add(new StreamContent(avatarStream, (int)avatarStream.Length), "\"Avatar\"", filename);
            }

            var result = await _httpClient.PostAsync($"api/users/register", formContent);

            Response res;

            if (result.StatusCode == System.Net.HttpStatusCode.OK)
            {
                OnChange.Invoke();
                return "Users is successfully registered! You can now loggin in our App!";
            }

            res = await result.Content.ReadFromJsonAsync<Response>();

            OnChange.Invoke();
            return res.Data;
        }
    }
}
