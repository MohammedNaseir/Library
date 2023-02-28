using AutoMapper;
using library.Core.ViewModels;
using library.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace library.Infrastructure.AutoMapper
{
    public class MapperProfile:Profile
    {
        public MapperProfile()
        {
            //Category
            CreateMap<Category, CategoryViewModel>();
                //.ForMember(dest =>dest.Name,opt=>opt.MapFrom(src=>src.Name));
            CreateMap<CategoryVM, Category>().ReverseMap();
            
                        
        }
    }
}
