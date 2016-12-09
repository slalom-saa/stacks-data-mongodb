using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Slalom.Stacks.Configuration;
using Slalom.Stacks.Data;
using Slalom.Stacks.Data.MongoDb;

#pragma warning disable 4014

namespace ConsoleClient
{
    public class Program
    {
        public static void Main(string[] args)
        {
            new Program().Run();
            Console.WriteLine("Press any key to stop...");
            Console.ReadKey();
        }

        public async Task Run()
        {
            try
            {
                using (var container = new ApplicationContainer(this))
                {
                    container.UseMongoDbRepositories();

                    await container.Domain.AddAsync(new Item { Name = "name" });

                    await container.Domain.AddAsync(new Item { Name = "name 2" });


                    Console.WriteLine(container.Domain.OpenQuery<Item>().Count());
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
            Console.WriteLine("Done executing");
        }
    }
}
