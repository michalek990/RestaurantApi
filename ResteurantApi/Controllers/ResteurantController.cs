using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ResteurantApi.Entities;
using ResteurantApi.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using ResteurantApi.Services;

namespace ResteurantApi.Controllers
{
    [Route("api/resteurant")]
    [ApiController]
    [Authorize]
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


        [HttpPost]
        [Authorize(Roles = "Manager,Admin")]
        public ActionResult CreateResteurant([FromBody] CreateResteurantDto dto)
        {
            //PRZYPISANIE DO UTWORZONEJ RESTEURACJI ID TWORCY!!!!!  
            var userId = int.Parse(User.FindFirst(c => c.Type == ClaimTypes.NameIdentifier).Value);
            var id = _resteurantService.Create(dto);

            return Created($"/api/restaurant/{id}", null);
        }

        [HttpGet]
        //[Authorize(Policy = "HasNationality")]
        //[Authorize(Policy = "Atleast20")]
        [AllowAnonymous]
        public ActionResult<IEnumerable<ResteurantDto>> GetAll([FromQuery]ResteurantQuery query)
        {
            //nowa wersja
            var resteurantsDtos = _resteurantService.GetAll(query);

            return Ok(resteurantsDtos);
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

        [HttpDelete("{id}")]
        public ActionResult Delete([FromRoute] int id)
        {
            _resteurantService.Delete(id);
            return NoContent();
        }

        

        [HttpGet("{id}")]
        [AllowAnonymous]
        public ActionResult<ResteurantDto> Get([FromRoute] int id)
        {
            var resteurants = _resteurantService.GetById(id);
            

            return Ok(resteurants);
            
        }
    }
}
