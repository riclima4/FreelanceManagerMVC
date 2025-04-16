using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FreelanceManager.IO.Clients;

namespace FreelanceManager.Services.Clients
{
    public interface IClientsService
    {
        Task<List<ClientDto>> GetAllAsync();
        Task<ClientDto> GetByIdAsync(Guid id);
        Task<ClientDto> CreateAsync(ClientModel model);
        Task<ClientDto> UpdateAsync(Guid id, ClientModel model);
        Task DeleteAsync(Guid id);
        Task<bool> CanDeleteAsync(Guid id);
        Task<string> GetNextCodeAsync();

    }
}