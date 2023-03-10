using System.ComponentModel.DataAnnotations;

namespace FreshFarmMarket.Models
{
    public class PreviousPasswords
    {
        public int Id { get; set; }
        [Required]
        public string userId { get; set; }
        [Required]
        public string passwordHash { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;

    }
}
