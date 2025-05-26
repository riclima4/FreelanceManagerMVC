using FreelanceManager.IO.ApplicationUsers;
using Microsoft.AspNetCore.Identity;

namespace FreelanceManager.Data
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser
    {
        public ApplicationUser()
        {
        }
        public ApplicationUser(ApplicationUserModel model)
        {
            Email = model.Email;
            UserName = model.UserName;
            FullName = model.FullName;
            PhoneNumber = model.PhoneNumber;
            DailyHours = model.DailyHours;
            SecurityStamp = Guid.NewGuid().ToString();
            LockoutEnabled = false;
            EmailConfirmed = true;
            CreatedAt = DateTime.Now;
        }

        public string FullName { get; set; } = string.Empty;
        public string DailyHours { get; set; } = string.Empty;
        public DateTime? CreatedAt { get; set; }
        public string CreatedBy { get; set; } = "unknown";
        public DateTime? UpdatedAt { get; set; }
        public string UpdatedBy { get; set; } = "unknown";
        public bool IsDeleted { get; set; } = false;
    }

}
