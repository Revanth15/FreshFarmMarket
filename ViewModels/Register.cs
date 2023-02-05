using System.ComponentModel.DataAnnotations;


namespace FreshFarmMarket.ViewModels
{
    public class Register
    {

        [Required]
        [RegularExpression(@"^[A-Za-z ]+$", ErrorMessage = "Full Name should only contain alphabets")]
        public string fullName { get; set; }

        [Required]
        [CreditCard]
        public string creditCardNo { get; set; } 

        [Required]
        public string gender { get; set; }

        [Required]
        [DataType(DataType.PhoneNumber)]
        public int mobileNo { get; set; }

        [Required]
        public string deliveryAddress { get; set; }

        //[DataType(DataType.Upload)]
        //[FileExtensions(Extensions = "jpg")]
        public string imageUrl { get; set; }

        [Required]
        public string aboutMe { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        public string email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Compare(nameof(password), ErrorMessage = "Password and confirmation password does not match")]
        public string confirmPassword { get; set; }

    }
}
