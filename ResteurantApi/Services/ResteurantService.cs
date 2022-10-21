using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using ResteurantApi.Entities;
using ResteurantApi.Models;
using System.Collections.Generic;
using System.Linq;
using AutoMapper.Configuration.Conventions;
using Microsoft.Extensions.Logging;
using ResteurantApi.Exceptions;

namespace ResteurantApi.Services
{
    public interface IResteurantService
    {
        void Update(int id, UpdateResteurantDto dto);
        ResteurantDto GetById(int id);
        void Delete(int id);
        IEnumerable<ResteurantDto> GetAll();
        int Create(CreateResteurantDto dto);
    }

    public class ResteurantService : IResteurantService
    {
        private readonly ResteurantDBContext _dbContext;
        private readonly IMapper _mapper;
        private readonly ILogger<ResteurantService> _logger;
        public ResteurantService(ResteurantDBContext dBContext, IMapper mapper, ILogger<ResteurantService> logger)
        {
            _dbContext = dBContext;
            _mapper = mapper;
            _logger = logger;
        }

        public void Update(int id, UpdateResteurantDto dto)
        {
            //pobranie informacji o resteuracji z bazy danych
            var resteurants = _dbContext
                .Resteurants
                .FirstOrDefault(r => r.Id == id);


            if (resteurants == null)
            {
                throw new NotFoundException("Resteurant not found");

            }
            resteurants.Name = dto.Name;
            resteurants.Description = dto.Description;
            resteurants.HasDelivered = dto.HasDelivered;

            _dbContext.SaveChanges();
            
        }

        public ResteurantDto GetById(int id)
        {
            var resteurants = _dbContext
                .Resteurants
                .Include(r => r.Address)
                .Include(r => r.Dishes)
                .FirstOrDefault(r => r.Id == id);

            if (resteurants == null)
            {
               throw new NotFoundException("Resteurant not found");

            }
            var result = _mapper.Map<ResteurantDto>(resteurants);

            return result;
        }

        public void Delete(int id)
        {
            _logger.LogError($"Restuerant with id  {id}  DELETE action invoked");

            var resteurants = _dbContext
                .Resteurants
                .FirstOrDefault(r => r.Id == id);

            if (resteurants == null)
            {
                throw new NotFoundException("Resteurant not found");

            }

            _dbContext.Resteurants.Remove(resteurants);
            _dbContext.SaveChanges();
            
        }


        public IEnumerable<ResteurantDto> GetAll()
        {
            var resteurants = _dbContext
                .Resteurants
                .Include(r => r.Address)
                .Include(r => r.Dishes)
                .ToList();

            var result = _mapper.Map<List<ResteurantDto>>(resteurants);

            return result;
        }

        public int Create(CreateResteurantDto dto)
        {
            var resteurant = _mapper.Map<Resteurant>(dto);
            _dbContext.Resteurants.Add(resteurant);
            _dbContext.SaveChanges();

            return resteurant.Id;
        }
     }
}
