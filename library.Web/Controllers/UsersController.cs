using CloudinaryDotNet.Actions;
using library.Core.Constants;
using library.Data.Models;
using library.Infrastructure.Services.Authors;
using library.Infrastructure.Services.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Text;
using library.Web.Services.Email;

namespace library.Web.Controllers
{
    [Authorize(Roles =AppRoles.Admin)]
    public class UsersController : Controller
    {
        private readonly IUser _userService;
        private readonly IEmailSender _emailSender;
        private readonly IWebHostEnvironment _webHostEnviroment;
        private readonly IEmailBodyBuilder _emailBodyBuilder;
        public UsersController(IUser userService, IEmailSender emailSender, IWebHostEnvironment webHostEnviroment, IEmailBodyBuilder emailBodyBuilder)
        {
            _userService = userService;
            _emailSender = emailSender;
            _webHostEnviroment = webHostEnviroment;
            _emailBodyBuilder = emailBodyBuilder;
        }

        public async  Task<IActionResult> Index()
        {
            //var filePath = $"{_webHostEnviroment.WebRootPath}/templates/email.html";
            //StreamReader str = new(filePath);
            //var body = str.ReadToEnd();
            //str.Close();
            //body = body.Replace()
            //await _emailSender.SendEmailAsync("mohammed2562000@gmail.com", "Test", "test");
            
            var users =  _userService.GetUsers();
            return View(users);
        }

        [HttpGet]
        [AjaxOnly]
        public async Task<IActionResult> Create()
        {
            var viewModel = new UserFormViewModel
            {
                Roles= await _userService.GetRoles()
            };
            return PartialView("_Form", viewModel);
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UserFormViewModel model)
        {
            if(!ModelState.IsValid)            
                return BadRequest();
            var createdBy = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
            var (viewModel,errors,code,user) = await _userService.Create(model, createdBy);
            if (errors != null)
            {
                return BadRequest(string.Join(", ", errors));
            }
            else
            {

                code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code!));
                var callbackUrl = Url.Page(
                    "/Account/ConfirmEmail",
                    pageHandler: null,
                    values: new { area = "Identity", userId = user!.Id, code },
                    protocol: Request.Scheme);
                
                var placeholders = new Dictionary<string, string>()
                {
                    { "imageUrl","https://res.cloudinary.com/devcreed/image/upload/v1668732314/icon-positive-vote-1_rdexez.svg"},
                    { "header",$"Hey {user.FullName}, thanks for joining us!"},
                    { "body","please confirm your email"},
                    { "url",$"{HtmlEncoder.Default.Encode(callbackUrl!)}"},
                    { "linkTitle","Active Account!"}
                };
                var body = _emailBodyBuilder.GetEmailBody(
                    template: EmailTemplates.Email,placeholders);

                await _emailSender.SendEmailAsync(user.Email, "Confirm your email", body);

            }
            return PartialView("_UserRow", viewModel);
        }

        [HttpGet]
        [AjaxOnly]
        public async Task<IActionResult> ResetPassword(string id)
        {
            var user = await _userService.GetUser(id);
            if (user == null)
                return NotFound();
            var viewModel = new ResetPasswordFormViewModel { Id = user.Id };
            return PartialView("_ResetPasswordForm", viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordFormViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest();
            var user = await _userService.GetUser(model.Id);
            if (user == null)
                return NotFound();
            var currentPasswordHash = user.PasswordHash;
            var result = await _userService.ChangePassAsync(user,model.Password);
            if (result.Succeeded)
            {
                user.LastUpdatedById = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
                user.LastUpdatedOn= DateTime.Now;
                await _userService.UpdateAsync(user);
                var viewModel = _userService.MapToUserViewModel(user);
                return PartialView("_UserRow", viewModel);

            }
            user.PasswordHash = currentPasswordHash;
            await _userService.UpdateAsync(user);   
            return PartialView("_UserRow", string.Join(", ", result.Errors.Select(e=>e.Description)));
        }

        [HttpGet]
        [AjaxOnly]
        public async Task<IActionResult> Edit(string id)
        {
            var user = await _userService.GetUser(id);
            if (user == null)
                return NotFound();
            var viewModel = _userService.MapToUserForm(user);
            viewModel.SelectedRoles = await _userService.GetRolesList(user);
            viewModel.Roles = await _userService.GetRoles();
            return PartialView("_Form", viewModel);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UserFormViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var user = await _userService.GetUser(model?.Id);
            
            if (user == null)
                return NotFound();
            
            user = _userService.MapToApplicationUser(model,user);
            user.LastUpdatedById = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
            user.LastUpdatedOn = DateTime.Now;

            var result = await _userService.UpdateResult(user);
            if (result.Succeeded)
            {
                var cuurrentRoles = await _userService.GetRolesList(user);
                var rolesUpdated = !cuurrentRoles.SequenceEqual(model.SelectedRoles);
                if (rolesUpdated)
                {
                    await _userService.RemoveFromRole(user, cuurrentRoles);
                    await _userService.AddToRole(user, model.SelectedRoles);
                }
                var viewModel = _userService.MapToUserViewModel(user);
                return PartialView("_UserRow", viewModel);
            }
            return BadRequest(string.Join(", ", result.Errors.Select(e => e.Description)));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Unlock(string id)
        { 
            var user = await _userService.GetUser(id);
            if(user is null) return NotFound();
            await _userService.Unlock(user);
            return Ok();
        }

            public async Task<IActionResult> AllowUsername(UserViewModel model)
        {
            bool isAllowed = await _userService.IsUsernameExists(model);
            //var isAllowed = user is null || user.UserName.Equals(model.Username);
            return Json(isAllowed);
        }
        public async Task<IActionResult> AllowEmail(UserViewModel model)
        {
            bool isAllowed = await _userService.IsEmailExists(model);
            //var isAllowed = user is null || user.Email.Equals(model.Email);
            return Json(isAllowed);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleStatus(string id)
        {
            ApplicationUser user = await _userService.GetUser(id);
            if (user is null)
                return NotFound();
            user.IsDeleted = !user.IsDeleted;
            user.LastUpdatedOn = DateTime.Now;
            user.LastUpdatedById = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
            await _userService.UpdateAsync(user);
            return Ok(user.LastUpdatedOn.ToString());
        }


    }
}
