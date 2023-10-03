namespace AseIsthmusAPI.Data.AseIsthmusModels
{
    public class sp_GetLoanBalance_Result
    {
        public int LoanBalanceId { get; set; }

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
        public int OriginalTerm { get; set; }
        public decimal OriginalAmountRequested { get; set; }
        public decimal TotalPaid { get; set; }
        public string LoanName { get; set; }      
    }
}
