using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace hotelASP.Migrations
{
    /// <inheritdoc />
    public partial class reservationTableChnaged : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "KeyCode",
                table: "Reservations",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "KeyCode",
                table: "Reservations");
        }
    }
}
