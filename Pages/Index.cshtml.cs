using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using FreshFarmMarket.ViewModels;
using Microsoft.AspNetCore.Identity;
using FreshFarmMarket.Models;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.DataProtection;
using System.Web;
using FreshFarmMarket.Services;

namespace FreshFarmMarket.Pages
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private UserManager<ApplicationUser> userManager { get; }
        private SignInManager<ApplicationUser> signInManager { get; }
        private readonly AuditLogService _auditlogService;

        public IndexModel(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager,ILogger<IndexModel> logger, AuditLogService auditlogService)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            _auditlogService = auditlogService;
            _logger = logger;
        }

        
        public string fullName { get; set; }
        
        public string creditCardNo { get; set; }
        
        public string gender { get; set; }
        
        [DataType(DataType.PhoneNumber)]
        public int mobileNo { get; set; }
        
        public string deliveryAddress { get; set; }
        
        public string aboutMe { get; set; }

        public string password { get; set; }

        [DataType(DataType.EmailAddress)]
        public string email { get; set; }

        public string? ImageURL { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            AuditLog log = new();
            var user = await userManager.GetUserAsync(User);
            var dataProtectionProvider = DataProtectionProvider.Create("EncryptData");
            var protector = dataProtectionProvider.CreateProtector("MySecretKey");
            if (user != null)
            {
                fullName = HttpUtility.HtmlEncode(user.fullName);
                creditCardNo = HttpUtility.HtmlEncode(protector.Unprotect(user.creditCardNo));
                gender = HttpUtility.HtmlEncode(user.gender);
                email = HttpUtility.HtmlEncode(user.NormalizedEmail).ToLower();
                mobileNo = user.mobileNo;
                deliveryAddress = HttpUtility.HtmlEncode(user.deliveryAddress);
                aboutMe = HttpUtility.HtmlEncode(user.aboutMe);
                password = HttpUtility.HtmlEncode(user.PasswordHash);
                ImageURL = user.imageURL ?? "/images/people/avatar-1.png";
                log.userEmail = user.Email;
                log.LogName = "User visited /Index";
                _auditlogService.AddLog(log);
                return Page();
            }
            else
            {
                return RedirectToPage("/Login");
            }
        }
    }
}