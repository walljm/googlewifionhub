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
                    SeenFrom = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
                    SeenTo = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
                    HwType = table.Column<string>(type: "TEXT", nullable: true),
                    Flags = table.Column<string>(type: "TEXT", nullable: true),
                    HwAddress = table.Column<string>(type: "TEXT", nullable: true),
                    Mask = table.Column<string>(type: "TEXT", nullable: true),
                    Interface = table.Column<string>(type: "TEXT", nullable: true)
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
                    Flags = table.Column<string>(type: "TEXT", nullable: true),
                    Mask = table.Column<string>(type: "TEXT", nullable: true),
                    Interface = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArpHistory", x => new { x.IpAddress, x.HwAddress, x.SeenFrom });
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Arp");

            migrationBuilder.DropTable(
                name: "ArpHistory");
        }
    }
}
