using Microsoft.AspNetCore.Mvc;

namespace HolidayHome.Controllers
{
    public class SupportController : Controller
    {
        public IActionResult Help()
        {
            return View();
        }

        public IActionResult Contact()
        {
            return View();
        }

        public IActionResult FAQ()
        {
            return View();
        }

        public IActionResult Report()
        {
            return View();
        }
    }
}
