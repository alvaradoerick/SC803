using System;
using System.Collections.Generic;

namespace AseIsthmusAPI.Data.AseIsthmusModels 
{ 

    public partial class TransactionLog
    {
        public int TransactionLogId { get; set; }

        public string PersonId { get; set; } = null!;

        public string TableName { get; set; } = null!;

        public DateTime TransactionDate { get; set; }

        public virtual User Person { get; set; } = null!;
    }
}
