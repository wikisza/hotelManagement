namespace hotelASP.Views.Rooms.Helpers
{
    public class GrammarHelper
    {
        public static string PeopleText(int count)
        {
            if(count == 1) return "dla 1 osoby";
            if(count >= 2 && count <= 4) return $"dla {count} osób";
            return $"dla {count} osób";
        }

        public static string BedsText(int count)
        {
            if(count == 1) return "1 łóżko";
            if(count >= 2 && count <= 4) return $"{count} łóżka";
            return $"{count} łóżek";
        }
    }
}
