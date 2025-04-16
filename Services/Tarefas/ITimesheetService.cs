
using FreelanceManager.IO.Tarefas;

namespace FreelanceManager.Services.Tarefas
{

    public interface ITimesheetService
    {
        Task<List<TimesheetDto>> GetAllAsync();
        Task<TimesheetDto> GetByIdAsync(Guid id);
        Task<TimesheetDto> CreateAsync(TimesheetModel model);
        Task<TimesheetDto> UpdateAsync(Guid id, TimesheetModel model);
        Task DeleteAsync(Guid id);
        Task<bool> CanDeleteAsync(Guid id);
        Task<string> GetNextCodeAsync();

    }
}