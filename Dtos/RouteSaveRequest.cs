namespace GeziRotasi.API.Dtos
{
    public class RouteSaveRequest
    {
        public RouteResponseDto Route { get; set; } = null!;
        public int UserId { get; set; }
        public double[] Start { get; set; } = null!;
        public double[]? End { get; set; }
    }
}
