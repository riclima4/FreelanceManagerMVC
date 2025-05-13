using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FreelanceManager.Data;
using FreelanceManager.Data.Enum;
using FreelanceManager.IO._shared;

namespace FreelanceManager.IO.Projects
{
    public class ProjectInviteDto : BaseDto<ProjectInvite>
    {
        public ProjectInviteDto() { }
        public ProjectInviteDto(ProjectInvite entity)
        {
            Id = entity.Id;
            ProjectId = entity.ProjectId;
            InvitedApplicationUserId = entity.InvitedApplicationUserId;
            SenderApplicationUserId = entity.SenderApplicationUserId;
            Status = entity.Status;
            InvitedAt = entity.InvitedAt;
            Description = entity.Description;
            Project = entity.Project;
            SenderApplicationUser = entity.SenderApplicationUser;

        }
        public Guid ProjectId { get; set; }
        public string InvitedApplicationUserId { get; set; }
        public string SenderApplicationUserId { get; set; }
        public string Description { get; set; }
        public DateTime InvitedAt { get; set; }
        public ProjectInviteStatus Status { get; set; }
        public Project Project { get; set; }
        public ApplicationUser SenderApplicationUser { get; set; }


    }
}