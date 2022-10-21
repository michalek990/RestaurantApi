using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Microsoft.Extensions.Configuration;
using ResteurantApi.Entities;
using ResteurantApi.Exceptions;
using ResteurantApi.Models;

namespace ResteurantApi.Services
{
    public interface IDishService
    {
        int Create(int resteurantId, CreateDishDto dto);
        DishDto GetById(int resteurantId, int dishId);
        List<DishDto> GetAll(int resteurantId);
        void DeleteAll(int resteurantId);
    }
    public class DishService : IDishService
    {
        private readonly ResteurantDBContext _context;
        private readonly IMapper _mapper;
        public DishService(ResteurantDBContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;

        }

        public int Create(int resteurantId, CreateDishDto dto)
        {
            var resteurant = GetResteurantById(resteurantId);

            var dishEntity = _mapper.Map<Dish>(dto);

            //ustalamy id resteuracji dla konkretnego id dania
            dishEntity.ResteurantId = resteurantId;

            _context.Dishes.Add(dishEntity);
            _context.SaveChanges();
            return dishEntity.Id;

        }

        public DishDto GetById(int resteurantId, int dishId)
        {
            var resteurant = GetResteurantById(resteurantId);

            var dish = _context.Dishes.FirstOrDefault(d => d.Id == dishId);
            if (dish is null || dish.ResteurantId != resteurantId)
            {
                throw new NotFoundException("Dish not found");
            }

            var dishDto = _mapper.Map<DishDto>(dish);
            return dishDto;
        }

        public List<DishDto> GetAll(int resteurantId)
        {
            var resteurant = GetResteurantById(resteurantId);

            

            var dishDto = _mapper.Map<List<DishDto>>(resteurant.Dishes);
            return dishDto;
        }

        public void DeleteAll(int resteurantId)
        {
            var resteurant = GetResteurantById(resteurantId);
            

           _context.RemoveRange(resteurant.Dishes);
           _context.SaveChanges();

            
        }

        private Resteurant GetResteurantById(int resteurantId)
        {
            var resteurant = _context
                .Resteurants
                .Include(r => r.Dishes)//chcemy dla danej resteuracji pokazac jej dania
                .FirstOrDefault(r => r.Id == resteurantId);

            if (resteurant == null)
            {
                throw new NotFoundException("Resteurant not found");
            }

            return resteurant;
        }

    }
}
