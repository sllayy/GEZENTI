using System.ComponentModel.DataAnnotations.Schema;

namespace GeziRotasi.API.Models
{
    public class WorkingHour
    {
        public int Id { get; set; } // Bu tablonun kendi benzersiz ID'si

        // Haftanın gününü temsil eder (Pazartesi=1, Pazar=7)
        // DayOfWeek enum'ı standart olarak Pazar=0 ile başlar, bu yüzden int kullanmak daha esnektir.
        public int DayOfWeek { get; set; }

        // TimeOnly, sadece saat bilgisini (saat:dakika) tutmak için .NET'in modern ve doğru veri tipidir.
        public TimeOnly? OpenTime { get; set; }
        public TimeOnly? CloseTime { get; set; }

        // Bu, bu çalışma saatinin hangi POI'a ait olduğunu belirtir.
        // Bu bir "Foreign Key" (Yabancı Anahtar) ilişkisidir.
        public int PoiId { get; set; }
        [ForeignKey("PoiId")]
        public Poi Poi { get; set; }
    }
}