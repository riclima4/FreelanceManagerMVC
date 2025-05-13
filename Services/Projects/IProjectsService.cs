using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FreelanceManager.IO.Projects;
using FreelanceManager.IO.Tarefas;

namespace FreelanceManager.Services.Projects
{
    public interface IProjectsService
    {
        Task<List<ProjectDto>> GetAllAsync();
        Task<ProjectDto> GetByIdAsync(Guid id);
        Task<List<ProjectDto>> GetByUserIdAsync(string userId);
        Task<ProjectDto> CreateAsync(ProjectModel model);
        Task<ProjectDto> UpdateAsync(Guid id, ProjectModel model);
        Task DeleteAsync(Guid id);
        Task<bool> CanDeleteAsync(Guid id);
        Task<string> GetNextCodeAsync();
        Task<List<ProjectDto>> GetProjectContainsInProjectUsersAsync(string userId);


        #region ProjectUsers
        Task<List<ProjectUserDto>> GetProjectUsersAsync(Guid projectId);
        Task<ProjectUserDto> GetProjectUserByIdAsync(Guid id);
        Task<ProjectUserDto> CreateProjectUserAsync(ProjectUserModel model);
        Task DeleteProjectUserAsync(ProjectUserModel model);
        Task<bool> DeleteProjectUserByUserIdAndProjectId(string id, Guid projectId);
        Task<bool> CanSaveProjectUserAsync(ProjectUserModel model);
        #endregion
        #region Tarefas
        Task<List<TarefaDto>> GetTarefasByProjectIdAndAssociatedUserIdAsync(Guid projectId, string AssociatedUserId);
        Task<List<TarefaDto>> GetTarefasAsync(Guid projectId);
        #endregion
        #region ProjectInvites
        Task<List<ProjectInviteDto>> GetProjectInvitesAsync(Guid projectId);
        Task<ProjectInviteDto> CreateProjectInviteAsync(ProjectInviteModel model);
        Task DeleteProjectInviteAsync(ProjectInviteModel model);
        Task<ProjectInviteDto> UpdateProjectInviteAsync(Guid id, ProjectInviteModel model);
        Task<ProjectInviteDto> GetProjectInviteByIdAsync(Guid id);
        Task<List<ProjectInviteDto>> GetProjectInvitesByUserIdAsync(string userId);
        #endregion
    }
}