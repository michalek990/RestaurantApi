using System;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

using ResteurantApi.Entities;
using ResteurantApi.Models;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using AutoMapper.Configuration.Conventions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using ResteurantApi.Authorization;
using ResteurantApi.Exceptions;

namespace ResteurantApi.Services
{
    public interface IResteurantService
    {
        void Update(int id, UpdateResteurantDto dto);
        ResteurantDto GetById(int id);
        void Delete(int id);
        PageResult<ResteurantDto> GetAll(ResteurantQuery query);
        int Create(CreateResteurantDto dto);
    }

    public class ResteurantService : IResteurantService
    {
        private readonly ResteurantDBContext _dbContext;
        private readonly IMapper _mapper;
        private readonly ILogger<ResteurantService> _logger;
        private readonly IAuthorizationService _authorizationService;
        private readonly IUserContextService _uerContextService;

        public ResteurantService(ResteurantDBContext dBContext, IMapper mapper, ILogger<ResteurantService> logger, IAuthorizationService authorizationService, IUserContextService uerContextService)
        {
            _dbContext = dBContext;
            _mapper = mapper;
            _logger = logger;
            _authorizationService = authorizationService;
            _uerContextService = uerContextService;
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


           var authorizationResult= _authorizationService.AuthorizeAsync(_uerContextService.User, resteurants,
                new ResourceOperationRequirement(ResourceOperation.Update)).Result;


           if (!authorizationResult.Succeeded)
           {
               throw new ForbidException();
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

            var authorizationResult = _authorizationService.AuthorizeAsync(_uerContextService.User, resteurants,
                new ResourceOperationRequirement(ResourceOperation.Update)).Result;


            if (!authorizationResult.Succeeded)
            {
                throw new ForbidException();
            }

            _dbContext.Resteurants.Remove(resteurants);
            _dbContext.SaveChanges();
            
        }


        public PageResult<ResteurantDto> GetAll(ResteurantQuery query)
        {
            //szukanie resteuracji wedlug frazy
            var baseQuery = _dbContext
                .Resteurants
                .Include(r => r.Address)
                .Include(r => r.Dishes)
                //szukanie tych zapytan ktore zawieraja dana fraze nie zwracajac uwagi na wielkosc liter
                .Where(r => query.SearchPhrase == null || (r.Name.ToLower().Contains(query.SearchPhrase.ToLower()) || r.Description.ToLower()
                .Contains(query.SearchPhrase.ToLower())));

            //sprwadzamy czy ktos podal jak ma byc sortowane
            if (string.IsNullOrEmpty(query.SortBy))
            {
                var collumnsSelector = new Dictionary<string, Expression<Func<Resteurant, object>>>
                {
                    //mamy podane typy tabel dla ktorych mozemy sortowac

                    { nameof(Resteurant.Name), r => r.Name },
                    { nameof(Resteurant.Description), r => r.Description },
                    { nameof(Resteurant.Category), r => r.Category },
                };
                //zmienna pomocnicza przechowujaca dane po czym bedziemy sortowac
                var selectedColumn = collumnsSelector[query.SortBy];

                //jesli jest sortowanie rosnace to sortuj po nazwie
                baseQuery = query.SortDirection == SortDirection.ASC
                    ? baseQuery.OrderBy(selectedColumn)//rosnaca
                    : baseQuery.OrderByDescending(selectedColumn);//malejaca
            }


            //tutaj wszystkie wyniki przeliczamy na strony i elemanty na stronach
            //funkcja ta oblicza ilosc elemetnow na stronie i ilosc stron
            var resteurants =  baseQuery
                .Skip(query.PageSize*(query.PageNumber-1))
                .Take(query.PageSize)
                .ToList();

            //liczenie
            var totalItemsCount = baseQuery.Count();

            var result = _mapper.Map<List<ResteurantDto>>(resteurants);

            var pageResult = new PageResult<ResteurantDto>(result, totalItemsCount, query.PageSize, query.PageNumber);

            return pageResult;
        }

        public int Create(CreateResteurantDto dto)
        {
            var resteurant = _mapper.Map<Resteurant>(dto);
            resteurant.CreatedById = _uerContextService.GetUserId;
            _dbContext.Resteurants.Add(resteurant);
            _dbContext.SaveChanges();

            return resteurant.Id;
        }
     }
}
