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
            Task.Factory.StartNew(() => new Program().Start());
            Console.WriteLine("Running application.  Press any key to halt...");
            Console.ReadKey();
        }

        public async Task Start()
        {
            try
            {
                using (var container = new ApplicationContainer(this))
                {
                    container.UseMongoDbRepositories();

                    await container.Domain.AddAsync(new Item { Name = "name" }, new Item { Name = "name 2" });

                    var count = container.Domain.OpenQuery<Item>().Count();
                }
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Execution completed successfully.  Press any key to exit...");
                Console.ResetColor();
            }
            catch (Exception exception)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(exception);
                Console.ResetColor();
            }
        }
    }
}
