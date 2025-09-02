using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

#nullable enable

namespace GeziRotasi.API.Dtos
{
    public class PastRouteDto
    {
        public int Id { get; set; }
        public string? StartLocation { get; set; }
        public string? EndLocation { get; set; }
        public double Distance { get; set; }
        public double Duration { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}