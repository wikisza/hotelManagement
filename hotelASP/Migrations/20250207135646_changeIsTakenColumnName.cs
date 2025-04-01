using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace hotelASP.Migrations
{
    /// <inheritdoc />
    public partial class changeIsTakenColumnName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Is_taken",
                table: "Room",
                newName: "IsTaken");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsTaken",
                table: "Room",
                newName: "Is_taken");
        }
    }
}
