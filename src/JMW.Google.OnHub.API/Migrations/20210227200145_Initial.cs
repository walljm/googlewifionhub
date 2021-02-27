using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace JMW.Google.OnHub.API.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Arp",
                columns: table => new
                {
                    IpAddress = table.Column<string>(type: "TEXT", nullable: false),
                    FirstSeen = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
                    LastSeen = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
                    HwAddress = table.Column<string>(type: "TEXT", nullable: false),
                    HwType = table.Column<string>(type: "TEXT", nullable: true),
                    Interface = table.Column<string>(type: "TEXT", nullable: false),
                    Flags = table.Column<string>(type: "TEXT", nullable: true),
                    Mask = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Arp", x => x.IpAddress);
                });

            migrationBuilder.CreateTable(
                name: "ArpHistory",
                columns: table => new
                {
                    IpAddress = table.Column<string>(type: "TEXT", nullable: false),
                    HwAddress = table.Column<string>(type: "TEXT", nullable: false),
                    SeenFrom = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
                    SeenTo = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
                    HwType = table.Column<string>(type: "TEXT", nullable: true),
                    Interface = table.Column<string>(type: "TEXT", nullable: false),
                    Flags = table.Column<string>(type: "TEXT", nullable: true),
                    Mask = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArpHistory", x => new { x.IpAddress, x.HwAddress, x.SeenFrom });
                });

            migrationBuilder.CreateTable(
                name: "Interface",
                columns: table => new
                {
                    IfIndex = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Info = table.Column<string>(type: "TEXT", nullable: false),
                    State = table.Column<string>(type: "TEXT", nullable: false),
                    MAC = table.Column<string>(type: "TEXT", nullable: false),
                    BRD = table.Column<string>(type: "TEXT", nullable: false),
                    MTU = table.Column<string>(type: "TEXT", nullable: true),
                    Qdisc = table.Column<string>(type: "TEXT", nullable: true),
                    Group = table.Column<string>(type: "TEXT", nullable: true),
                    Qlen = table.Column<string>(type: "TEXT", nullable: true),
                    Link = table.Column<string>(type: "TEXT", nullable: true),
                    Promiscuity = table.Column<string>(type: "TEXT", nullable: true),
                    NumRxQueues = table.Column<string>(type: "TEXT", nullable: true),
                    RxBytes = table.Column<string>(type: "TEXT", nullable: true),
                    RxPackets = table.Column<string>(type: "TEXT", nullable: true),
                    RxErrors = table.Column<string>(type: "TEXT", nullable: true),
                    RxDropped = table.Column<string>(type: "TEXT", nullable: true),
                    RxOverrun = table.Column<string>(type: "TEXT", nullable: true),
                    RxMcast = table.Column<string>(type: "TEXT", nullable: true),
                    NumTxQueues = table.Column<string>(type: "TEXT", nullable: true),
                    TxBytes = table.Column<string>(type: "TEXT", nullable: true),
                    TxPackets = table.Column<string>(type: "TEXT", nullable: true),
                    TxErrors = table.Column<string>(type: "TEXT", nullable: true),
                    TxDropped = table.Column<string>(type: "TEXT", nullable: true),
                    TxOverrun = table.Column<string>(type: "TEXT", nullable: true),
                    TxMcast = table.Column<string>(type: "TEXT", nullable: true),
                    StpPriority = table.Column<string>(type: "TEXT", nullable: true),
                    StpCost = table.Column<string>(type: "TEXT", nullable: true),
                    StpHairpin = table.Column<string>(type: "TEXT", nullable: true),
                    StpGuard = table.Column<string>(type: "TEXT", nullable: true),
                    StpRootBlock = table.Column<string>(type: "TEXT", nullable: true),
                    StpFastLeave = table.Column<string>(type: "TEXT", nullable: true),
                    StpLearning = table.Column<string>(type: "TEXT", nullable: true),
                    StpFlood = table.Column<string>(type: "TEXT", nullable: true),
                    StpMcastFastLeave = table.Column<string>(type: "TEXT", nullable: true),
                    StpForwardDelay = table.Column<string>(type: "TEXT", nullable: true),
                    StpHelloTime = table.Column<string>(type: "TEXT", nullable: true),
                    StpMaxAge = table.Column<string>(type: "TEXT", nullable: true),
                    SeenFrom = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
                    SeenTo = table.Column<DateTimeOffset>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Interface", x => x.IfIndex);
                });

            migrationBuilder.CreateTable(
                name: "InterfaceInets",
                columns: table => new
                {
                    IfIndex = table.Column<string>(type: "TEXT", nullable: false),
                    Inet = table.Column<string>(type: "TEXT", nullable: false),
                    InetType = table.Column<string>(type: "TEXT", nullable: false),
                    InetScope = table.Column<string>(type: "TEXT", nullable: true),
                    InetValidLifetime = table.Column<string>(type: "TEXT", nullable: true),
                    InetPreferredLifetime = table.Column<string>(type: "TEXT", nullable: true),
                    SeenFrom = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
                    SeenTo = table.Column<DateTimeOffset>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InterfaceInets", x => new { x.Inet, x.IfIndex });
                });

            migrationBuilder.CreateTable(
                name: "Mac",
                columns: table => new
                {
                    HwAddress = table.Column<string>(type: "TEXT", nullable: false),
                    SeenFrom = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
                    SeenTo = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
                    IfIndex = table.Column<string>(type: "TEXT", nullable: false),
                    IsLocal = table.Column<string>(type: "TEXT", nullable: true),
                    Age = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Mac", x => x.HwAddress);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Arp");

            migrationBuilder.DropTable(
                name: "ArpHistory");

            migrationBuilder.DropTable(
                name: "Interface");

            migrationBuilder.DropTable(
                name: "InterfaceInets");

            migrationBuilder.DropTable(
                name: "Mac");
        }
    }
}
