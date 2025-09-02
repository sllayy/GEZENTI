using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace GeziRotasi.API.Migrations
{
    /// <inheritdoc />
    public partial class AddRouteAndFeedback : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Routes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    StartLocation = table.Column<string>(type: "text", nullable: true),
                    EndLocation = table.Column<string>(type: "text", nullable: true),
                    Distance = table.Column<double>(type: "double precision", nullable: false),
                    Duration = table.Column<double>(type: "double precision", nullable: false),
                    Geometry = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Routes", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RouteFeedbacks_RouteId",
                table: "RouteFeedbacks",
                column: "RouteId");

            migrationBuilder.AddForeignKey(
                name: "FK_RouteFeedbacks_Routes_RouteId",
                table: "RouteFeedbacks",
                column: "RouteId",
                principalTable: "Routes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RouteFeedbacks_Routes_RouteId",
                table: "RouteFeedbacks");

            migrationBuilder.DropTable(
                name: "Routes");

            migrationBuilder.DropIndex(
                name: "IX_RouteFeedbacks_RouteId",
                table: "RouteFeedbacks");
        }
    }
}
