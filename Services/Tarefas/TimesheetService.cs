using System.Runtime.CompilerServices;
using FreelanceManager.Data;
using FreelanceManager.Data.UnitOfWork;
using FreelanceManager.IO.Tarefas;
using Microsoft.EntityFrameworkCore;

namespace FreelanceManager.Services.Tarefas;

public class TimesheetService : ITimesheetService
{
    private IUnitOfWork _unitOfWork;

    public TimesheetService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<List<TimesheetDto>> GetAllAsync() => await _unitOfWork.TimesheetsRepository.GetEntityAsNoTracking().Select(t => new TimesheetDto(t)).ToListAsync();
    public async Task<TimesheetDto> GetByIdAsync(Guid id) => await _unitOfWork.TimesheetsRepository.GetEntityAsNoTracking(t => t.Id == id).Select(t => new TimesheetDto(t)).FirstAsync();

    public async Task<List<TimesheetDto>> GetByProjectIdAsyncAndUserId(Guid projectId,string userId)
    {
        return await _unitOfWork.TimesheetsRepository
            .GetEntityAsNoTracking(t => t.Tarefa.ProjectId == projectId && t.Tarefa.AssociatedUser.Id == userId)
            .Select(t => new TimesheetDto(t))
            .ToListAsync();
    }

    public async Task<TimesheetDto> CreateAsync(TimesheetModel model)
    {
        int newNumber = await GetNextNumberAsync();
        var entity = await _unitOfWork.TimesheetsRepository.CreateAsync(new Timesheet(model, newNumber));
        return await GetByIdAsync(entity.Id);
    }

    public async Task<TimesheetDto> UpdateAsync(Guid id, TimesheetModel model)
    {
        var entity = await _unitOfWork.TimesheetsRepository.GetEntityAsNoTracking(t => t.Id == id).FirstAsync();

        entity.Date = model.Date;
        entity.Hours = model.Hours;
        entity.Notes = model.Notes;

        await _unitOfWork.TimesheetsRepository.Edit(entity);

        return await GetByIdAsync(id);
    }

    public async Task DeleteAsync(Guid id)
    {
        await _unitOfWork.TimesheetsRepository.Delete(id);
    }
    public Task<bool> CanDeleteAsync(Guid id) => Task.FromResult(true);

    private async Task<int> GetNextNumberAsync()
    {
        List<int> lastInternalNumber = await _unitOfWork.
            TimesheetsRepository.
            GetEntityAsNoTracking().
            Select(entity => entity.InternalNumber).
            ToListAsync();

        int newNumber = 1;
        if (lastInternalNumber.Any())
            newNumber = lastInternalNumber.Max() + 1;

        return newNumber;
    }
    public async Task<string> GetNextCodeAsync()
    {
        int nextNumber = await GetNextNumberAsync();
        return $"TS-{nextNumber:D6}";
    }
}