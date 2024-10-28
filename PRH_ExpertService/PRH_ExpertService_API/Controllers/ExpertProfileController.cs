using Microsoft.AspNetCore.Mvc;

namespace PRH_ExpertService_API.Controllers
{
    public class ExpertProfileController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
