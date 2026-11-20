using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TAJAN_TRAVEL.Migrations
{
    /// <inheritdoc />
    public partial class addwishlist : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "WishlistItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SessionId = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    ItemType = table.Column<int>(type: "int", nullable: false),
                    TourId = table.Column<int>(type: "int", nullable: true),
                    DestinationId = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WishlistItems", x => x.Id);
                    table.CheckConstraint("CK_WishlistItem_OneTarget", "([ItemType] = 0 AND [TourId] IS NOT NULL AND [DestinationId] IS NULL) OR ([ItemType] = 1 AND [DestinationId] IS NOT NULL AND [TourId] IS NULL)");
                    table.ForeignKey(
                        name: "FK_WishlistItems_Destinations_DestinationId",
                        column: x => x.DestinationId,
                        principalTable: "Destinations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WishlistItems_Tours_TourId",
                        column: x => x.TourId,
                        principalTable: "Tours",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WishlistItems_DestinationId",
                table: "WishlistItems",
                column: "DestinationId");

            migrationBuilder.CreateIndex(
                name: "IX_WishlistItems_SessionId",
                table: "WishlistItems",
                column: "SessionId");

            migrationBuilder.CreateIndex(
                name: "IX_WishlistItems_SessionId_DestinationId",
                table: "WishlistItems",
                columns: new[] { "SessionId", "DestinationId" },
                unique: true,
                filter: "[DestinationId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_WishlistItems_SessionId_TourId",
                table: "WishlistItems",
                columns: new[] { "SessionId", "TourId" },
                unique: true,
                filter: "[TourId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_WishlistItems_TourId",
                table: "WishlistItems",
                column: "TourId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WishlistItems");
        }
    }
}
