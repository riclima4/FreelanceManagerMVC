using FreelanceManager.Data;
using FreelanceManager.Data.Enum;
using FreelanceManager.IO.Tarefas;
using FreelanceManager.IO.Reports;
using FreelanceManager.Services.Tarefas;
using FreelanceManager.Services.Projects;
using FreelanceManager.Services.Clients;
using System.Globalization;
using Microsoft.AspNetCore.Identity;
using Syncfusion.Drawing;
using Syncfusion.Pdf;
using Syncfusion.Pdf.Graphics;

namespace FreelanceManager.Services.Utils
{
    public class ReportService : IReportService
    {
        private readonly ITimesheetService _timesheetService;
        private readonly IProjectsService _projectsService;
        private readonly IClientsService _clientsService;
        private readonly ITarefasService _tarefasService;
        private readonly UserManager<ApplicationUser> _userManager;

        public ReportService(
            ITimesheetService timesheetService, 
            IProjectsService projectsService,
            IClientsService clientsService,
            ITarefasService tarefasService,
            UserManager<ApplicationUser> userManager)
        {
            _timesheetService = timesheetService;
            _projectsService = projectsService;
            _clientsService = clientsService;
            _tarefasService = tarefasService;
            _userManager = userManager;
        }        /// <summary>
        /// Calculates if a user has excessive hours for a specific date by comparing total registered hours vs user's configured daily hours
        /// </summary>
        private async Task<bool> HasExcessiveHoursAsync(string userId, DateTime date, double totalDailyHours)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user != null && !string.IsNullOrEmpty(user.DailyHours))
                {
                    if (double.TryParse(user.DailyHours, out double userDailyHoursThreshold))
                    {
                        return totalDailyHours > userDailyHoursThreshold;
                    }
                }
                
