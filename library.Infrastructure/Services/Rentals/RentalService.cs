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

namespace library.Infrastructure.Services.Rentals
{
    public class RentalService : IRentalService
    {
        private readonly IMapper _mapper;
        private readonly libraryDbContext _db;
		public RentalService(libraryDbContext db, IMapper mapper )
        {
            _db = db;
            _mapper = mapper;
  
        }

        public Subscriber GetSubscriber(int subscriberId)
        {
            var subscriber = _db.Subscribers
                .Include(c => c.Subscriptions)
                .Include(c=>c.Rentals)
                .ThenInclude(c=>c.RentalCopies)
                .SingleOrDefault(c => c.Id == subscriberId);

            return subscriber!;
        }
        public BookCopy GetBookCopy(string sKey)
		{
			var copy = _db.BookCopies
                .Include(c=>c.Book)
                .SingleOrDefault(c => c.SerialNumber.ToString() == sKey && !c.IsDeleted && !c.Book!.IsDeleted);
           
            return copy;
		}
		public void SaveChanges()
		{
			_db.SaveChanges();
		}
		public BookCopyViewModel MapToBookCopyViewModel(BookCopy BookCopy)
		{
			return _mapper.Map<BookCopyViewModel>(BookCopy);
		}
        public List<BookCopy> GetListOfCopies(RentalFormViewModel model)
        {
            return _db.BookCopies
                .Include(c=>c.Book)
                .Include(c=>c.Rentals)
                .Where(c=>model.SelectedCopies.Contains(c.SerialNumber))
                .ToList();
        }
        public List<int> GetBookIdInRental(int subscriberId)
        {
            return _db.Rentals
                .Include(c => c.RentalCopies)
                .ThenInclude(c => c.BookCopy)
                .Where(r => r.SubscriberId == subscriberId)
                .SelectMany(r => r.RentalCopies)
                .Where(c => !c.ReturnDate.HasValue)
                .Select(c => c.BookCopy!.BookId)
                .ToList();
        }
        public bool CopyInRental(BookCopy copy)
        {
            return _db.RentalCopies.Any(x => x.BookCopyId == copy.Id && !x.ReturnDate.HasValue);
        }

    }
}
