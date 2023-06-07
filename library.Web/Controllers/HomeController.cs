

namespace library.Web.Controllers
{
    [Authorize(Roles =AppRoles.Admin)]
    public class HomeController : Controller
    {
        public HomeController()
        {

        }
        [AllowAnonymous]
        public IActionResult Index()
        {
            return View();
        }
        [Authorize]
        public IActionResult Privacy()
        {
            return View();
        }
    }
}