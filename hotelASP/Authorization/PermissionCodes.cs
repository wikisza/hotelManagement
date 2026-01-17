namespace hotelASP.Authorization
{
    /// <summary>
    /// Sta³e zawieraj¹ce kody uprawnieñ - zapewnia type safety i IntelliSense
    /// </summary>
    public static class PermissionCodes
    {
        // Menu
        public const string MenuManage = "MENU_MANAGE";
        public const string MenuView = "MENU_VIEW";
        public const string MenuItemAdd = "MENU_ITEM_ADD";
        public const string MenuItemEdit = "MENU_ITEM_EDIT";
        public const string MenuItemDelete = "MENU_ITEM_DELETE";
        public const string MenuCategoryAdd = "MENU_CATEGORY_ADD";
        public const string MenuCategoryEdit = "MENU_CATEGORY_EDIT";
        public const string MenuCategoryDelete = "MENU_CATEGORY_DELETE";

        // Zamówienia
        public const string OrderView = "ORDER_VIEW";
        public const string OrderCreate = "ORDER_CREATE";
        public const string OrderUpdateStatus = "ORDER_UPDATE_STATUS";
        public const string OrderHistory = "ORDER_HISTORY";
        public const string OrderCancel = "ORDER_CANCEL";

        // Rezerwacje
        public const string ReservationView = "RESERVATION_VIEW";
        public const string ReservationCreate = "RESERVATION_CREATE";
        public const string ReservationEdit = "RESERVATION_EDIT";
        public const string ReservationCancel = "RESERVATION_CANCEL";
        public const string GuestCheckIn = "GUEST_CHECKIN";
        public const string GuestCheckOut = "GUEST_CHECKOUT";

        // Pokoje
        public const string RoomView = "ROOM_VIEW";
        public const string RoomManage = "ROOM_MANAGE";
        public const string RoomStatusUpdate = "ROOM_STATUS_UPDATE";

        // Rachunki
        public const string BillView = "BILL_VIEW";
        public const string BillCreate = "BILL_CREATE";
        public const string BillEdit = "BILL_EDIT";
        public const string PaymentProcess = "PAYMENT_PROCESS";

        // U¿ytkownicy
        public const string UserView = "USER_VIEW";
        public const string UserCreate = "USER_CREATE";
        public const string UserEdit = "USER_EDIT";
        public const string UserDelete = "USER_DELETE";
        public const string RoleManage = "ROLE_MANAGE";

        // Klienci
        public const string CustomerView = "CUSTOMER_VIEW";
        public const string CustomerCreate = "CUSTOMER_CREATE";
        public const string CustomerEdit = "CUSTOMER_EDIT";
        public const string CustomerDelete = "CUSTOMER_DELETE";

        // System
        public const string SystemConfig = "SYSTEM_CONFIG";
        public const string ReportsView = "REPORTS_VIEW";
        public const string SettingsManage = "SETTINGS_MANAGE";
    }
}