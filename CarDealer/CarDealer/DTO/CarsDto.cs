using CarDealer.Models;
using System.Collections.Generic;

namespace CarDealer.DTO
{
    public class CarsDto
    {
        public string Make { get; set; }
        public string Model { get; set; }
        public decimal Price { get; set; }
        public long TravelledDistance { get; set; }
        public ICollection<Part> parts { get; set; }
    }
}
