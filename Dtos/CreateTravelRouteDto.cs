namespace GeziRotasi.API.Dtos
{
    public class CreateTravelRouteDto
    {
        public int StartPoiId { get; set; }
        public int EndPoiId { get; set; }
        public List<int> PoiIds { get; set; } = new();
        public bool OptimizeOrder { get; set; } = true;
        public bool? DisabledAccess { get; set; }
        public int? MaxWalkingDistanceMeters { get; set; }
    }
}