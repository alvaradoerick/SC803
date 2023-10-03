namespace AseIsthmusAPI.Data.DTOs
{
    public class EmailDto
    {
        public string To { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
    }

    public class ActivateUserEmailDto
    {
        public string EmailAddress { get; set; } = null!;
    }

    public class LoanEmailDto
    {
        public string FullName { get; set; } = null!;
        public string EmailAddress { get; set; } = null!;
    }

    public class RequestLoanReviewEmailDto
    {
        public string LoanRequestId { get; set; } = null!;
        public string FullName { get; set; } = null!;
    }

    public class RespondLoanReviewEmailDto
    {
        public string LoanRequestId { get; set; } = null!;
        public string FullName { get; set; } = null!;
        public string Status { get; set; }
    }


    public class RequestAgreementInformationDto
    {
        public string PersonId { get; set; } = null!;

        public string EmailAddress { get; set; } = null!;

        public string FullName { get; set; } = null!;

        public string Title { get; set; } = null!;
    }
}
