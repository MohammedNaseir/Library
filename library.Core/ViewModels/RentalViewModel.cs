﻿using library.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace library.Core.ViewModels
{
    public class RentalViewModel
    {
        public int Id { get; set; }
        public SubscriberViewModel? Subscriber { get; set; }
        public DateTime StartDate { get; set; } = DateTime.Today;
        public DateTime CreatedOn { get; set; } 
        public bool PenaltyPaid { get; set; }
        public IEnumerable<RentalCopyViewModel> RentalCopies { get; set; } = new List<RentalCopyViewModel>();
        public int TotalDelayInDays
        {
            get
            {
                return RentalCopies.Sum(x => x.DelayInDays);
            }
        }      
        public int NumberOfCopies
        {
            get
            {
                return RentalCopies.Count();
            }
        }

    }
}
