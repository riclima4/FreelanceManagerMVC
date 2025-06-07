using System;
using System.Collections.Generic;

namespace FreelanceManager.IO.Reports
{
    public class DailyReport
    {
        public DateTime Date { get; set; }
        public List<TaskEntry> Tasks { get; set; } = new();
        public double TotalDailyHours { get; set; }
        public decimal TotalDailyEarnings { get; set; }
        public bool IsExcessiveHours { get; set; }
    }
}
