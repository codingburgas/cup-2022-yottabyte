using System;
using Yottabyte.Shared;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Yottabyte.Server.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {

        }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Comic>().HasData(
                new Comic { Id = 1, Name = "Marvel" },
                new Comic { Id = 2, Name = "DC" }
            );

            modelBuilder.Entity<SuperHero>().HasData(
                new SuperHero
                {
                    Id = 1,
                    FirstName = "Peter",
                    LastName = "Parker",
                    HeroName = "Spiderman",
                    ComicId = 1,
                    EditorId = "auth0|627171d97157bd006ec9f6f8"
                },
                new SuperHero
                {
                    Id = 2,
                    FirstName = "Bruce",
                    LastName = "Waynce",
                    HeroName = "Batman",
                    ComicId = 2,
                    EditorId = "auth0|627171d97157bd006ec9f6f8"
                });

            modelBuilder.Entity<Event>().HasData(
                new Event
                {
                    Id = 1,
                    Location = "Burgas, Seagarden, Salt Mines",
                    Lat = "42.50345878261488",
                    Long = "27.48397350311279",
                    StartTime = new DateTime().AddDays(1),
                    ImageURL = "https://media.architecturaldigest.com/photos/5af4aed7da68792ef45e50a4/master/w_3865,h_2576,c_limit/16%20Nacpan.jpg"
                });
        }

        public DbSet<SuperHero> SuperHeroes { get; set; }

        public DbSet<Comic> Comics { get; set; }

        public DbSet<User> User { get; set; }

        public DbSet<Yottabyte.Shared.Event> Event { get; set; }
    }
}
