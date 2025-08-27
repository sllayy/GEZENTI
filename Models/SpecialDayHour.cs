using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GeziRotasi.API.Models
{
    public class SpecialDayHour
    {
        public int Id { get; set; }

        /// <summary>
        /// Bu özel saatin geçerli olduğu tarih (Yıl-Ay-Gün).
        /// </summary>
        [Required]
        public DateOnly Date { get; set; }

        /// <summary>
        /// O güne özel bir açıklama (örn: "Ramazan Bayramı", "Resmi Tatil").
        /// </summary>
        [StringLength(100)]
        public string Description { get; set; }

        /// <summary>
        /// O güne özel açılış saati. Eğer null ise, o gün kapalı demektir.
        /// </summary>
        public TimeOnly? OpenTime { get; set; }

        /// <summary>
        /// O güne özel kapanış saati.
        /// </summary>
        public TimeOnly? CloseTime { get; set; }

        // Bu özel günün hangi POI'a ait olduğunu belirten ilişki (Foreign Key).
        public int PoiId { get; set; }
        [ForeignKey("PoiId")]
        public Poi Poi { get; set; }
    }
}