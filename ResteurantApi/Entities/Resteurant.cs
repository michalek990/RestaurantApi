using System.Collections.Generic;

namespace ResteurantApi.Entities
{
    public class Resteurant
    {
        //tworzenie danych pol dla resteuracji
        public int Id { get; set; }//na tej podstawie entity rozpozanje pole

        public string Name { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }    
        public bool HasDelivered { get; set; }
        public string ContactEmail { get; set; }
        public string ContactNumber { get; set; }


        //referancja do tabeli z adresem, szukamy po id adresu
        public int AddressId { get; set; }
        //towrzymy obiekt adresu i obiekt liste dań
        public virtual Address Address { get; set; }
        public virtual List<Dish> Dishes{ get; set; }
}
}
