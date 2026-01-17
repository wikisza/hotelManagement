namespace hotelASP.Authorization
{
    /// <summary>
    /// Enum definiuj¹cy szczegó³owe uprawnienia w systemie
    /// </summary>
    public enum Permission
    {
        // Zarz¹dzanie menu
        ManageMenu,
        ViewMenu,
        AddMenuItem,
        EditMenuItem,
        DeleteMenuItem,
        AddMenuCategory,
        EditMenuCategory,
        DeleteMenuCategory,

        // Zarz¹dzanie zamówieniami
        ViewOrders,
        CreateOrder,
        UpdateOrderStatus,
        ViewOrderHistory,
        CancelOrder,

        // Zarz¹dzanie rezerwacjami
        ViewReservations,
        CreateReservation,
        EditReservation,
        CancelReservation,
        CheckInGuest,
        CheckOutGuest,

        // Zarz¹dzanie pokojami
        ViewRooms,
        ManageRooms,
        UpdateRoomStatus,

        // Zarz¹dzanie rachunkami
        ViewBills,
        CreateBill,
        EditBill,
        ProcessPayment,

        // Zarz¹dzanie u¿ytkownikami
        ViewUsers,
        CreateUser,
        EditUser,
        DeleteUser,
        ManageUserRoles,

        // Zarz¹dzanie klientami
        ViewCustomers,
        CreateCustomer,
        EditCustomer,
        DeleteCustomer,

        // Konfiguracja systemu
        AccessSystemConfiguration,
        ViewReports,
        ManageSettings
    }
}