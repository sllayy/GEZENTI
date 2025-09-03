using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace GeziRotasi.API.Migrations
{
    /// <inheritdoc />
    public partial class SeedCategories : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "Depth", "Name", "ParentId", "Path", "Slug" },
                values: new object[,]
                {
                    { 1, 0, "Alışveriş", null, "/images/categories/alisveris.png", "" },
                    { 2, 0, "Eğlence", null, "/images/categories/eglence.png", "" },
                    { 3, 0, "Karayolu", null, "/images/categories/karayolu.png", "" },
                    { 4, 0, "Kültür", null, "/images/categories/kultur.png", "" },
                    { 5, 0, "KültürelTesisler", null, "/images/categories/kulturel.png", "" },
                    { 6, 0, "OnemliNoktalar", null, "/images/categories/onemli.png", "" },
                    { 7, 0, "TarihiTuristikTesisler", null, "/images/categories/tarihi.png", "" },
                    { 8, 0, "Yemek", null, "/images/categories/yemek.png", "" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 8);
        }
    }
}
