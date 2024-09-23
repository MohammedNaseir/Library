using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace library.Core.ViewModels
{
    public class DashboardViewModel
    {
        public int NumberOfCopies { get; set; }
        public int NumberOfSubscribers { get; set; }
        public IEnumerable<BookViewModel> LastAddedBooks { get; set; } = new List<BookViewModel>();
        public IEnumerable<BookViewModel> TopBooks { get; set; } = new List<BookViewModel>();
    }

}
