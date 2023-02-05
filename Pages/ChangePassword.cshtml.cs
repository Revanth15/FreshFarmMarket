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
        public ChangePasswordModel(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager, AuditLogService auditlogService)
        {
            this.signInManager = signInManager;
            this.userManager = userManager;
            _auditlogService = auditlogService;
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

            var result = await userManager.ChangePasswordAsync(user, Password, NewPassword);

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
