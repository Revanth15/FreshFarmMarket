using FreshFarmMarket.Models;
using FreshFarmMarket.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FreshFarmMarket.Pages
{
    public class LoginOTPAuthModel : PageModel
    {
		private readonly SignInManager<ApplicationUser> signInManager;
		private readonly UserManager<ApplicationUser> userManager;
		private readonly ILogger<IndexModel> _logger;
		private readonly AuditLogService _auditlogService;
		public LoginOTPAuthModel(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager, ILogger<IndexModel> logger, AuditLogService auditlogService)
        {
            this.signInManager = signInManager;
            this.userManager = userManager;
            _logger = logger;
            _auditlogService = auditlogService;
        }
        [BindProperty]
		public string OTP { get; set; }
		public void OnGet()
        {
        }

		public async Task<IActionResult> OnPostAsync(string email)
		{
			var user = await userManager.FindByEmailAsync(email);
			var test = await signInManager.TwoFactorSignInAsync("Email", OTP, false, false);
			Console.WriteLine(test.Succeeded);
			if (test.Succeeded)
			{
				await signInManager.SignInAsync(user, false);
				if (user.lastPasswordChangeDate.AddMinutes(180) < DateTime.Now)
				{
					AuditLog log2 = new();
					log2.userEmail = user.Email;
					log2.LogName = "User has been prompted to change password";
					_auditlogService.AddLog(log2);
					return Redirect("/changepassword");
				}

				AuditLog log = new();
				log.userEmail = user.Email;
				log.LogName = "User Logged in Successfully";
				_auditlogService.AddLog(log);
				return RedirectToPage("/Index");
			}
			return Page();
		}

	}
}
