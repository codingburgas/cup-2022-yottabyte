using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Yottabyte.Server.Migrations
{
    public partial class Addeventclass : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Event",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Location = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Lat = table.Column<double>(type: "float", nullable: false),
                    Long = table.Column<double>(type: "float", nullable: false),
                    StartTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ImageURL = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Event", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Event",
                columns: new[] { "Id", "ImageURL", "Lat", "Location", "Long", "StartTime" },
                values: new object[] { 1, "https://media.architecturaldigest.com/photos/5af4aed7da68792ef45e50a4/master/w_3865,h_2576,c_limit/16%20Nacpan.jpg", 42.503458782614878, "Burgas, Seagarden, Salt Mines", 27.483973503112789, new DateTime(1, 1, 2, 0, 0, 0, 0, DateTimeKind.Unspecified) });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Event");
        }
    }
}
