using library.Infrastructure.Services.Dashboard;
using Microsoft.AspNetCore.Mvc;

namespace library.Web.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly IDashboardServire _dashboardServire;

        public DashboardController(IDashboardServire dashboardServire)
        {
            _dashboardServire = dashboardServire;
        }
        public IActionResult Index() 
        {
            var numberOfCopies = _dashboardServire.NumberOfCopies();
            var numberOfSubscribers  =  _dashboardServire.NumberOfSubscribers();
            DashboardViewModel model = new DashboardViewModel
            {
                NumberOfSubscribers = numberOfSubscribers,
                NumberOfCopies = numberOfCopies,
                LastAddedBooks = _dashboardServire.RecentBooks(),
                TopBooks=_dashboardServire.GetTopBooks()
            };
            return View(model);
        }
        [AjaxOnly]
        public IActionResult GetRentalsPerDay(DateTime? startDate, DateTime? endDate)
        {
             startDate ??= DateTime.Today.AddDays(-29);
             endDate ??= DateTime.Today;

            var data = _dashboardServire.GetChartData(startDate,endDate);
            return Ok(data);
        }
        [AjaxOnly]
        public IActionResult GetSubscribersPerCity()
        {
            var data = _dashboardServire.GetSubscribersPerCity();
            return Ok(data);

        }
    }
}
