using AutoMapper;
using library.Core.ViewModels;
using library.Data.Models;

namespace library.Infrastructure.AutoMapper
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            //Category
            CreateMap<Category, CategoryViewModel>();
            //.ForMember(dest =>dest.Name,opt=>opt.MapFrom(src=>src.Name));
            CreateMap<CategoryVM, Category>().ReverseMap();

            //Author
            CreateMap<Author, AuthorViewModel>();
            CreateMap<AuthorFormVM, Author>().ReverseMap();

        }
    }
}
