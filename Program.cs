using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.DataProtection;
using System;
using Newtonsoft.Json;

namespace DataProtector
{
    class Program
    {
        static void Main(string[] args)
        {
            // add data protection services
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddDataProtection();
            ServiceProvider services = serviceCollection.BuildServiceProvider();

            // create an instance of MyClass using the service provider
            Protection instance = ActivatorUtilities.CreateInstance<Protection>(services);
            instance.RunSample();
        }

        public class Protection
        {
            IDataProtector _protector;

            // the 'provider' parameter is provided by DI
            public Protection(IDataProtectionProvider provider)
            {
                _protector = provider.CreateProtector("Stage1");
            }

            public void RunSample()
            {
                var data = new { user = "Markus", password = "hallo" };
                var result = JsonConvert.SerializeObject(data);

                // protect the payload
                var protectedPayload = _protector.Protect(result.ToString());
                Console.WriteLine($"Protect returned: {protectedPayload}");

                // unprotect the payload
                try
                {
                    string unprotectedPayload = _protector.Unprotect(protectedPayload);
                    Console.WriteLine($"Unprotect returned: {unprotectedPayload}");
                }
                catch (Exception)
                {
                    Console.WriteLine("Can not decyrpt");
                    
                }
            }
        }

        
    }
}
