using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace AseIsthmusAPI.Data.AseIsthmusModels
{
    [Keyless]
    [Table("LoanCalculationTypes")]
    public partial class LoanCalculationType
    {
        public string PersonId { get; set; }
        public int LoansTypeId { get; set; }
        public int Term { get; set; }
        public decimal Amount { get; set; }
    }
}
