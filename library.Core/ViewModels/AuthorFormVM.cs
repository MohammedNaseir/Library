using library.Core.Constants;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;


namespace library.Core.ViewModels
{
    public class AuthorFormVM
    {
        // this Author Form View Model
        public int Id { get; set; }

        [MaxLength(50, ErrorMessage = Errors.MaxLength), Display(Name = "Author")]
        [Remote("AllowItem", null, AdditionalFields = ("Id"), ErrorMessage = "category wirh name Isexists")]
        public string? Name { get; set; }

    }
}
