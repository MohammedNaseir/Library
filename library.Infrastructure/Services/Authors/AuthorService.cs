using AutoMapper;
using library.Core.Exceptions;
using library.Core.ViewModels;
using library.Data;
using library.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace library.Infrastructure.Services.Authors
{
    public class AuthorService : IAuthorService
    {
        private readonly IMapper _mapper;
        private readonly libraryDbContext _db;
        public AuthorService(libraryDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        public async Task<AuthorViewModel> Create(AuthorFormVM author)
        {

            var Author = _mapper.Map<Author>(author);
            
            await _db.Authors.AddAsync(Author);
            await _db.SaveChangesAsync();
            var viewModel = _mapper.Map<AuthorViewModel>(Author);
            return viewModel;
        }

        public Author GetAuthor(int Id)
        {
            return _db.Authors.FirstOrDefault(c => c.Id == Id); ;
        }

        public async Task<AuthorFormVM> Get(int Id)
        {
            var Author = await _db.Authors.SingleOrDefaultAsync(x => x.Id == Id && !x.IsDeleted);
            if (Author == null)
            {
                throw new EntityNotFoundException();
            }
            var AuthorVM = _mapper.Map<AuthorFormVM>(Author);
            
            return (AuthorVM);
        }


        public async Task<IEnumerable<AuthorViewModel>> GetAuthorList()
        {
            var Authors = await _db.Authors
                .AsNoTracking()
                .ToListAsync();
            var viewModel = _mapper.Map<IEnumerable<AuthorViewModel>>(Authors);
            return (viewModel);
        }

        public async Task<AuthorViewModel> Update(AuthorFormVM model)
        {
            var Author = await _db.Authors.FindAsync(model.Id);
            Author = _mapper.Map(model, Author);
            //Author.Name = model.Name;
            Author.LastUpdatedOn = DateTime.Now;
            await _db.SaveChangesAsync();
            var viewModel = _mapper.Map<AuthorViewModel>(Author);
            return viewModel;
        }

        public Author IsAuthorExists(AuthorFormVM Author)
        {
            return _db.Authors.SingleOrDefault(x => x.Name == Author.Name);
        }
        public void SaveChanges()
        {
            _db.SaveChanges();
        }
    }
}
