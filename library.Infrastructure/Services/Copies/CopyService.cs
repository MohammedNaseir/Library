using AutoMapper;
using library.Core.ViewModels;
using library.Data;
using library.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace library.Infrastructure.Services.Copies
{
    public class CopyService:ICopyService
    {
        private readonly IMapper _mapper;
        private readonly libraryDbContext _db;

        public CopyService(libraryDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        public BookCopy GetCopy(int Id)
        {
            return _db.BookCopies.FirstOrDefault(c => c.Id == Id);
        }
        public void SaveChanges()
        {
            _db.SaveChanges();
        }
        public Book GetBook(int Id)
        {
            return _db.Books.Find(Id); ;
        }
        public BookCopyViewModel Create(BookCopy bookCopy)
        {
            
            var viewModel = _mapper.Map<BookCopyViewModel>(bookCopy);
            return viewModel;
        }
        public BookCopyFormViewModel Edit(int id)
        {
            var copy = _db.BookCopies.Include(c=> c.Book).SingleOrDefault(c=>c.Id == id);
            var viewModel = _mapper.Map<BookCopyFormViewModel>(copy);
            viewModel.ShowRentalInput = copy.Book!.IsAvailableForRental;
            return viewModel;       
        }
        public BookCopyViewModel UpdatePost(BookCopyFormViewModel model)
        {
            var copy = _db.BookCopies.Include(c => c.Book).SingleOrDefault(c => c.Id == model.Id);
            copy.IsAvailableForRental = copy.Book!.IsAvailableForRental && model.IsAvailableForRental;
            copy.EditionNumber = model.EditionNumber;
            copy.LastUpdatedOn=DateTime.Now;
            _db.SaveChanges();
            return _mapper.Map<BookCopyViewModel>(copy);

        }
    }
}
