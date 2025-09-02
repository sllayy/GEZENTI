namespace GeziRotasi.API.Models
{
    public class Route
    {
        public int Id { get; set; }

        public int UserId { get; set; } 

        public string StartLocation { get; set; } = string.Empty;  
        public string EndLocation { get; set; } = string.Empty;

        public double Distance { get; set; }
        public double Duration { get; set; }

        public string Geometry { get; set; } = string.Empty;  

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<RouteFeedback> Feedbacks { get; set; } = new List<RouteFeedback>();
    }
}
