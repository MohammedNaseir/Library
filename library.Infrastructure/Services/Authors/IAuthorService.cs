using library.Core.ViewModels;
using library.Data.Models;

namespace library.Infrastructure.Services.Authors

{
    public interface IAuthorService
    {
        Task<AuthorViewModel> Create(AuthorFormVM author);
        Task<AuthorViewModel> Update(AuthorFormVM model);
        Task<AuthorFormVM> Get(int Id);
        Task<IEnumerable<AuthorViewModel>> GetAuthorList();
        Author GetAuthor(int Id);
        Author IsAuthorExists(AuthorFormVM author);
    }
}
