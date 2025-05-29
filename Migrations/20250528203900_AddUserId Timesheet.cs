using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FreelanceManager.Migrations
{
    /// <inheritdoc />
    public partial class AddUserIdTimesheet : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Timesheets",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Timesheets");
        }
    }
}
