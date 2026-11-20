using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TAJAN_TRAVEL.Migrations
{
    /// <inheritdoc />
    public partial class addStory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CoverImageCaption",
                table: "BlogPosts",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsFeatured",
                table: "BlogPosts",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "ReadTimeMinutes",
                table: "BlogPosts",
                type: "int",
                nullable: false,
                defaultValue: 5);

            migrationBuilder.AddColumn<int>(
                name: "DisplayOrder",
                table: "BlogCategories",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "ShowInFilter",
                table: "BlogCategories",
                type: "bit",
                nullable: false,
                defaultValue: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CoverImageCaption",
                table: "BlogPosts");

            migrationBuilder.DropColumn(
                name: "IsFeatured",
                table: "BlogPosts");

            migrationBuilder.DropColumn(
                name: "ReadTimeMinutes",
                table: "BlogPosts");

            migrationBuilder.DropColumn(
                name: "DisplayOrder",
                table: "BlogCategories");

            migrationBuilder.DropColumn(
                name: "ShowInFilter",
                table: "BlogCategories");
        }
    }
}
