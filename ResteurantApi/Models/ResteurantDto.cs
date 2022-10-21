using System.Collections.Generic;

namespace ResteurantApi.Models
{
    public class ResteurantDto
    {
        public int Id { get; set; }//na tej podstawie entity rozpozanje pole
        public string Name { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public bool HasDelivered { get; set; }

        public string City { get; set; }
        public string Street { get; set; }
        public string PostalCode { get; set; }

        public List<DishDto> Dishes { get; set; }
    }
}
