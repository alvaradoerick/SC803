using Microsoft.EntityFrameworkCore;

namespace AseIsthmusAPI.Data
{
    [Keyless]
    public class sp_GetLoanCalculation_Result
    {
        public decimal AvailEmployeeAmt { get; set; }
        public decimal AvailEmployerAmt { get; set; }
        public decimal TotalAvailAmount { get; set; }
        public decimal BiweeklyFee { get; set; }
        public decimal TotalAmtToPay { get; set; }
        public decimal Rate { get; set; }
        public int Term { get; set; }
    }
}
