using Microsoft.AspNetCore.Mvc;
using RedisCacheDemo.DBContext;
using RedisCacheDemo.Model;
using RedisCacheDemo.Services;

namespace RedisCacheDemo.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly DBContextClass _dbContextClass;
        private readonly ICacheService _cacheService;

        public ProductController(DBContextClass dbContextClass,
                                 ICacheService cacheService)
        {
            _dbContextClass = dbContextClass;
            _cacheService = cacheService;
        }

        [HttpGet("products")]
        public IEnumerable<Product> Get() {
            var cacheData = _cacheService.GetData<IEnumerable<Product>>("product");
            if (cacheData != null ) {
                return cacheData;
            }

            var expiryTime = DateTimeOffset.Now.AddMinutes(5);
            var productList = _dbContextClass.Products.ToList();
            _cacheService.SetData<IEnumerable<Product>>("product", productList, expiryTime);
            return productList;
        }


        [HttpGet("product")]
        public Product Get(int id)
        {
            var cacheData = _cacheService.GetData<IEnumerable<Product>>("product");
            if (cacheData != null)
            {
                var filteredData = cacheData.Where(x => x.ProductId == id).FirstOrDefault();
                return filteredData;
            }

            var data = _dbContextClass.Products.Where(x => x.ProductId == id).FirstOrDefault();
            return data;
        }

        [HttpPost("addProduct")]
        public async Task<Product> Post(Product product)
        {
            var obj = _dbContextClass.Products.AddAsync(product);
            _cacheService.RemoveData("product");
            await _dbContextClass.SaveChangesAsync();
            return obj.Result.Entity;
        }

        [HttpPut("updateProduct")]
        public void Put(Product product)
        {
            _dbContextClass.Update(product);
            _dbContextClass.SaveChanges();
            _cacheService.RemoveData("product");
        }

        [HttpDelete("deleteProduct")]
        public void Delete(int id)
        {
            var filteredData = _dbContextClass.Products.Where(x => x.ProductId == id).FirstOrDefault();
            if (filteredData != null)
            {
                _dbContextClass.Remove(filteredData);
                _cacheService.RemoveData("product");
                _dbContextClass.SaveChanges();
            }
            
        }

    }

}
