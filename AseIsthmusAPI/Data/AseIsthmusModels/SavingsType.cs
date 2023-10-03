using System;
using System.Collections.Generic;

namespace AseIsthmusAPI.Data.AseIsthmusModels
{

    public partial class SavingsType
    {
        public int SavingsTypeId { get; set; }

        public string? Description { get; set; }

        public DateTime? ApplicationDeadline { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public bool IsActive { get; set; }

        public virtual ICollection<SavingsRequest> SavingsRequests { get; set; } = new List<SavingsRequest>();
    }
}