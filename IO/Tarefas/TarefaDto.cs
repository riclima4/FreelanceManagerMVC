using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FreelanceManager.Data;
using FreelanceManager.Data.Enum;
using FreelanceManager.IO._shared;

namespace FreelanceManager.IO.Tarefas
{
    public class TarefaDto : BaseDto<Tarefa>
    {
        public TarefaDto() { }
        public TarefaDto(Tarefa entity) : base(entity)
        {
            Id = entity.Id;
            Code = entity.Code;
            Name = entity.Name;
            Description = entity.Description;
            Notes = entity.Notes;
            StartDate = entity.StartDate;
            EndDate = entity.EndDate;
            Status = entity.Status;
            ProjectId = entity.ProjectId;
            ApplicationUserId = entity.ApplicationUserId;
            AssociatedUserId = entity.AssociatedUserId;
            HourlyRate = entity.HourlyRate;
            StatusDescription = entity.Status.ToString();

        }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Notes { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public Guid? ProjectId { get; set; }
        public string ApplicationUserId { get; set; }
        public string AssociatedUserId { get; set; }
        public decimal? HourlyRate { get; set; }
        public TarefaStatus Status { get; set; } = TarefaStatus.Created;
        public string StatusDescription { get; set; }

    }
}