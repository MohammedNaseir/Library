﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.ComponentModel.DataAnnotations;
using System.Text;
using library.Data.Models;

using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using library.Web.Services.Email;

namespace library.Web.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class ResendEmailConfirmationModel : PageModel
    {
		private readonly IEmailBodyBuilder _emailBodyBuilder;
		private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailSender _emailSender;

		public ResendEmailConfirmationModel(UserManager<ApplicationUser> userManager, IEmailSender emailSender, IEmailBodyBuilder emailBodyBuilder)
		{
			_userManager = userManager;
			_emailSender = emailSender;
			_emailBodyBuilder = emailBodyBuilder;
		}

		/// <summary>
		///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
		///     directly from your code. This API may change or be removed in future releases.
		/// </summary>
		[BindProperty]
        public InputModel Input { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public class InputModel
        {
            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
            //[EmailAddress]
            public string Username { get; set; }
        }

        public void OnGet(string username)
        {
			Input = new()
			{
				Username = username,
			};
		}

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

			var userName = Input.Username.ToUpper();
			var user = await _userManager.Users
				.SingleOrDefaultAsync(u => (u.NormalizedUserName == userName || u.NormalizedEmail == userName) && !u.IsDeleted);

			if (user == null)
			{
				ModelState.AddModelError(string.Empty, "Verification email sent. Please check your email.");
				return Page();
			}

			var userId = await _userManager.GetUserIdAsync(user);
			var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
			code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
			var callbackUrl = Url.Page(
				"/Account/ConfirmEmail",
				pageHandler: null,
				values: new { userId = userId, code = code },
			protocol: Request.Scheme);
            
			var placeholders = new Dictionary<string, string>()
                {
                    { "imageUrl","https://res.cloudinary.com/decm7aqke/image/upload/v1690286570/icon-positive-vote-2_jcxdww_lwsyqe.svg"},
                    { "header",$"Hey {user.FullName}, thanks for joining us!"},
                    { "body","please confirm your email"},
                    { "url",$"{HtmlEncoder.Default.Encode(callbackUrl!)}"},
                    { "LinkTitle","Active Account!"}
                };
            var body = _emailBodyBuilder.GetEmailBody(
                template: EmailTemplates.Email,placeholders);

			await _emailSender.SendEmailAsync(
				user.Email,
				"Confirm your email", body);

			ModelState.AddModelError(string.Empty, "Verification email sent. Please check your email.");
			return Page();
		}
    }
}
