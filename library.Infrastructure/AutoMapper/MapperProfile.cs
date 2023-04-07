using AutoMapper;
using library.Core.ViewModels;
using library.Data.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace library.Infrastructure.AutoMapper
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            //Category
            CreateMap<Category, CategoryViewModel>();
            CreateMap<Category, CategoryVM>().ReverseMap();
            //.ForMember(dest =>dest.Name,opt=>opt.MapFrom(src=>src.Name));
            CreateMap<Category, SelectListItem>()
                  .ForMember(dest => dest.Value, opt => opt.MapFrom(src => src.Id))
                  .ForMember(dest => dest.Text, opt => opt.MapFrom(src => src.Name));

            //Authors
            CreateMap<Author, AuthorViewModel>();
            CreateMap<AuthorFormVM, Author>().ReverseMap();
            CreateMap<Author, SelectListItem>()
                .ForMember(dest => dest.Value, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Text, opt => opt.MapFrom(src => src.Name));

            //Books
            CreateMap<BookFormVM,Book>()
                .ReverseMap()
                .ForMember(dest => dest.Categories, opt => opt.Ignore());
            CreateMap<Book, BookViewModel>()
               .ForMember(dest => dest.Author, opt => opt.MapFrom(src => src.Author!.Name))
               .ForMember(dest => dest.Categories, opt => opt.MapFrom(src => src.Categories.Select(c=>c.Category!.Name).ToList()));
        }
    }
}
