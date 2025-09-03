using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace GeziRotasi.API.Migrations
{
    /// <inheritdoc />
    public partial class AddTravelRoutesDbSet : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Budget",
                table: "UserPreferences");

            migrationBuilder.CreateTable(
                name: "TravelRoutes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Category = table.Column<string>(type: "text", nullable: true),
                    AverageRating = table.Column<double>(type: "double precision", nullable: false),
                    DistanceKm = table.Column<double>(type: "double precision", nullable: false),
                    Duration = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TravelRoutes", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TravelRoutes");

            migrationBuilder.AddColumn<decimal>(
                name: "Budget",
                table: "UserPreferences",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);
        }
    }
}
