using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using FreshFarmMarket.ViewModels;
using Microsoft.AspNetCore.Identity;
using FreshFarmMarket.Models;
using FreshFarmMarket.Services;
using AspNetCore.ReCaptcha;
using System.Web;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;

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
        private readonly UserManager<ApplicationUser> userManager;
        private readonly AuditLogService _auditlogService;
        public readonly IEmailService _emailSender;
        public LoginModel(SignInManager<ApplicationUser> signInManager, reCaptchaService reCaptchaService, ILogger<IndexModel> logger, AuditLogService auditlogService, IEmailService emailSender, UserManager<ApplicationUser> userManager)
        {
            this.signInManager = signInManager;
            this.userManager = userManager;
            _reCaptchaService = reCaptchaService;
            _emailSender = emailSender;
            _logger = logger;
            _auditlogService = auditlogService;
        }

        public string message { get; set; }

        [BindProperty]
        public string token { get; set; }
        [BindProperty]
        public string browser { get; set; }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                _logger.LogWarning(token.ToString());
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
                _logger.LogWarning(reCaptcharesult.Result.score.ToString());
                var user = await userManager.FindByNameAsync(LModel.Email);

                //var identityResult = await signInManager.PasswordSignInAsync(LModel.Email, LModel.Password,
                //LModel.RememberMe, lockoutOnFailure: true);
                if(userManager.PasswordHasher.VerifyHashedPassword(user, user.PasswordHash, LModel.Password).ToString().Equals("Success"))
                {
                    var identityResult = await signInManager.PasswordSignInAsync(LModel.Email, LModel.Password,
                        LModel.RememberMe, lockoutOnFailure: true);
                    _logger.LogWarning(identityResult.RequiresTwoFactor.ToString());
                    if (identityResult.RequiresTwoFactor)
                    {
                        var Token = await userManager.GenerateTwoFactorTokenAsync(user, "Email");
                        _logger.LogWarning(Token);
                        var res = _emailSender.SendEmail(
                            user.Email,
                                "OTP",
                            $"One-Time Password {Token}",
                            null,null);
                        return RedirectToPage("/LoginOTPAuth", new { email = LModel.Email });
                    }
                    if (identityResult.Succeeded)
                    {
                        if (user.lastPasswordChangeDate.AddMinutes(180) < DateTime.Now)
                        {
                            _logger.LogWarning("User redirected to change password");
                            return Redirect("/changepassword");
                        }
                        _logger.LogWarning(browser);
                        AuditLog log = new();
                        log.userEmail = LModel.Email;
                        log.LogName = "User Logged in Successfully";
                        _auditlogService.AddLog(log);
                        return RedirectToPage("Index");
                    }
                    if (identityResult.IsLockedOut)
                    {
                        AuditLog log = new();
                        log.userEmail = LModel.Email;
                        log.LogName = "User tried to log in but is Locked out";
                        _auditlogService.AddLog(log);
                        message = "User account locked out!";
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Username or Password incorrect!");
                    //message = "Username or Password incorrect!";
                }
            }
            return Page();
        }
    }
}
