using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ResteurantApi.Entities;
using ResteurantApi.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ResteurantApi.Services;

namespace ResteurantApi.Controllers
{
    [Route("api/resteurant")]
    [ApiController]
    public class ResteurantController : ControllerBase
    {

       // private readonly ResteurantDBContext _dbContext;
        //private readonly IMapper _mapper;
        private readonly IResteurantService _resteurantService;

        //public ResteurantController(ResteurantDBContext dbContext, IMapper mapper)
        //{
           // _dbContext = dbContext;
          //  _mapper = mapper;
        //}

        public ResteurantController(IResteurantService resteurantService)
        {

            _resteurantService = resteurantService;

        }
        [HttpDelete("{id}")]
        public ActionResult Delete([FromRoute] int id)
        {
            _resteurantService.Delete(id);
            return NoContent();
        }

        [HttpPut("{id}")]
        public ActionResult Update([FromBody] UpdateResteurantDto dto, [FromRoute] int id)
        {
           // if (!ModelState.IsValid)
           // {
           //     return BadRequest(ModelState);
          //  }

           _resteurantService.Update(id, dto);

           return Ok();

        }

        [HttpPost]
        public ActionResult CreateResteurant([FromBody] CreateResteurantDto dto)
        {
          
            
         //   if (!ModelState.IsValid)
          //  {
         //       return BadRequest(ModelState);
         //   }

            var id = _resteurantService.Create(dto);

            return Created($"/api/restaurant/{id}", null);
        }



        [HttpGet]
        public ActionResult<IEnumerable<ResteurantDto>> GetAll()
        {
            //var resteurantsDtos = resteurants.Select(r => new ResteurantDto()
            //{
            //   Name = r.Name,
            //  Category = r.Category,
            // City = r.Address.City
            //});
            //zwracamy do klinta listy resteuracji ale tylko konkretne dane
            // var resteurantsDtos = _mapper.Map<List<ResteurantDto>>(resteurants);


            //nowa wersja
            var resteurantsDtos = _resteurantService.GetAll();

            return Ok(resteurantsDtos);
        }
        [HttpGet("{id}")]
        public ActionResult<ResteurantDto> Get([FromRoute] int id)
        {
            var resteurants = _resteurantService.GetById(id);
            

            return Ok(resteurants);
            
        }
    }
}
