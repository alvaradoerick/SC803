using Microsoft.EntityFrameworkCore;

namespace AseIsthmusAPI.Data.AseIsthmusModels
{
    [Keyless]
    public class sp_GetContributionBalance_Result
    {
        public int ContributionBalanceId { get; set; }

        public string NumberId { get; set; } = null!;

        public string Name { get; set; }

        public DateTime DeductedDate { get; set; }
        public DateTime? ApprovedDate { get; set; }

        public decimal EmployeeContribution { get; set; }

        public decimal EmployerContribution { get; set; }

        public decimal TotalContribution { get; set; }

        public decimal TotalEmployeeContribution { get; set; }

        public decimal TotalEmployerContribution { get; set; }

    
    }
}
