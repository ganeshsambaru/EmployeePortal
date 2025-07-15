namespace EmployeePortal.ViewModels
{
    public class DashboardStatsViewModel
    {
        public int TotalEmployees { get; set; }
        public decimal AverageSalary { get; set; }
        public int MaleCount { get; set; }
        public int FemaleCount { get; set; }
        public int OtherCount { get; set; }
    }
}
