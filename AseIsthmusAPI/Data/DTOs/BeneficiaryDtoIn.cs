using Newtonsoft.Json;

namespace AseIsthmusAPI.Data.DTOs
{
    
    public class BeneficiaryDtoIn
    {
        public string BeneficiaryName { get; set; } 

        public string BeneficiaryNumberId { get; set; } 

        public string BeneficiaryKin { get; set; } 
        public decimal BeneficiaryPercentage { get; set; }
        public string? PersonId { get; set; } 
    }
}
