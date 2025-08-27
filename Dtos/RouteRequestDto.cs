using System.Collections.Generic; // List<> kullanabilmek için gerekli.
using System.ComponentModel.DataAnnotations; // Validasyon kuralları için gerekli.

// Sadece bir tane, doğru ve tutarlı bir namespace olmalı.
namespace GeziRotasi.API.Dtos
{
    /// <summary>
    /// Rota optimizasyonu için API'ye gönderilecek olan istek modelidir.
    /// </summary>
    public class RouteRequestDto
    {
        /// <summary>
        /// Ulaşım modu (örn: "driving", "walking", "cycling"). Varsayılan: "driving".
        /// </summary>
        [Required(ErrorMessage = "Ulaşım modu (mode) belirtilmelidir.")]
        public string Mode { get; set; } = "driving";

        /// <summary>
        /// Rota oluşturulacak olan noktaların koordinat listesi.
        /// Her bir eleman [boylam, enlem] formatında olmalıdır.
        /// </summary>
        [Required]
        [MinLength(2, ErrorMessage = "Rota oluşturmak için en az 2 nokta gereklidir.")]
        public List<double[]> Coordinates { get; set; } = new();

        /// <summary>
        /// OSRM'e, verilen noktaların sırasını optimize edip etmeyeceğini söyler.
        /// Varsayılan: true.
        /// </summary>
        public bool OptimizeOrder { get; set; } = true;

        /// <summary>
        /// Cevabın GeoJSON formatında olup olmayacağını belirtir.
        /// Varsayılan: true.
        /// </summary>
        public bool GeoJson { get; set; } = true;

        /// <summary>
        /// Rotanın başlangıç noktasına geri dönüp dönmeyeceğini belirtir.
        /// Varsayılan: false.
        /// </summary>
        public bool ReturnToStart { get; set; } = false;

        /// <summary>
        /// Yaya modunda otoyollardan kaçınılıp kaçınılmayacağını belirtir.
        /// Varsayılan: true.
        /// </summary>
        public bool AvoidHighwaysOnFoot { get; set; } = true;    

        /// <summary>
        /// İstenen alternatif rota sayısı.
        /// Varsayılan: 0 (alternatif yok).
        /// </summary>
        [Range(0, 3, ErrorMessage = "En fazla 3 alternatif rota istenebilir.")]
        public int Alternatives { get; set; } = 0;
    }
}
