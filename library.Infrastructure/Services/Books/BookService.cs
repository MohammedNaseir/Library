using AutoMapper;
using library.Core.ViewModels;
using library.Data;
using library.Data.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace library.Infrastructure.Services.Books
{
    public class BookService : IBookService
    {
        private readonly IMapper _mapper;
        private readonly libraryDbContext _db;
       
        
        public BookService(libraryDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        public void Create(BookFormVM bookFormVM)
        {
            var book = _mapper.Map<Book>(bookFormVM);
            // to map selected catefories
            foreach (var category in bookFormVM.SelectedCategories)
            {
                book.Categories.Add(new BookCategory { CategoryId = category });
            }
            _db.Add(book);
            _db.SaveChanges();
        }
        
        
        public void Update(BookFormVM model)
        {
            var book = GetBook(model.Id);
            book = _mapper.Map(model,book);          
            // to map selected catefories        
            //book = _mapper.Map(bookFormVM, book);
            book.LastUpdatedOn = DateTime.Now;
            foreach (var category in model.SelectedCategories)
            {
                book.Categories.Add(new BookCategory { CategoryId = category });
            }
            _db.SaveChanges();
        }
        public Book GetBook(int id)
        {
            return _db.Books.Include(x=>x.Categories).SingleOrDefault(x => x.Id == id);
        }
        public BookFormVM EditBookGet(Book book)
        { 
           return _mapper.Map<BookFormVM>(book);
        }
        public IEnumerable<SelectListItem> GetAuthors()
        {
            var authors = _db.Authors.Where(a => !a.IsDeleted)
                .OrderBy(a => a.Name)
                .ToList();
            return _mapper.Map<IEnumerable<SelectListItem>>(authors);
        }

    

        public IEnumerable<SelectListItem> GetCategories()
        {
            var categories = _db.Categories.Where(a => !a.IsDeleted)
                 .OrderBy(a => a.Name)
                 .ToList();
            return _mapper.Map<IEnumerable<SelectListItem>>(categories);
        }

        public Book IsBookExists(BookFormVM book)
        {
            return _db.Books.SingleOrDefault(x => x.Title == book.Title
            && x.AuthorId ==book.AuthorId);
        }
    }
}
