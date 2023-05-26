using library.Core.Constants;
using library.Data.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace library.Core.ViewModels
{
    public class BookCopyFormViewModel
    {
        public int Id { get; set; }
        public int BookId { get; set; }
        public bool IsAvailableForRental { get; set; }
        [Range(1,1000,ErrorMessage =Errors.invalidEditionNumber)]
        public int EditionNumber { get; set; }
        [Display(Name = "Is available for rental?")]
        public bool ShowRentalInput { get; set; }
    }
}
