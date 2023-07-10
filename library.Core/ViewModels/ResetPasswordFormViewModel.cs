using library.Core.Constants;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace library.Core.ViewModels
{
    public class ResetPasswordFormViewModel
    {
        public string Id { get; set; } = null!;

        [DataType(DataType.Password),
            StringLength(100, ErrorMessage = Errors.MaxMinLength, MinimumLength = 8),
            RegularExpression(RegexPatterns.Password, ErrorMessage = Errors.WeakPassword)]
        public string Password { get; set; } = null!;

        [DataType(DataType.Password), Display(Name = "Confirm password"),
            Compare("Password", ErrorMessage = Errors.ConfirmPassNotMatche)]
        public string ConfirmPassword { get; set; } = null!;
    }
}
