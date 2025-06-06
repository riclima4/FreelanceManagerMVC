using FreelanceManager.Data;
using FreelanceManager.IO._shared;


namespace FreelanceManager.IO.Tarefas
{
    public class TimesheetDto : BaseDto<Timesheet>
    {
        public TimesheetDto(Timesheet entity) : base(entity)
        {
            Id = entity.Id;
            Date = entity.Date ?? DateTime.Now;
            Hours = entity.Hours;
            Notes = entity.Notes;
            UserId = entity.UserId;
            TarefaId = entity.TarefaId;

            if (entity.Tarefa != null)
            {
                Tarefa = new TarefaDto(entity.Tarefa);
            }
        }
        public DateTime Date { get; set; }
        public string Hours { get; set; }
        public string Notes { get; set; }
        public Guid TarefaId { get; set; }
        public string UserId { get; set; }
        public ApplicationUser User { get; set; } 
        public TarefaDto Tarefa { get; set; }
        
        public double HoursWorked => double.TryParse(Hours, out double hours) ? hours : 0.0;
        
        public decimal HourlyRate => Tarefa?.HourlyRate ?? 0m;
    }
}