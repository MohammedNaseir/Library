using library.Core.ViewModels;
using library.Data.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace library.Infrastructure.Services.Books
{
    public interface IBookService
    {
        IEnumerable<SelectListItem> GetAuthors();
        IEnumerable<SelectListItem> GetCategories();
        IQueryable<Book> GetBooks();
        int Create(BookFormVM bookFormVM, string claim);
        void Update(BookFormVM bookFormVM);
        Book GetBook(int id);
        BookFormVM EditBookGet(Book book);
        IEnumerable<BookViewModel> BookMap(List<Book> Books);
        Book IsBookExists(BookFormVM book);
        BookViewModel GetBookViewModel(int id);
        void SaveChanges();
    }
}