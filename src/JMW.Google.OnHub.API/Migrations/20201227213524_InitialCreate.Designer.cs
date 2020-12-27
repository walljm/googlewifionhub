﻿// <auto-generated />
using System;
using JMW.Google.OnHub.API.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace JMW.Google.OnHub.API.Migrations
{
    [DbContext(typeof(ApplicationContext))]
    [Migration("20201227213524_InitialCreate")]
    partial class InitialCreate
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "5.0.1");

            modelBuilder.Entity("JMW.Google.OnHub.API.Model.Arp", b =>
                {
                    b.Property<string>("IpAddress")
                        .HasColumnType("TEXT");

                    b.Property<string>("Device")
                        .HasColumnType("TEXT");

                    b.Property<string>("Flags")
                        .HasColumnType("TEXT");

                    b.Property<string>("HwAddress")
                        .HasColumnType("TEXT");

                    b.Property<string>("HwType")
                        .HasColumnType("TEXT");

                    b.Property<string>("Mask")
                        .HasColumnType("TEXT");

                    b.Property<DateTimeOffset>("SeenFrom")
                        .HasColumnType("TEXT");

                    b.Property<DateTimeOffset>("SeenTo")
                        .HasColumnType("TEXT");

                    b.HasKey("IpAddress");

                    b.ToTable("Arp");
                });
#pragma warning restore 612, 618
        }
    }
}