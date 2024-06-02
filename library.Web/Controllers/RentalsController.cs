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

            var (errorMessage, maxAllowedCopies) = ValidateSubscriber(subscriber);

            if (!string.IsNullOrEmpty(errorMessage))
                return View("NotAllowedRental", errorMessage);

            var viewModel = new RentalFormViewModel
            {
                SubscriberKey = sKey,
                MaxAllowedCopies = maxAllowedCopies
            };
            return View("Form",viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(RentalFormViewModel model)
        {
            if (!ModelState.IsValid)
                return View("Form",model);

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
                    return View("NotAllowedRental", Errors.NotAvailableForRental);
                if (copy.Rentals.Any(c => !c.ReturnDate.HasValue))
                    return View("NotAllowedRental", Errors.CopyIsInRental);
                if (currentSubsciberRental.Any(bookId => bookId == copy.BookId))
                    return View("NotAllowedRental", $"This Subscriber already has a copy for {copy.Book.Title} ");
                copies.Add(new RentalCopy { BookCopyId = copy.Id });
            }

            Rental rental = new()
            {
                RentalCopies = copies,
                CreatedById = User.FindFirst(ClaimTypes.NameIdentifier)!.Value
            };

            subscriber.Rentals.Add(rental);
            _rentalService.SaveChanges();
            return RedirectToAction(nameof(Details),new {id=rental.Id});
        }

        public IActionResult Edit(int id)
        {
            var rental = _rentalService.GetRentalDetails(id);
            // if i want to allow edit only in day  CreatedOn
            if (rental == null /*||*//*rental.CreatedOn.Date != DateTime.Today*/)
                return NotFound();
            
            var subscriber = _rentalService.GetSubscriber(rental.SubscriberId);

            var (errorMessage, maxAllowedCopies) = ValidateSubscriber(subscriber!,rental.Id);

            if (!string.IsNullOrEmpty(errorMessage))
                return View("NotAllowedRental", errorMessage);            
            var currentcopiesids = _rentalService.GetRentalCopyIds(rental);
            var currentCopies = _rentalService.GetCurrentCopies(currentcopiesids);
            var viewModel = new RentalFormViewModel
            {
                SubscriberKey = _dataProtector.Protect(subscriber.Id.ToString()),
                MaxAllowedCopies = maxAllowedCopies,
                CurrentCopies= currentCopies
            };
            return View("Form",viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(RentalFormViewModel model)
        { 
            if (!ModelState.IsValid)
                return View("Form", model);
            var rental = _rentalService.GetRentalInclueRentlCopies((int)model.Id!);
            if (rental is null/* || rental.CreatedOn.Date != DateTime.Now*/)
                return NotFound();
            
            var subscriberId = int.Parse(_dataProtector.Unprotect(model.SubscriberKey));
            var subscriber = _rentalService.GetSubscriber(subscriberId);

            var (errorMessage, maxAllowedCopies) = ValidateSubscriber(subscriber!,model.Id);

            if (!string.IsNullOrEmpty(errorMessage))
                return View("NotAllowedRental", errorMessage);
            var selectedCopies = _rentalService.GetListOfCopies(model);

            var currentSubsciberRental = _rentalService.GetBookIdInRentalEdit(subscriberId,(int)model.Id);

            List<RentalCopy> copies = new();

            foreach (var copy in selectedCopies)
            {
                if (!copy.IsAvailableForRental || !copy.Book!.IsAvailableForRental)
                    return View("NotAllowedRental", Errors.NotAvailableForRental);
                
                if (copy.Rentals.Any(c => !c.ReturnDate.HasValue && c.RentalId !=(int)model.Id))
                    return View("NotAllowedRental", Errors.CopyIsInRental);
                
                if (currentSubsciberRental.Any(bookId => bookId == copy.BookId))
                    return View("NotAllowedRental", $"This Subscriber already has a copy for {copy.Book.Title} ");
                copies.Add(new RentalCopy { BookCopyId = copy.Id });
            }
            rental.RentalCopies =copies;
            rental.LastUpdatedById = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
            rental.LastUpdatedOn = DateTime.Now;
            _rentalService.SaveChanges();
            return RedirectToAction(nameof(Details), new {id=rental.Id});
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult MakeAsDelete(int id)
        {
            var rental = _rentalService.GetRental(id);
            
            if (rental is null || rental.CreatedOn.Date != DateTime.Today)
                return NotFound();
            
            rental.IsDeleted = true;
            rental.LastUpdatedOn = DateTime.Now;
            rental.LastUpdatedById = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
            
            _rentalService.SaveChanges();
            var copyCount = _rentalService.GetRentalCopiesCount(id);
            return Ok(copyCount);
        }

        public IActionResult Details(int id)
        {
            var rental = _rentalService.GetRentalDetails(id);
            if(rental ==null)
                return NotFound();
            var viewModel = _rentalService.MapToRentalViewModel(rental);
            return View(viewModel);
        }
        
        public IActionResult Return(int id)
        {
            var rental = _rentalService.GetRentalsReturnDetails(id);

            if (rental == null && rental!.CreatedOn.Date == DateTime.Now)
                return NotFound();

            var subscriber = _rentalService.GetSubscriberWithSubsription(rental.SubscriberId);

            var viewModel = new RentalReturnFormViewModel
            {
                Id = id,
                Copies = _rentalService.MapToIListBookCopyViewModel(rental.RentalCopies.Where(c => !c.ReturnDate.HasValue).ToList()),
                SelectedCopies = rental.RentalCopies.Where(c => !c.ReturnDate.HasValue).Select(c => new ReturnCopyViewModel { Id = c.BookCopyId }).ToList(),
                AllowExtend = !subscriber!.IsBlackListed
                              &&subscriber.Subscriptions.Last().EndDate >=rental.StartDate.AddDays((int) RentalConfiguration.MaxRentalDuration)
                              && rental.StartDate.AddDays((int) RentalConfiguration.RentalDuration) >= DateTime.Today
            };
            return View(viewModel);
        }
       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Return(RentalReturnFormViewModel model)
        {
            var rental = _rentalService.GetRentalsReturnDetails(model.Id);

            if (rental == null && rental!.CreatedOn.Date == DateTime.Now)
                return NotFound();

            if (!ModelState.IsValid)
            {
                model.Copies = _rentalService.MapToIListBookCopyViewModel(rental.RentalCopies.Where(c => !c.ReturnDate.HasValue).ToList());
                return View(model);
            }
            var subscriber = _rentalService.GetSubscriberWithSubsription(rental.SubscriberId);
            
            if(model.SelectedCopies.Any(x=>x.IsReturned.HasValue && !x.IsReturned.Value))
            {
                if (subscriber.IsBlackListed)
                {
                    model.Copies = _rentalService.MapToIListBookCopyViewModel(rental.RentalCopies.Where(c => !c.ReturnDate.HasValue).ToList());
                    ModelState.AddModelError("", Errors.RentalNotAllowedForBlacklisted);
                    return View(model);
                }
                if (subscriber.Subscriptions.Last().EndDate < rental.StartDate.AddDays((int)RentalConfiguration.MaxRentalDuration))
                {
                    model.Copies = _rentalService.MapToIListBookCopyViewModel(rental.RentalCopies.Where(c => !c.ReturnDate.HasValue).ToList());
                    ModelState.AddModelError("", Errors.RentalNotAllowedForInactive);
                    return View(model);
                }
                if (rental.StartDate.AddDays((int)RentalConfiguration.RentalDuration) < DateTime.Today)
                {
                    model.Copies = _rentalService.MapToIListBookCopyViewModel(rental.RentalCopies.Where(c=>!c.ReturnDate.HasValue).ToList());
                    ModelState.AddModelError("", Errors.ExtendNotAllowed);
                    return View(model);
                }
            }

            var isUpdated = false;

            foreach (var copy in model.SelectedCopies)
            {
                if (!copy.IsReturned.HasValue) continue;

                var currentCopy = rental.RentalCopies.SingleOrDefault(c => c.BookCopyId == copy.Id);

                if (currentCopy is null) continue;

                if (copy.IsReturned.HasValue && copy.IsReturned.Value)
                {
                    if (currentCopy.ReturnDate.HasValue) continue;

                    currentCopy.ReturnDate = DateTime.Now;
                    isUpdated = true;
                }

                if (copy.IsReturned.HasValue && !copy.IsReturned.Value)
                {
                    if (currentCopy.ExtendedOn.HasValue) continue;

                    currentCopy.ExtendedOn = DateTime.Now;
                    currentCopy.EndDate = currentCopy.RentalDate.AddDays((int)RentalConfiguration.MaxRentalDuration);
                    isUpdated = true;
                }
            }

            if (isUpdated)
            {
                rental.LastUpdatedOn = DateTime.Now;
                rental.LastUpdatedById = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
                rental.PenaltyPaid = model.PenaltyPaid;

                _rentalService.SaveChanges();
            }

            return RedirectToAction(nameof(Details), new { id = rental.Id });

        }
        public IActionResult RentalHistory(int id)
        {
            var copyHistory = _rentalService.GetHistory(id);
            var viewModel = _rentalService.MapToCopyHistoryViewModel(copyHistory);
            return View(viewModel);
        }
        private (string errorMessage,int? MaxAllowedCopies) ValidateSubscriber(Subscriber subscriber,int? retalId=null)
        {
            if (subscriber.IsBlackListed)
                return (Errors.BlackListedSubscriber, null);
            if (subscriber.Subscriptions.Last().EndDate < DateTime.Today.AddDays((int)RentalConfiguration.RentalDuration))
                //return View("NotAllowedRental", Errors.InactiveSubscriber);
                return (Errors.InactiveSubscriber, null);
            var currentRental = subscriber.Rentals
                .Where(r=>retalId==null|| r.Id!=retalId)
                .SelectMany(c => c.RentalCopies)
                .Count(s => s.ReturnDate == null);

            var availableAllowedRental = (int)RentalConfiguration.MaxAllowedCopies - currentRental;

            if (availableAllowedRental.Equals(0))
                //return View("NotAllowedRental", Errors.MaxCopiesReached);
                return (Errors.MaxCopiesReached, null);
            return (string.Empty, availableAllowedRental);
        
        }
    }
}