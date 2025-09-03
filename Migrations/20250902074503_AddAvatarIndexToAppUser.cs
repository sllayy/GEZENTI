using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GeziRotasi.API.Migrations
{
    /// <inheritdoc />
    public partial class AddAvatarIndexToAppUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AvatarIndex",
                table: "AspNetUsers",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AvatarIndex",
                table: "AspNetUsers");
        }
    }
}
