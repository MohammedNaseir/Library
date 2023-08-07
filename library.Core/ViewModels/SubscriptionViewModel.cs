using library.Core.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace library.Core.ViewModels
{
    public class SubscriptionViewModel
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime CreatedOn { get; set; }
        public string Status
        {
            get
            {
                return DateTime.Today > EndDate ? SubscriptionStatus.Expired : DateTime.Today < StartDate ? string.Empty : SubscriptionStatus.Active;
            }
        }
    }
}
