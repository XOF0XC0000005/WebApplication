using Market.Abstractions;
using Market.Models.DTO;
using Market.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Market.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly IProductRepository _repository;

        public ProductController(IProductRepository repository)
        {
            _repository = repository;
        }

        [HttpGet("get_cache_statistic")]
        public ActionResult<string> GetCacheStatistic() 
        {
            var fileName = _repository.GetStatisticFile();
            return "https://" + Request.Host.ToString() + "/static/" + fileName;
        }

        [HttpGet("get_products")]
        public IActionResult GetProducts()
        {
            var products = _repository.GetProducts();
            return Ok(products);
        }

        [HttpGet(template: "get_products_csv")]
        public FileContentResult GetProductsCsv() 
        {
            var result = _repository.GetProductsCsv();
            return File(new System.Text.UTF8Encoding().GetBytes(result), "text/csv", "products_report.csv");
        }

        [HttpPost("add_products")]
        public IActionResult AddProduct([FromBody] ProductDto productDto)
        {
            var result = _repository.AddProduct(productDto);
            return Ok(result);
        }

        [HttpPatch("patch_cost_product/{id}")]
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

        [HttpDelete("delete_product/{id}")]
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
