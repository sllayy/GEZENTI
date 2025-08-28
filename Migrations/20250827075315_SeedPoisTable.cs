using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace GeziRotasi.API.Migrations
{
    /// <inheritdoc />
    public partial class SeedPoisTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Pois",
                columns: new[] { "Id", "Category", "Description", "ExternalApiId", "Latitude", "Longitude", "Name", "OpeningHours", "PhoneNumber", "Website" },
                values: new object[,]
                {
                    { 1, "Müze", "Osmanlı İmparatorluğu'nun...", null, 41.011600000000001, 28.9834, "Topkapı Sarayı", null, null, null },
                    { 2, "Alışveriş", "Dünyanın en eski ve en büyük...", null, 41.0107, 28.9681, "Kapalıçarşı", null, null, null },
                    { 3, "Tarihi Yapı", "Mavi Camii olarak da bilinir...", null, 41.005400000000002, 28.976800000000001, "Sultanahmet Camii", null, null, null },
                    { 4, "Müze", "Tarihi yarımadanın kalbinde...", null, 41.008600000000001, 28.98, "Ayasofya Tarih ve Deneyim Müzesi", null, null, null },
                    { 5, "Müze", "Büyüleyici atmosferi ile...", null, 41.008400000000002, 28.977900000000002, "Yerebatan Sarnıcı", null, null, null }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Pois",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Pois",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Pois",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Pois",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Pois",
                keyColumn: "Id",
                keyValue: 5);
        }
    }
}
