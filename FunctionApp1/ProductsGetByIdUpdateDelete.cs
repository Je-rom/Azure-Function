using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace FunctionApp1
{
    public class ProductsGetByIdUpdateDelete
    {
        private readonly AppDbContext _appDbContext;
        public ProductsGetByIdUpdateDelete(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        [FunctionName("ProductsGetByIdUpdateDelete")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "put", "delete", Route = "product/{id}")] HttpRequest req,
            int id)
        {
            if (req.Method == HttpMethods.Get)
            {
                var product = await _appDbContext.Products.FirstOrDefaultAsync(p => p.Id == id);
                if (product == null) return new NotFoundResult();
                
                return new OkObjectResult(product);

            }
            else if (req.Method == HttpMethods.Put)
            {
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                var product = JsonConvert.DeserializeObject<Product>(requestBody);
                product.Id = id;
                _appDbContext.Products.Update(product);
                await _appDbContext.SaveChangesAsync();

                return new OkObjectResult(product);
            }
            else
            {
                var product = await _appDbContext.Products.FirstOrDefaultAsync(p =>p.Id == id);
                if(product == null) return new NotFoundResult();

                _appDbContext.Products.Remove(product);
                await _appDbContext.SaveChangesAsync();

                return new OkObjectResult(product);

            }
        }
    }
}
