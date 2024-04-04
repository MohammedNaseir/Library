using library.Core.Constants;
using library.Data.Models;
using library.Infrastructure.Services.Books;
using library.Infrastructure.Services.Rentals;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace library.Web.Controllers
{
    [Authorize(Roles = AppRoles.Reception)]

    public class RentalsController : Controller
    {
        private readonly IDataProtector _dataProtector;
        private readonly IRentalService _rentalService;
        public RentalsController(
            IRentalService rentalService,
            IDataProtectionProvider dataProtector)
        {
            _rentalService = rentalService;
            _dataProtector = dataProtector.CreateProtector("MySecureKey");
        }
        public IActionResult Create(string sKey)
        {
            var subscriberId = int.Parse(_dataProtector.Unprotect(sKey));
            var subscriber = _rentalService.GetSubscriber(subscriberId);

            if (subscriber == null)
                return NotFound();

            var(errorMessage,maxAllowedCopies) = ValidateSubscriber(subscriber);
            
            if(!string.IsNullOrEmpty(errorMessage))
                return View("NotAllowedRental", errorMessage);

            var viewModel = new RentalFormViewModel
            {
                SubscriberKey = sKey,
                MaxAllowedCopies = maxAllowedCopies
            };
            return View(viewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(RentalFormViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var subscriberId = int.Parse(_dataProtector.Unprotect(model.SubscriberKey));
            var subscriber = _rentalService.GetSubscriber(subscriberId);

            if (subscriber == null)
                return NotFound();

            var (errorMessage, maxAllowedCopies) = ValidateSubscriber(subscriber);

            if (!string.IsNullOrEmpty(errorMessage))
                return View("NotAllowedRental", errorMessage);
            var selectedCopies = _rentalService.GetListOfCopies(model);

            var currentSubsciberRental = _rentalService.GetBookIdInRental(subscriberId);

            List<RentalCopy> copies = new();
           
            foreach (var copy in selectedCopies)
            {
                if (!copy.IsAvailableForRental || !copy.Book!.IsAvailableForRental)
                    return View("NotAllowedRental",Errors.NotAvailableForRental);
                if (!copy.Rentals.Any(c => !c.ReturnDate.HasValue))
                    return View("NotAllowedRental",Errors.CopyIsInRental);
                if(currentSubsciberRental.Any(bookId => bookId == copy.BookId))
                    return View("NotAllowedRental",$"This Subscriber already has a copy for {copy.Book.Title} ");
                copies.Add(new RentalCopy { BookCopyId = copy.Id });
            }
            
            Rental rental = new()
            {
                RentalCopies = copies,
                CreatedById = User.FindFirst(ClaimTypes.NameIdentifier)!.Value
            };

            subscriber.Rentals.Add(rental);
            _rentalService.SaveChanges();
            return Ok();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult GetCopyDetails(SearchFormViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest();
            var copy = _rentalService.GetBookCopy(model.Value);
            if (copy == null)
                return NotFound(Errors.InvalidSerialNumber);
            if (!copy.IsAvailableForRental || !copy.Book!.IsAvailableForRental)
                return BadRequest(Errors.NotAvailableForRental);

            //Todo:check that copy is not rental by another user
            var copyInRental = _rentalService.CopyInRental(copy);
            if (copyInRental)
                return BadRequest(Errors.CopyIsInRental);

            var viewModel = _rentalService.MapToBookCopyViewModel(copy);
            return PartialView("_CopyDetails", viewModel);
        }

        private(string errorMessage,int? MaxAllowedCopies) ValidateSubscriber(Subscriber subscriber)
        {
            if (subscriber.IsBlackListed)
                return (Errors.BlackListedSubscriber, null);
            if (subscriber.Subscriptions.Last().EndDate < DateTime.Today.AddDays((int)RentalConfiguration.RentalDuration))
                //return View("NotAllowedRental", Errors.InactiveSubscriber);
                return (Errors.InactiveSubscriber, null);
            var currentRental = subscriber.Rentals.SelectMany(c => c.RentalCopies).Count(s => s.ReturnDate == null);

            var availableAllowedRental = (int)RentalConfiguration.MaxAllowedCopies - currentRental;

            if (availableAllowedRental.Equals(0))
                //return View("NotAllowedRental", Errors.MaxCopiesReached);
                return (Errors.MaxCopiesReached, null);
            return (string.Empty, availableAllowedRental);

        }
    }
}