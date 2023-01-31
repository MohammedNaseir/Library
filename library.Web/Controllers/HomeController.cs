using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace library.Web.Controllers
{
    public class HomeController : Controller
    {
        

        public IActionResult Index()
        {
            return View();
        }

       
    }
}