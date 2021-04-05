using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace Backend.Db
{
    public class BgDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Game> Games { get; set; }

        public DbSet<Maintenance> Maintenance { get; set; }

        public DbSet<ErrorReport> ErrorReports { get; set; }

        public DbSet<Player> Player { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            var pw = Secrets.GetPw();

#if DEBUG
            var cnn = ConnectionsString["test"];
#else
            var cnn = ConnectionsString["prod"];
#endif
            var connectionString = cnn.Replace("{pw}", pw);
            options.UseSqlServer(connectionString);
            //options.LogTo(Console.WriteLine);
        }

        internal static User GetDbUser(string userId)
        {

            if (string.IsNullOrWhiteSpace(userId))
                userId = Guid.Empty.ToString();
            using (var db = new Db.BgDbContext())
            {
                return db.Users.SingleOrDefault(u => u.Id.ToString() == userId);
            }
        }

        public static IConfigurationSection ConnectionsString { get; internal set; }

        //protected override void OnModelCreating(ModelBuilder builder)
        //{
        //    base.OnModelCreating(builder);
        //}

    }
}
