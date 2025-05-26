using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FreelanceManager.Data;
using FreelanceManager.Data.Enum;
using FreelanceManager.Data.UnitOfWork;
using FreelanceManager.IO.Projects;
using FreelanceManager.IO.Tarefas;
using Microsoft.EntityFrameworkCore;

namespace FreelanceManager.Services.Projects
{
    public class ProjectsService : IProjectsService
    {
        private IUnitOfWork _unitOfWork;
        public ProjectsService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<List<ProjectDto>> GetAllAsync() => await _unitOfWork.ProjectsRepository.GetEntityAsNoTracking().Select(c => new ProjectDto(c)).ToListAsync();

        public async Task<List<ProjectDto>> GetByUserIdAsync(string userId)
        {

            return await _unitOfWork
                .ProjectsRepository
                .GetEntityAsNoTracking(p => p.ApplicationUserId == userId)
                .Include(p => p.Client)
                .Include(p => p.Tarefas)
                .Include(p => p.ProjectUsers)
                .Select(p => new ProjectDto(p))
                .ToListAsync();
        }
        public async Task<ProjectDto> GetByIdAsync(Guid id) => await _unitOfWork.ProjectsRepository.GetEntityAsNoTracking(v => v.Id == id).Include(c => c.Client).Include(v => v.Tarefas).ThenInclude(u => u.AssociatedUser).Include(pu => pu.ProjectUsers).ThenInclude(u => u.ApplicationUser).Select(a => new ProjectDto(a)).FirstAsync();

        public async Task<List<ProjectDto>> GetProjectContainsInProjectUsersAsync(string userId)
        {
            return await _unitOfWork.
                ProjectsRepository.
                GetEntityAsNoTracking(entity => entity.ProjectUsers.Any(p => p.ApplicationUserId == userId)).
                Include(entity => entity.Client).
                Include(entity => entity.Tarefas).
                Include(entity => entity.ProjectUsers).
                Select(entity => new ProjectDto(entity)).
                ToListAsync();
        }

        public async Task<ProjectDto> CreateAsync(ProjectModel model)
        {
            int newNumber = await GetNextNumberAsync();
            model.Code = await GetNextCodeAsync();
            var entity = await _unitOfWork.ProjectsRepository.CreateAsync(new Project(model, newNumber));
            // Ao criar o project é necessário criar o ProjectUser
            var modelProjectUser = new ProjectUserModel
            {
                ApplicationUserId = model.ApplicationUserId!,
                ProjectId = entity.Id,
                Role = ApplicationUserType.Admin,
                JoinedAt = DateTime.Now,
            };

            await CreateProjectUserAsync(modelProjectUser);

            return await GetByIdAsync(entity.Id);
        }

        public async Task<ProjectDto> UpdateProjectStatusAsync(Guid id, ProjectStatus status)
        {
            var entity = await _unitOfWork.
                ProjectsRepository.
                GetEntityAsNoTracking(p => p.Id == id).
                FirstAsync();

            entity.Status = status;

            await _unitOfWork.ProjectsRepository.Edit(entity);

            return await GetByIdAsync(id);
        }

        public async Task<ProjectDto> UpdateAsync(Guid id, ProjectModel model)
        {
            var entity = await _unitOfWork.
                 ProjectsRepository.
                 GetEntityAsNoTracking(p => p.Id == id).
                 FirstAsync();

            entity.Code = model.Code;
            entity.Name = model.Name;
            entity.Description = model.Description;
            entity.Status = model.Status;
            entity.Notes = model.Notes;
            entity.ClientId = model.ClientId;
            entity.IsActive = model.IsActive;

            await _unitOfWork.ProjectsRepository.Edit(entity);

            return await GetByIdAsync(id);
        }

        public async Task DeleteAsync(Guid id)
        {
            await _unitOfWork.ProjectsRepository.FakeDelete(id);
        }

        public Task<bool> CanDeleteAsync(Guid id) => Task.FromResult(true);

        private async Task<int> GetNextNumberAsync()
        {
            List<int> lastInternalNumber = await _unitOfWork.
                ProjectsRepository.
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
            return $"PROJ{newNumber.ToString("0000")}";
        }

        #region ProjectUsers



        public async Task<List<ProjectUserDto>> GetProjectUsersAsync(Guid projectId)
        {
            return await _unitOfWork.
                ProjectUsersRepository.
                GetEntityAsNoTracking(entity => entity.ProjectId == projectId).
                Include(entity => entity.ApplicationUser).
                Include(entity => entity.Project).
                Select(entity => new ProjectUserDto(entity)).
                ToListAsync();
        }
        public async Task<ProjectUserDto> GetProjectUserByIdAsync(Guid id)
        {
            return await _unitOfWork.
              ProjectUsersRepository.
              GetEntityAsNoTracking(entity => entity.Id == id).
              Include(entity => entity.ApplicationUser).
              Include(entity => entity.Project).
              Select(entity => new ProjectUserDto(entity)).
              FirstAsync();
        }

        public async Task<ProjectUserDto> CreateProjectUserAsync(ProjectUserModel model)
        {
            ProjectUser entity = await _unitOfWork.
              ProjectUsersRepository.
              CreateAsync(new ProjectUser(model));

            return await GetProjectUserByIdAsync(entity.Id);
        }

