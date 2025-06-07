using System;
using System.Collections.Generic;

namespace FreelanceManager.IO.Reports
{    public class ProjectDailyReport
    {
        public DateTime Date { get; set; }
        public List<ProjectTaskEntry> Tasks { get; set; } = new();
        public double TotalDailyHours { get; set; }
        public decimal TotalDailyEarnings { get; set; }
        public List<string> Users { get; set; } = new();
        public bool IsExcessiveHours { get; set; }
    }
}
