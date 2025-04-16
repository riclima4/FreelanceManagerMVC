using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FreelanceManager.Data.Base;
using FreelanceManager.Data.Enum;
using FreelanceManager.IO.Projects;

namespace FreelanceManager.Data
{
    public class ProjectUser : EntityBase
    {
        public ProjectUser() { }
        public ProjectUser(ProjectUserModel model)
        {
            ProjectId = model.ProjectId;
            ApplicationUserId = model.ApplicationUserId;
            Role = model.Role;
            JoinedAt = model.JoinedAt;
            RemovedAt = model.RemovedAt;
        }
        public Guid ProjectId { get; set; }
        public string ApplicationUserId { get; set; }
        public ApplicationUserType Role { get; set; } = ApplicationUserType.Normal;
        public DateTime? JoinedAt { get; set; }
        public DateTime? RemovedAt { get; set; }
        public string Notes { get; set; }
        public Project Project { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
    }
}