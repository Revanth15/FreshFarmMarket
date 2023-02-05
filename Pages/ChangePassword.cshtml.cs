using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using FreshFarmMarket.Models;
using System.ComponentModel.DataAnnotations;
using FreshFarmMarket.Services;

namespace FreshFarmMarket.Pages
{
    [BindProperties]
    public class ChangePasswordModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly AuditLogService _auditlogService;
        private readonly PreviousPasswordsService _previousPasswordsService;
        public ChangePasswordModel(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager, AuditLogService auditlogService, PreviousPasswordsService previousPasswordsService)
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
        public string NewPassword { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Compare(nameof(NewPassword), ErrorMessage = "Passwords must match")]
        public string ConfirmPassword { get; set; }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await userManager.GetUserAsync(User);

            var prevHashs = _previousPasswordsService.Getlast2passwords(user);

            var lastPassApprove = true;
            var changeApprove = true;

            if (user.lastPasswordChangeDate.AddMinutes(2) > DateTime.Now)
            {
                TempData["FlashMessage.Text"] = "Password Age! Password cannot be change within 2 Minutes";
                TempData["FlashMessage.Type"] = "danger";
                lastPassApprove = false;
            }

            if (prevHashs != null)
            {
                foreach (var passhash in prevHashs)
                {
                    var check = userManager.PasswordHasher.VerifyHashedPassword(user, passhash.passwordHash, NewPassword);
                    if (check == PasswordVerificationResult.Success)
                    {
                        TempData["FlashMessage.Text"] = "Password History! Password cannot be same as the previous 2 passwords";
                        TempData["FlashMessage.Type"] = "danger";
                        changeApprove = false;
                        break;
                    }
                }
            }

            if (!lastPassApprove)
            {
                TempData["FlashMessage.Text"] = "Password Age! Password cannot be change within 2 Minutes";
                TempData["FlashMessage.Type"] = "danger";
                return Redirect("/changepassword");
            }

            if (!changeApprove)
            {
                TempData["FlashMessage.Text"] = "Password History! Password cannot be same as the previous 2 passwords";
                TempData["FlashMessage.Type"] = "danger";
                return Redirect("/changepassword");
            }

            var result = await userManager.ChangePasswordAsync(user, Password, NewPassword);
            user.lastPasswordChangeDate = DateTime.Now;
            var updateResult = await userManager.UpdateAsync(user);
            PreviousPasswords hash = new();
            hash.userId = user.Id;
            hash.passwordHash = user.PasswordHash;
            _previousPasswordsService.AddHash(hash);

            AuditLog log = new();
            log.userEmail = user.Email;
            log.LogName = "User has successfully changed their password";
            _auditlogService.AddLog(log);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return Page();
            }

            TempData["FlashMessage.Text"] = "Password changed successfully";
            TempData["FlashMessage.Type"] = "success";

            return Redirect("/");
        }
    }
}
