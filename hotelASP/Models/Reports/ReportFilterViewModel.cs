using System.ComponentModel.DataAnnotations;

namespace hotelASP.Models.Reports
{
    public class ReportFilterViewModel
    {
        [Required(ErrorMessage = "Data pocz¹tkowa jest wymagana")]
        [Display(Name = "Data od")]
        public DateTime DateFrom { get; set; } = DateTime.Now.AddMonths(-1);

        [Required(ErrorMessage = "Data koñcowa jest wymagana")]
        [Display(Name = "Data do")]
        public DateTime DateTo { get; set; } = DateTime.Now;

        [Display(Name = "Typ raportu")]
        public ReportType ReportType { get; set; }
    }

    public enum ReportType
    {
        Income,
        Customers,
        Orders
    }
}