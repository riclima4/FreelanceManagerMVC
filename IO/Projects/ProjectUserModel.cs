using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FreelanceManager.Data.Entities.Enum;

namespace FreelanceManager.IO.Projects
{
    public class ProjectUserModel
    {
        public Guid? Id { get; set; }
        public Guid ProjectId { get; set; }
        public string ApplicationUserId { get; set; }
        public ApplicationUserType Role { get; set; } = ApplicationUserType.Normal;
        public DateTime? JoinedAt { get; set; }
        public DateTime? RemovedAt { get; set; }
        public string Notes { get; set; }
    }
}