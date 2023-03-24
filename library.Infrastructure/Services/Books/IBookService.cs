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
        void Create(BookFormVM bookFormVM);
        void Update(BookFormVM bookFormVM);
        Book GetBook(int id);
        BookFormVM EditBookGet(Book book);
        Book IsBookExists(BookFormVM book);


    }
}
