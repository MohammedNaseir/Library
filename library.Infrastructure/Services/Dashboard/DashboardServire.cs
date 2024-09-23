using AutoMapper;
using library.Core.ViewModels;
using library.Data;
using library.Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.AspNetCore.Hosting.Internal.HostingApplication;

namespace library.Infrastructure.Services.Dashboard
{
    public class DashboardServire : IDashboardServire
    {
        private readonly IMapper _mapper;
        private readonly libraryDbContext _db;
        public DashboardServire(libraryDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }
        public int NumberOfCopies()
        {
            return _db.Books.Count(c => !c.IsDeleted);
        }
        public int NumberOfSubscribers()
        {
            return _db.Subscribers.Count(c => !c.IsDeleted);
        }
        public IEnumerable<BookViewModel> RecentBooks()
        {

            var Books = _db.Books.Where(x => !x.IsDeleted)
                                .Include(a => a.Author)
                                .OrderByDescending(x => x.Id)
                                .Take(8)
                                .ToList();
            return _mapper.Map<IEnumerable<BookViewModel>>(Books);
        }
        public IEnumerable<BookViewModel> GetTopBooks()
        {
            return _db.RentalCopies
                .Include(x => x.BookCopy)
                .ThenInclude(x => x!.Book)
                .ThenInclude(x => x!.Author)
                .GroupBy(c => new
                {
                    c.BookCopy!.BookId,
                    c.BookCopy!.Book!.Title,
                    c.BookCopy!.Book!.ImageThumbnailUrl,
                    AuthorName = c.BookCopy!.Book!.Author!.Name
                }).Select(b => new
                {
                    b.Key.BookId,
                    b.Key.Title,
                    b.Key.ImageThumbnailUrl,
                    b.Key.AuthorName,
                    Count = b.Count()
                })
                .OrderByDescending(x => x.Count)
                .Take(6)
                .Select(x => new BookViewModel
                {
                    Id = x.BookId,
                    Title = x.Title,
                    ImageThumbnailUrl = x.ImageThumbnailUrl,
                    Author = x.AuthorName
                }).ToList();



        }

        public List<ChartItemViewModel> GetChartData(DateTime? startDate, DateTime? endDate)
        {
            return _db.RentalCopies
                .Where(x => x.RentalDate >= startDate && x.RentalDate <= endDate)
                .GroupBy(x => new { Date = x.RentalDate })
                .Select(g => new ChartItemViewModel
                {
                    Label = g.Key.Date.ToString("d MMM"),
                    Value = g.Count().ToString()
                }).ToList();
        }
        public List<ChartItemViewModel> GetSubscribersPerCity()
        {
            return _db.Subscribers
               .Include(s => s.Governorate)
               .Where(s => !s.IsDeleted)
               .GroupBy(s => new { GovernorateName = s.Governorate!.Name })
               .Select(g => new ChartItemViewModel
               {
                   Label = g.Key.GovernorateName,
                   Value = g.Count().ToString()
               })
               .ToList();
        }
    }
}