using library.Core.Constants;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;


namespace library.Core.ViewModels
{
    public class AuthorFormVM
    {
        // this Author Form View Model
        public int Id { get; set; }

        [MaxLength(50, ErrorMessage = Errors.MaxLength), Display(Name = "Author")
            , RegularExpression(RegexPatterns.CharactersOnly_Eng, ErrorMessage = Errors.OnlyEnglishLetters)]
        [Remote("AllowItem", null, AdditionalFields = ("Id"), ErrorMessage = Errors.Duplicated)]
        public string? Name { get; set; }

    }
}
