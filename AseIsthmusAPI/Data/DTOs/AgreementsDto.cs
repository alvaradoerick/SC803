namespace AseIsthmusAPI.Data.DTOs
{
        public class ManageAgreementDto
        {
            public string Title { get; set; } = null!;
            public string Description { get; set; } = null!;
            public IFormFile? Image { get; set; } = null!;
            public int CategoryAgreementId { get; set; }
            public bool IsActive { get; set; }

        }

   

    public class AgreementDataDto
        {
            public int AgreementId { get; set; }
            public string Title { get; set; } = null!;
            public string Description { get; set; } = null!;
            public string? Image { get; set; } = null!;
            public int CategoryAgreementId { get; set; }
            public string CategoryName { get; set; } = null!;
            public bool IsActive { get; set; }
            public string? PersonId { get; set; } = null!;
    }
}
