using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using FreshFarmMarket.ViewModels;
using Microsoft.AspNetCore.Identity;
using FreshFarmMarket.Models;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.DataProtection;

namespace FreshFarmMarket.Pages
{
    public class RegisterModel : PageModel
    {
        private UserManager<ApplicationUser> userManager { get; }
        private SignInManager<ApplicationUser> signInManager { get; }
        private IWebHostEnvironment _environment;


        public RegisterModel(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IWebHostEnvironment environment)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            _environment = environment;
        }

        [BindProperty]
        public string fullName { get; set; }
        [BindProperty]
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
                    TempData["FlashMessage.Type"] = "error";
                    TempData["FlashMessage.Text"] = "Input required";
                }

                newUser.fullName = fullName;
                newUser.email = email;
                newUser.password = password;
                newUser.confirmPassword = confirmPassword;
                newUser.creditCardNo = creditCardNo;
                newUser.gender = gender;
                newUser.mobileNo = mobileNo;
                newUser.deliveryAddress = deliveryAddress;
                newUser.aboutMe = aboutMe;

                var user = new ApplicationUser()
                {
                    UserName = email,
                    Email = email,
                    fullName = fullName,
                    creditCardNo = protector.Protect(creditCardNo),
                    gender = gender,
                    mobileNo = mobileNo,
                    deliveryAddress = deliveryAddress,
                    imageURL = newUser.imageUrl,
                    aboutMe = aboutMe
                };
                var result = await userManager.CreateAsync(user, newUser.password);
                if (result.Succeeded)
                {
                    await signInManager.SignInAsync(user, false);
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
