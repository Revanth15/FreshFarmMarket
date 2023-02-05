using FreshFarmMarket.Models;
using Microsoft.AspNetCore.Identity;

namespace FreshFarmMarket.Services
{
    public class AuditLogService
    {

        private UserManager<ApplicationUser> _userManager;
        private SignInManager<ApplicationUser> _signInManager;
        private readonly AuthDbContext _db;

        public AuditLogService(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager, AuthDbContext db)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _db = db;
        }

        public void AddLog(AuditLog log)
        {
            _db.AuditLog.Add(log);
            _db.SaveChanges();
        }
    }
}
