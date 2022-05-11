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
        public DbSet<SuperHero> SuperHeroes { get; set; }

        public DbSet<Comic> Comics { get; set; }

        public DbSet<User> User { get; set; }

        public DbSet<Event> Event { get; set; }
    }
}
