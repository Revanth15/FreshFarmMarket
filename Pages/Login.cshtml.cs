using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using FreshFarmMarket.ViewModels;
using Microsoft.AspNetCore.Identity;
using FreshFarmMarket.Models;
using FreshFarmMarket.Services;
using AspNetCore.ReCaptcha;
using System.Web;

namespace FreshFarmMarket.Pages
{
    //[ValidateReCaptcha]
    public class LoginModel : PageModel
    {
        [BindProperty]
        public Login LModel { get; set; }
        private readonly ILogger<IndexModel> _logger;

        private readonly reCaptchaService _reCaptchaService;

        private readonly SignInManager<ApplicationUser> signInManager;
        public LoginModel(SignInManager<ApplicationUser> signInManager, reCaptchaService reCaptchaService, ILogger<IndexModel> logger)
        {
            this.signInManager = signInManager;
            _reCaptchaService = reCaptchaService;
            _logger = logger;
        }

        public string message { get; set; }

        [BindProperty]
        public string token { get; set; }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                var reCaptcharesult = _reCaptchaService.tokenVerify(token);
                //_logger.LogWarning(reCaptcharesult.Result.score.ToString());
                //_logger.LogWarning(reCaptcharesult.Result.success.ToString());
                //_logger.LogWarning(HttpUtility.HtmlEncode(LModel.Password));
                //_logger.LogWarning(HttpUtility.HtmlEncode(LModel.Email));
                if (!reCaptcharesult.Result.success && reCaptcharesult.Result.score <= 0.5)
                {
                    message = "You are not a human!";
                    return Page();
                }
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
