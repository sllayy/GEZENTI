using System.Collections.Generic;
using System.Text.Json; // JsonElement'i kullanabilmek için gerekli.

// Sadece bir tane, doğru ve tutarlı bir namespace olmalı.
namespace GeziRotasi.API.Dtos
{
    /// <summary>
    /// Rota optimizasyonu sonucunda API'den dönecek olan cevap modelidir.
    /// </summary>
    public class RouteResponseDto
    {
        /// <summary>
        /// Oluşturulan rotanın toplam mesafesi (metre cinsinden).
        /// </summary>
        public double Distance { get; set; }

        /// <summary>
        /// Oluşturulan rotanın tahmini toplam süresi (saniye cinsinden).
        /// </summary>
        public double Duration { get; set; }

        /// <summary>
        /// Rotanın çizgisini içeren GeoJSON geometri nesnesi.
        /// (Bu, haritada rotayı çizdirmek için kullanılır).
        /// </summary>
        public JsonElement Geometry { get; set; }

        /// <summary>
        /// Eğer rota optimizasyonu (`OptimizeOrder=true`) yapıldıysa,
        /// noktaların en verimli ziyaret edilme sırasını içeren liste.
        /// Örnek: [0, 2, 1, 3] -> Önce ilk noktayı, sonra üçüncü noktayı, sonra ikinci noktayı ziyaret et.
        /// </summary>
        public List<int>? WaypointOrder { get; set; }
    }
}
