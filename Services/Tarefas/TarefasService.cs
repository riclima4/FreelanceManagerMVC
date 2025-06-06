using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FreelanceManager.Data;
using FreelanceManager.Data.UnitOfWork;
using FreelanceManager.IO.Tarefas;
using Microsoft.EntityFrameworkCore;

namespace FreelanceManager.Services.Tarefas
{
    public class TarefasService : ITarefasService
    {
        private IUnitOfWork _unitOfWork;

        public TarefasService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }


        public async Task<List<TarefaDto>> GetAllAsync() => await _unitOfWork.TarefasRepository.GetEntityAsNoTracking().Select(t => new TarefaDto(t)).ToListAsync();

        public async Task<TarefaDto> GetByIdAsync(Guid id) => await _unitOfWork.TarefasRepository.GetEntityAsNoTracking(t => t.Id == id).Select(t => new TarefaDto(t)).FirstAsync();

        public async Task<List<TarefaDto>> GetAllByApplicationUserIdAsync(string id) => await _unitOfWork.TarefasRepository.GetEntityAsNoTracking(t => t.ApplicationUserId == id).Select(t => new TarefaDto(t)).ToListAsync();
        public async Task<List<TarefaDto>> GetPersonalByApplicationUserIdAsync(string id) => await _unitOfWork.TarefasRepository.GetEntityAsNoTracking(t => t.ApplicationUserId == id && t.ProjectId == null).Select(t => new TarefaDto(t)).ToListAsync();

        public async Task<TarefaDto> CreateAsync(TarefaModel model)
        {
            int newNumber = await GetNextNumberAsync();
            model.Code = await GetNextCodeAsync();
            var entity = await _unitOfWork.TarefasRepository.CreateAsync(new Tarefa(model, newNumber));
            await _unitOfWork.CommitAsync();
            return await GetByIdAsync(entity.Id);
        }

        public async Task<TarefaDto> UpdateAsync(Guid id, TarefaModel model)
        {
            var entity = await _unitOfWork.TarefasRepository.GetEntityAsNoTracking(t => t.Id == id).FirstAsync();

            entity.Code = model.Code;
            entity.Name = model.Name;
            entity.Description = model.Description;
            entity.StartDate = model.StartDate;
            entity.EndDate = model.EndDate;
            entity.Notes = model.Notes;
            entity.Status = model.Status;
            entity.AssociatedUserId = model.AssociatedUserId;
            entity.ProjectId = model.ProjectId;
            entity.HourlyRate = model.HourlyRate;
            await _unitOfWork.TarefasRepository.Edit(entity);
            await _unitOfWork.CommitAsync();

            return await GetByIdAsync(id);
        }

        public async Task DeleteAsync(Guid id)
        {
            await _unitOfWork.TarefasRepository.Delete(id);
        }

        public Task<bool> CanDeleteAsync(Guid id) => Task.FromResult(true);

        private async Task<int> GetNextNumberAsync()
        {
            List<int> lastInternalNumber = await _unitOfWork.
                TarefasRepository.
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
            int newNumber = await GetNextNumberAsync();
            return $"TAR{newNumber.ToString("0000")}";
        }        public async Task<List<TarefaDto>> GetTarefasByProjectIdAsync(Guid projectId)
        {
            return await _unitOfWork.TarefasRepository
                .GetEntityAsNoTracking(t => t.ProjectId == projectId)
                .Include(t => t.ApplicationUser)
                .Include(t => t.AssociatedUser)
                .Select(t => new TarefaDto(t))
                .ToListAsync();
        }

        public async Task<List<TimesheetDto>> GetTimesheetsByProjectIdAsync(Guid projectId, DateTime startDate, DateTime endDate)
        {
            return await _unitOfWork.TimesheetsRepository
                .GetEntityAsNoTracking(t => t.Tarefa.ProjectId == projectId && 
                                           t.Date >= startDate && 
                                           t.Date <= endDate)
                .Include(t => t.Tarefa)
                .ThenInclude(t => t.AssociatedUser)
                .Select(t => new TimesheetDto(t))
                .ToListAsync();
        }
    }
}