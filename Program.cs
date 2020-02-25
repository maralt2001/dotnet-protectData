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

            // create an instance of Protection Class using the service provider
            Protection instance = ActivatorUtilities.CreateInstance<Protection>(services);
            instance.RunSample();
        }

        // class accept any kind of data Object
        public class Protection
        {
            private IDataProtector _protector;
            private readonly IDataProtectionProvider _provider;

            // the 'provider' parameter is provided by DI
            public Protection(IDataProtectionProvider provider)
            {
                _provider = provider;
               
            }

            public string GetEncryptData<T>(T data, string purpose)
            {
                _protector = _provider.CreateProtector(purpose);

                if (data is object)
                {
                    string result = JsonConvert.SerializeObject(data);
                    return _protector.Protect(result);
                }
                else
                {
                    return _protector.Protect(Convert.ToString(data));
                }
                
                
            }

            public T GetDecryptData<T>(string data, string purpose )
            {
                try
                {
                    _protector = _provider.CreateProtector(purpose);
                    string unprotectedPayload = _protector.Unprotect(data);
                    return JsonConvert.DeserializeObject<T>(unprotectedPayload);
                    
                }
                catch (Exception)
                {
                    return JsonConvert.DeserializeObject<T>(string.Empty);

                }
                
            }

            public void RunSample()
            {

                Person person = new Person { Name = "markus", Password = "hallo" };
                var result = GetEncryptData<Person>(person, "stage1");
                Console.WriteLine(result);

                var decrypt = GetDecryptData<Person>(result, "stage1");
                
                if (decrypt != null)
                {
                    Console.WriteLine(decrypt.AsString());
                }
                else
                {
                    Console.WriteLine("Something went wrong while decrypt");
                }
                
                Console.ReadLine();



            }
        }

        public class Person
        {
            public string Name { get; set; }
            public string Password { get; set; }

            public string AsString() => $"Name: {this.Name}, Password: {this.Password}";
            
        }

        
    }
}
