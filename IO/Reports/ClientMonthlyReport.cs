using System;
using System.Collections.Generic;

namespace FreelanceManager.IO.Reports
{
    public class ClientMonthlyReport
    {
        public Guid ClientId { get; set; }
        public string ClientName { get; set; } = string.Empty;
        public int Year { get; set; }
        public int Month { get; set; }
        public List<ProjectMonthlyReport> ProjectReports { get; set; } = new();
        public double TotalMonthlyHours { get; set; }
        public decimal TotalMonthlyEarnings { get; set; }
        public List<string> Users { get; set; } = new();
    }
}
