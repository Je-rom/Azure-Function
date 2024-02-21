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
    public class ProductsGetAllCreate
    {
        private readonly AppDbContext _appDbContext;
        public ProductsGetAllCreate(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        [FunctionName("ProductsGetAllCreate")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "products")] HttpRequest req)
        {
            if(req.Method == HttpMethods.Post)
            {
                //create new product
                //read the http body
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                var product = JsonConvert.DeserializeObject<Product>(requestBody);
                _appDbContext.Products.Add(product);
                await _appDbContext.SaveChangesAsync();
                return new CreatedResult("/product", product);
            }

            var products = await _appDbContext.Products.ToListAsync();
            return new OkObjectResult(products);
        }
    }
}


/*"Server=DESKTOP-PTV417N;Database=AzDB;Trusted_Connection=True;TrustServerCertificate=True;"*/