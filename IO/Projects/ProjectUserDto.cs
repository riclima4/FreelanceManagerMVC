using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FreelanceManager.Data;
using FreelanceManager.Data.Enum;
using FreelanceManager.IO._shared;
using FreelanceManager.IO.ApplicationUsers;

namespace FreelanceManager.IO.Projects
{
    public class ProjectUserDto : BaseDto<ProjectUser>
    {
        public ProjectUserDto()
        {

        }
        public ProjectUserDto(ProjectUser entity)
        {
            Id = entity.Id;
            ProjectId = entity.ProjectId;
            ApplicationUserId = entity.ApplicationUserId;
            Role = entity.Role;
            JoinedAt = entity.JoinedAt;
            RemovedAt = entity.RemovedAt;
            ApplicationUser = entity.ApplicationUser != null ? new ApplicationUserDto(entity.ApplicationUser) : new ApplicationUserDto();
        }
        public Guid ProjectId { get; set; }
        public string ApplicationUserId { get; set; }
        public ApplicationUserType Role { get; set; }
        public DateTime? JoinedAt { get; set; }
        public DateTime? RemovedAt { get; set; }
        public ApplicationUserDto ApplicationUser { get; set; }

    }
}