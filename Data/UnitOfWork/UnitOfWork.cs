using FreelanceManager.Data.Entities;
using FreelanceManager.Data.Repositories;
using FreelanceManager.Data.UnitOfWork;

namespace FreelanceManager.Data.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        private readonly ApplicationDbContext _databaseContext;

        #region repositories
        private IAppRepository<Client> _clientsRepository;
        private IAppRepository<Project> _projectsRepository;
        private IAppRepository<ProjectUser> _projectUsersRepository;
        private IAppRepository<Tarefa> _tarefasRepository;
        private IAppRepository<ProjectInvite> _projectInvitesRepository;
        private IAppRepository<Timesheet> _timesheetRepository;


        #endregion

        public UnitOfWork(ApplicationDbContext dbContext)
        {
            _databaseContext = dbContext;
        }

        #region repositories
        public IAppRepository<Client> ClientsRepository
        {
            get { return _clientsRepository ??= new AppRepository<Client>(_databaseContext); }
        }
        public IAppRepository<Project> ProjectsRepository
        {
            get { return _projectsRepository ??= new AppRepository<Project>(_databaseContext); }
        }
        public IAppRepository<ProjectUser> ProjectUsersRepository
        {
            get { return _projectUsersRepository ??= new AppRepository<ProjectUser>(_databaseContext); }
        }
        public IAppRepository<Tarefa> TarefasRepository
        {
            get { return _tarefasRepository ??= new AppRepository<Tarefa>(_databaseContext); }
        }

        public IAppRepository<ProjectInvite> ProjectInvitesRepository
        {
            get { return _projectInvitesRepository ??= new AppRepository<ProjectInvite>(_databaseContext); }

        }

        public IAppRepository<Timesheet> TimesheetsRepository
        {
            get { return _timesheetRepository ??= new AppRepository<Timesheet>(_databaseContext); }
        }

        #endregion

        public int Commit() => _databaseContext.SaveChanges();
        public void Rollback() => _databaseContext.Dispose();
        public async Task<int> CommitAsync() => await _databaseContext.SaveChangesAsync();
        public async Task RollbackAsync() => await _databaseContext.DisposeAsync();

        public void Dispose()
        {
            _databaseContext?.Dispose();
            GC.SuppressFinalize(this);
        }

        public void DisposeAsync()
        {
            _databaseContext?.DisposeAsync();
            GC.SuppressFinalize(this);
        }
    }
}
