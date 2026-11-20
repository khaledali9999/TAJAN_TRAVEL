using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TAJAN_TRAVEL.Migrations
{
    /// <inheritdoc />
    public partial class InquiryCountryLanguage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "ChildrenAges",
                table: "Inquiries",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(60)",
                oldMaxLength: 60,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Country",
                table: "Inquiries",
                type: "nvarchar(80)",
                maxLength: 80,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Language",
                table: "Inquiries",
                type: "nvarchar(40)",
                maxLength: 40,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Country",
                table: "Inquiries");

            migrationBuilder.DropColumn(
                name: "Language",
                table: "Inquiries");

            migrationBuilder.AlterColumn<string>(
                name: "ChildrenAges",
                table: "Inquiries",
                type: "nvarchar(60)",
                maxLength: 60,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);
        }
    }
}
