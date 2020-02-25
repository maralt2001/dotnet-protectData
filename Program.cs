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
            var user = new Person { Name = "markus", password = "hallo" };
            // create an instance of MyClass using the service provider
            Protection<Person> instance = ActivatorUtilities.CreateInstance<Protection<Person>>(services, user);
            instance.RunSample();
        }

        public class Protection<T>
        {
            readonly IDataProtector _protector;
            readonly T _data;
            // the 'provider' parameter is provided by DI
            public Protection(IDataProtectionProvider provider, T data)
            {
                _protector = provider.CreateProtector("Stage1");
                _data = data;
            }

            public void RunSample()
            {
                
                var result = JsonConvert.SerializeObject(_data);

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

        public class Person
        {
            public string Name { get; set; }
            public string password { get; set; }
        }

        
    }
}
