using System;
using System.Collections.Generic;

namespace AseIsthmusAPI.Data.AseIsthmusModels
{

    public partial class LoansType
    {
        public int LoansTypeId { get; set; }

        public string Description { get; set; } = null!;

        public int ContributionUsageId { get; set; }

        public decimal PercentageEmployeeCont { get; set; }

        public decimal? PercentageEmployerCont { get; set; }

        public int Term { get; set; }

        public decimal InterestRate { get; set; }

        public bool IsActive { get; set; }

        public virtual ContributionUsage ContributionUsage { get; set; } = null!;

        public virtual ICollection<LoanRequest> LoanRequests { get; set; } = new List<LoanRequest>();
    }
}
