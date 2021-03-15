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

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            var pw = Secrets.GetPw();

#if DEBUG
            var cnn = ConnectionsString["test"];
#else
            var cnn = ConnectionsString["prod"];
#endif
            var connectionString = cnn.Replace("{pw}", pw);
            //options.UseSqlServer($"Data Source=ekmandbs.database.windows.net;Initial Catalog=backgammon-db;User ID=kristian;Password={pw};Connect Timeout=60;Encrypt=True;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False");
            options.UseSqlServer(connectionString);
        }

        public static IConfigurationSection ConnectionsString { get; internal set; }

        //protected override void OnModelCreating(ModelBuilder builder)
        //{
        //    base.OnModelCreating(builder);
        //}

    }
}
