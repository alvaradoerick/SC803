using Microsoft.EntityFrameworkCore;

namespace AseIsthmusAPI.Data.AseIsthmusModels
{
    [Keyless]
    public class sp_GetSavingsBalance_Result
    {
        public int SavingsBalanceId { get; set; }
        public int SavingsRequestId { get; set; }
        public decimal LastAmountDeducted { get; set; }
        public DateTime LastDeductedDate { get; set; }
        public int Installment { get; set; }
        public decimal TotalSaved { get; set; }
        public decimal SavingsRequestedAmount { get; set; }
        public DateTime SavingsEndDate { get; set; }
        public DateTime SavingsStartDate { get; set; }      
        public string SavingsName { get; set; }
        public bool IsActive { get; set; }

    }
}