        public async Task DeleteProjectUserAsync(ProjectUserModel model)
        {
            var entity = await _unitOfWork.
                ProjectUsersRepository.
                GetEntity(entity =>
                entity.ProjectId == model.ProjectId).
                FirstAsync();

            if (entity is not null)
            {
                await _unitOfWork.ProjectUsersRepository.Delete(entity);
            }
        }
        public async Task<bool> DeleteProjectUserByUserIdAndProjectId(string id, Guid projectId)
        {
            var entity = await _unitOfWork.
                ProjectUsersRepository.
                GetEntity(entity => entity.ApplicationUserId == id && entity.ProjectId == projectId).
                FirstAsync();

            if (entity is not null)
            {
                await _unitOfWork.ProjectUsersRepository.Delete(entity);
            }
            return true;
        }
        public async Task<bool> CanSaveProjectUserAsync(ProjectUserModel model)
        {
            var existUser = await _unitOfWork.
              ProjectUsersRepository.
              GetEntityAsNoTracking(entity => entity.ProjectId == model.ProjectId && entity.ApplicationUserId == model.ApplicationUserId).
              Include(entity => entity.ApplicationUser).
              Include(entity => entity.Project).
              Select(entity => new ProjectUserDto(entity)).
              AnyAsync();

            return existUser is false;
        }
        #endregion
        #region Tarefas

        public async Task<List<TarefaDto>> GetTarefasByProjectIdAndAssociatedUserIdAsync(Guid projectId, string AssociatedUserId)
        {
            return await _unitOfWork.
                TarefasRepository.
                GetEntityAsNoTracking(entity => entity.ProjectId == projectId && entity.AssociatedUserId == AssociatedUserId).
                Select(entity => new TarefaDto(entity)).
                ToListAsync();
        }
        public async Task<List<TarefaDto>> GetTarefasAsync(Guid projectId)
        {
            return await _unitOfWork.
                TarefasRepository.
                GetEntityAsNoTracking(entity => entity.ProjectId == projectId).
                Include(entity => entity.ApplicationUser).
                Include(entity => entity.Project).
                Include(entity => entity.AssociatedUser).
                Select(entity => new TarefaDto(entity)).
                ToListAsync();
        }

        #endregion

        #region ProjectInvites
        public async Task<List<ProjectInviteDto>> GetProjectInvitesAsync(Guid projectId)
        {
            return await _unitOfWork.
                ProjectInvitesRepository.
                GetEntityAsNoTracking(entity => entity.ProjectId == projectId).
                Select(entity => new ProjectInviteDto(entity)).
                ToListAsync();
        }

        public async Task<ProjectInviteDto> CreateProjectInviteAsync(ProjectInviteModel model)
        {
            var entity = await _unitOfWork.
                ProjectInvitesRepository.
                CreateAsync(new ProjectInvite(model));
            await _unitOfWork.CommitAsync();
            return await GetProjectInviteByIdAsync(entity.Id);
        }
        public async Task<List<ProjectInviteDto>> GetProjectInvitesByUserIdAsync(string userId)
        {
            return await _unitOfWork.
                ProjectInvitesRepository.
                GetEntityAsNoTracking(entity => entity.InvitedApplicationUserId == userId).
                Include(entity => entity.Project).
                Include(entity => entity.SenderApplicationUser).
                Select(entity => new ProjectInviteDto(entity)).
                ToListAsync();
        }
        public async Task<ProjectInviteDto> GetProjectInviteByIdAsync(Guid id)
        {
            return await _unitOfWork.
                ProjectInvitesRepository.
                GetEntityAsNoTracking(entity => entity.Id == id).
                Include(entity => entity.InvitedApplicationUser).
                Include(entity => entity.SenderApplicationUser).
                Include(entity => entity.Project).
                Select(entity => new ProjectInviteDto(entity)).
                FirstAsync();
        }
        public async Task DeleteProjectInviteAsync(ProjectInviteModel model)
        {
            var entity = await _unitOfWork.
                ProjectInvitesRepository.
                GetEntity(entity => entity.ProjectId == model.ProjectId).
                FirstAsync();

            if (entity is not null)
            {
                await _unitOfWork.ProjectInvitesRepository.Delete(entity);
            }
        }
        public async Task<ProjectInviteDto> UpdateProjectInviteAsync(Guid id, ProjectInviteModel model)
        {
            var entity = await _unitOfWork.
                ProjectInvitesRepository.
                GetEntityAsNoTracking(entity => entity.Id == id).
                FirstAsync();

            entity.Status = model.Status;
            entity.Description = model.Description;


            await _unitOfWork.ProjectInvitesRepository.Edit(entity);
            
            if(entity.Status == ProjectInviteStatus.Accepted){
                var projectUser = new ProjectUserModel{
                    ApplicationUserId = entity.InvitedApplicationUserId,
                    ProjectId = entity.ProjectId,
                    Role = ApplicationUserType.Normal,
                    JoinedAt = DateTime.Now,
                };
                await CreateProjectUserAsync(projectUser);
            }
            return await GetProjectInviteByIdAsync(id);
        }


        #endregion
    }
}