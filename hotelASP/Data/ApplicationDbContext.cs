using hotelASP.Entities;
using Microsoft.EntityFrameworkCore;

namespace hotelASP.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) 
        {
        }

        // DbSets - istniejące encje
        public DbSet<UserAccount> Users { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Room> Rooms { get; set; } 
        public DbSet<RoomType> Types { get; set; }
        public DbSet<Standard> Standards { get; set; }
        public DbSet<Reservation> Reservations { get; set; }
        public DbSet<RoomDescription> RoomDescriptions { get; set; }
        public DbSet<RoomDescriptionOption> RoomDescriptionOptions { get; set; }
        public DbSet<MenuCategory> MenuCategories { get; set; }
        public DbSet<MenuItem> MenuItems { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetails> OrderDetails { get; set; }
        public DbSet<Bill> Bills { get; set; }
        
        // DbSets - nowe dla systemu ról i uprawnień
        public DbSet<Entities.Role> Roles { get; set; }
        public DbSet<Entities.Permission> Permissions { get; set; }
        public DbSet<RolePermission> RolePermissions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Relacje użytkowników
            modelBuilder.Entity<UserAccount>()
                .HasOne(u => u.Role)
                .WithMany(r => r.Users)
                .HasForeignKey(u => u.RoleId);

            // Relacje klientów i rezerwacji
            modelBuilder.Entity<Customer>()
                .HasMany(c => c.Reservations)
                .WithOne(r => r.Customer)
                .HasForeignKey(r => r.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            // Relacje pokoi
            modelBuilder.Entity<Room>()
                .HasOne(u => u.RoomType)
                .WithMany(r => r.Rooms)
                .HasForeignKey(u => u.IdType);

            modelBuilder.Entity<Room>()
                .HasOne(u => u.Standard)
                .WithMany(r => r.Rooms)
                .HasForeignKey(u => u.IdStandard);

            modelBuilder.Entity<Room>()
                .HasMany(u => u.RoomDescriptions)
                .WithOne(u => u.Room)
                .HasForeignKey(rd => rd.IdRoom);

            modelBuilder.Entity<Room>()
                .HasMany(u => u.Reservations)
                .WithOne(r => r.Room)
                .HasForeignKey(u => u.IdRoom);

            // Relacje opisów pokoi
            modelBuilder.Entity<RoomDescription>()
                .HasKey(rd => new { rd.IdRoom, rd.IdOption });

            modelBuilder.Entity<RoomDescription>()
                .HasOne(rd => rd.Room)
                .WithMany(r => r.RoomDescriptions)
                .HasForeignKey(rd => rd.IdRoom);

            modelBuilder.Entity<RoomDescriptionOption>()
                .HasMany(rd => rd.RoomDescriptions)
                .WithOne(o => o.DescriptionOption)
                .HasForeignKey(rd => rd.IdOption);

            // Relacje zamówień
            modelBuilder.Entity<Order>()
                .HasOne(o => o.Reservation)
                .WithMany(r => r.Orders)
                .HasForeignKey(o => o.ReservationId);

            modelBuilder.Entity<Order>()
                .HasMany(o => o.OrderDetails)
                .WithOne(od => od.Order)
                .HasForeignKey(od => od.OrderId);

            // Relacje menu
            modelBuilder.Entity<MenuItem>()
                .HasMany(mi => mi.OrderDetails)
                .WithOne(od => od.MenuItem)
                .HasForeignKey(od => od.MenuItemId);

            modelBuilder.Entity<MenuItem>()
                .HasOne(mi => mi.MenuCategory)
                .WithMany(mc => mc.MenuItems)
                .HasForeignKey(mi => mi.MenuCategoryId);

            // Relacje rachunków
            modelBuilder.Entity<Bill>()
                .HasOne(b => b.Reservation)
                .WithOne(r => r.Bills)
                .HasForeignKey<Bill>(b => b.ReservationId)
                .OnDelete(DeleteBehavior.Cascade);

            // Konfiguracja ról i uprawnień
            modelBuilder.Entity<Entities.Role>(entity =>
            {
                entity.HasIndex(e => e.Name).IsUnique();
                entity.Property(e => e.Name).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Description).HasMaxLength(200);
            });

            modelBuilder.Entity<Entities.Permission>(entity =>
            {
                entity.HasIndex(e => e.Code).IsUnique();
                entity.Property(e => e.Code).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Category).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Description).HasMaxLength(200);
            });

            modelBuilder.Entity<RolePermission>(entity =>
            {
                entity.HasIndex(e => new { e.RoleId, e.PermissionId }).IsUnique();
                
                entity.HasOne(rp => rp.Role)
                    .WithMany(r => r.RolePermissions)
                    .HasForeignKey(rp => rp.RoleId)
                    .OnDelete(DeleteBehavior.Cascade);
                
                entity.HasOne(rp => rp.Permission)
                    .WithMany(p => p.RolePermissions)
                    .HasForeignKey(rp => rp.PermissionId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Seed danych
            SeedRolesAndPermissions(modelBuilder);
        }

        private void SeedRolesAndPermissions(ModelBuilder modelBuilder)
        {
            // Stała data dla seed data (ważne dla migracji)
            var seedDate = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            // Seed ról systemowych
            var roles = new[]
            {
                new Entities.Role 
                { 
                    Id = 1, 
                    Name = "Admin", 
                    Description = "Administrator systemu z pełnymi uprawnieniami", 
                    IsSystemRole = true, 
                    CreatedAt = seedDate 
                },
                new Entities.Role 
                { 
                    Id = 2, 
                    Name = "Manager", 
                    Description = "Menedżer z rozszerzonymi uprawnieniami", 
                    IsSystemRole = true, 
                    CreatedAt = seedDate 
                },
                new Entities.Role 
                { 
                    Id = 3, 
                    Name = "Employee", 
                    Description = "Pracownik z podstawowymi uprawnieniami", 
                    IsSystemRole = true, 
                    CreatedAt = seedDate 
                },
                new Entities.Role 
                { 
                    Id = 4, 
                    Name = "Guest", 
                    Description = "Gość z ograniczonymi uprawnieniami", 
                    IsSystemRole = true, 
                    CreatedAt = seedDate 
                }
            };

            // Seed uprawnień systemowych
            var permissions = new List<Entities.Permission>
            {
                // Zarządzanie menu (1-8)
                new Entities.Permission { Id = 1, Code = "MENU_MANAGE", Name = "Zarządzanie menu", Category = "Menu", Description = "Dostęp do zarządzania menu", IsSystemPermission = true, CreatedAt = seedDate },
                new Entities.Permission { Id = 2, Code = "MENU_VIEW", Name = "Podgląd menu", Category = "Menu", Description = "Możliwość przeglądania menu", IsSystemPermission = true, CreatedAt = seedDate },
                new Entities.Permission { Id = 3, Code = "MENU_ITEM_ADD", Name = "Dodawanie pozycji menu", Category = "Menu", Description = "Możliwość dodawania nowych pozycji", IsSystemPermission = true, CreatedAt = seedDate },
                new Entities.Permission { Id = 4, Code = "MENU_ITEM_EDIT", Name = "Edycja pozycji menu", Category = "Menu", Description = "Możliwość edytowania istniejących pozycji", IsSystemPermission = true, CreatedAt = seedDate },
                new Entities.Permission { Id = 5, Code = "MENU_ITEM_DELETE", Name = "Usuwanie pozycji menu", Category = "Menu", Description = "Możliwość usuwania pozycji", IsSystemPermission = true, CreatedAt = seedDate },
                new Entities.Permission { Id = 6, Code = "MENU_CATEGORY_ADD", Name = "Dodawanie kategorii", Category = "Menu", Description = "Możliwość tworzenia nowych kategorii", IsSystemPermission = true, CreatedAt = seedDate },
                new Entities.Permission { Id = 7, Code = "MENU_CATEGORY_EDIT", Name = "Edycja kategorii", Category = "Menu", Description = "Możliwość edytowania kategorii", IsSystemPermission = true, CreatedAt = seedDate },
                new Entities.Permission { Id = 8, Code = "MENU_CATEGORY_DELETE", Name = "Usuwanie kategorii", Category = "Menu", Description = "Możliwość usuwania kategorii", IsSystemPermission = true, CreatedAt = seedDate },

                // Zamówienia (9-13)
                new Entities.Permission { Id = 9, Code = "ORDER_VIEW", Name = "Podgląd zamówień", Category = "Zamówienia", Description = "Możliwość przeglądania zamówień", IsSystemPermission = true, CreatedAt = seedDate },
                new Entities.Permission { Id = 10, Code = "ORDER_CREATE", Name = "Tworzenie zamówień", Category = "Zamówienia", Description = "Możliwość składania zamówień", IsSystemPermission = true, CreatedAt = seedDate },
                new Entities.Permission { Id = 11, Code = "ORDER_UPDATE_STATUS", Name = "Zmiana statusu zamówień", Category = "Zamówienia", Description = "Możliwość aktualizacji statusu", IsSystemPermission = true, CreatedAt = seedDate },
                new Entities.Permission { Id = 12, Code = "ORDER_HISTORY", Name = "Historia zamówień", Category = "Zamówienia", Description = "Dostęp do historii zamówień", IsSystemPermission = true, CreatedAt = seedDate },
                new Entities.Permission { Id = 13, Code = "ORDER_CANCEL", Name = "Anulowanie zamówień", Category = "Zamówienia", Description = "Możliwość anulowania zamówień", IsSystemPermission = true, CreatedAt = seedDate },

                // Rezerwacje (14-19)
                new Entities.Permission { Id = 14, Code = "RESERVATION_VIEW", Name = "Podgląd rezerwacji", Category = "Rezerwacje", Description = "Możliwość przeglądania rezerwacji", IsSystemPermission = true, CreatedAt = seedDate },
                new Entities.Permission { Id = 15, Code = "RESERVATION_CREATE", Name = "Tworzenie rezerwacji", Category = "Rezerwacje", Description = "Możliwość tworzenia nowych rezerwacji", IsSystemPermission = true, CreatedAt = seedDate },
                new Entities.Permission { Id = 16, Code = "RESERVATION_EDIT", Name = "Edycja rezerwacji", Category = "Rezerwacje", Description = "Możliwość edytowania rezerwacji", IsSystemPermission = true, CreatedAt = seedDate },
                new Entities.Permission { Id = 17, Code = "RESERVATION_CANCEL", Name = "Anulowanie rezerwacji", Category = "Rezerwacje", Description = "Możliwość anulowania rezerwacji", IsSystemPermission = true, CreatedAt = seedDate },
                new Entities.Permission { Id = 18, Code = "GUEST_CHECKIN", Name = "Zameldowanie gości", Category = "Rezerwacje", Description = "Możliwość meldowania gości", IsSystemPermission = true, CreatedAt = seedDate },
                new Entities.Permission { Id = 19, Code = "GUEST_CHECKOUT", Name = "Wymeldowanie gości", Category = "Rezerwacje", Description = "Możliwość wymeldowania gości", IsSystemPermission = true, CreatedAt = seedDate },

                // Pokoje (20-22)
                new Entities.Permission { Id = 20, Code = "ROOM_VIEW", Name = "Podgląd pokoi", Category = "Pokoje", Description = "Możliwość przeglądania pokoi", IsSystemPermission = true, CreatedAt = seedDate },
                new Entities.Permission { Id = 21, Code = "ROOM_MANAGE", Name = "Zarządzanie pokojami", Category = "Pokoje", Description = "Możliwość zarządzania pokojami", IsSystemPermission = true, CreatedAt = seedDate },
                new Entities.Permission { Id = 22, Code = "ROOM_STATUS_UPDATE", Name = "Zmiana statusu pokoju", Category = "Pokoje", Description = "Możliwość zmiany statusu pokoju", IsSystemPermission = true, CreatedAt = seedDate },

                // Rachunki (23-26)
                new Entities.Permission { Id = 23, Code = "BILL_VIEW", Name = "Podgląd rachunków", Category = "Rachunki", Description = "Możliwość przeglądania rachunków", IsSystemPermission = true, CreatedAt = seedDate },
                new Entities.Permission { Id = 24, Code = "BILL_CREATE", Name = "Tworzenie rachunków", Category = "Rachunki", Description = "Możliwość tworzenia rachunków", IsSystemPermission = true, CreatedAt = seedDate },
                new Entities.Permission { Id = 25, Code = "BILL_EDIT", Name = "Edycja rachunków", Category = "Rachunki", Description = "Możliwość edytowania rachunków", IsSystemPermission = true, CreatedAt = seedDate },
                new Entities.Permission { Id = 26, Code = "PAYMENT_PROCESS", Name = "Przetwarzanie płatności", Category = "Rachunki", Description = "Możliwość przetwarzania płatności", IsSystemPermission = true, CreatedAt = seedDate },

                // Użytkownicy (27-31)
                new Entities.Permission { Id = 27, Code = "USER_VIEW", Name = "Podgląd użytkowników", Category = "Użytkownicy", Description = "Możliwość przeglądania użytkowników", IsSystemPermission = true, CreatedAt = seedDate },
                new Entities.Permission { Id = 28, Code = "USER_CREATE", Name = "Tworzenie użytkowników", Category = "Użytkownicy", Description = "Możliwość dodawania użytkowników", IsSystemPermission = true, CreatedAt = seedDate },
                new Entities.Permission { Id = 29, Code = "USER_EDIT", Name = "Edycja użytkowników", Category = "Użytkownicy", Description = "Możliwość edytowania użytkowników", IsSystemPermission = true, CreatedAt = seedDate },
                new Entities.Permission { Id = 30, Code = "USER_DELETE", Name = "Usuwanie użytkowników", Category = "Użytkownicy", Description = "Możliwość usuwania użytkowników", IsSystemPermission = true, CreatedAt = seedDate },
                new Entities.Permission { Id = 31, Code = "ROLE_MANAGE", Name = "Zarządzanie rolami", Category = "Użytkownicy", Description = "Możliwość zarządzania rolami i uprawnieniami", IsSystemPermission = true, CreatedAt = seedDate },

                // Klienci (32-35)
                new Entities.Permission { Id = 32, Code = "CUSTOMER_VIEW", Name = "Podgląd klientów", Category = "Klienci", Description = "Możliwość przeglądania klientów", IsSystemPermission = true, CreatedAt = seedDate },
                new Entities.Permission { Id = 33, Code = "CUSTOMER_CREATE", Name = "Dodawanie klientów", Category = "Klienci", Description = "Możliwość dodawania klientów", IsSystemPermission = true, CreatedAt = seedDate },
                new Entities.Permission { Id = 34, Code = "CUSTOMER_EDIT", Name = "Edycja klientów", Category = "Klienci", Description = "Możliwość edytowania klientów", IsSystemPermission = true, CreatedAt = seedDate },
                new Entities.Permission { Id = 35, Code = "CUSTOMER_DELETE", Name = "Usuwanie klientów", Category = "Klienci", Description = "Możliwość usuwania klientów", IsSystemPermission = true, CreatedAt = seedDate },

                // System (36-38)
                new Entities.Permission { Id = 36, Code = "SYSTEM_CONFIG", Name = "Konfiguracja systemu", Category = "System", Description = "Dostęp do konfiguracji systemu", IsSystemPermission = true, CreatedAt = seedDate },
                new Entities.Permission { Id = 37, Code = "REPORTS_VIEW", Name = "Podgląd raportów", Category = "System", Description = "Możliwość przeglądania raportów", IsSystemPermission = true, CreatedAt = seedDate },
                new Entities.Permission { Id = 38, Code = "SETTINGS_MANAGE", Name = "Zarządzanie ustawieniami", Category = "System", Description = "Możliwość zmiany ustawień", IsSystemPermission = true, CreatedAt = seedDate }
            };

            // Przypisanie uprawnień do ról
            var rolePermissions = new List<RolePermission>();
            int rpId = 1;

            // Admin - wszystkie uprawnienia (1-38)
            for (int i = 1; i <= 38; i++)
            {
                rolePermissions.Add(new RolePermission 
                { 
                    Id = rpId++, 
                    RoleId = 1, 
                    PermissionId = i, 
                    AssignedAt = seedDate 
                });
            }

            // Manager - większość uprawnień (bez zarządzania systemem i rolami)
            int[] managerPermissions = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 32, 33, 34, 37 };
            foreach (var permId in managerPermissions)
            {
                rolePermissions.Add(new RolePermission 
                { 
                    Id = rpId++, 
                    RoleId = 2, 
                    PermissionId = permId, 
                    AssignedAt = seedDate 
                });
            }

            // Employee - podstawowe uprawnienia operacyjne
            int[] employeePermissions = { 2, 9, 10, 11, 12, 14, 15, 18, 19, 20, 22, 23, 24, 32, 33 };
            foreach (var permId in employeePermissions)
            {
                rolePermissions.Add(new RolePermission 
                { 
                    Id = rpId++, 
                    RoleId = 3, 
                    PermissionId = permId, 
                    AssignedAt = seedDate 
                });
            }

            // Guest - tylko podgląd
            int[] guestPermissions = { 2, 9 };
            foreach (var permId in guestPermissions)
            {
                rolePermissions.Add(new RolePermission 
                { 
                    Id = rpId++, 
                    RoleId = 4, 
                    PermissionId = permId, 
                    AssignedAt = seedDate 
                });
            }

            modelBuilder.Entity<Entities.Role>().HasData(roles);
            modelBuilder.Entity<Entities.Permission>().HasData(permissions);
            modelBuilder.Entity<RolePermission>().HasData(rolePermissions);
        }
    }
}
