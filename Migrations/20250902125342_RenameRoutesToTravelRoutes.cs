using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GeziRotasi.API.Migrations
{
    /// <inheritdoc />
    public partial class RenameRoutesToTravelRoutes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.RenameTable(
                name: "Routes",
                newName: "TravelRoutes"
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
       name: "TravelRoutes",
       newName: "Routes"
             );
        }
    }
}
