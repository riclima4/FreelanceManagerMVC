using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FreelanceManager.Data.Base;
using FreelanceManager.Data.Enum;
using FreelanceManager.IO.Tarefas;

namespace FreelanceManager.Data
{
    public class Tarefa : EntityBase
    {
        public Tarefa() { }
        public Tarefa(TarefaModel model, int number)
        {
            Code = model.Code;
            Name = model.Name;
            Description = model.Description;
            HourlyRate = model.HourlyRate;
            Notes = model.Notes;
            InternalNumber = number;
            ApplicationUserId = model.ApplicationUserId?.ToString();

        }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal? HourlyRate { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public TarefaStatus Status { get; set; } = TarefaStatus.Created;
        public string Notes { get; set; } = string.Empty;
        public int InternalNumber { get; set; }
        public string ApplicationUserId { get; set; }
        public string AssociatedUserId { get; set; }
        public Guid? ProjectId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
        public Project Project { get; set; }
    }
}