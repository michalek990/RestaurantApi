using System.ComponentModel.DataAnnotations;

namespace ResteurantApi.Models
{
    public class UpdateResteurantDto
    {
        [Required]
        [MaxLength(25)]
        public string Name { get; set; }
        public string Description { get; set; }
        public bool HasDelivered { get; set; }
    }
}
