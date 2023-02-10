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
    public class RegisterModel : PageModel
    {
        private UserManager<ApplicationUser> userManager { get; }
        private SignInManager<ApplicationUser> signInManager { get; }
        private RoleManager<IdentityRole> roleManager;
        private readonly AuditLogService _auditlogService;
        private IWebHostEnvironment _environment;
        private readonly PreviousPasswordsService _previousPasswordsService;

        public RegisterModel(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IWebHostEnvironment environment, AuditLogService auditlogService, PreviousPasswordsService previousPasswordsService, RoleManager<IdentityRole> roleManager)
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
        [BindProperty]
        public string password { get; set; }
        [BindProperty]
        [Compare(nameof(password), ErrorMessage = "Password and confirmation password does not match")]
        public string confirmPassword { get; set; }

        [BindProperty]
        public IFormFile? Upload { get; set; }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            Register newUser = new();
            if (ModelState.IsValid)
            {
                var dataProtectionProvider = DataProtectionProvider.Create("EncryptData");
                var protector = dataProtectionProvider.CreateProtector("MySecretKey");
                if (Upload != null)
                {
                    if (Upload.Length > 2 * 1024 * 1024)
                    {
                        ModelState.AddModelError("Upload",
                        "File size cannot exceed 2MB.");
                        return Page();
                    }
                    var uploadsFolder = "uploads/userUploads";
                    var imageFile = Guid.NewGuid() + Path.GetExtension(
                    Upload.FileName);
                    var imagePath = Path.Combine(_environment.ContentRootPath,
                    "wwwroot", uploadsFolder, imageFile);
                    using var fileStream = new FileStream(imagePath, FileMode.Create);
                    await Upload.CopyToAsync(fileStream);
                    newUser.imageUrl = string.Format("/{0}/{1}", uploadsFolder, imageFile);
                }
                else
                {
                    TempData["FlashMessage.Type"] = "danger";
                    TempData["FlashMessage.Text"] = "Input required";
                }

                //newUser.fullName = HttpUtility.HtmlEncode(fullName);
                //newUser.email = HttpUtility.HtmlEncode(email);
                newUser.password = HttpUtility.HtmlEncode(password);
                //newUser.confirmPassword = HttpUtility.HtmlEncode(confirmPassword);
                //newUser.creditCardNo = HttpUtility.HtmlEncode(creditCardNo);
                //newUser.gender = HttpUtility.HtmlEncode(gender);
                //newUser.mobileNo = mobileNo;
                //newUser.deliveryAddress = HttpUtility.HtmlEncode(deliveryAddress);
                //newUser.aboutMe = HttpUtility.HtmlEncode(aboutMe);

                var user = new ApplicationUser()
                {
                    UserName = HttpUtility.HtmlEncode(email),
                    Email = HttpUtility.HtmlEncode(email),
                    fullName = HttpUtility.HtmlEncode(fullName),
                    creditCardNo = protector.Protect(HttpUtility.HtmlEncode(creditCardNo)),
                    gender = HttpUtility.HtmlEncode(gender),
                    mobileNo = mobileNo,
                    deliveryAddress = HttpUtility.HtmlEncode(deliveryAddress),
                    imageURL = newUser.imageUrl,
                    aboutMe = HttpUtility.HtmlEncode(aboutMe),
                    lastPasswordChangeDate = DateTime.Now,
                    TwoFactorEnabled = true,
                    EmailConfirmed = true
                };

                IdentityRole role = await roleManager.FindByIdAsync("Admin");
                if(role == null)
                {
                    IdentityResult Iresult = await roleManager.CreateAsync(new IdentityRole("Admin"));
                    if (!Iresult.Succeeded)
                    {
                        ModelState.AddModelError("", "Create role admin failed");
                    }
                }

                var result = await userManager.CreateAsync(user, newUser.password);
                PreviousPasswords hash = new();
                hash.userId = user.Id;
                hash.passwordHash = user.PasswordHash;
                _previousPasswordsService.AddHash(hash);
                if (result.Succeeded)
                {
                    //result = await userManager.AddToRoleAsync(user, "Admin");
                    AuditLog log = new();
                    log.userEmail = HttpUtility.HtmlEncode(email);
                    log.LogName = "User registered successfully";
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
