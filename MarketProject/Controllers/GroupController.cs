using Market.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Market.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GroupController : ControllerBase
    {
        [HttpDelete("deleteGroup")]
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
    }
}
