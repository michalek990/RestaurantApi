namespace ResteurantApi.Entities
{
    public class Dish
    {
        public int Id { get; set; }

        public string Name { get; set; }
        public  string Description { get; set; }
        public decimal Price { get; set; }


        //tworzymy obiekt klasy resteurant
        public int ResteurantId { get; set; }
        //referencje klasy resteurant
        public virtual Resteurant Resteurant { get; set; }
    }
}
