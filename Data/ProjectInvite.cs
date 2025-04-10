using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FreelanceManager.Data.Entities.Base;
using FreelanceManager.Data.Entities.Enum;
using FreelanceManager.IO.Projects;

namespace FreelanceManager.Data
{
    public class ProjectInvite : EntityBase
    {
        public ProjectInvite()
        {

        }
        public ProjectInvite(ProjectInviteModel model)
        {
            ProjectId = model.ProjectId;
            InvitedApplicationUserId = model.InvitedApplicationUserId;
            SenderApplicationUserId = model.SenderApplicationUserId;
            Status = model.Status;
            InvitedAt = model.InvitedAt;
            Description = model.Description;
        }
        public Guid ProjectId { get; set; }
        public string InvitedApplicationUserId { get; set; }
        public string SenderApplicationUserId { get; set; }
        public ProjectInviteStatus Status { get; set; } = ProjectInviteStatus.Pending;
        public DateTime InvitedAt { get; set; } = DateTime.UtcNow;
        public string Description { get; set; } = string.Empty;
        public ApplicationUser InvitedApplicationUser { get; set; }
        public ApplicationUser SenderApplicationUser { get; set; }
        public Project Project { get; set; }

    }
}