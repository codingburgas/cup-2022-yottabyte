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
    public class SuperHeroService : ISuperHeroService
    {
        private readonly HttpClient _httpClient;
        private readonly HttpClient _Auth0httpClient;

        public SuperHeroService(IHttpClientFactory clientFac)
        {
            _httpClient = clientFac.CreateClient("ServerAPI");
            _Auth0httpClient = clientFac.CreateClient("Auth0API");
        }

        public List<Comic> Comics { get; set; } = new List<Comic>();
        public List<SuperHero> Heroes { get; set; } = new List<SuperHero>();

        public event Action OnChange;

        public async Task<List<SuperHero>> CreateSuperHeroes(SuperHero hero)
        {
            var result = await _httpClient.PostAsJsonAsync($"api/superhero", hero);
            
            if (result.IsSuccessStatusCode)
            {
                Heroes = await result.Content.ReadFromJsonAsync<List<SuperHero>>();
            }

            OnChange.Invoke();
            return Heroes;
        }

        public async Task<List<SuperHero>> DeleteSuperHeroes(int id)
        {
            var result = await _httpClient.DeleteAsync($"api/superhero/{id}");

            if (result.IsSuccessStatusCode)
            {
                Heroes = await result.Content.ReadFromJsonAsync<List<SuperHero>>();
            }

            OnChange.Invoke();
            return Heroes;
        }

        public async Task GetComics()
        {
            Comics = await _httpClient.GetFromJsonAsync<List<Comic>>("api/superhero/comics");
        }

        public async Task<SuperHero> GetSuperHeroes(int id)
        {
            return await _httpClient.GetFromJsonAsync<SuperHero>($"api/superhero/{id}");
        }

        public async Task<List<SuperHero>> GetSuperHeroes()
        {
            Heroes = await _httpClient.GetFromJsonAsync<List<SuperHero>>("api/superhero");
            return Heroes;
        }

        public async Task<string> GetUsername(string id)
        {
            //var result = await _Auth0httpClient.GetFromJsonAsync<JsonContent>($"api/v2/users/{id}");

            return "Asd";// result.ToString();
        }

        public async Task<List<SuperHero>> UpdateSuperHeroes(SuperHero hero, int id)
        {
            var result = await _httpClient.PutAsJsonAsync($"api/superhero/{id}", hero);
            
            if (result.IsSuccessStatusCode)
            {
                Heroes = await result.Content.ReadFromJsonAsync<List<SuperHero>>();
            }
            
            //OnChange.Invoke();
            return Heroes;
        }
    }
}
