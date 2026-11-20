using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TAJAN_TRAVEL.Migrations
{
    /// <inheritdoc />
    public partial class addCorrectInf : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AvailabilityEn",
                table: "Tours",
                type: "nvarchar(120)",
                maxLength: 120,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AvailabilityFr",
                table: "Tours",
                type: "nvarchar(120)",
                maxLength: 120,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DurationHours",
                table: "Tours",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ExcludedEn",
                table: "Tours",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ExcludedFr",
                table: "Tours",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HighlightsEn",
                table: "Tours",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HighlightsFr",
                table: "Tours",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImportantInfoEn",
                table: "Tours",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImportantInfoFr",
                table: "Tours",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IncludedEn",
                table: "Tours",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IncludedFr",
                table: "Tours",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PaymentNoteEn",
                table: "Tours",
                type: "nvarchar(300)",
                maxLength: 300,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PaymentNoteFr",
                table: "Tours",
                type: "nvarchar(300)",
                maxLength: 300,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "QuoteEn",
                table: "Tours",
                type: "nvarchar(600)",
                maxLength: 600,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "QuoteFr",
                table: "Tours",
                type: "nvarchar(600)",
                maxLength: 600,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AvailabilityEn",
                table: "Tours");

            migrationBuilder.DropColumn(
                name: "AvailabilityFr",
                table: "Tours");

            migrationBuilder.DropColumn(
                name: "DurationHours",
                table: "Tours");

            migrationBuilder.DropColumn(
                name: "ExcludedEn",
                table: "Tours");

            migrationBuilder.DropColumn(
                name: "ExcludedFr",
                table: "Tours");

            migrationBuilder.DropColumn(
                name: "HighlightsEn",
                table: "Tours");

            migrationBuilder.DropColumn(
                name: "HighlightsFr",
                table: "Tours");

            migrationBuilder.DropColumn(
                name: "ImportantInfoEn",
                table: "Tours");

            migrationBuilder.DropColumn(
                name: "ImportantInfoFr",
                table: "Tours");

            migrationBuilder.DropColumn(
                name: "IncludedEn",
                table: "Tours");

            migrationBuilder.DropColumn(
                name: "IncludedFr",
                table: "Tours");

            migrationBuilder.DropColumn(
                name: "PaymentNoteEn",
                table: "Tours");

            migrationBuilder.DropColumn(
                name: "PaymentNoteFr",
                table: "Tours");

            migrationBuilder.DropColumn(
                name: "QuoteEn",
                table: "Tours");

            migrationBuilder.DropColumn(
                name: "QuoteFr",
                table: "Tours");
        }
    }
}
