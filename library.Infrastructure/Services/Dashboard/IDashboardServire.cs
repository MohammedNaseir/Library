using library.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace library.Infrastructure.Services.Dashboard
{
    public interface IDashboardServire
    {
        int NumberOfSubscribers();
        int NumberOfCopies();
        IEnumerable<BookViewModel> GetTopBooks();

        IEnumerable<BookViewModel> RecentBooks();
        List<ChartItemViewModel> GetChartData(DateTime? startDate, DateTime? endDate);
        List<ChartItemViewModel> GetSubscribersPerCity();
    }
}
