using System;
using System.Collections.Generic;

namespace FreelanceManager.IO.Reports
{
    public class PersonalMonthlyReport
    {
        public string UserId { get; set; } = string.Empty;
        public int Year { get; set; }
        public int Month { get; set; }
        public List<DailyReport> DailyReports { get; set; } = new();
        public double TotalMonthlyHours { get; set; }
        public decimal TotalMonthlyEarnings { get; set; }
    }
}
