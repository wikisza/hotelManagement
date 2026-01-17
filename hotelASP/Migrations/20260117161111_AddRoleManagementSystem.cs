using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace hotelASP.Migrations
{
    /// <inheritdoc />
    public partial class AddRoleManagementSystem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Roles",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Roles",
                type: "varchar(200)",
                maxLength: 200,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<bool>(
                name: "IsSystemRole",
                table: "Roles",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Roles",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Permissions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Code = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Description = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Category = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsSystemPermission = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Permissions", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "RolePermissions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    RoleId = table.Column<int>(type: "int", nullable: false),
                    PermissionId = table.Column<int>(type: "int", nullable: false),
                    AssignedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RolePermissions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RolePermissions_Permissions_PermissionId",
                        column: x => x.PermissionId,
                        principalTable: "Permissions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RolePermissions_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.InsertData(
                table: "Permissions",
                columns: new[] { "Id", "Category", "Code", "CreatedAt", "Description", "IsSystemPermission", "Name" },
                values: new object[,]
                {
                    { 1, "Menu", "MENU_MANAGE", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Dostęp do zarządzania menu", true, "Zarządzanie menu" },
                    { 2, "Menu", "MENU_VIEW", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Możliwość przeglądania menu", true, "Podgląd menu" },
                    { 3, "Menu", "MENU_ITEM_ADD", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Możliwość dodawania nowych pozycji", true, "Dodawanie pozycji menu" },
                    { 4, "Menu", "MENU_ITEM_EDIT", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Możliwość edytowania istniejących pozycji", true, "Edycja pozycji menu" },
                    { 5, "Menu", "MENU_ITEM_DELETE", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Możliwość usuwania pozycji", true, "Usuwanie pozycji menu" },
                    { 6, "Menu", "MENU_CATEGORY_ADD", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Możliwość tworzenia nowych kategorii", true, "Dodawanie kategorii" },
                    { 7, "Menu", "MENU_CATEGORY_EDIT", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Możliwość edytowania kategorii", true, "Edycja kategorii" },
                    { 8, "Menu", "MENU_CATEGORY_DELETE", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Możliwość usuwania kategorii", true, "Usuwanie kategorii" },
                    { 9, "Zamówienia", "ORDER_VIEW", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Możliwość przeglądania zamówień", true, "Podgląd zamówień" },
                    { 10, "Zamówienia", "ORDER_CREATE", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Możliwość składania zamówień", true, "Tworzenie zamówień" },
                    { 11, "Zamówienia", "ORDER_UPDATE_STATUS", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Możliwość aktualizacji statusu", true, "Zmiana statusu zamówień" },
                    { 12, "Zamówienia", "ORDER_HISTORY", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Dostęp do historii zamówień", true, "Historia zamówień" },
                    { 13, "Zamówienia", "ORDER_CANCEL", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Możliwość anulowania zamówień", true, "Anulowanie zamówień" },
                    { 14, "Rezerwacje", "RESERVATION_VIEW", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Możliwość przeglądania rezerwacji", true, "Podgląd rezerwacji" },
                    { 15, "Rezerwacje", "RESERVATION_CREATE", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Możliwość tworzenia nowych rezerwacji", true, "Tworzenie rezerwacji" },
                    { 16, "Rezerwacje", "RESERVATION_EDIT", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Możliwość edytowania rezerwacji", true, "Edycja rezerwacji" },
                    { 17, "Rezerwacje", "RESERVATION_CANCEL", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Możliwość anulowania rezerwacji", true, "Anulowanie rezerwacji" },
                    { 18, "Rezerwacje", "GUEST_CHECKIN", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Możliwość meldowania gości", true, "Zameldowanie gości" },
                    { 19, "Rezerwacje", "GUEST_CHECKOUT", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Możliwość wymeldowania gości", true, "Wymeldowanie gości" },
                    { 20, "Pokoje", "ROOM_VIEW", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Możliwość przeglądania pokoi", true, "Podgląd pokoi" },
                    { 21, "Pokoje", "ROOM_MANAGE", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Możliwość zarządzania pokojami", true, "Zarządzanie pokojami" },
                    { 22, "Pokoje", "ROOM_STATUS_UPDATE", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Możliwość zmiany statusu pokoju", true, "Zmiana statusu pokoju" },
                    { 23, "Rachunki", "BILL_VIEW", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Możliwość przeglądania rachunków", true, "Podgląd rachunków" },
                    { 24, "Rachunki", "BILL_CREATE", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Możliwość tworzenia rachunków", true, "Tworzenie rachunków" },
                    { 25, "Rachunki", "BILL_EDIT", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Możliwość edytowania rachunków", true, "Edycja rachunków" },
                    { 26, "Rachunki", "PAYMENT_PROCESS", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Możliwość przetwarzania płatności", true, "Przetwarzanie płatności" },
                    { 27, "Użytkownicy", "USER_VIEW", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Możliwość przeglądania użytkowników", true, "Podgląd użytkowników" },
                    { 28, "Użytkownicy", "USER_CREATE", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Możliwość dodawania użytkowników", true, "Tworzenie użytkowników" },
                    { 29, "Użytkownicy", "USER_EDIT", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Możliwość edytowania użytkowników", true, "Edycja użytkowników" },
                    { 30, "Użytkownicy", "USER_DELETE", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Możliwość usuwania użytkowników", true, "Usuwanie użytkowników" },
                    { 31, "Użytkownicy", "ROLE_MANAGE", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Możliwość zarządzania rolami i uprawnieniami", true, "Zarządzanie rolami" },
                    { 32, "Klienci", "CUSTOMER_VIEW", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Możliwość przeglądania klientów", true, "Podgląd klientów" },
                    { 33, "Klienci", "CUSTOMER_CREATE", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Możliwość dodawania klientów", true, "Dodawanie klientów" },
                    { 34, "Klienci", "CUSTOMER_EDIT", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Możliwość edytowania klientów", true, "Edycja klientów" },
                    { 35, "Klienci", "CUSTOMER_DELETE", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Możliwość usuwania klientów", true, "Usuwanie klientów" },
                    { 36, "System", "SYSTEM_CONFIG", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Dostęp do konfiguracji systemu", true, "Konfiguracja systemu" },
                    { 37, "System", "REPORTS_VIEW", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Możliwość przeglądania raportów", true, "Podgląd raportów" },
                    { 38, "System", "SETTINGS_MANAGE", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Możliwość zmiany ustawień", true, "Zarządzanie ustawieniami" }
                });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "Description", "IsSystemRole", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Administrator systemu z pełnymi uprawnieniami", true, null });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "Description", "IsSystemRole", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Menedżer z rozszerzonymi uprawnieniami", true, null });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAt", "Description", "IsSystemRole", "Name", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Pracownik z podstawowymi uprawnieniami", true, "Employee", null });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CreatedAt", "Description", "IsSystemRole", "Name", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Gość z ograniczonymi uprawnieniami", true, "Guest", null });

            migrationBuilder.InsertData(
                table: "RolePermissions",
                columns: new[] { "Id", "AssignedAt", "PermissionId", "RoleId" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, 1 },
                    { 2, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 2, 1 },
                    { 3, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 3, 1 },
                    { 4, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 4, 1 },
                    { 5, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 5, 1 },
                    { 6, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 6, 1 },
                    { 7, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 7, 1 },
                    { 8, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 8, 1 },
                    { 9, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 9, 1 },
                    { 10, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 10, 1 },
                    { 11, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 11, 1 },
                    { 12, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 12, 1 },
                    { 13, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 13, 1 },
                    { 14, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 14, 1 },
                    { 15, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 15, 1 },
                    { 16, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 16, 1 },
                    { 17, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 17, 1 },
                    { 18, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 18, 1 },
                    { 19, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 19, 1 },
                    { 20, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 20, 1 },
                    { 21, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 21, 1 },
                    { 22, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 22, 1 },
                    { 23, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 23, 1 },
                    { 24, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 24, 1 },
                    { 25, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 25, 1 },
                    { 26, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 26, 1 },
                    { 27, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 27, 1 },
                    { 28, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 28, 1 },
                    { 29, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 29, 1 },
                    { 30, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 30, 1 },
                    { 31, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 31, 1 },
                    { 32, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 32, 1 },
                    { 33, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 33, 1 },
                    { 34, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 34, 1 },
                    { 35, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 35, 1 },
                    { 36, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 36, 1 },
                    { 37, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 37, 1 },
                    { 38, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 38, 1 },
                    { 39, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, 2 },
                    { 40, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 2, 2 },
                    { 41, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 3, 2 },
                    { 42, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 4, 2 },
                    { 43, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 5, 2 },
                    { 44, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 6, 2 },
                    { 45, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 7, 2 },
                    { 46, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 8, 2 },
                    { 47, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 9, 2 },
                    { 48, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 10, 2 },
                    { 49, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 11, 2 },
                    { 50, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 12, 2 },
                    { 51, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 13, 2 },
                    { 52, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 14, 2 },
                    { 53, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 15, 2 },
                    { 54, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 16, 2 },
                    { 55, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 17, 2 },
                    { 56, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 18, 2 },
                    { 57, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 19, 2 },
                    { 58, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 20, 2 },
                    { 59, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 21, 2 },
                    { 60, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 22, 2 },
                    { 61, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 23, 2 },
                    { 62, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 24, 2 },
                    { 63, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 25, 2 },
                    { 64, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 26, 2 },
                    { 65, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 32, 2 },
                    { 66, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 33, 2 },
                    { 67, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 34, 2 },
                    { 68, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 37, 2 },
                    { 69, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 2, 3 },
                    { 70, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 9, 3 },
                    { 71, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 10, 3 },
                    { 72, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 11, 3 },
                    { 73, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 12, 3 },
                    { 74, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 14, 3 },
                    { 75, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 15, 3 },
                    { 76, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 18, 3 },
                    { 77, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 19, 3 },
                    { 78, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 20, 3 },
                    { 79, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 22, 3 },
                    { 80, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 23, 3 },
                    { 81, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 24, 3 },
                    { 82, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 32, 3 },
                    { 83, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 33, 3 },
                    { 84, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 2, 4 },
                    { 85, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 9, 4 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Roles_Name",
                table: "Roles",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Permissions_Code",
                table: "Permissions",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RolePermissions_PermissionId",
                table: "RolePermissions",
                column: "PermissionId");

            migrationBuilder.CreateIndex(
                name: "IX_RolePermissions_RoleId_PermissionId",
                table: "RolePermissions",
                columns: new[] { "RoleId", "PermissionId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RolePermissions");

            migrationBuilder.DropTable(
                name: "Permissions");

            migrationBuilder.DropIndex(
                name: "IX_Roles_Name",
                table: "Roles");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Roles");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Roles");

            migrationBuilder.DropColumn(
                name: "IsSystemRole",
                table: "Roles");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Roles");

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 3,
                column: "Name",
                value: "Receptionist");

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 4,
                column: "Name",
                value: "Cleaner");

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "Name" },
                values: new object[] { 5, "Chef" });
        }
    }
}
