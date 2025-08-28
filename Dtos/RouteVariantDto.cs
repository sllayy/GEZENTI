namespace GeziRotasi.API.Dtos
{
    public class RouteVariantDto
    {
        public double Distance { get; set; }
        public double Duration { get; set; }
        public object Geometry { get; set; } = default!;
        public bool IsPrimary { get; set; }
    }
}
