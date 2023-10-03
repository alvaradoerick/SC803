using System;
using System.Collections.Generic;

namespace AseIsthmusAPI.Data.AseIsthmusModels
{
    public partial class Canton
    {
        public int CantonId { get; set; }

        public string CantonName { get; set; } = null!;

        public int ProvinceId { get; set; }

        public virtual ICollection<District> Districts { get; set; } = new List<District>();

        public virtual Province Province { get; set; } = null!;
    }
}