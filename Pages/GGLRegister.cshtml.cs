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
    public class GGLRegisterModel : PageModel
    {
        private UserManager<ApplicationUser> userManager { get; }
        private SignInManager<ApplicationUser> signInManager { get; }
        private RoleManager<IdentityRole> roleManager;
        private readonly AuditLogService _auditlogService;
        private IWebHostEnvironment _environment;
        private readonly PreviousPasswordsService _previousPasswordsService;

        public GGLRegisterModel(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IWebHostEnvironment environment, AuditLogService auditlogService, PreviousPasswordsService previousPasswordsService, RoleManager<IdentityRole> roleManager)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            _environment = environment;
            _auditlogService = auditlogService;
            _previousPasswordsService = previousPasswordsService;
            this.roleManager = roleManager;
        }

        [BindProperty]
        public string fullName { get; set; }
        [BindProperty]
        [CreditCard]
        public string creditCardNo { get; set; }
        [BindProperty]
        public string gender { get; set; }
        [BindProperty]
        [DataType(DataType.PhoneNumber)]
        public int mobileNo { get; set; }
        [BindProperty]
        public string deliveryAddress { get; set; }
        [BindProperty]
        public string aboutMe { get; set; }
        [BindProperty]
        [DataType(DataType.EmailAddress)]
        public string email { get; set; }

        public string? imageUrl { get; set; }


        public void OnGet(string Email, string name,string pfp)
        {
            fullName = name;
            email = Email;
            imageUrl = pfp;

        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                var dataProtectionProvider = DataProtectionProvider.Create("EncryptData");
                var protector = dataProtectionProvider.CreateProtector("MySecretKey");
                
                var user = new ApplicationUser()
                {
                    UserName = HttpUtility.HtmlEncode(email),
                    Email = HttpUtility.HtmlEncode(email),
                    fullName = HttpUtility.HtmlEncode(fullName),
                    creditCardNo = protector.Protect(HttpUtility.HtmlEncode(creditCardNo)),
                    gender = HttpUtility.HtmlEncode(gender),
                    mobileNo = mobileNo,
                    deliveryAddress = HttpUtility.HtmlEncode(deliveryAddress),
                    imageURL = imageUrl,
                    aboutMe = HttpUtility.HtmlEncode(aboutMe),
                    lastPasswordChangeDate = DateTime.Now
                };

                IdentityRole role = await roleManager.FindByIdAsync("Admin");
                if (role == null)
                {
                    IdentityResult Iresult = await roleManager.CreateAsync(new IdentityRole("Admin"));
                    if (!Iresult.Succeeded)
                    {
                        ModelState.AddModelError("", "Create role admin failed");
                    }
                }

                var result = await userManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    AuditLog log = new();
                    log.userEmail = HttpUtility.HtmlEncode(email);
                    log.LogName = "User registered successfully through Google";
                    _auditlogService.AddLog(log);
                    return RedirectToPage("Index");
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            return Page();
        }

    }
}
