﻿using JMW.Google.OnHub.API.Model;
using Microsoft.EntityFrameworkCore;

namespace JMW.Google.OnHub.API.Data
{
    public class ApplicationContext : DbContext
    {
        public DbSet<Mac> Mac { get; set; }

        public DbSet<Arp> Arp { get; set; }
        public DbSet<ArpHistory> ArpHistory { get; set; }

        public DbSet<Interface> Interface { get; set; }
        public DbSet<IpInfo> InterfaceInets { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlite("Data Source=app.db");

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Mac>()
                .HasKey(o => o.HwAddress); // we only want one entry for each IP in curent arp cache

            modelBuilder.Entity<Arp>()
                        .HasKey(o => o.IpAddress); // we only want one entry for each IP in curent arp cache

            modelBuilder.Entity<ArpHistory>()
                .HasKey(o =>
                    new // composite key
                    {
                        o.IpAddress,
                        o.HwAddress,
                        o.SeenFrom
                    }
                );

            modelBuilder.Entity<Interface>()
                .HasKey(o => o.IfIndex);

            modelBuilder.Entity<IpInfo>()
                .HasKey(o => new { o.Inet, o.IfIndex });
        }
    }
}