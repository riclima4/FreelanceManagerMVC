using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FreelanceManager.Data;

namespace FreelanceManager.IO.ApplicationUsers
{
    public class ApplicationUserDto
    {
        public ApplicationUserDto()
        {

        }
        public ApplicationUserDto(ApplicationUser entity)
        {
            Id = entity.Id;
            UserName = entity.UserName;
            FullName = entity.FullName;
            Email = entity.Email;
            PhoneNumber = entity.PhoneNumber;
            LockoutEnabled = entity.LockoutEnabled;
        }
        public ApplicationUserDto(ApplicationUser entity, List<string> roles = null)
        {
            Id = entity.Id;
            UserName = entity.UserName;
            FullName = entity.FullName;
            Email = entity.Email;
            PhoneNumber = entity.PhoneNumber;
            LockoutEnabled = entity.LockoutEnabled;
            Roles = roles;
        }

        public string Id { get; set; }
        public string UserName { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public bool LockoutEnabled { get; set; }
        public List<string> Roles { get; set; } = new();
    }
}