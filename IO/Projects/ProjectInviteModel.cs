using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FreelanceManager.Data.Entities.Enum;

namespace FreelanceManager.IO.Projects
{
    public class ProjectInviteModel
    {
        public Guid ProjectId { get; set; }
        public string InvitedApplicationUserId { get; set; } = string.Empty;
        public string SenderApplicationUserId { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime InvitedAt { get; set; } = DateTime.UtcNow;
        public ProjectInviteStatus Status { get; set; } = ProjectInviteStatus.Pending;


    }
}