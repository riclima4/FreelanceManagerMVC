namespace FreelanceManager.IO.Reports
{
    public class ProjectTaskEntry
    {
        public string TaskName { get; set; } = string.Empty;
        public double Hours { get; set; }
        public decimal HourlyRate { get; set; }
        public decimal Earnings { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Notes { get; set; } = string.Empty;
    }
}
