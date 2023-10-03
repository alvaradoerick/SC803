namespace AseIsthmusAPI.Data.AseIsthmusModels
{
    public class sp_GetAllActiveAgreements_Result
    {
        public int AgreementId { get; set; }
        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string Image { get; set; } = null!;
        public int CategoryAgreementId { get; set; }
    }
}