                // Fallback to default 8 hours if user or DailyHours not found/invalid
                return totalDailyHours > 8.0;
            }
            catch
            {
                // Fallback to default 8 hours if any error occurs
                return totalDailyHours > 8.0;
            }
        }

        #region Personal Reports (Requirement 12)        /// <summary>
        /// Generates a personal report for a specific user and date range
        /// </summary>
        public async Task<PersonalMonthlyReport> GeneratePersonalReportAsync(string userId, DateTime startDate, DateTime endDate)
        {
            var userTimesheets = await _timesheetService.GetByUserIdAsync(userId);
            var monthlyTimesheets = userTimesheets
                .Where(t => t.Date >= startDate && t.Date <= endDate)
                .ToList(); // Include ALL task statuses

            var report = new PersonalMonthlyReport
            {
                UserId = userId,
                Year = startDate.Year,
                Month = startDate.Month,
                DailyReports = new List<DailyReport>(),
                TotalMonthlyHours = 0,
                TotalMonthlyEarnings = 0
            };

            // Group by day
            var dailyGroups = monthlyTimesheets.GroupBy(t => t.Date.Date);

            foreach (var dailyGroup in dailyGroups.OrderBy(g => g.Key))
            {
                var dailyReport = await GenerateDailyReportAsync(dailyGroup.Key, dailyGroup.ToList(), userId);
                report.DailyReports.Add(dailyReport);
                report.TotalMonthlyHours += dailyReport.TotalDailyHours;
                report.TotalMonthlyEarnings += dailyReport.TotalDailyEarnings;
            }

            return report;
        }        /// <summary>
        /// Generates a PDF report for personal timesheet within a date range
        /// </summary>
        public async Task<MemoryStream> GeneratePersonalReportPdfAsync(string userId, DateTime startDate, DateTime endDate)
        {
            var report = await GeneratePersonalReportAsync(userId, startDate, endDate);
            return CreatePersonalReportPdf(report, startDate, endDate);
        }

        #endregion

        #region Project Reports (Requirement 13)        /// <summary>
        /// Generates a report for a specific project within a date range
        /// </summary>
        public async Task<ProjectMonthlyReport> GenerateProjectReportAsync(Guid projectId, DateTime startDate, DateTime endDate)
        {
            var projectTimesheets = await _timesheetService.GetByProjectIdWithDateRangeAsync(projectId, startDate, endDate);
            var filteredTimesheets = projectTimesheets
                .Where(t => t.Tarefa?.Status != TarefaStatus.InProgress)
                .ToList();

            var project = await _projectsService.GetByIdAsync(projectId);

            var report = new ProjectMonthlyReport
            {
                ProjectId = projectId,
                ProjectName = project.Name,
                Year = startDate.Year,
                Month = startDate.Month,
                DailyReports = new List<ProjectDailyReport>(),
                TotalMonthlyHours = 0,
                TotalMonthlyEarnings = 0,
                Users = new List<string>()
            };

            // Group by day
            var dailyGroups = filteredTimesheets.GroupBy(t => t.Date.Date);           
             foreach (var dailyGroup in dailyGroups.OrderBy(g => g.Key))
            {
                var dailyReport = await GenerateProjectDailyReportAsync(dailyGroup.Key, dailyGroup.ToList());
                report.DailyReports.Add(dailyReport);
                report.TotalMonthlyHours += dailyReport.TotalDailyHours;
                report.TotalMonthlyEarnings += dailyReport.TotalDailyEarnings;
            }

            // Get unique users
            report.Users = filteredTimesheets.Select(t => t.User?.FullName ?? "Unknown User").Distinct().ToList();

            return report;
        }        /// <summary>
        /// Generates a PDF report for project timesheet within a date range
        /// </summary>
        public async Task<MemoryStream> GenerateProjectReportPdfAsync(Guid projectId, DateTime startDate, DateTime endDate)
        {
            var report = await GenerateProjectReportAsync(projectId, startDate, endDate);
            return CreateProjectReportPdf(report, startDate, endDate);
        }

        #endregion

        #region Client Reports (Requirement 13)        /// <summary>
        /// Generates a report for a specific client within a date range
        /// </summary>
        public async Task<ClientMonthlyReport> GenerateClientReportAsync(Guid clientId, DateTime startDate, DateTime endDate)
        {
            var client = await _clientsService.GetByIdAsync(clientId);
            var clientProjects = await _projectsService.GetByClientIdAsync(clientId);

            var report = new ClientMonthlyReport
            {
                ClientId = clientId,
                ClientName = client.Name,
                Year = startDate.Year,
                Month = startDate.Month,
                ProjectReports = new List<ProjectMonthlyReport>(),
                TotalMonthlyHours = 0,
                TotalMonthlyEarnings = 0,
                Users = new List<string>()
            };

            var allUsers = new HashSet<string>();

            foreach (var project in clientProjects)
            {
                var projectReport = await GenerateProjectReportAsync(project.Id, startDate, endDate);
                if (projectReport.DailyReports.Any())
                {
                    report.ProjectReports.Add(projectReport);
                    report.TotalMonthlyHours += projectReport.TotalMonthlyHours;
                    report.TotalMonthlyEarnings += projectReport.TotalMonthlyEarnings;
                    
                    foreach (var user in projectReport.Users)
                    {
                        allUsers.Add(user);
                    }
                }
            }

            report.Users = allUsers.ToList();

            return report;
        }        /// <summary>
        /// Generates a PDF report for client timesheet within a date range
        /// </summary>
        public async Task<MemoryStream> GenerateClientReportPdfAsync(Guid clientId, DateTime startDate, DateTime endDate)
        {
            var report = await GenerateClientReportAsync(clientId, startDate, endDate);
            return CreateClientReportPdf(report, startDate, endDate);
        }

        #endregion

        #region Helper Methods        /// <summary>
        /// Generates a daily report with user-specific excessive hours detection
        /// Groups timesheet entries by task and sums hours per task per day
        /// </summary>
        private async Task<DailyReport> GenerateDailyReportAsync(DateTime date, List<TimesheetDto> dailyTimesheets, string userId)
        {
            var dailyReport = new DailyReport
            {
                Date = date,
                Tasks = new List<TaskEntry>(),
                TotalDailyHours = 0,
                TotalDailyEarnings = 0,
                IsExcessiveHours = false
            };

            // Group by task (TaskId) and sum hours per task
            var taskGroups = dailyTimesheets
                .Where(t => t.Tarefa != null) // Only include timesheets with valid tasks
                .GroupBy(t => new { 
                    TaskId = t.Tarefa.Id,
                    TaskName = t.Tarefa.Name ?? "Unknown Task",
                    ProjectId = t.Tarefa.ProjectId
                })
                .ToList();

            foreach (var taskGroup in taskGroups)
            {
                // Get project name using ProjectId
                var projectName = "Unknown Project";
                if (taskGroup.Key.ProjectId.HasValue)
                {
                    try
                    {
                        var project = await _projectsService.GetByIdAsync(taskGroup.Key.ProjectId.Value);
                        projectName = project?.Name ?? "Unknown Project";
                    }
                    catch
                    {
                        // Keep default project name if error occurs
                    }
                }

                // Sum hours and calculate total earnings for this task
                var totalHours = taskGroup.Sum(t => t.HoursWorked);
                var weightedRate = taskGroup.Sum(t => t.HoursWorked * (double)t.HourlyRate) / totalHours;
                var totalEarnings = (decimal)totalHours * (decimal)weightedRate;

                // Combine notes from all timesheet entries for this task
                var notes = string.Join("; ", taskGroup
                    .Where(t => !string.IsNullOrWhiteSpace(t.Notes))
                    .Select(t => t.Notes)
                    .Distinct());

                var taskEntry = new TaskEntry
                {
                    TaskName = taskGroup.Key.TaskName,
                    ProjectName = projectName,
                    Hours = totalHours,
                    HourlyRate = (decimal)weightedRate,
                    Earnings = totalEarnings,
                    Notes = notes
                };

                dailyReport.Tasks.Add(taskEntry);
                dailyReport.TotalDailyHours += taskEntry.Hours;
                dailyReport.TotalDailyEarnings += taskEntry.Earnings;
            }

            // Check if daily hours exceed user's configured threshold
            dailyReport.IsExcessiveHours = await HasExcessiveHoursAsync(userId, date, dailyReport.TotalDailyHours);

            return dailyReport;
        }
        private async Task<ProjectDailyReport> GenerateProjectDailyReportAsync(DateTime date, List<TimesheetDto> dailyTimesheets)
        {
            var dailyReport = new ProjectDailyReport
            {
                Date = date,
                Tasks = new List<ProjectTaskEntry>(),
                TotalDailyHours = 0,
                TotalDailyEarnings = 0,
                Users = new List<string>(),
                IsExcessiveHours = false
            };

            foreach (var timesheet in dailyTimesheets)
            {
                var taskEntry = new ProjectTaskEntry
                {
                    TaskName = timesheet.Tarefa?.Name ?? "Unknown Task",
                    Hours = timesheet.HoursWorked,
                    HourlyRate = timesheet.HourlyRate,
                    Earnings = (decimal)timesheet.HoursWorked * timesheet.HourlyRate,
                    UserName = timesheet.User?.FullName ?? "Unknown User",
                    Notes = timesheet.Notes
                };

                dailyReport.Tasks.Add(taskEntry);
                dailyReport.TotalDailyHours += taskEntry.Hours;
                dailyReport.TotalDailyEarnings += taskEntry.Earnings;
            }

            dailyReport.Users = dailyTimesheets.Select(t => t.User?.FullName ?? "Unknown User").Distinct().ToList();

            var userGroups = dailyTimesheets.GroupBy(t => t.UserId);
            foreach (var userGroup in userGroups)
            {
                var userDailyHours = userGroup.Sum(t => t.HoursWorked);
                var hasExcessiveHours = await HasExcessiveHoursAsync(userGroup.Key, date, userDailyHours);
                if (hasExcessiveHours)
                {
                    dailyReport.IsExcessiveHours = true;
                    break;
                }
            }

            return dailyReport;
        }
        private MemoryStream CreatePersonalReportPdf(PersonalMonthlyReport report, DateTime startDate, DateTime endDate)
        {
            var pdfDocument = new PdfDocument();
            var page = pdfDocument.Pages.Add();
            var graphics = page.Graphics;
            var pageWidth = page.Size.Width;

            // Fonts and brushes
            var titleFont = new PdfStandardFont(PdfFontFamily.Helvetica, 20, PdfFontStyle.Bold);
            var headerFont = new PdfStandardFont(PdfFontFamily.Helvetica, 14, PdfFontStyle.Bold);
            var bodyFont = new PdfStandardFont(PdfFontFamily.Helvetica, 10);
            var smallFont = new PdfStandardFont(PdfFontFamily.Helvetica, 8);

            var blackBrush = PdfBrushes.Black;
            var redBrush = PdfBrushes.Red;
            var grayBrush = new PdfSolidBrush(Color.FromArgb(128, 128, 128));
            var lightGrayBrush = new PdfSolidBrush(Color.FromArgb(240, 240, 240));
            var darkGrayBrush = new PdfSolidBrush(Color.FromArgb(60, 60, 60));

            // Grid layout dimensions
            float margin = 30;
            float yPosition = margin;
            float tableStartY = 0;

            // Column widths for the task details table
            float col1Width = 120; // Date
            float col2Width = 200; // Task Name
            float col3Width = 120; // Project Name
            float col4Width = 60;  // Hours
            float col5Width = 80;  // Rate
            float col6Width = 80;  // Earnings
            float totalTableWidth = col1Width + col2Width + col3Width + col4Width + col5Width + col6Width;

            // Header Section
            graphics.DrawString("PERSONAL REPORT", titleFont, blackBrush, new PointF(margin, yPosition));
            yPosition += 35;

            // Report period and summary in a box
            var summaryBoxHeight = 80;
            var summaryRect = new RectangleF(margin, yPosition, pageWidth - 2 * margin, summaryBoxHeight);
            graphics.DrawRectangle(new PdfPen(grayBrush, 1), summaryRect);
            graphics.DrawRectangle(lightGrayBrush, summaryRect);            yPosition += 15;
            graphics.DrawString($"Report Period: {startDate:dd/MM/yyyy} - {endDate:dd/MM/yyyy}",
                headerFont, blackBrush, new PointF(margin + 15, yPosition));
            yPosition += 25;

            yPosition += 45;

            // Table Header
            tableStartY = yPosition;
            var headerHeight = 25;
            var headerRect = new RectangleF(margin, yPosition, totalTableWidth, headerHeight);
            graphics.DrawRectangle(new PdfPen(blackBrush, 1), headerRect);
            graphics.DrawRectangle(darkGrayBrush, headerRect);

            // Draw column headers
            var headerY = yPosition + 8;
            graphics.DrawString("Date", new PdfStandardFont(PdfFontFamily.Helvetica, 10, PdfFontStyle.Bold),
                PdfBrushes.White, new PointF(margin + 5, headerY));
            graphics.DrawString("Task Name", new PdfStandardFont(PdfFontFamily.Helvetica, 10, PdfFontStyle.Bold),
                PdfBrushes.White, new PointF(margin + col1Width + 5, headerY));
            graphics.DrawString("Project", new PdfStandardFont(PdfFontFamily.Helvetica, 10, PdfFontStyle.Bold),
                PdfBrushes.White, new PointF(margin + col1Width + col2Width + 5, headerY));
            graphics.DrawString("Hours", new PdfStandardFont(PdfFontFamily.Helvetica, 10, PdfFontStyle.Bold),
                PdfBrushes.White, new PointF(margin + col1Width + col2Width + col3Width + 5, headerY));
            graphics.DrawString("Rate", new PdfStandardFont(PdfFontFamily.Helvetica, 10, PdfFontStyle.Bold),
                PdfBrushes.White, new PointF(margin + col1Width + col2Width + col3Width + col4Width + 5, headerY));
            graphics.DrawString("Earnings", new PdfStandardFont(PdfFontFamily.Helvetica, 10, PdfFontStyle.Bold),
                PdfBrushes.White, new PointF(margin + col1Width + col2Width + col3Width + col4Width + col5Width + 5, headerY));

            yPosition += headerHeight;

            // Draw vertical grid lines for header
            for (int i = 1; i < 6; i++)
            {
                float lineX = margin;
                if (i == 1) lineX += col1Width;
                else if (i == 2) lineX += col1Width + col2Width;
                else if (i == 3) lineX += col1Width + col2Width + col3Width;
                else if (i == 4) lineX += col1Width + col2Width + col3Width + col4Width;
                else if (i == 5) lineX += col1Width + col2Width + col3Width + col4Width + col5Width;

                graphics.DrawLine(new PdfPen(PdfBrushes.White, 1),
                    new PointF(lineX, tableStartY),
                    new PointF(lineX, tableStartY + headerHeight));
            }

            // Table rows for daily data
            bool alternateRow = false;
            foreach (var daily in report.DailyReports.OrderBy(d => d.Date))
            {
                // Check if we need a new page
                if (yPosition > page.Size.Height - 100)
                {
                    page = pdfDocument.Pages.Add();
                    graphics = page.Graphics;
                    yPosition = margin;
                    alternateRow = false;
                }

                foreach (var task in daily.Tasks)
                {
                    var rowHeight = 20;
                    var rowRect = new RectangleF(margin, yPosition, totalTableWidth, rowHeight);

                    // Alternate row coloring
                    if (alternateRow)
                    {
                        graphics.DrawRectangle(lightGrayBrush, rowRect);
                    }

                    // Draw row border
                    graphics.DrawRectangle(new PdfPen(grayBrush, 0.5f), rowRect);

                    // Determine text color (red for excessive hours days)
                    var textBrush = daily.IsExcessiveHours ? redBrush : blackBrush;
                    var rowY = yPosition + 6;

                    // Draw cell content
                    graphics.DrawString(daily.Date.ToString("dd/MM/yyyy"), bodyFont, textBrush,
                        new PointF(margin + 3, rowY));

                    // Truncate task name if too long
                    var taskName = task.TaskName.Length > 25 ? task.TaskName.Substring(0, 22) + "..." : task.TaskName;
                    graphics.DrawString(taskName, bodyFont, textBrush,
                        new PointF(margin + col1Width + 3, rowY));

                    // Truncate project name if too long
                    var projectName = task.ProjectName.Length > 15 ? task.ProjectName.Substring(0, 12) + "..." : task.ProjectName;
                    graphics.DrawString(projectName, bodyFont, textBrush,
                        new PointF(margin + col1Width + col2Width + 3, rowY));

                    graphics.DrawString($"{task.Hours:F1}h", bodyFont, textBrush,
                        new PointF(margin + col1Width + col2Width + col3Width + 3, rowY));

                    graphics.DrawString($"${task.HourlyRate:F0}", bodyFont, textBrush,
                        new PointF(margin + col1Width + col2Width + col3Width + col4Width + 3, rowY));

                    graphics.DrawString($"{task.Earnings:C}", bodyFont, textBrush,
                        new PointF(margin + col1Width + col2Width + col3Width + col4Width + col5Width + 3, rowY));

                    // Draw vertical grid lines
                    for (int i = 1; i < 6; i++)
                    {
                        float lineX = margin;
                        if (i == 1) lineX += col1Width;
                        else if (i == 2) lineX += col1Width + col2Width;
                        else if (i == 3) lineX += col1Width + col2Width + col3Width;
                        else if (i == 4) lineX += col1Width + col2Width + col3Width + col4Width;
                        else if (i == 5) lineX += col1Width + col2Width + col3Width + col4Width + col5Width;

                        graphics.DrawLine(new PdfPen(grayBrush, 0.5f),
                            new PointF(lineX, yPosition),
                            new PointF(lineX, yPosition + rowHeight));
                    }

                    yPosition += rowHeight;
                    alternateRow = !alternateRow;
                }
            }

            // Footer with summary
            yPosition += 20;
            if (yPosition > page.Size.Height - 60)
            {
                page = pdfDocument.Pages.Add();
                graphics = page.Graphics;
                yPosition = margin;
            }

            var footerRect = new RectangleF(margin, yPosition, totalTableWidth, 30);
            graphics.DrawRectangle(new PdfPen(blackBrush, 1), footerRect);
            graphics.DrawRectangle(darkGrayBrush, footerRect);

            graphics.DrawString($"TOTAL: {report.TotalMonthlyHours:F2} hours | {report.TotalMonthlyEarnings:C}€",
                new PdfStandardFont(PdfFontFamily.Helvetica, 12, PdfFontStyle.Bold),
                PdfBrushes.White, new PointF(margin + 10, yPosition + 9));

            // Excessive hours legend
            yPosition += 45;
            graphics.DrawString("* Red entries indicate days with excessive hours (above your daily limit)",
                smallFont, redBrush, new PointF(margin, yPosition));

            var stream = new MemoryStream();
            pdfDocument.Save(stream);
            pdfDocument.Close(true);
            stream.Position = 0;

            return stream;
        }
        private MemoryStream CreateProjectReportPdf(ProjectMonthlyReport report, DateTime startDate, DateTime endDate)
        {
            var pdfDocument = new PdfDocument();
            var page = pdfDocument.Pages.Add();
            var graphics = page.Graphics;
            var pageWidth = page.Size.Width;          
            var titleFont = new PdfStandardFont(PdfFontFamily.Helvetica, 20, PdfFontStyle.Bold);
            var headerFont = new PdfStandardFont(PdfFontFamily.Helvetica, 14, PdfFontStyle.Bold);
            var bodyFont = new PdfStandardFont(PdfFontFamily.Helvetica, 10);
            var smallFont = new PdfStandardFont(PdfFontFamily.Helvetica, 8);

            var blackBrush = PdfBrushes.Black;
            var redBrush = PdfBrushes.Red;
            var grayBrush = new PdfSolidBrush(Color.FromArgb(128, 128, 128));
            var lightGrayBrush = new PdfSolidBrush(Color.FromArgb(240, 240, 240));
            var darkGrayBrush = new PdfSolidBrush(Color.FromArgb(60, 60, 60));

            float margin = 30;
            float yPosition = margin;
            float tableStartY = 0;            // Column widths for the task details table (optimized to fit page width)
            float col1Width = 85;  // Date
            float col2Width = 110; // Task Name (reduced further)
            float col3Width = 100; // User Name (reduced)
            float col4Width = 50;  // Hours (reduced)
            float col5Width = 70;  // Rate (reduced)
            float col6Width = 70;  // Earnings (reduced)
            float totalTableWidth = col1Width + col2Width + col3Width + col4Width + col5Width + col6Width;// Header Section
            graphics.DrawString("PROJECT REPORT", titleFont, blackBrush, new PointF(margin, yPosition));
            yPosition += 35;

            // Project info and summary in a box
            var summaryBoxHeight = 100;
            var summaryRect = new RectangleF(margin, yPosition, pageWidth - 2 * margin, summaryBoxHeight);
            graphics.DrawRectangle(new PdfPen(grayBrush, 1), summaryRect);
            graphics.DrawRectangle(lightGrayBrush, summaryRect);

            yPosition += 15; graphics.DrawString($"Project: {report.ProjectName}", headerFont, blackBrush, new PointF(margin + 15, yPosition));
            yPosition += 25;

            graphics.DrawString($"Report Period: {startDate:dd/MM/yyyy} - {endDate:dd/MM/yyyy}",
                bodyFont, grayBrush, new PointF(margin + 15, yPosition));
            yPosition += 20;

            graphics.DrawString($"Total Hours: {report.TotalMonthlyHours:F2} hours", bodyFont, blackBrush, new PointF(margin + 15, yPosition));
            graphics.DrawString($"Total Earnings: {report.TotalMonthlyEarnings:C}", bodyFont, blackBrush, new PointF(margin + 300, yPosition));
            yPosition += 20;

            graphics.DrawString($"Contributors: {string.Join(", ", report.Users)}", smallFont, grayBrush, new PointF(margin + 15, yPosition));

            yPosition += 50;

            // Table Header
            tableStartY = yPosition;
            var headerHeight = 25;
            var headerRect = new RectangleF(margin, yPosition, totalTableWidth, headerHeight);
            graphics.DrawRectangle(new PdfPen(blackBrush, 1), headerRect);
            graphics.DrawRectangle(darkGrayBrush, headerRect);

            // Draw column headers
            var headerY = yPosition + 8;
            graphics.DrawString("Date", new PdfStandardFont(PdfFontFamily.Helvetica, 10, PdfFontStyle.Bold),
                PdfBrushes.White, new PointF(margin + 5, headerY));
            graphics.DrawString("Task Name", new PdfStandardFont(PdfFontFamily.Helvetica, 10, PdfFontStyle.Bold),
                PdfBrushes.White, new PointF(margin + col1Width + 5, headerY));
            graphics.DrawString("User", new PdfStandardFont(PdfFontFamily.Helvetica, 10, PdfFontStyle.Bold),
                PdfBrushes.White, new PointF(margin + col1Width + col2Width + 5, headerY));
            graphics.DrawString("Hours", new PdfStandardFont(PdfFontFamily.Helvetica, 10, PdfFontStyle.Bold),
                PdfBrushes.White, new PointF(margin + col1Width + col2Width + col3Width + 5, headerY));
            graphics.DrawString("Rate", new PdfStandardFont(PdfFontFamily.Helvetica, 10, PdfFontStyle.Bold),
                PdfBrushes.White, new PointF(margin + col1Width + col2Width + col3Width + col4Width + 5, headerY));
            graphics.DrawString("Earnings", new PdfStandardFont(PdfFontFamily.Helvetica, 10, PdfFontStyle.Bold),
                PdfBrushes.White, new PointF(margin + col1Width + col2Width + col3Width + col4Width + col5Width + 5, headerY));

            yPosition += headerHeight;

            // Draw vertical grid lines for header
            for (int i = 1; i < 6; i++)
            {
                float lineX = margin;
                if (i == 1) lineX += col1Width;
                else if (i == 2) lineX += col1Width + col2Width;
                else if (i == 3) lineX += col1Width + col2Width + col3Width;
                else if (i == 4) lineX += col1Width + col2Width + col3Width + col4Width;
                else if (i == 5) lineX += col1Width + col2Width + col3Width + col4Width + col5Width;

                graphics.DrawLine(new PdfPen(PdfBrushes.White, 1),
                    new PointF(lineX, tableStartY),
                    new PointF(lineX, tableStartY + headerHeight));
            }

            // Table rows for daily data
            bool alternateRow = false;
            foreach (var daily in report.DailyReports.OrderBy(d => d.Date))
            {
                foreach (var task in daily.Tasks)
                {
                    // Check if we need a new page
                    if (yPosition > page.Size.Height - 100)
                    {
                        page = pdfDocument.Pages.Add();
                        graphics = page.Graphics;
                        yPosition = margin;
                        alternateRow = false;
                    }

                    var rowHeight = 20;
                    var rowRect = new RectangleF(margin, yPosition, totalTableWidth, rowHeight);

                    // Alternate row coloring
                    if (alternateRow)
                    {
                        graphics.DrawRectangle(lightGrayBrush, rowRect);
                    }                    // Draw row border
                    graphics.DrawRectangle(new PdfPen(grayBrush, 0.5f), rowRect);

                    // Determine text color (red for excessive hours days)
                    var textBrush = daily.IsExcessiveHours ? redBrush : blackBrush;
                    var rowY = yPosition + 6;                    // Draw cell content
                    graphics.DrawString(daily.Date.ToString("dd/MM/yyyy"), bodyFont, textBrush,
                        new PointF(margin + 3, rowY));                    // Truncate task name if too long (adjusted for smaller column)
                    var taskName = task.TaskName.Length > 13 ? task.TaskName.Substring(0, 10) + "..." : task.TaskName;
                    graphics.DrawString(taskName, bodyFont, textBrush,
                        new PointF(margin + col1Width + 3, rowY));                    // Truncate user name if too long
                    var userName = task.UserName.Length > 12 ? task.UserName.Substring(0, 9) + "..." : task.UserName;
                    graphics.DrawString(userName, bodyFont, textBrush,
                        new PointF(margin + col1Width + col2Width + 3, rowY));                    graphics.DrawString($"{task.Hours:F1}h", bodyFont, textBrush,
                        new PointF(margin + col1Width + col2Width + col3Width + 3, rowY));

                    graphics.DrawString($"${task.HourlyRate:F0}", bodyFont, textBrush,
                        new PointF(margin + col1Width + col2Width + col3Width + col4Width + 3, rowY));

                    graphics.DrawString($"{task.Earnings:C}", bodyFont, textBrush,
                        new PointF(margin + col1Width + col2Width + col3Width + col4Width + col5Width + 3, rowY));

                    // Draw vertical grid lines
                    for (int i = 1; i < 6; i++)
                    {
                        float lineX = margin;
                        if (i == 1) lineX += col1Width;
                        else if (i == 2) lineX += col1Width + col2Width;
                        else if (i == 3) lineX += col1Width + col2Width + col3Width;
                        else if (i == 4) lineX += col1Width + col2Width + col3Width + col4Width;
                        else if (i == 5) lineX += col1Width + col2Width + col3Width + col4Width + col5Width;

                        graphics.DrawLine(new PdfPen(grayBrush, 0.5f),
                            new PointF(lineX, yPosition),
                            new PointF(lineX, yPosition + rowHeight));
                    }

                    yPosition += rowHeight;
                    alternateRow = !alternateRow;
                }
            }

            // Footer with summary
            yPosition += 20;
            if (yPosition > page.Size.Height - 60)
            {
                page = pdfDocument.Pages.Add();
                graphics = page.Graphics;
                yPosition = margin;
            }

            var footerRect = new RectangleF(margin, yPosition, totalTableWidth, 30);
            graphics.DrawRectangle(new PdfPen(blackBrush, 1), footerRect);
            graphics.DrawRectangle(darkGrayBrush, footerRect);            graphics.DrawString($"PROJECT TOTAL: {report.TotalMonthlyHours:F2} hours | {report.TotalMonthlyEarnings:C}€",
                new PdfStandardFont(PdfFontFamily.Helvetica, 12, PdfFontStyle.Bold),
                PdfBrushes.White, new PointF(margin + 10, yPosition + 9));

            // Excessive hours legend
            yPosition += 45;
            graphics.DrawString("* Red entries indicate days with excessive hours (above user's daily limit)",
                smallFont, redBrush, new PointF(margin, yPosition));

            var stream = new MemoryStream();
            pdfDocument.Save(stream);
            pdfDocument.Close(true);
            stream.Position = 0;

            return stream;
        }
        private MemoryStream CreateClientReportPdf(ClientMonthlyReport report, DateTime startDate, DateTime endDate)
        {
            var pdfDocument = new PdfDocument();
            var page = pdfDocument.Pages.Add();
            var graphics = page.Graphics;
            var pageWidth = page.Size.Width;

            // Fonts and brushes
            var titleFont = new PdfStandardFont(PdfFontFamily.Helvetica, 20, PdfFontStyle.Bold);
            var headerFont = new PdfStandardFont(PdfFontFamily.Helvetica, 14, PdfFontStyle.Bold);
            var bodyFont = new PdfStandardFont(PdfFontFamily.Helvetica, 10);
            var smallFont = new PdfStandardFont(PdfFontFamily.Helvetica, 8);

            var blackBrush = PdfBrushes.Black;
            var grayBrush = new PdfSolidBrush(Color.FromArgb(128, 128, 128));
            var lightGrayBrush = new PdfSolidBrush(Color.FromArgb(240, 240, 240));
            var darkGrayBrush = new PdfSolidBrush(Color.FromArgb(60, 60, 60));

            float margin = 30;
            float yPosition = margin;            // Column widths for the projects table (match summary box width)
            float totalTableWidth = pageWidth - 2 * margin;
            float col3Width = 120; // Earnings
            float col2Width = 100; // Hours
            float col1Width = totalTableWidth - col2Width - col3Width; // Project Name (remaining space)

            // Header Section
            graphics.DrawString("CLIENT REPORT", titleFont, blackBrush, new PointF(margin, yPosition));
            yPosition += 35;

            // Client info and summary in a box
            var summaryBoxHeight = 100;
            var summaryRect = new RectangleF(margin, yPosition, pageWidth - 2 * margin, summaryBoxHeight);
            graphics.DrawRectangle(new PdfPen(grayBrush, 1), summaryRect);
            graphics.DrawRectangle(lightGrayBrush, summaryRect);

            yPosition += 15;
            graphics.DrawString($"Client: {report.ClientName}", headerFont, blackBrush, new PointF(margin + 15, yPosition));
            yPosition += 25;

            graphics.DrawString($"Report Period: {startDate:dd/MM/yyyy} - {endDate:dd/MM/yyyy}",
                bodyFont, grayBrush, new PointF(margin + 15, yPosition));
            yPosition += 20;

            graphics.DrawString($"Total Hours: {report.TotalMonthlyHours:F2} hours", bodyFont, blackBrush, new PointF(margin + 15, yPosition));
            graphics.DrawString($"Total Earnings: {report.TotalMonthlyEarnings:C}", bodyFont, blackBrush, new PointF(margin + 300, yPosition));
            yPosition += 20;

            graphics.DrawString($"Contributors: {string.Join(", ", report.Users)}", smallFont, grayBrush, new PointF(margin + 15, yPosition));

            yPosition += 50;

            // Projects Table Header
            var headerHeight = 25;
            var headerRect = new RectangleF(margin, yPosition, totalTableWidth, headerHeight);
            graphics.DrawRectangle(new PdfPen(blackBrush, 1), headerRect);
            graphics.DrawRectangle(darkGrayBrush, headerRect);

            // Draw column headers
            var headerY = yPosition + 8;
            graphics.DrawString("Project Name", new PdfStandardFont(PdfFontFamily.Helvetica, 10, PdfFontStyle.Bold),
                PdfBrushes.White, new PointF(margin + 5, headerY));
            graphics.DrawString("Hours", new PdfStandardFont(PdfFontFamily.Helvetica, 10, PdfFontStyle.Bold),
                PdfBrushes.White, new PointF(margin + col1Width + 5, headerY));
            graphics.DrawString("Earnings", new PdfStandardFont(PdfFontFamily.Helvetica, 10, PdfFontStyle.Bold),
                PdfBrushes.White, new PointF(margin + col1Width + col2Width + 5, headerY));

            yPosition += headerHeight;

            // Draw horizontal line under header
            graphics.DrawLine(new PdfPen(blackBrush, 1),
                new PointF(margin, yPosition),
                new PointF(margin + totalTableWidth, yPosition));

            // Project rows
            bool alternateRow = false;
            float rowHeight = 20;

            foreach (var projectReport in report.ProjectReports)
            {
                if (yPosition > page.Size.Height - 100)
                {
                    page = pdfDocument.Pages.Add();
                    graphics = page.Graphics;
                    yPosition = margin;
                }

                // Alternate row background
                if (alternateRow)
                {
                    var rowRect = new RectangleF(margin, yPosition, totalTableWidth, rowHeight);
                    graphics.DrawRectangle(lightGrayBrush, rowRect);
                }

                // Draw horizontal line
                graphics.DrawLine(new PdfPen(grayBrush, 0.5f),
                    new PointF(margin, yPosition),
                    new PointF(margin + totalTableWidth, yPosition));

                var rowY = yPosition + 6;

                // Truncate project name if too long
                var projectName = projectReport.ProjectName.Length > 25 ? projectReport.ProjectName.Substring(0, 22) + "..." : projectReport.ProjectName;
                graphics.DrawString(projectName, bodyFont, blackBrush,
                    new PointF(margin + 3, rowY));

                graphics.DrawString($"{projectReport.TotalMonthlyHours:F1}h", bodyFont, blackBrush,
                    new PointF(margin + col1Width + 3, rowY));

                graphics.DrawString($"{projectReport.TotalMonthlyEarnings:C}", bodyFont, blackBrush,
                    new PointF(margin + col1Width + col2Width + 3, rowY));

                // Draw vertical grid lines
                for (int i = 1; i < 3; i++)
                {
                    float lineX = margin;
                    if (i == 1) lineX += col1Width;
                    else if (i == 2) lineX += col1Width + col2Width;

                    graphics.DrawLine(new PdfPen(grayBrush, 0.5f),
                        new PointF(lineX, yPosition),
                        new PointF(lineX, yPosition + rowHeight));
                }

                yPosition += rowHeight;
                alternateRow = !alternateRow;
            }

            // Final horizontal line
            graphics.DrawLine(new PdfPen(blackBrush, 1),
                new PointF(margin, yPosition),
                new PointF(margin + totalTableWidth, yPosition));

            // Footer with summary
            yPosition += 20;
            if (yPosition > page.Size.Height - 60)
            {
                page = pdfDocument.Pages.Add();
                graphics = page.Graphics;
                yPosition = margin;
            }

            var footerRect = new RectangleF(margin, yPosition, totalTableWidth, 30);
            graphics.DrawRectangle(new PdfPen(blackBrush, 1), footerRect);
            graphics.DrawRectangle(darkGrayBrush, footerRect);

            graphics.DrawString($"CLIENT TOTAL: {report.TotalMonthlyHours:F2} hours | {report.TotalMonthlyEarnings:C}€",
                new PdfStandardFont(PdfFontFamily.Helvetica, 12, PdfFontStyle.Bold),
                PdfBrushes.White, new PointF(margin + 10, yPosition + 9));

            var stream = new MemoryStream();
            pdfDocument.Save(stream);
            pdfDocument.Close(true);
            stream.Position = 0;

            return stream;
        }

        #endregion
    }
}