using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


//specify the the startup file where we want to execute the configuration method on when the host is created and configuured
[assembly: FunctionsStartup(typeof(FunctionApp1.Startup))]

namespace FunctionApp1
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder) //configure the builder// add DbContext to the DI container
        {
            //read connection string
            var keyVaultUrl = new Uri(Environment.GetEnvironmentVariable("KeyVaultUrl"));
            //connect to azure key vault, we need secret client
            var secretClient = new SecretClient(keyVaultUrl, new DefaultAzureCredential());
            //read the secret ("sql" is the key of the secret)
            var readSecret = secretClient.GetSecret("sql").Value.Value;
            //add DbContext 
            builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(readSecret));
        }
    }
}
