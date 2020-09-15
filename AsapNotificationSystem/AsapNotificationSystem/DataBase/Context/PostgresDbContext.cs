using AsapNotificationSystem.DataBase.Config;
using AsapNotificationSystem.DataBase.Models;
using Microsoft.EntityFrameworkCore;

namespace AsapNotificationSystem.DataBase.Context
{
    public class PostgresDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }

        public string DbName { get; }

        private string password;
        private string login;
        private string host;
        private int port;

        public PostgresDbContext(DbConfig config)
        {
            DbName = config.DbName;
            host = config.Host;
            port = config.Port;
            login = config.Login;
            password = config.Password;

            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql($"Host={host};Port={port};Database={DbName};Username={login};Password={password}");
        }
    }
}
