using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace hotelASP.Migrations
{
    /// <inheritdoc />
    public partial class roomTableIsEmptyValue : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsEmpty",
                table: "Rooms",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsEmpty",
                table: "Rooms");
        }
    }
}
