using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GeziRotasi.API.Migrations
{
    /// <inheritdoc />
    public partial class AddExtraPoiFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "GirisUcreti",
                table: "Pois",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "Pois",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OrtalamaZiyaretSuresi",
                table: "Pois",
                type: "integer",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Pois",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "GirisUcreti", "ImageUrl", "OrtalamaZiyaretSuresi" },
                values: new object[] { null, null, null });

            migrationBuilder.UpdateData(
                table: "Pois",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "GirisUcreti", "ImageUrl", "OrtalamaZiyaretSuresi" },
                values: new object[] { null, null, null });

            migrationBuilder.UpdateData(
                table: "Pois",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "GirisUcreti", "ImageUrl", "OrtalamaZiyaretSuresi" },
                values: new object[] { null, null, null });

            migrationBuilder.UpdateData(
                table: "Pois",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "GirisUcreti", "ImageUrl", "OrtalamaZiyaretSuresi" },
                values: new object[] { null, null, null });

            migrationBuilder.UpdateData(
                table: "Pois",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "GirisUcreti", "ImageUrl", "OrtalamaZiyaretSuresi" },
                values: new object[] { null, null, null });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GirisUcreti",
                table: "Pois");

            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "Pois");

            migrationBuilder.DropColumn(
                name: "OrtalamaZiyaretSuresi",
                table: "Pois");
        }
    }
}
