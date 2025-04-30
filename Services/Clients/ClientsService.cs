using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FreelanceManager.Data;
using FreelanceManager.Data.UnitOfWork;
using FreelanceManager.IO.Clients;
using Microsoft.EntityFrameworkCore;

namespace FreelanceManager.Services.Clients
{
    public class ClientsService : IClientsService
    {
        readonly IUnitOfWork _unitOfWork;

        public ClientsService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<List<ClientDto>> GetAllAsync() => await _unitOfWork.ClientsRepository.GetEntityAsNoTracking().Select(c => new ClientDto(c)).ToListAsync();
        public async Task<List<ClientDto>> GetAllByApplicationUserIdAsync(string userId) => await _unitOfWork.ClientsRepository.GetEntityAsNoTracking(c => c.ApplicationUserId == userId).Select(c => new ClientDto(c)).ToListAsync();

        public async Task<ClientDto> GetByIdAsync(Guid id) => await _unitOfWork.ClientsRepository.GetEntityAsNoTracking(v => v.Id == id).Select(a => new ClientDto(a)).FirstAsync();

        public async Task<ClientDto> CreateAsync(ClientModel model)
        {
            int newNumber = await GetNextNumberAsync();
            var entity = await _unitOfWork.ClientsRepository.CreateAsync(new Client(model, newNumber));
            return await GetByIdAsync(entity.Id);
        }

        public async Task<ClientDto> UpdateAsync(Guid id, ClientModel model)
        {
            var entity = await _unitOfWork.
                 ClientsRepository.
                 GetEntityAsNoTracking(p => p.Id == id).
                 FirstAsync();

            entity.IsActive = model.IsActive;
            entity.Name = model.Name;
            entity.FiscalNumber = model.FiscalNumber;
            entity.Street = model.Street;
            entity.Address = model.Address;
            entity.ZipCode = model.ZipCode;
            entity.City = model.City;
            entity.Country = model.Country;
            entity.Website = model.Website;
            entity.Email = model.Email;
            entity.Phone = model.Phone;
            entity.Notes = model.Notes;

            await _unitOfWork.ClientsRepository.Edit(entity);

            return await GetByIdAsync(id);
        }

        public async Task DeleteAsync(Guid id)
        {
            await _unitOfWork.ClientsRepository.FakeDelete(id);
        }

        public Task<bool> CanDeleteAsync(Guid id) => Task.FromResult(true);

        private async Task<int> GetNextNumberAsync()
        {
            List<int> lastInternalNumber = await _unitOfWork.
                ClientsRepository.
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
            return $"CLI{newNumber.ToString("0000")}";
        }

    }
}