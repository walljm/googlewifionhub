﻿// <auto-generated />
using System;
using JMW.Google.OnHub.API.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace JMW.Google.OnHub.API.Migrations
{
    [DbContext(typeof(ApplicationContext))]
    partial class ApplicationContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "5.0.3");

            modelBuilder.Entity("JMW.Google.OnHub.API.Model.Arp", b =>
                {
                    b.Property<string>("IpAddress")
                        .HasColumnType("TEXT");

                    b.Property<DateTimeOffset>("FirstSeen")
                        .HasColumnType("TEXT");

                    b.Property<string>("Flags")
                        .HasColumnType("TEXT");

                    b.Property<string>("HwAddress")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("HwType")
                        .HasColumnType("TEXT");

                    b.Property<string>("Interface")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<DateTimeOffset>("LastSeen")
                        .HasColumnType("TEXT");

                    b.Property<string>("Mask")
                        .HasColumnType("TEXT");

                    b.HasKey("IpAddress");

                    b.ToTable("Arp");
                });

            modelBuilder.Entity("JMW.Google.OnHub.API.Model.ArpHistory", b =>
                {
                    b.Property<string>("IpAddress")
                        .HasColumnType("TEXT");

                    b.Property<string>("HwAddress")
                        .HasColumnType("TEXT");

                    b.Property<DateTimeOffset>("SeenFrom")
                        .HasColumnType("TEXT");

                    b.Property<string>("Flags")
                        .HasColumnType("TEXT");

                    b.Property<string>("HwType")
                        .HasColumnType("TEXT");

                    b.Property<string>("Interface")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Mask")
                        .HasColumnType("TEXT");

                    b.Property<DateTimeOffset>("SeenTo")
                        .HasColumnType("TEXT");

                    b.HasKey("IpAddress", "HwAddress", "SeenFrom");

                    b.ToTable("ArpHistory");
                });

            modelBuilder.Entity("JMW.Google.OnHub.API.Model.Interface", b =>
                {
                    b.Property<string>("IfIndex")
                        .HasColumnType("TEXT");

                    b.Property<string>("BRD")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Group")
                        .HasColumnType("TEXT");

                    b.Property<string>("Info")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Link")
                        .HasColumnType("TEXT");

                    b.Property<string>("MAC")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("MTU")
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("NumRxQueues")
                        .HasColumnType("TEXT");

                    b.Property<string>("NumTxQueues")
                        .HasColumnType("TEXT");

                    b.Property<string>("Promiscuity")
                        .HasColumnType("TEXT");

                    b.Property<string>("Qdisc")
                        .HasColumnType("TEXT");

                    b.Property<string>("Qlen")
                        .HasColumnType("TEXT");

                    b.Property<string>("RxBytes")
                        .HasColumnType("TEXT");

                    b.Property<string>("RxDropped")
                        .HasColumnType("TEXT");

                    b.Property<string>("RxErrors")
                        .HasColumnType("TEXT");

                    b.Property<string>("RxMcast")
                        .HasColumnType("TEXT");

                    b.Property<string>("RxOverrun")
                        .HasColumnType("TEXT");

                    b.Property<string>("RxPackets")
                        .HasColumnType("TEXT");

                    b.Property<DateTimeOffset>("SeenFrom")
                        .HasColumnType("TEXT");

                    b.Property<DateTimeOffset>("SeenTo")
                        .HasColumnType("TEXT");

                    b.Property<string>("State")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("StpCost")
                        .HasColumnType("TEXT");

                    b.Property<string>("StpFastLeave")
                        .HasColumnType("TEXT");

                    b.Property<string>("StpFlood")
                        .HasColumnType("TEXT");

                    b.Property<string>("StpForwardDelay")
                        .HasColumnType("TEXT");

                    b.Property<string>("StpGuard")
                        .HasColumnType("TEXT");

                    b.Property<string>("StpHairpin")
                        .HasColumnType("TEXT");

                    b.Property<string>("StpHelloTime")
                        .HasColumnType("TEXT");

                    b.Property<string>("StpLearning")
                        .HasColumnType("TEXT");

                    b.Property<string>("StpMaxAge")
                        .HasColumnType("TEXT");

                    b.Property<string>("StpMcastFastLeave")
                        .HasColumnType("TEXT");

                    b.Property<string>("StpPriority")
                        .HasColumnType("TEXT");

                    b.Property<string>("StpRootBlock")
                        .HasColumnType("TEXT");

                    b.Property<string>("TxBytes")
                        .HasColumnType("TEXT");

                    b.Property<string>("TxDropped")
                        .HasColumnType("TEXT");

                    b.Property<string>("TxErrors")
                        .HasColumnType("TEXT");

                    b.Property<string>("TxMcast")
                        .HasColumnType("TEXT");

                    b.Property<string>("TxOverrun")
                        .HasColumnType("TEXT");

                    b.Property<string>("TxPackets")
                        .HasColumnType("TEXT");

                    b.HasKey("IfIndex");

                    b.ToTable("Interface");
                });

            modelBuilder.Entity("JMW.Google.OnHub.API.Model.IpInfo", b =>
                {
                    b.Property<string>("Inet")
                        .HasColumnType("TEXT");

                    b.Property<string>("IfIndex")
                        .HasColumnType("TEXT");

                    b.Property<string>("InetPreferredLifetime")
                        .HasColumnType("TEXT");

                    b.Property<string>("InetScope")
                        .HasColumnType("TEXT");

                    b.Property<string>("InetType")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("InetValidLifetime")
                        .HasColumnType("TEXT");

                    b.Property<DateTimeOffset>("SeenFrom")
                        .HasColumnType("TEXT");

                    b.Property<DateTimeOffset>("SeenTo")
                        .HasColumnType("TEXT");

                    b.HasKey("Inet", "IfIndex");

                    b.ToTable("InterfaceInets");
                });

            modelBuilder.Entity("JMW.Google.OnHub.API.Model.Mac", b =>
                {
                    b.Property<string>("HwAddress")
                        .HasColumnType("TEXT");

                    b.Property<string>("Age")
                        .HasColumnType("TEXT");

                    b.Property<string>("IfIndex")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("IsLocal")
                        .HasColumnType("TEXT");

                    b.Property<DateTimeOffset>("SeenFrom")
                        .HasColumnType("TEXT");

                    b.Property<DateTimeOffset>("SeenTo")
                        .HasColumnType("TEXT");

                    b.HasKey("HwAddress");

                    b.ToTable("Mac");
                });
#pragma warning restore 612, 618
        }
    }
}
