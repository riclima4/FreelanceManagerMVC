using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using FreelanceManager.Data.Enum;

namespace FreelanceManager.IO.Projects
{
    public class ProjectModel
    {
        public Guid? Id { get; set; }
        public string Code { get; set; }
        [Required, MaxLength(100)]
        public string Name { get; set; }
        [Required, MaxLength(500)]
        public string Description { get; set; }
        public Guid? ClientId { get; set; }
        public string ApplicationUserId { get; set; } = string.Empty;
        public ProjectStatus Status { get; set; } = ProjectStatus.Created;
        [MaxLength(500)]
        public string Notes { get; set; } = string.Empty;
        public bool IsActive { get; set; }

    }
}