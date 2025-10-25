using Microsoft.AspNetCore.Mvc;

namespace hotelASP.Controllers
{
    public class OrderController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
