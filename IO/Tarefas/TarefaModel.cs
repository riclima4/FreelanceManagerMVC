using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using FreelanceManager.Data.Enum;

namespace FreelanceManager.IO.Tarefas
{
    public class TarefaModel
    {
        public Guid? Id { get; set; }
        public string Code { get; set; }
        [Required, MaxLength(100)]
        public string Name { get; set; }
        [Required, MaxLength(100)]
        public string Description { get; set; }
        public decimal? HourlyRate { get; set; } = 0;
        [MaxLength(100)]
        public string Notes { get; set; }
        public Guid? ProjectId { get; set; }
        public string ApplicationUserId { get; set; }
        public string AssociatedUserId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public TarefaStatus Status { get; set; } = TarefaStatus.Created;

    }
}