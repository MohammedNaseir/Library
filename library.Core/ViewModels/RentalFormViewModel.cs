using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace library.Core.ViewModels
{
    public class RentalFormViewModel
    {
        public int? Id { get; set; }
        public string SubscriberKey { get; set; } = null!;
        public IList<int> SelectedCopies { get; set; } = new List<int>();
        public IEnumerable<BookCopyViewModel> CurrentCopies { get; set; }=new List<BookCopyViewModel>();
        public int? MaxAllowedCopies { get; set; } 

    }
}
