using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace hotelASP.Migrations
{
    /// <inheritdoc />
    public partial class alteredReservationsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Reservations_IdRoom",
                table: "Reservations",
                column: "IdRoom");

            migrationBuilder.AddForeignKey(
                name: "FK_Reservations_Rooms_IdRoom",
                table: "Reservations",
                column: "IdRoom",
                principalTable: "Rooms",
                principalColumn: "IdRoom",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reservations_Rooms_IdRoom",
                table: "Reservations");

            migrationBuilder.DropIndex(
                name: "IX_Reservations_IdRoom",
                table: "Reservations");
        }
    }
}
