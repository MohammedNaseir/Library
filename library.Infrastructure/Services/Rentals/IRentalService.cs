using library.Core.ViewModels;
using library.Data.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace library.Infrastructure.Services.Rentals
{
    public interface IRentalService
    {
        //IEnumerable<SelectListItem> GetAuthors();
        //IEnumerable<SelectListItem> GetCategories();
        //IQueryable<Book> GetBooks();
        //int Create(BookFormVM bookFormVM, string claim);
        //void Update(BookFormVM bookFormVM);
        bool CopyInRental(BookCopy copy);
        List<int> GetBookIdInRental(int subscriberId);
        List<BookCopy> GetListOfCopies(RentalFormViewModel model);
        BookCopy GetBookCopy(string sKey);
        Subscriber GetSubscriber(int subscriberId);

        //BookFormVM EditBookGet(Book book);
        //IEnumerable<BookViewModel> BookMap(List<Book> Books);
        //Book IsBookExists(BookFormVM book);
        //BookViewModel GetBookViewModel(int id);
        BookCopyViewModel MapToBookCopyViewModel(BookCopy BookCopy);


		void SaveChanges();
    }
}