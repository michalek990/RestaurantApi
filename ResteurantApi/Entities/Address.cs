namespace ResteurantApi.Entities
{
    public class Address
    {
        public int Id { get; set; }

        public string City { get; set; }

        public string Street { get; set; }

        public string PostalCode { get; set; }

        //referencja do adresu
        public virtual Resteurant Resteurant { get; set; }

    }
}
