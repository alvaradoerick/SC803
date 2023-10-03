using System;
using System.Collections.Generic;


namespace AseIsthmusAPI.Data.AseIsthmusModels
{
    public partial class LoanBalance
    {
        public int LoanBalanceId { get; set; }

        public string PersonId { get; set; } = null!;

        public int PaymentNumber { get; set; }

        public DateTime? PaymentDate { get; set; }

        public decimal? BeginningBalance { get; set; }

        public decimal? ScheduledPayment { get; set; }

        public decimal? ExtraPayment { get; set; }

        public decimal? TotalPayment { get; set; }

        public decimal? Principal { get; set; }

        public decimal? Interest { get; set; }

        public decimal? EndingBalance { get; set; }

        public decimal? CumulativeInterest { get; set; }

        public decimal? InterestRate { get; set; }

        public string? Currency { get; set; }

        public int LoanRequestId { get; set; }

        public virtual LoanRequest LoanRequest { get; set; } = null!;

        public virtual User Person { get; set; } = null!;
    }
}
