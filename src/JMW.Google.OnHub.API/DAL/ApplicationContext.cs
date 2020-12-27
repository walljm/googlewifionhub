using JMW.Google.OnHub.API.Model;
using Microsoft.EntityFrameworkCore;

namespace JMW.Google.OnHub.API.Data
{
    public class ApplicationContext : DbContext
    {
        public DbSet<Arp> Arp { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlite("Data Source=app.db");

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Arp>().HasKey(o => o.IpAddress);
        }
    }
}