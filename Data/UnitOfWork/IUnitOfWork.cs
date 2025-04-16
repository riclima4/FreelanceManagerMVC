using FreelanceManager.Data.Repositories;

namespace FreelanceManager.Data.UnitOfWork
{
    public interface IUnitOfWork
    {
        #region repositories
        IAppRepository<Client> ClientsRepository { get; }
        IAppRepository<Project> ProjectsRepository { get; }
        IAppRepository<ProjectUser> ProjectUsersRepository { get; }
        IAppRepository<Tarefa> TarefasRepository { get; }
        IAppRepository<ProjectInvite> ProjectInvitesRepository { get; }
        IAppRepository<Timesheet> TimesheetsRepository { get; }
        #endregion

        int Commit();
        void Rollback();
        Task<int> CommitAsync();
        Task RollbackAsync();
        void Dispose();
        void DisposeAsync();
    }
}
