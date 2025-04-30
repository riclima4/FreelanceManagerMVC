using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FreelanceManager.IO.Tarefas;

namespace FreelanceManager.Services.Tarefas
{
    public interface ITarefasService
    {
        Task<List<TarefaDto>> GetAllAsync();
        Task<TarefaDto> GetByIdAsync(Guid id);
        Task<List<TarefaDto>> GetAllByApplicationUserIdAsync(string id);
        Task<List<TarefaDto>> GetPersonalByApplicationUserIdAsync(string id);
        Task<TarefaDto> CreateAsync(TarefaModel model);
        Task<TarefaDto> UpdateAsync(Guid id, TarefaModel model);
        Task DeleteAsync(Guid id);
        Task<bool> CanDeleteAsync(Guid id);
        Task<string> GetNextCodeAsync();

    }
}