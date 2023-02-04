using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using FreshFarmMarket.ViewModels;
using Microsoft.AspNetCore.Identity;
using FreshFarmMarket.Models;

namespace FreshFarmMarket.Pages
{
    public class LoginModel : PageModel
    {
        [BindProperty]
        public Login LModel { get; set; }

        private readonly SignInManager<ApplicationUser> signInManager;
        public LoginModel(SignInManager<ApplicationUser> signInManager)
        {
            this.signInManager = signInManager;
        }

        public string message { get; set; }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                var identityResult = await signInManager.PasswordSignInAsync(LModel.Email, LModel.Password,
                LModel.RememberMe, lockoutOnFailure: true);
                if (identityResult.Succeeded)
                {
                    return RedirectToPage("Index");
                }
                if (identityResult.IsLockedOut)
                {
                    message = "User account locked out!";
                }
                else
                {
                    message = "Username or Password incorrect!";
                }
            }
            return Page();
        }
    }
}
