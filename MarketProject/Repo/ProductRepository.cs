using AutoMapper;
using Market.Abstractions;
using Market.Models;
using Market.Models.DTO;
using Market.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Caching.Memory;
using System.Text;

namespace Market.Repo
{
    public class ProductRepository : IProductRepository
    {
        private readonly IMapper _mapper;
        private readonly IMemoryCache _cache;
        private ProductContext _context;

        public ProductRepository(IMapper mapper, IMemoryCache memoryCache, ProductContext context)
        {
            _mapper = mapper;
            _cache = memoryCache;
            _context = context;
        }

        public int AddGroup(ProductGroupDto group)
        {
            using (_context)
            {
                var entityGroup = _context.ProductGroups.FirstOrDefault(p => p.Name.ToLower() == group.Name.ToLower());

                if (entityGroup == null)
                {
                    entityGroup = _mapper.Map<ProductGroup>(group);
                    _context.ProductGroups.Add(entityGroup);
                    _context.SaveChanges();

                    _cache.Remove("groups");
                }

                return entityGroup.Id;
            }
        }

        public int AddProduct(ProductDto product)
        {
            using (_context)
            {
                var entityProduct = _context.Products.FirstOrDefault(p => p.Name.ToLower() == product.Name.ToLower());

                if (entityProduct == null)
                {
                    entityProduct = _mapper.Map<Product>(product);
                    _context.Products.Add(entityProduct);
                    _context.SaveChanges();

                    _cache.Remove("products");
                }

                return entityProduct.Id;
            }
        }

        public IEnumerable<ProductGroupDto> GetGroups()
        {
            if (_cache.TryGetValue("groups", out List<ProductGroupDto> groups))
                return groups;

            using (_context)
            {
                var groupsList = _context.ProductGroups.Select(x => _mapper.Map<ProductGroupDto>(x)).ToList();
                _cache.Set("groups", groupsList, TimeSpan.FromMinutes(30));
                return groupsList;
            }
        }

        public IEnumerable<ProductDto> GetProducts()
        {
            if (_cache.TryGetValue("products", out List<ProductDto> products))
                return products;

            using (_context)
            {
                var productsList = _context.Products.Select(x => _mapper.Map<ProductDto>(x)).ToList();
                _cache.Set("products", productsList, TimeSpan.FromMinutes(30));
                return productsList;
            }
        }

        public string GetProductsCsv()
        {
           using (_context)
            {
                var products = _context.Products.Select(x => _mapper.Map<ProductDto>(x)).ToList();
                return GetCsv(products);
            }
        }

        public string GetStatisticFile()
        {
            var cacheStatistic = _cache.GetCurrentStatistics();
            var content = $"Current Entry Count:{cacheStatistic.CurrentEntryCount};\nCurrent Estimated Size:{cacheStatistic.CurrentEstimatedSize};\nTotal Misses:{cacheStatistic.TotalMisses};\nTotalHits:{cacheStatistic.TotalHits};\n";
            string fileName = "statistic" + DateTime.Now.ToBinary().ToString() + ".csv";
            File.WriteAllText(Path.Combine(Directory.GetCurrentDirectory(), "StaticFiles", fileName), content);
            return fileName;
        }

        private string GetCsv(IEnumerable<ProductDto> products)
        {
            StringBuilder sb = new StringBuilder();

            foreach (var product in products)
            {
                sb.AppendLine($"{product.Id}; {product.Name};\n");
            }

            return sb.ToString();
        }
    }
}
