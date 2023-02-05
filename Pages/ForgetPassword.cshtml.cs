using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using System.Text.Encodings.Web;
using System.Text;
using FreshFarmMarket.Models;
using FreshFarmMarket.Services;
using Microsoft.AspNetCore.Authorization;
using System.ComponentModel.DataAnnotations;

namespace FreshFarmMarket.Pages
{
    [AllowAnonymous]
    public class ForgetPasswordModel : PageModel
    {
        public readonly IEmailService _emailSender;
        public readonly UserManager<ApplicationUser> _userManager; 
        private readonly ILogger<IndexModel> _logger;

        public ForgetPasswordModel(IEmailService emailSender, UserManager<ApplicationUser> userManager, ILogger<IndexModel> logger)
        {
            _emailSender = emailSender;
            _userManager = userManager;
            _logger = logger;
        }

        [BindProperty]
        [Required]
        public string Email { get; set; }
        public void OnGet()
        {
        }
        
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            var user = await _userManager.FindByEmailAsync(Email);

            TempData["FlashMessage.Text"] = $"A reset password email will be sent to {user.Email} if it is valid";
            TempData["FlashMessage.Type"] = "info";

            if (user == null)
            {
                return Page();
            }

            var lastPassApprove = true;

            if (user.lastPasswordChangeDate.AddMinutes(2) > DateTime.Now)
            {
                TempData["FlashMessage.Text"] = "Password Age! Password cannot be change within 2 Minutes";
                TempData["FlashMessage.Type"] = "danger";
                lastPassApprove = false;
            }


            if (!lastPassApprove)
            {
                TempData["FlashMessage.Text"] = "Password Age! Password cannot be change within 2 Minutes";
                TempData["FlashMessage.Type"] = "danger";
                return Redirect("/changepassword");
            }


            var specialCode = await _userManager.GeneratePasswordResetTokenAsync(user);
            specialCode = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(specialCode));
            var callbackUrl = Url.Page(
            "/ResetPassword",
                pageHandler: null,
                values: new { code = specialCode, username = user.UserName },
                protocol: Request.Scheme);

            var result = _emailSender.SendEmail(
                Email,
                "Reset Password",
                $"You have requested a password reset.<br>Please reset your password by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>!.",
                null,
                null);
            
            if (!result)
            {
                TempData["FlashMessage.Text"] = $"Failed to send email.";
                TempData["FlashMessage.Type"] = "danger";
            }
            return Page();
        }
    }
}
