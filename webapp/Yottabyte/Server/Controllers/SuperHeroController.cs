using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Yottabyte.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Yottabyte.Server.Data;
using Microsoft.EntityFrameworkCore;

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
            new SuperHero { 
                Id = 1,
                FirstName = "Peter",
                LastName = "Parker",
                HeroName = "Spiderman",
                Comic = comics[0],
                EditorId = "auth0|627171d97157bd006ec9f6f8"
            },
            new SuperHero 
            { 
                Id = 2,
                FirstName = "Bruce", 
                LastName = "Waynce", 
                HeroName = "Batman",
                Comic = comics[1],
                EditorId = "auth0|627171d97157bd006ec9f6f8"
            }
        };
        private readonly DataContext _context;

        public SuperHeroController(DataContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetSuperHero()
        {
            return base.Ok(await GetDbHeroes());
        }

        private async Task<List<SuperHero>> GetDbHeroes()
        {
            return await _context.SuperHeroes.Include(sh => sh.Comic).ToListAsync();
        }

        [HttpGet("comics")]
        public async Task<IActionResult> GetComic()
        {
            return Ok(await _context.Comics.ToListAsync());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetSingleSuperHero(int id)
        {
            var hero = await _context.SuperHeroes
                .Include(sh => sh.Comic)
                .FirstOrDefaultAsync(h => h.Id == id);

            if (hero == null)
            {
                return NotFound("Super Hero wasn't found. Sad qsha. :(");
            }

            return Ok(hero);
        }

        [HttpPost]
        public async Task<IActionResult> CreateSuperHero(SuperHero hero)
        {
            _context.SuperHeroes.Add(hero);
            await _context.SaveChangesAsync();
            
            return Ok(await GetDbHeroes());
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSuperHero(SuperHero hero, int id)
        {
            var updateHero = await _context.SuperHeroes
                .Include(sh => sh.Comic)
                .FirstOrDefaultAsync(h=> h.Id == id);

            if (updateHero == null)
            {
                return NotFound("Super Hero wasn't found. Sad qsha. :(");
            }

            updateHero.FirstName = hero.FirstName;
            updateHero.LastName = hero.LastName;
            updateHero.HeroName = hero.HeroName;
            updateHero.ComicId = hero.ComicId;

            await _context.SaveChangesAsync();

            return Ok(await GetDbHeroes());
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSuperHero(int id)
        {
            var deleteHero = await _context.SuperHeroes
                .Include(sh => sh.Comic)
                .FirstOrDefaultAsync(h => h.Id == id);

            if (deleteHero == null)
            {
                return NotFound("Super Hero wasn't found. Sad qsha. :(");
            }
            _context.SuperHeroes.Remove(deleteHero);
            await _context.SaveChangesAsync();

            return Ok(await GetDbHeroes());
        }
    }
}
