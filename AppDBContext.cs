using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace SteamLamp
{
    public class AppDbContext : DbContext
    {
        public AppDbContext() : base("name=DefaultConnection")
        {
            Database.SetInitializer(new DropCreateDatabaseIfModelChanges<AppDbContext>());
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Friend> Friends { get; set; }
    }
}