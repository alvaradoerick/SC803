using System;
using System.Collections.Generic;

namespace AseIsthmusAPI.Data.AseIsthmusModels
{

    public partial class ContributionBalance
    {
        public int ContributionBalanceId { get; set; }

        public string PersonId { get; set; } = null!;

        public DateTime DeductedDate { get; set; }

        public decimal EmployeeContribution { get; set; }

        public decimal EmployerContribution { get; set; }

        public virtual User Person { get; set; } = null!;
    }
}