namespace GeziRotasi.API.Dtos
{
    public class CreateReviewDto
    {
        public int PoiId { get; set; }
        public int Rating { get; set; }
        public string Emoji { get; set; }
        public string Comment { get; set; }
    }
}