using library.Core.ViewModels;
using library.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace library.Infrastructure.Services.Copies
{
    public interface ICopyService
    {
        public BookCopy GetCopy(int Id);
        public void SaveChanges();
        public Book GetBook(int Id);
        public BookCopyViewModel Create(BookCopy bookCopy);
        public BookCopyFormViewModel Edit(int id);
        public BookCopyViewModel UpdatePost(BookCopyFormViewModel model, string claimvalue);
    }
}
