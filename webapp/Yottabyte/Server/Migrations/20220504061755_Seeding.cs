using Microsoft.EntityFrameworkCore.Migrations;

namespace Yottabyte.Server.Migrations
{
    public partial class Seeding : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SuperHeroes_Comics_ComicId",
                table: "SuperHeroes");

            migrationBuilder.AlterColumn<int>(
                name: "ComicId",
                table: "SuperHeroes",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.InsertData(
                table: "Comics",
                columns: new[] { "Id", "Name" },
                values: new object[] { 1, "Marvel" });

            migrationBuilder.InsertData(
                table: "Comics",
                columns: new[] { "Id", "Name" },
                values: new object[] { 2, "DC" });

            migrationBuilder.InsertData(
                table: "SuperHeroes",
                columns: new[] { "Id", "ComicId", "EditorId", "FirstName", "HeroName", "LastName" },
                values: new object[] { 1, 1, "auth0|627171d97157bd006ec9f6f8", "Peter", "Spiderman", "Parker" });

            migrationBuilder.InsertData(
                table: "SuperHeroes",
                columns: new[] { "Id", "ComicId", "EditorId", "FirstName", "HeroName", "LastName" },
                values: new object[] { 2, 2, "auth0|627171d97157bd006ec9f6f8", "Bruce", "Batman", "Waynce" });

            migrationBuilder.AddForeignKey(
                name: "FK_SuperHeroes_Comics_ComicId",
                table: "SuperHeroes",
                column: "ComicId",
                principalTable: "Comics",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SuperHeroes_Comics_ComicId",
                table: "SuperHeroes");

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

            migrationBuilder.AlterColumn<int>(
                name: "ComicId",
                table: "SuperHeroes",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_SuperHeroes_Comics_ComicId",
                table: "SuperHeroes",
                column: "ComicId",
                principalTable: "Comics",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
