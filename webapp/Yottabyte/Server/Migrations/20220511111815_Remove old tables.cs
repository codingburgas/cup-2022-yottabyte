using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Yottabyte.Server.Migrations
{
    public partial class Removeoldtables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Event",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "SuperHeroes",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "SuperHeroes",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Comics",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Comics",
                keyColumn: "Id",
                keyValue: 2);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Comics",
                columns: new[] { "Id", "Name" },
                values: new object[] { 1, "Marvel" });

            migrationBuilder.InsertData(
                table: "Comics",
                columns: new[] { "Id", "Name" },
                values: new object[] { 2, "DC" });

            migrationBuilder.InsertData(
                table: "Event",
                columns: new[] { "Id", "ImageURL", "Lat", "Location", "Long", "StartTime" },
                values: new object[] { 1, "https://media.architecturaldigest.com/photos/5af4aed7da68792ef45e50a4/master/w_3865,h_2576,c_limit/16%20Nacpan.jpg", "42.50345878261488", "Burgas, Seagarden, Salt Mines", "27.48397350311279", new DateTime(1, 1, 2, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.InsertData(
                table: "SuperHeroes",
                columns: new[] { "Id", "ComicId", "EditorId", "FirstName", "HeroName", "LastName" },
                values: new object[] { 1, 1, "auth0|627171d97157bd006ec9f6f8", "Peter", "Spiderman", "Parker" });

            migrationBuilder.InsertData(
                table: "SuperHeroes",
                columns: new[] { "Id", "ComicId", "EditorId", "FirstName", "HeroName", "LastName" },
                values: new object[] { 2, 2, "auth0|627171d97157bd006ec9f6f8", "Bruce", "Batman", "Waynce" });
        }
    }
}
