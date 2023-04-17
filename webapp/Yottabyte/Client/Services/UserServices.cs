﻿using Yottabyte.Shared.Models.Auth;
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
using Yottabyte.Shared.Models;

namespace Yottabyte.Client.Services
{
    /// <summary>
    /// User services handler
    /// </summary>
    public class UserServices : IUsersServices
    {
        /// <summary>
        /// HTTP client, which communicates with the HTTP Server
        /// </summary>
        private readonly HttpClient _httpClient;

        /// <summary>
        /// Constructor for the user service
        /// </summary>
        /// <param name="clientFac">Client factory to get the HTTP client</param>
        public UserServices(IHttpClientFactory clientFac)
        {
            _httpClient = clientFac.CreateClient("ServerAPI");
        }

        /// <summary>
        /// Action for checking if a data needs to be updated in the DOM
        /// </summary>
        public event Action OnChange;

        public async Task<String> CreateUser(UserIM user, Stream avatarStream, string filename)
        {
            var formContent = new MultipartFormDataContent
            {
                { new StringContent(user.FirstName), "\"FName\"" },
                { new StringContent(user.LastName), "\"LName\"" },
                { new StringContent(user.Email), "\"Email\"" },
                { new StringContent(user.Password), "\"Password\"" },
            };

            if (avatarStream != null)
            {;
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
            return res.Status;
        }
    }
}
