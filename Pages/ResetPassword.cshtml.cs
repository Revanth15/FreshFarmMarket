using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using FreshFarmMarket.Models;
using System.ComponentModel.DataAnnotations;
using System.Text;
using FreshFarmMarket.Services;

namespace FreshFarmMarket.Pages
{
    [AllowAnonymous]
    [BindProperties]
    public class ResetPasswordModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly UserManager<ApplicationUser> userManager; 
        private readonly AuditLogService _auditlogService;
        private readonly PreviousPasswordsService _previousPasswordsService;
        public ResetPasswordModel(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager, AuditLogService auditlogService, PreviousPasswordsService previousPasswordsService)
        {
            this.signInManager = signInManager;
            this.userManager = userManager;
            _auditlogService = auditlogService;
            _previousPasswordsService = previousPasswordsService;
        }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Compare(nameof(Password), ErrorMessage = "Passwords must match")]
        public string ConfirmPassword { get; set; }
        public void OnGet()
        {

        }

        public async Task<IActionResult> OnPostAsync(string code, string username)
        {
            if (!ModelState.IsValid)
            {
                TempData["FlashMessage.Text"] = "Passwords do not match";
                TempData["FlashMessage.Type"] = "danger";
                return Page();
            }
            var user = await userManager.FindByNameAsync(username);
            var changeApprove = true;
            if (user == null)
            {
                TempData["FlashMessage.Text"] = "Invalid Tokens";
                TempData["FlashMessage.Type"] = "danger";
                return Redirect("/");
            }
            var prevHashs = _previousPasswordsService.Getlast2passwords(user);

            if (prevHashs != null)
            {
                foreach (var passhash in prevHashs)
                {
                    var check = userManager.PasswordHasher.VerifyHashedPassword(user, passhash.passwordHash, Password);
                    //Console.WriteLine(check.ToString());
                    if (check == PasswordVerificationResult.Success)
                    {
                        TempData["FlashMessage.Text"] = "Password History! Password cannot be same as the previous 2 passwords";
                        TempData["FlashMessage.Type"] = "danger";
                        changeApprove = false;
                        break;
                    }
                }
            }

            if (!changeApprove)
            {
                TempData["FlashMessage.Text"] = "Password History! Password cannot be same as the previous 2 passwords";
                TempData["FlashMessage.Type"] = "danger";
                return Redirect("/changepassword");
            }

            var token = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
            var result = await userManager.ResetPasswordAsync(user, token, Password);
            user.lastPasswordChangeDate = DateTime.Now;
            var updateResult = await userManager.UpdateAsync(user);
            PreviousPasswords hash = new();
            hash.userId = user.Id;
            hash.passwordHash = user.PasswordHash;
            _previousPasswordsService.AddHash(hash);
            AuditLog log = new();
            log.userEmail = user.Email;
            log.LogName = "User has sucessfully reset their password";
            _auditlogService.AddLog(log);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                TempData["FlashMessage.Text"] = "Invalid Tokens";
                TempData["FlashMessage.Type"] = "danger";
                return Page();
            }

            TempData["FlashMessage.Text"] = "Successfully reset password!";
            TempData["FlashMessage.Type"] = "success";
            return RedirectToPage("/Login");
            
        }
    }
}
