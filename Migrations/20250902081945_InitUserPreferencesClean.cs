using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GeziRotasi.API.Migrations
{
    /// <inheritdoc />
    public partial class InitUserPreferencesClean : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Budget zaten kaldırıldığı için burada hiçbir şey yapmıyoruz.
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Geri almak gerekirse Budget kolonunu ekleyebiliriz.
            migrationBuilder.AddColumn<decimal>(
                name: "Budget",
                table: "UserPreferences",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);
        }
    }
}
