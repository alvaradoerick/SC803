using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace AseIsthmusAPI.Data.AseIsthmusModels
{

    public partial class Province
    {
        public int ProvinceId { get; set; }

        public string? ProvinceName { get; set; }

        [JsonIgnore]
        public virtual ICollection<Canton> Cantons { get; set; } = new List<Canton>();
    }
}