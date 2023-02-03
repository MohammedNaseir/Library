using Microsoft.AspNetCore.Mvc;

namespace library.Web.Controllers
{
    public class CategoriesController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
