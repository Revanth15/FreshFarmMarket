using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using FreshFarmMarket.ViewModels;
using Microsoft.AspNetCore.Identity;
using FreshFarmMarket.Models;
using FreshFarmMarket.Services;

namespace FreshFarmMarket.Pages
{
    public class LogoutModel : PageModel
    {
        private UserManager<ApplicationUser> userManager { get; }
        private SignInManager<ApplicationUser> signInManager { get; }
        private readonly AuditLogService _auditlogService;

        public LogoutModel(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, AuditLogService auditlogService)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            _auditlogService = auditlogService;
        }
        public void OnGet() { 
        
        }

        public async Task<IActionResult> OnPostLogoutAsync()
        {
            AuditLog log = new();
            var user = await userManager.GetUserAsync(User);
            log.userEmail = user.Email;
            await signInManager.SignOutAsync();
            log.LogName = "User visited /Logout and Logged out";
            _auditlogService.AddLog(log);
            return RedirectToPage("Login");
        }
        public async Task<IActionResult> OnPostDontLogoutAsync()
        {
            AuditLog log = new();
            var user = await userManager.GetUserAsync(User);
            log.userEmail = user.Email;
            log.LogName = "User visited /Logout and DID NOT log out";
            _auditlogService.AddLog(log);
            return RedirectToPage("Index");
        }
    }
}
