using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using FreshFarmMarket.ViewModels;
using Microsoft.AspNetCore.Identity;
using FreshFarmMarket.Models;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.DataProtection;

namespace FreshFarmMarket.Pages
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private UserManager<ApplicationUser> userManager { get; }
        private SignInManager<ApplicationUser> signInManager { get; }

        public IndexModel(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager,ILogger<IndexModel> logger)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
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
            var user = await userManager.GetUserAsync(User);
            var dataProtectionProvider = DataProtectionProvider.Create("EncryptData");
            var protector = dataProtectionProvider.CreateProtector("MySecretKey");
            if (user != null)
            {
                fullName = user.fullName;
                creditCardNo = protector.Unprotect(user.creditCardNo);
                gender = user.gender;
                email = user.NormalizedEmail.ToLower();
                mobileNo = user.mobileNo;
                deliveryAddress = user.deliveryAddress;
                aboutMe = user.aboutMe;
                password = user.PasswordHash;
                ImageURL = user.imageURL ?? "/images/people/avatar-1.png";
                return Page();
            }
            else
            {
                return RedirectToPage("/Login");
            }
        }
    }
}