using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace FreshFarmMarket.Models
{
	public class ApplicationUser : IdentityUser
	{

		public string fullName { get; set; }
		public string creditCardNo { get; set; }
		public string gender { get; set; }
		public int mobileNo { get; set; }
		public string deliveryAddress { get; set; }
		public string? imageURL { get; set; }
		public string aboutMe { get; set; }
		public DateTime lastPasswordChangeDate { get; set; }
	}
}
