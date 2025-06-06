using FreelanceManager.Data;
using FreelanceManager.IO._shared;


namespace FreelanceManager.IO.Tarefas
{
    public class TimesheetDto : BaseDto<Timesheet>
    {
        public TimesheetDto()
        {
            Hours = string.Empty;
            Notes = string.Empty;
        }

        public TimesheetDto(Timesheet entity) : base(entity)
        {
            Id = entity.Id;
            Date = entity.Date ?? DateTime.Now;
            Data = entity.Date ?? DateTime.Now; // Add alias for Date
            Hours = entity.Hours;
            Notes = entity.Notes;
            UserId = entity.UserId;
            TarefaId = entity.TarefaId;
            
            // Parse hours for calculations
            if (double.TryParse(entity.Hours, out double parsedHours))
            {
                HoursWorked = parsedHours;
            }
            
            // Get hourly rate from tarefa if available
            if (entity.Tarefa != null)
            {
                HourlyRate = entity.Tarefa.HourlyRate ?? 0;
                User = entity.Tarefa.AssociatedUser;
            }
        }

        public DateTime Date { get; set; }
        public DateTime Data { get; set; } // Alias for Date (Portuguese)
        public string Hours { get; set; }
        public double? HoursWorked { get; set; } // Parsed hours for calculations
        public decimal HourlyRate { get; set; } // Hourly rate from task
        public string Notes { get; set; }
        public Guid TarefaId { get; set; }
        public string UserId { get; set; }
        public ApplicationUser User { get; set; } // User information
    }
}