using FreshFarmMarket.Models;
using Microsoft.AspNetCore.Identity;

namespace FreshFarmMarket.Services
{
    public class PreviousPasswordsService
    {
        private UserManager<ApplicationUser> _userManager;
        private SignInManager<ApplicationUser> _signInManager;
        private readonly AuthDbContext _db;

        public PreviousPasswordsService(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager, AuthDbContext db)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _db = db;
        }

        public List<PreviousPasswords>  Getlast2passwords(ApplicationUser user)
        {
            var hashs = _db.PreviousPasswords.Where(item => item.userId == user.Id)
                    .OrderBy(item => item.CreatedDate)
                    .Take(2).ToList();
            return hashs;
        }

        public void AddHash(PreviousPasswords hash)
        {
            _db.PreviousPasswords.Add(hash);
            _db.SaveChanges();
        }
    }
}
