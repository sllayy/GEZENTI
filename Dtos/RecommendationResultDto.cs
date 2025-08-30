namespace GeziRotasi.API.Dtos
{
    public class RecommendationResultDto
    {
        public List<int> PoiIds { get; set; } = new();  
        public int TotalFound { get; set; }             
        public double RadiusKmUsed { get; set; }     
    }
}
