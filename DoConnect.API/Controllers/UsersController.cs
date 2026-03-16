using Microsoft.AspNetCore.Mvc;

namespace DoConnect.API.Controllers
{
    public class UsersController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
