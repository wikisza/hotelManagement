using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace hotelASP.Migrations
{
    /// <inheritdoc />
    public partial class roomsTablesUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Room",
                table: "Room");

            migrationBuilder.RenameTable(
                name: "Room",
                newName: "Rooms");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Rooms",
                table: "Rooms",
                column: "IdRoom");

            migrationBuilder.CreateIndex(
                name: "IX_Rooms_IdStandard",
                table: "Rooms",
                column: "IdStandard");

            migrationBuilder.CreateIndex(
                name: "IX_Rooms_IdType",
                table: "Rooms",
                column: "IdType");

            migrationBuilder.AddForeignKey(
                name: "FK_Rooms_Standards_IdStandard",
                table: "Rooms",
                column: "IdStandard",
                principalTable: "Standards",
                principalColumn: "IdStandard",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Rooms_Types_IdType",
                table: "Rooms",
                column: "IdType",
                principalTable: "Types",
                principalColumn: "IdType",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Rooms_Standards_IdStandard",
                table: "Rooms");

            migrationBuilder.DropForeignKey(
                name: "FK_Rooms_Types_IdType",
                table: "Rooms");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Rooms",
                table: "Rooms");

            migrationBuilder.DropIndex(
                name: "IX_Rooms_IdStandard",
                table: "Rooms");

            migrationBuilder.DropIndex(
                name: "IX_Rooms_IdType",
                table: "Rooms");

            migrationBuilder.RenameTable(
                name: "Rooms",
                newName: "Room");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Room",
                table: "Room",
                column: "IdRoom");
        }
    }
}
