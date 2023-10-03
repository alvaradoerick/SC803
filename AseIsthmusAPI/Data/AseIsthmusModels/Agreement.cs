using System;
using System.Collections.Generic;

namespace AseIsthmusAPI.Data.AseIsthmusModels
{

    public partial class Agreement
    {
        public int AgreementId { get; set; }

        public string Title { get; set; } = null!;

        public string? Description { get; set; }

        public string? Image { get; set; }

        public int CategoryAgreementId { get; set; }

        public bool IsActive { get; set; }

        public virtual CategoryAgreement CategoryAgreement { get; set; } = null!;
    }
}