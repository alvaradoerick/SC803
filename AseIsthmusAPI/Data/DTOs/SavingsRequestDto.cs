namespace AseIsthmusAPI.Data.DTOs
{
    public class ManageSavingsRequestDto
    {
        public int SavingsTypeId { get; set; }

        public decimal Amount { get; set; }
      
    }

    public class ManageSavingsRequestByAdminDto
    {
        public bool? IsApproved { get; set; }

        public string? Comments { get; set; }
        public decimal Amount { get; set; }

    }

    public class CancelSavingsRequestDto
    {
        public string? Comments { get; set; }
    }

    public class SavingsRequestDataDto
    {
        public int SavingsRequestId { get; set; }
        public string PersonId { get; set; }
        public string Name { get; set; }
        public string NumberId { get; set; }
        public int SavingsTypeId { get; set; }
        public string SavingsTypeName { get; set; }

        public DateTime? ApplicationDate { get; set; }

        public decimal Amount { get; set; }

        public bool IsActive { get; set; }

        public DateTime? ApprovedDate { get; set; }

        public bool? IsApproved { get; set; }

        public string? Comments { get; set; }

        public bool IsCanceled { get; set; }

    }
    public class GetPendingSavingsCount
    {
        public int Count { get; set; }

        public DateTime? LastSubmittedDate { get; set; }

    }
}