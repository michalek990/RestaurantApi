using System.ComponentModel.DataAnnotations;

namespace ResteurantApi.Models
{
    public class CreateResteurantDto
    {

        [Required]
        [MaxLength(25)]
        public string Name { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public bool HasDelivered { get; set; }
        public string ContactEmail { get; set; }
        public string ContactNumber { get; set; }

        [Required]
        [MaxLength(50)]
        public string City { get; set; }

        [Required]
        [MaxLength(50)]
        public string Street { get; set; }
        public string PostalCode  { get; set; }
    }
}
