using Yottabyte.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Yottabyte.Client.Services
{
    public interface ISuperHeroService
    {
        event Action OnChange;
        public List<Comic> Comics { get; set; }
        public List<SuperHero> Heroes { get; set; }
        Task<List<SuperHero>> GetSuperHeroes();
        Task GetComics();
        Task<SuperHero> GetSuperHeroes(int id);
        Task<List<SuperHero>> CreateSuperHeroes(SuperHero hero);
        Task<List<SuperHero>> UpdateSuperHeroes(SuperHero hero, int id);
        Task<List<SuperHero>> DeleteSuperHeroes(int id);
        Task<string> GetUsername(string id);
    }
}
