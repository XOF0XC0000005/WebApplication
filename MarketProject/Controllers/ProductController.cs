using Market.Models;
using Market.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Market.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProductController : ControllerBase
    {
        [HttpGet("getProducts")]
        public IActionResult GetProducts()
        {
            try
            {
                using (var context = new ProductContext())
                {
                    var products = context.Products.Select(x => new Product()
                    {
                        Id = x.Id,
                        Name = x.Name,
                        Description = x.Description
                    });

                    return Ok(products);
                }
            }
            catch
            {
                return StatusCode(500);
            }
        }

        [HttpPost("postProducts")]
        public IActionResult PutProduct([FromQuery] string name, string description, int cost, int groupId)
        {
            try
            {
                using (var context = new ProductContext())
                {

                    if (!context.Products.Any(x => x.Name.ToLower().Equals(name.ToLower())))
                    {
                        context.Add(new Product()
                        {
                            Name = name,
                            Description = description,
                            Cost = cost,
                            ProductGroupId = groupId
                        });

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

        [HttpPatch("patchCostProduct")]
        public IActionResult PatchCostProduct([FromQuery] int id, int newCost)
        {
            try
            {
                using (var context = new ProductContext())
                {
                    if (context.Products.Any(x => x.Id == id))
                    {
                        var product = context.Products.FirstOrDefault(x => x.Id == id);
                        product.Cost = newCost;
                        context.SaveChanges();
                        return Ok();
                    }
                    else
                    {
                        return StatusCode(404);
                    }
                }
            }
            catch
            {
                return StatusCode(500);
            }
        }

        [HttpDelete("deleteProduct")]
        public IActionResult DeleteProduct(int id)
        {
            try
            {
                using (var context = new ProductContext())
                {
                    if (context.Products.Any(x => x.Id == id))
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
