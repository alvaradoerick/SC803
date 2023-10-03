namespace AseIsthmusAPI.Data.DTOs
{
    public class LocationDto
    {
        public int ProvinceId { get; set; }

        public string? ProvinceName { get; set; }

        public int CantonId { get; set; }

        public string CantonName { get; set; } = null!;

        public int DistrictId { get; set; }

        public string DistrictName { get; set; } = null!;
    }
}
