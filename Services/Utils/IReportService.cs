using FreelanceManager.IO.Reports;
using FreelanceManager.Services.Utils;

namespace FreelanceManager.Services.Utils
{    public interface IReportService
    {
        Task<PersonalMonthlyReport> GeneratePersonalReportAsync(string userId, DateTime startDate, DateTime endDate);
        Task<MemoryStream> GeneratePersonalReportPdfAsync(string userId, DateTime startDate, DateTime endDate);

        Task<ProjectMonthlyReport> GenerateProjectReportAsync(Guid projectId, DateTime startDate, DateTime endDate);
        Task<MemoryStream> GenerateProjectReportPdfAsync(Guid projectId, DateTime startDate, DateTime endDate);

        Task<ClientMonthlyReport> GenerateClientReportAsync(Guid clientId, DateTime startDate, DateTime endDate);
        Task<MemoryStream> GenerateClientReportPdfAsync(Guid clientId, DateTime startDate, DateTime endDate);
    }
}
