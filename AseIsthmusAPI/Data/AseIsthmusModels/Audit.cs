using System;
using System.Collections.Generic;

namespace AseIsthmusAPI.Data.AseIsthmusModels
{
    public partial class Audit
    {
        public int AuditId { get; set; }

        public string TableName { get; set; } = null!;

        public string PersonId { get; set; } = null!;

        public string Description { get; set; } = null!;

        public DateTime Time { get; set; }

        public virtual User Person { get; set; } = null!;
    }
}
