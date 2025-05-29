using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FreelanceManager.Data.Base;
using FreelanceManager.IO.Tarefas;

namespace FreelanceManager.Data
{
    public class Timesheet : EntityBase
    {
        public Timesheet() { }

        public Timesheet(TimesheetModel model, int number)
        {
            TarefaId = model.TarefaId;
            Notes = model.Notes;
            Hours = model.Hours;
            InternalNumber = number;
            Date = model.Date ?? DateTime.Now;
            UserId = model.UserId;

        }

        public Guid TarefaId { get; set; }
        public string Notes { get; set; }
        public string Hours { get; set; }
        public int InternalNumber { get; set; }
        public DateTime? Date { get; set; }
        public Tarefa Tarefa { get; set; }
        public string UserId { get; set; }
    }
}