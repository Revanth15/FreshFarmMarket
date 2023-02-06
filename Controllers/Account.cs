using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using FreshFarmMarket.Models;
using FreshFarmMarket.Services;
using System.Security.Claims;

namespace FreshFarmMarket.Controllers
{ 
    public class Account : Controller
    {
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly UserManager<ApplicationUser> userManager;
        private IWebHostEnvironment _environment;
        public Account(SignInManager<ApplicationUser> signInManager, reCaptchaService reCaptchaService, UserManager<ApplicationUser> userManager, IWebHostEnvironment environment)
        {
            this.signInManager = signInManager;
            this.userManager = userManager;
            _environment = environment;
        }
        public IActionResult GoogleLogin(string returnUrl = null)
        {
            //await HttpContext.ChallengeAsync(GoogleDefaults.AuthenticationScheme, new AuthenticationProperties()
            //{
            //    RedirectUri = Url.Action("GoogleCallback", "Account", new { returnUrl })
            //});
            var redirectUrl = Url.Action(nameof(GoogleCallback), "Account", new { returnUrl });
            var properties = signInManager.ConfigureExternalAuthenticationProperties("Google", redirectUrl);
            return Challenge(properties, "Google");
        }
        public async Task<IActionResult> GoogleCallback(string returnUrl = null, string remoteError = null)
        {
            if (remoteError != null)
            {
                return Redirect("/Login");
            }

            var info = await signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                return Redirect("/Register");
            }

            // Obtain the user information
            var email = info.Principal.FindFirstValue(ClaimTypes.Email);
            var name = info.Principal.FindFirstValue(ClaimTypes.Name);
            var pfp = info.Principal.FindFirstValue("image");

            // Use the user information for your application logic

            // Redirect to the original URL
            var user = await userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return Redirect(returnUrl ?? "/GGLRegister?email=" + email + "&name=" + name + "&pfp=" + pfp);
            }
            else
            {
                await signInManager.SignInAsync(user, true);
                TempData["FlashMessage.Type"] = "success";
                TempData["FlashMessage.Text"] = string.Format("Successful Login");
                return Redirect("/");
            }
        }
    }
}
