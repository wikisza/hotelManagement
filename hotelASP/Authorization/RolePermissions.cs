namespace hotelASP.Authorization
{
    /// <summary>
    /// Klasa mapuj¹ca role na uprawnienia
    /// </summary>
    public static class RolePermissions
    {
        private static readonly Dictionary<UserRole, HashSet<Permission>> _rolePermissionsMap = new()
        {
            {
                UserRole.Admin, new HashSet<Permission>
                {
                    // Admin ma wszystkie uprawnienia
                    Permission.ManageMenu,
                    Permission.ViewMenu,
                    Permission.AddMenuItem,
                    Permission.EditMenuItem,
                    Permission.DeleteMenuItem,
                    Permission.AddMenuCategory,
                    Permission.EditMenuCategory,
                    Permission.DeleteMenuCategory,
                    Permission.ViewOrders,
                    Permission.CreateOrder,
                    Permission.UpdateOrderStatus,
                    Permission.ViewOrderHistory,
                    Permission.CancelOrder,
                    Permission.ViewReservations,
                    Permission.CreateReservation,
                    Permission.EditReservation,
                    Permission.CancelReservation,
                    Permission.CheckInGuest,
                    Permission.CheckOutGuest,
                    Permission.ViewRooms,
                    Permission.ManageRooms,
                    Permission.UpdateRoomStatus,
                    Permission.ViewBills,
                    Permission.CreateBill,
                    Permission.EditBill,
                    Permission.ProcessPayment,
                    Permission.ViewUsers,
                    Permission.CreateUser,
                    Permission.EditUser,
                    Permission.DeleteUser,
                    Permission.ManageUserRoles,
                    Permission.ViewCustomers,
                    Permission.CreateCustomer,
                    Permission.EditCustomer,
                    Permission.DeleteCustomer,
                    Permission.AccessSystemConfiguration,
                    Permission.ViewReports,
                    Permission.ManageSettings
                }
            },
            {
                UserRole.Manager, new HashSet<Permission>
                {
                    // Manager ma wiêkszoœæ uprawnieñ oprócz zarz¹dzania systemem
                    Permission.ManageMenu,
                    Permission.ViewMenu,
                    Permission.AddMenuItem,
                    Permission.EditMenuItem,
                    Permission.DeleteMenuItem,
                    Permission.AddMenuCategory,
                    Permission.EditMenuCategory,
                    Permission.DeleteMenuCategory,
                    Permission.ViewOrders,
                    Permission.CreateOrder,
                    Permission.UpdateOrderStatus,
                    Permission.ViewOrderHistory,
                    Permission.CancelOrder,
                    Permission.ViewReservations,
                    Permission.CreateReservation,
                    Permission.EditReservation,
                    Permission.CancelReservation,
                    Permission.CheckInGuest,
                    Permission.CheckOutGuest,
                    Permission.ViewRooms,
                    Permission.ManageRooms,
                    Permission.UpdateRoomStatus,
                    Permission.ViewBills,
                    Permission.CreateBill,
                    Permission.EditBill,
                    Permission.ProcessPayment,
                    Permission.ViewCustomers,
                    Permission.CreateCustomer,
                    Permission.EditCustomer,
                    Permission.ViewReports
                }
            },
            {
                UserRole.Employee, new HashSet<Permission>
                {
                    // Pracownik ma podstawowe uprawnienia operacyjne
                    Permission.ViewMenu,
                    Permission.ViewOrders,
                    Permission.CreateOrder,
                    Permission.UpdateOrderStatus,
                    Permission.ViewOrderHistory,
                    Permission.ViewReservations,
                    Permission.CreateReservation,
                    Permission.CheckInGuest,
                    Permission.CheckOutGuest,
                    Permission.ViewRooms,
                    Permission.UpdateRoomStatus,
                    Permission.ViewBills,
                    Permission.CreateBill,
                    Permission.ViewCustomers,
                    Permission.CreateCustomer
                }
            },
            {
                UserRole.Guest, new HashSet<Permission>
                {
                    // Goœæ ma tylko uprawnienia do podgl¹du
                    Permission.ViewMenu,
                    Permission.ViewOrders
                }
            }
        };

        /// <summary>
        /// Sprawdza czy dana rola ma okreœlone uprawnienie
        /// </summary>
        public static bool HasPermission(UserRole role, Permission permission)
        {
            return _rolePermissionsMap.TryGetValue(role, out var permissions) && 
                   permissions.Contains(permission);
        }

        /// <summary>
        /// Pobiera wszystkie uprawnienia dla danej roli
        /// </summary>
        public static IEnumerable<Permission> GetPermissions(UserRole role)
        {
            return _rolePermissionsMap.TryGetValue(role, out var permissions) 
                ? permissions 
                : Enumerable.Empty<Permission>();
        }

        /// <summary>
        /// Konwertuje nazwê roli na enum
        /// </summary>
        public static UserRole? ParseRole(string roleName)
        {
            return Enum.TryParse<UserRole>(roleName, true, out var role) 
                ? role 
                : null;
        }

        /// <summary>
        /// Pobiera nazwê roli jako string
        /// </summary>
        public static string GetRoleName(UserRole role)
        {
            return role.ToString();
        }
    }
}