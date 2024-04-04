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
using CloudinaryDotNet;
using Microsoft.Extensions.Options;
using library.Infrastructure.Settings;

namespace library.Infrastructure.Services.Books
{
    public class BookService : IBookService
    {
        private readonly IMapper _mapper;
        private readonly libraryDbContext _db;
        private readonly Cloudinary _cloudinary;

        public BookService(libraryDbContext db, IMapper mapper, 
           IOptions<CloudinarySettings> cloudinary)
        {
            _db = db;
            _mapper = mapper;
            Account account = new()
            {
                Cloud = cloudinary.Value.Cloud,
                ApiKey = cloudinary.Value.ApiKey,
                ApiSecret = cloudinary.Value.ApiSecret
            };

            _cloudinary = new Cloudinary(account);
        }

        public int Create(BookFormVM bookFormVM,string claim)
        {
            var book = _mapper.Map<Book>(bookFormVM);

            // to map selected Categories
            foreach (var category in bookFormVM.SelectedCategories)
            {
                book.Categories.Add(new BookCategory { CategoryId = category });
            }
            book.CreatedById = claim;
            _db.Add(book);
            _db.SaveChanges();
            return (book.Id);
        }
        
        
        public void Update(BookFormVM model)
        {
            var book = GetBook(model.Id);
            book = _mapper.Map(model,book);          
            // to map selected catefories        
            //book = _mapper.Map(bookFormVM, book);
            book.LastUpdatedOn = DateTime.Now;
            //book.ImageUrl=
            foreach (var category in model.SelectedCategories)
            {
                book.Categories.Add(new BookCategory { CategoryId = category });
            }
            // to change availabity when changeing avalible in Book automaticlly
            if (!model.IsAvailableForRental)
                foreach (var copy in book.Copies)
                    copy.IsAvailableForRental = false;
            _db.SaveChanges();
        }
        public Book GetBook(int id)
        {
            return _db.Books
                .Include(x => x.Categories)
                .Include(x => x.Copies)
                .SingleOrDefault(x => x.Id == id);
              
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
        public BookViewModel GetBookViewModel(int id)
        {
            var book = _db.Books
                .Include(x => x.Author)
                .Include(b => b.Copies)
                .Include(x=>x.Categories)
                .ThenInclude(x=>x.Category)
                .SingleOrDefault(x => x.Id == id);
            var bookVM=_mapper.Map<BookViewModel>(book);
            return bookVM;
        }
        public IQueryable<Book> GetBooks()
        {
            return _db.Books
                .Include(b => b.Author)
                .Include(b => b.Categories)
                .ThenInclude(c => c.Category);
        }
        public void SaveChanges()
        {
            _db.SaveChanges();
        }
        public IEnumerable<BookViewModel> BookMap(List<Book> Books)
        {
            return _mapper.Map<IEnumerable<BookViewModel>>(Books);
        }
    }
}
