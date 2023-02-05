using System.ComponentModel.DataAnnotations;

namespace FreshFarmMarket.Models
{
    public class AuditLog
    {

        public int Id { get; set; }
        [Required] 
        public string userEmail { get; set; }
        [Required]
        public string LogName { get; set; }

        public DateTimeOffset CreatedDate { get; set; } = DateTimeOffset.Now;
    }
}
