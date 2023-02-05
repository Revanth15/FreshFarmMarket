using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using FreshFarmMarket.ViewModels;
using Microsoft.AspNetCore.Identity;
using FreshFarmMarket.Models;
using FreshFarmMarket.Services;
using AspNetCore.ReCaptcha;
using System.Web;
using Microsoft.Extensions.Logging;

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
                var user = await userManager.FindByNameAsync(LModel.Email);

                var identityResult = await signInManager.PasswordSignInAsync(LModel.Email, LModel.Password,
                LModel.RememberMe, lockoutOnFailure: true);
                if (identityResult.Succeeded)
                {
                    if (user.lastPasswordChangeDate.AddMinutes(30) < DateTime.Now)
                    {
                        _logger.LogWarning("failed");
                        return Redirect("/changepassword");
                    }
                    _logger.LogWarning(user.lastPasswordChangeDate.AddMinutes(30).ToString());
                    _logger.LogWarning(DateTime.Now.ToString());
                    //_logger.LogWarning(HttpContext.Connection.RemoteIpAddress.ToString());
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
                else
                {
                    message = "Username or Password incorrect!";
                }
            }
            return Page();
        }
    }
}
