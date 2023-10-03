namespace AseIsthmusAPI.Data.DTOs
{
    public class UpdateLoanRequestByAdminDto
    {
        public bool? IsApproved { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public string? Comments { get; set; }
    }
    public class LoanRequestDataDto
    {
        public int LoanRequestId { get; set; }

        public int LoansTypeId { get; set; }

        public decimal AmountRequested { get; set; }
        public string PersonId { get; set; }
        public string Name { get; set; }
        public string NumberId { get; set; }
        public string LoanTypeName { get; set; }

        public int Term { get; set; }

        public DateTime RequestedDate { get; set; }

        public string BankAccount { get; set; } = null!;

        public bool IsActive { get; set; }

        public bool? IsApproved { get; set; }

        public DateTime? ApprovedDate { get; set; }

        public bool? IsReviewRequired { get; set; }

        public bool? IsReviewApproved { get; set; }

        public DateTime? ReviewRequiredDate { get; set; }
        public string? Comments { get; set; }
    }
    public class ManageLoanRequestDto
    {
        public int LoansTypeId { get; set; }

        public decimal AmountRequested { get; set; }

        public int Term { get; set; }

        public string BankAccount { get; set; } = null!;

        public DateTime RequestedDate { get; set; }
        public string? Comments { get; set; }
    }

    public class GetPendingLoanCount
    {
        public int Count { get; set; }

        public DateTime LastSubmittedDate { get; set; }

    }

    public class UpdateLoanRequestRespondReviewDto
    {
        public bool? IsReviewApproved { get; set; }
        public string? Comments { get; set; }


        public class UpdateLoanRequestRequestReviewDto
        {
            public string? Comments { get; set; }
        }
    }
}
