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
        public IEnumerable<CopyHistoryViewModel> MapToCopyHistoryViewModel(IEnumerable<RentalCopy> copies)
        {
            return _mapper.Map<IEnumerable<CopyHistoryViewModel>>( copies );
        }
        public IEnumerable<RentalCopy> GetHistory(int id)
        {
            return _db.RentalCopies
                      .Include(c => c.Rental)
                      .ThenInclude(s => s!.Subscriber)
                      .Where(x => x.BookCopyId == id)
                      .OrderByDescending(c => c.RentalDate)
                      .ToList();
        }
        public Subscriber GetSubscriberWithSubsription(int subscriberId)
        {
            return _db.Subscribers.Include(s => s.Subscriptions).SingleOrDefault(x => x.Id == subscriberId)!;
        }
        public IList<RentalCopyViewModel> MapToIListBookCopyViewModel(List<RentalCopy> rentalCopies)
        {
            return _mapper.Map<IList<RentalCopyViewModel>>(rentalCopies);
        }
        public Rental GetRentalsReturnDetails(int id)
        {
            return _db.Rentals
                   .Include(r => r.RentalCopies)
                   .ThenInclude(x => x.BookCopy)
                   .ThenInclude(x => x!.Book)
                   .SingleOrDefault(r => r.Id == id)!;
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
        public List<int> GetBookIdInRentalEdit(int subscriberId,int id)
        {
            return _db.Rentals
                .Include(c => c.RentalCopies)
                .ThenInclude(c => c.BookCopy)
                .Where(r => r.SubscriberId == subscriberId && r.Id !=id)
                .SelectMany(r => r.RentalCopies)
                .Where(c => !c.ReturnDate.HasValue)
                .Select(c => c.BookCopy!.BookId)
                .ToList();
        }
        public bool CopyInRental(BookCopy copy)
        {
            return _db.RentalCopies.Any(x => x.BookCopyId == copy.Id && !x.ReturnDate.HasValue);
        }
        public Rental GetRental(int id)
        {
           var rental = _db.Rentals.Find(id);
           return rental!;
        }
      
        public Rental GetRentalDetails(int id)
        {
            var rental = _db.Rentals
                .Include(r => r.RentalCopies)
                .ThenInclude(c => c.BookCopy)
                .ThenInclude(c => c!.Book)
                .SingleOrDefault(r => r.Id == id);
            return rental!;
        }
        public List<int> GetRentalCopyIds(Rental rental)
        {
            return rental.RentalCopies.Select(x => x.BookCopyId).ToList();
        }
        public IEnumerable<BookCopyViewModel> GetCurrentCopies(List<int> copyIds)
        {
            var copies = _db.BookCopies
                  .Where(c=>copyIds.Contains(c.Id))
                  .Include(c=>c.Book)
                  .ToList();
            return _mapper.Map<IEnumerable<BookCopyViewModel>>(copies);
        }
        public RentalViewModel MapToRentalViewModel(Rental rental)
        {
            return _mapper.Map<RentalViewModel>(rental);
        }
        public int GetRentalCopiesCount(int id)
        {
            return _db.RentalCopies.Count(r => r.RentalId == id);
        }

        public Rental GetRentalInclueRentlCopies(int id)
        {
            return _db.Rentals.Include
                (x => x.RentalCopies)
                .SingleOrDefault(r => r.Id == id)!;
        }
    }
}
