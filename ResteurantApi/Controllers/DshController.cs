using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using ResteurantApi.Models;
using ResteurantApi.Services;

namespace ResteurantApi.Controllers
{
    [Route("api/resteurant/{resteurantId}/dish")]
    [ApiController]
    public class DshController : ControllerBase
    {
        private readonly IDishService _dishService;

        public DshController(IDishService dishService)
        {
            _dishService = dishService;
        }

        [HttpDelete]
        public ActionResult Delete([FromRoute] int resteuranId)
        {
            _dishService.DeleteAll(resteuranId);
            return NoContent();
        }

        [HttpPost]
        public ActionResult Post([FromRoute] int resteurantId, [FromBody]CreateDishDto dto)
        {
            var newDishId = _dishService.Create(resteurantId, dto);
            return Created($"api/resteurant/{resteurantId}/dish/{newDishId}", null);
        }

        [HttpGet("{dishId}")]
        public ActionResult<DishDto> Get([FromRoute] int resteurantId, [FromRoute] int dishId)
        {
            DishDto dish = _dishService.GetById(resteurantId, dishId);
            return Ok(dish);
        }

        [HttpGet]
        public ActionResult<List<DishDto>> Get([FromRoute] int resteurantId)
        {
            var result = _dishService.GetAll(resteurantId);
            return Ok(result);
        }

        
    }
}
