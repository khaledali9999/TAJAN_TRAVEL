using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TAJAN_TRAVEL.Models.Entities
{
    /// <summary>Price per person for a given group size (1 = single traveler, 2 = two persons, ...).</summary>
    public class TourPrice
    {
        public int Id { get; set; }

        public int TourId { get; set; }
        public Tour? Tour { get; set; }

        [Range(1, 50)]
        public int Persons { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal PricePerPerson { get; set; }
    }
}
