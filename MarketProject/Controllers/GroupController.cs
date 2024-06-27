using Market.Abstractions;
using Market.Models.DTO;
using Market.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Market.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GroupController : ControllerBase
    {
        private readonly IProductRepository _repository;

        public GroupController(IProductRepository repository)
        {
            _repository = repository;
        }

        [HttpDelete("delete_group/{id}")]
        public IActionResult DeleteGroup(int id)
        {
            try
            {
                using (var context = new ProductContext())
                {
                    if (context.ProductGroups.Any(x => x.Id == id))
                    {
                        context.Remove(id);
                        context.SaveChanges();
                        return Ok();
                    }
                    else
                    {
                        return StatusCode(409);
                    }
                }
            }
            catch
            {
                return StatusCode(500);
            }
        }

        [HttpPost("add_group")]
        public IActionResult AddGroup([FromBody] ProductGroupDto productGroupDto)
        {
            var result = _repository.AddGroup(productGroupDto);
            return Ok(result);
        }

        [HttpGet("get_groups")]
        public IActionResult GetGroups()
        {
            var groups = _repository.GetGroups();
            return Ok(groups);
        }
    }
}
