using library.Core.Constants;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UoN.ExpressiveAnnotations.NetCore.Attributes;

namespace library.Core.ViewModels
{
    public class UserFormViewModel
    {
        public string? Id { get; set; }
        
        [MaxLength(20, ErrorMessage = Errors.MaxLength),Display(Name ="Full Name")
            , RegularExpression(RegexPatterns.CharactersOnly_Eng, ErrorMessage = Errors.OnlyEnglishLetters)]
        public string FullName { get; set; } = null!;
        
        [MaxLength(20,ErrorMessage = Errors.MaxLength)]
        [Remote("AllowUsername", null!
            , AdditionalFields = ("Id"), ErrorMessage = Errors.Duplicated)
            , RegularExpression(RegexPatterns.Username, ErrorMessage = Errors.InvalidUsername)]
        public string UserName { get; set; } = null!;
        
        
        [MaxLength(200, ErrorMessage = Errors.MaxLength)]
        [Required]
        [EmailAddress]
        [Remote("AllowEmail", null!, AdditionalFields = ("Id"), ErrorMessage = Errors.Duplicated)]
        public string Email { get; set; } = null!;
        
        
        [StringLength(100, ErrorMessage = Errors.MaxMinLength, MinimumLength = 8)]
        [RegularExpression(RegexPatterns.Password,ErrorMessage = Errors.WeakPassword)]
        [DataType(DataType.Password)]
        [RequiredIf("Id == null", ErrorMessage = Errors.RequiredField)]
        public string? Password { get; set; } = null!;

        [DataType(DataType.Password), Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = Errors.ConfirmPassNotMatche)]
        [RequiredIf("Id == null", ErrorMessage = Errors.RequiredField)]
        public string ?ConfirmPassword { get; set; } = null!;

        [Display(Name = "Roles")]
        public IList<string> SelectedRoles { get; set; } = new List<string>();

        public IEnumerable<SelectListItem>? Roles { get; set; }

    }
}
