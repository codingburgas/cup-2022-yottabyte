using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Yottabyte.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace Yottabyte.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    public class SuperHeroController : ControllerBase
    {
        static List<Comic> comics = new List<Comic> {
            new Comic { Id = 1, Name = "Marvel"},
            new Comic { Id = 2, Name = "DC"}
        };

        static List<SuperHero> heroes = new List<SuperHero> {
            new SuperHero { Id = 1, FirstName = "Peter", LastName = "Parker", HeroName = "Spiderman", Comic = comics[0] },
            new SuperHero { Id = 2, FirstName = "Bruce", LastName = "Waynce", HeroName = "Batman", Comic = comics[1] } 
        };

        [HttpGet]
        public async Task<IActionResult> GetSuperHero()
        {
            return Ok(heroes);
        }

        [HttpGet("comics")]
        public async Task<IActionResult> GetComic()
        {
            return Ok(comics);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetSingleSuperHero(int id)
        {
            var hero = heroes.FirstOrDefault(h => h.Id == id);

            if (hero == null)
            {
                return NotFound("Super Hero wasn't found. Sad qsha. :(");
            }

            return Ok(hero);
        }

        [HttpPost]
        public async Task<IActionResult> CreateSuperHero(SuperHero hero)
        {
            hero.Id = heroes.Max(h => h.Id + 1);
            heroes.Add(hero);

            return Ok(heroes);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSuperHero(SuperHero hero, int id)
        {
            var updateHero = heroes.FirstOrDefault(h => h.Id == id);

            if (updateHero == null)
            {
                return NotFound("Super Hero wasn't found. Sad qsha. :(");
            }

            var index = heroes.IndexOf(updateHero);
            heroes[index] = hero;

            return Ok(heroes);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSuperHero(int id)
        {
            var hero = heroes.FirstOrDefault(h => h.Id == id);

            if (hero == null)
            {
                return NotFound("Super Hero wasn't found. Sad qsha. :(");
            }

            heroes.Remove(hero);

            return Ok(heroes);
        }
    }
}
