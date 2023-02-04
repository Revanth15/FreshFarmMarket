using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using FreshFarmMarket.ViewModels;
using Microsoft.AspNetCore.Identity;
using FreshFarmMarket.Models;

namespace FreshFarmMarket.Pages
{
    public class LogoutModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> signInManager;
        public LogoutModel(SignInManager<ApplicationUser> signInManager)
        {
            this.signInManager = signInManager;
        }
        public void OnGet() { }

        public async Task<IActionResult> OnPostLogoutAsync()
        {
            await signInManager.SignOutAsync();
            return RedirectToPage("Login");
        }
        public async Task<IActionResult> OnPostDontLogoutAsync()
        {
            return RedirectToPage("Index");
        }
    }
}
