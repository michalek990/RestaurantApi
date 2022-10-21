using Microsoft.EntityFrameworkCore;
using ResteurantApi.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;

namespace ResteurantApi
{
    public class ResteurantSeeder
    {

        private readonly ResteurantDBContext _dBContext;

        public ResteurantSeeder(ResteurantDBContext dbContext)
        {
            _dBContext = dbContext;
        }
        public void Seed()
        {
            //srapwdzamy polaczenie do bazy danych
            if (_dBContext.Database.CanConnect())
            {
                if (!_dBContext.Resteurants.Any())
                {
                    var resteurants = GetResteurants();
                    _dBContext.Resteurants.AddRange(resteurants);
                    _dBContext.SaveChanges();

                    
                }
            }
        }
        private IEnumerable<Resteurant> GetResteurants()
        {
            var resteurants = new List<Resteurant>()
            {
                new Resteurant()
                {
                    Name = "KFC",
                    Category = "FastFood",
                    Description = "KFC",
                    ContactEmail = "kfc@gmail.com",
                    HasDelivered = true,
                    Dishes = new List<Dish>()
                    {
                        new Dish()
                        {
                            Name = "Kubelek",
                            Price = 10.3M,
                        },
                        new Dish()
                        {
                            Name = "Kubelek XXL",
                            Price = 120.3M,
                        },
                    },
                    Address = new Address()
                    {
                        City = "Warszawa",
                        Street = "warszawska",
                        PostalCode = "21-222",
                    }
                },
                new Resteurant()
                {
                    Name = "Mcdonald",
                    Category = "FastFood",
                    Description = "Mcdonald",
                    ContactEmail = "Mcdonald@gmail.com",
                    HasDelivered = true,
                    Dishes = new List<Dish>()
                    {
                        new Dish()
                        {
                            Name = "Kubelek",
                            Price = 10.3M,
                        },
                        new Dish()
                        {
                            Name = "Kubelek XXL",
                            Price = 120.3M,
                        },
                    },
                    Address = new Address()
                    {
                        City = "Warszawa",
                        Street = "warszawska",
                        PostalCode = "21-222",
                    }
                },

            };
            return resteurants;
        }


    }
}
