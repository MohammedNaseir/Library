using library.Data.Models;
using library.Infrastructure.Services.Copies;
using Microsoft.AspNetCore.Mvc;

namespace library.Web.Controllers
{
    [Authorize(Roles = AppRoles.Archive)]
    public class BookCopeisController : Controller
    {
        private readonly ICopyService _copyService;
        public BookCopeisController(ICopyService copyService)
        {
            _copyService = copyService;
        }

        
        [AjaxOnly]
        public IActionResult Create(int bookId)
        {
            var book =_copyService.GetBook(bookId);
            if(book == null)
                return NotFound();

            var viewModel = new BookCopyFormViewModel 
            { 
                BookId = bookId,
                
                IsAvailableForRental = book.IsAvailableForRental
            };
            return PartialView("Form",viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(BookCopyFormViewModel model)
        {
            if(!ModelState.IsValid) 
                return BadRequest(); 
            var book = _copyService.GetBook(model.BookId);
            if (book == null)
                return NotFound();
              
            BookCopy copy = new()
            {
                EditionNumber = model.EditionNumber,
                IsAvailableForRental = book.IsAvailableForRental ? model.IsAvailableForRental :false
                ,CreatedById = User.FindFirst(ClaimTypes.NameIdentifier)!.Value
            };
            
            book.Copies.Add(copy);
            _copyService.SaveChanges();
            return PartialView("_BookCopyRow", _copyService.Create(copy));
        }

        [AjaxOnly]
        public IActionResult Edit(int id)
        {
            var copy = _copyService.Edit(id);
            if (copy == null) return NotFound();
            return PartialView("Form", copy);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(BookCopyFormViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest();
            var claimvalue = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
            var copy = _copyService.UpdatePost(model,claimvalue);
            if (copy == null)
                return NotFound();
            return PartialView("_BookCopyRow",copy);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ToggleStatus(int id)
        {
            var copy = _copyService.GetCopy(id);
            if (copy is null)
                return NotFound();
            copy.IsDeleted = !copy.IsDeleted;
            copy.LastUpdatedOn = DateTime.Now;
            copy.LastUpdatedById = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
            
            _copyService.SaveChanges();
            return Ok(copy.LastUpdatedOn.ToString());
        }
    }
}
