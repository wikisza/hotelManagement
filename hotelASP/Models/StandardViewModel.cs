using hotelASP.Entities;

namespace hotelASP.Models
{
    public class StandardViewModel
    {
        public Standard NewStandard { get; set; } = new Standard { StandardName = string.Empty };
        public List<Standard> Standards { get; set; } = new List<Standard>();
    }
}
