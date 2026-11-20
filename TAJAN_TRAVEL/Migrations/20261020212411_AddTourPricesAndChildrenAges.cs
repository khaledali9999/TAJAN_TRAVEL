using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TAJAN_TRAVEL.Migrations
{
    /// <inheritdoc />
    public partial class AddTourPricesAndChildrenAges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ChildrenAges",
                table: "Inquiries",
                type: "nvarchar(60)",
                maxLength: 60,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ChildrenAges",
                table: "Inquiries");
        }
    }
}
