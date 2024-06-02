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
        IEnumerable<RentalCopy> GetHistory(int id);
        IEnumerable<CopyHistoryViewModel> MapToCopyHistoryViewModel(IEnumerable<RentalCopy> copies);
        IList<RentalCopyViewModel> MapToIListBookCopyViewModel(List<RentalCopy> rentalCopies);
        Subscriber GetSubscriberWithSubsription(int subscriberId);
        Rental GetRentalsReturnDetails(int id);
        bool CopyInRental(BookCopy copy);
        List<int> GetBookIdInRental(int subscriberId);
        List<int> GetBookIdInRentalEdit(int subscriberId, int id);
        List<BookCopy> GetListOfCopies(RentalFormViewModel model);
        BookCopy GetBookCopy(string sKey);
        Subscriber GetSubscriber(int subscriberId);
        Rental GetRental(int id);
        BookCopyViewModel MapToBookCopyViewModel(BookCopy BookCopy);
        int GetRentalCopiesCount(int id);
        Rental GetRentalDetails(int id);
        RentalViewModel MapToRentalViewModel(Rental rental);
        List<int> GetRentalCopyIds(Rental rental);
        IEnumerable<BookCopyViewModel> GetCurrentCopies(List<int> copyIds);
        Rental GetRentalInclueRentlCopies(int id);
        void SaveChanges();
    }
}