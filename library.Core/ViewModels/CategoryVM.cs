using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace library.Core.ViewModels
{
    public class CategoryVM
    {
        // this Category Form View Model
        public int Id { get; set; }

        [MaxLength(50, ErrorMessage = "Max length cannot be more than 50 characters")]
        [Remote("AllowItem", null,AdditionalFields =("Id"),ErrorMessage ="category wirh name Isexists")]
        public string? Name { get; set; } 

    }
}
