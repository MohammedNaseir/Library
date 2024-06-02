using library.Core.Constants;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UoN.ExpressiveAnnotations.NetCore.Attributes;

namespace library.Core.ViewModels
{
    public class RentalReturnFormViewModel
    {

        public int Id { get; set; }

        [Display(Name = "Penalty Paid?")]
        [AssertThat("(TotalDelayInDays == 0 && PenaltyPaid == false) || PenaltyPaid == true", ErrorMessage = Errors.PenaltyShouldBePaid)]
        public bool PenaltyPaid { get; set; }

        public IList<RentalCopyViewModel> Copies { get; set; } = new List<RentalCopyViewModel>();

        public List<ReturnCopyViewModel> SelectedCopies { get; set; } = new();

        public bool AllowExtend { get; set; }

        public int TotalDelayInDays
        {
            get
            {
                return Copies.Sum(c => c.DelayInDays);
            }
        }

    }
}
