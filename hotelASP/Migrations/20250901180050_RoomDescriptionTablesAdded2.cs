using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace hotelASP.Migrations
{
    /// <inheritdoc />
    public partial class RoomDescriptionTablesAdded2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RoomDescription_RoomDescriptionOptions_IdOption",
                table: "RoomDescription");

            migrationBuilder.DropForeignKey(
                name: "FK_RoomDescription_Rooms_IdRoom",
                table: "RoomDescription");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RoomDescription",
                table: "RoomDescription");

            migrationBuilder.RenameTable(
                name: "RoomDescription",
                newName: "RoomDescriptions");

            migrationBuilder.RenameIndex(
                name: "IX_RoomDescription_IdOption",
                table: "RoomDescriptions",
                newName: "IX_RoomDescriptions_IdOption");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RoomDescriptions",
                table: "RoomDescriptions",
                columns: new[] { "IdRoom", "IdOption" });

            migrationBuilder.AddForeignKey(
                name: "FK_RoomDescriptions_RoomDescriptionOptions_IdOption",
                table: "RoomDescriptions",
                column: "IdOption",
                principalTable: "RoomDescriptionOptions",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RoomDescriptions_Rooms_IdRoom",
                table: "RoomDescriptions",
                column: "IdRoom",
                principalTable: "Rooms",
                principalColumn: "IdRoom",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RoomDescriptions_RoomDescriptionOptions_IdOption",
                table: "RoomDescriptions");

            migrationBuilder.DropForeignKey(
                name: "FK_RoomDescriptions_Rooms_IdRoom",
                table: "RoomDescriptions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RoomDescriptions",
                table: "RoomDescriptions");

            migrationBuilder.RenameTable(
                name: "RoomDescriptions",
                newName: "RoomDescription");

            migrationBuilder.RenameIndex(
                name: "IX_RoomDescriptions_IdOption",
                table: "RoomDescription",
                newName: "IX_RoomDescription_IdOption");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RoomDescription",
                table: "RoomDescription",
                columns: new[] { "IdRoom", "IdOption" });

            migrationBuilder.AddForeignKey(
                name: "FK_RoomDescription_RoomDescriptionOptions_IdOption",
                table: "RoomDescription",
                column: "IdOption",
                principalTable: "RoomDescriptionOptions",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RoomDescription_Rooms_IdRoom",
                table: "RoomDescription",
                column: "IdRoom",
                principalTable: "Rooms",
                principalColumn: "IdRoom",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
