using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TAJAN_TRAVEL.Migrations
{
    /// <inheritdoc />
    public partial class addEvents : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Adults",
                table: "Inquiries",
                type: "int",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.AddColumn<int>(
                name: "Children",
                table: "Inquiries",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "DateFlexibility",
                table: "Inquiries",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "Exact");

            migrationBuilder.AddColumn<int>(
                name: "Infants",
                table: "Inquiries",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "PreferredCategory",
                table: "Inquiries",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PreferredContact",
                table: "Inquiries",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "Email");

            migrationBuilder.AddColumn<DateTime>(
                name: "PreferredEndDate",
                table: "Inquiries",
                type: "datetime2",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "EventCategories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NameEn = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    NameFr = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Slug = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventCategories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TravelEvents",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TitleEn = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    TitleFr = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Slug = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    SummaryEn = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SummaryFr = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DescriptionEn = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DescriptionFr = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CoverImageUrl = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    EventCategoryId = table.Column<int>(type: "int", nullable: true),
                    Location = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    EventDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DateLabel = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ReadTimeMinutes = table.Column<int>(type: "int", nullable: false),
                    IsFeatured = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TravelEvents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TravelEvents_EventCategories_EventCategoryId",
                        column: x => x.EventCategoryId,
                        principalTable: "EventCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EventCategories_Slug",
                table: "EventCategories",
                column: "Slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TravelEvents_EventCategoryId",
                table: "TravelEvents",
                column: "EventCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_TravelEvents_Slug",
                table: "TravelEvents",
                column: "Slug",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TravelEvents");

            migrationBuilder.DropTable(
                name: "EventCategories");

            migrationBuilder.DropColumn(
                name: "Adults",
                table: "Inquiries");

            migrationBuilder.DropColumn(
                name: "Children",
                table: "Inquiries");

            migrationBuilder.DropColumn(
                name: "DateFlexibility",
                table: "Inquiries");

            migrationBuilder.DropColumn(
                name: "Infants",
                table: "Inquiries");

            migrationBuilder.DropColumn(
                name: "PreferredCategory",
                table: "Inquiries");

            migrationBuilder.DropColumn(
                name: "PreferredContact",
                table: "Inquiries");

            migrationBuilder.DropColumn(
                name: "PreferredEndDate",
                table: "Inquiries");
        }
    }
}
