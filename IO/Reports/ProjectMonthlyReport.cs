using System;
using System.Collections.Generic;

namespace FreelanceManager.IO.Reports
{
    public class ProjectMonthlyReport
    {
        public Guid ProjectId { get; set; }
        public string ProjectName { get; set; } = string.Empty;
        public int Year { get; set; }
        public int Month { get; set; }
        public List<ProjectDailyReport> DailyReports { get; set; } = new();
        public double TotalMonthlyHours { get; set; }
        public decimal TotalMonthlyEarnings { get; set; }
        public List<string> Users { get; set; } = new();
    }
}
