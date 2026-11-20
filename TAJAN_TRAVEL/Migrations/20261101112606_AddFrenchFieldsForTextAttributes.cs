using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TAJAN_TRAVEL.Migrations
{
    /// <inheritdoc />
    public partial class AddFrenchFieldsForTextAttributes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "LocationFr",
                table: "TravelEvents",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ClientCountryFr",
                table: "Testimonials",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CategoryFr",
                table: "FAQs",
                type: "nvarchar(80)",
                maxLength: 80,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BestTimeFr",
                table: "Destinations",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CategoryFr",
                table: "Destinations",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "KnownForFr",
                table: "Destinations",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CoverImageCaptionFr",
                table: "BlogPosts",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LocationFr",
                table: "TravelEvents");

            migrationBuilder.DropColumn(
                name: "ClientCountryFr",
                table: "Testimonials");

            migrationBuilder.DropColumn(
                name: "CategoryFr",
                table: "FAQs");

            migrationBuilder.DropColumn(
                name: "BestTimeFr",
                table: "Destinations");

            migrationBuilder.DropColumn(
                name: "CategoryFr",
                table: "Destinations");

            migrationBuilder.DropColumn(
                name: "KnownForFr",
                table: "Destinations");

            migrationBuilder.DropColumn(
                name: "CoverImageCaptionFr",
                table: "BlogPosts");
        }
    }
}
