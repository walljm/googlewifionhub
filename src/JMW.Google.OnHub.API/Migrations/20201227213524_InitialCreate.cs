using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace JMW.Google.OnHub.API.Migrations
{
    public partial class InitialCreate : Migration
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
                    Device = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Arp", x => x.IpAddress);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Arp");
        }
    }
}
