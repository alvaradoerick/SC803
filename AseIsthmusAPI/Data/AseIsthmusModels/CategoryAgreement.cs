using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AseIsthmusAPI.Data.AseIsthmusModels
{

    public partial class CategoryAgreement
    {
        public int CategoryAgreementId { get; set; }

        public string Description { get; set; } = null!;

        public bool IsActive { get; set; }

        public virtual ICollection<Agreement> Agreements { get; set; } = new List<Agreement>();
    }
}