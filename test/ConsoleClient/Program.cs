using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Slalom.Stacks.Configuration;
using Slalom.Stacks.Data;
using Slalom.Stacks.Data.MongoDb;
using Slalom.Stacks.Domain;
#pragma warning disable 4014

namespace ConsoleClient
{
    public class Item : Entity, IAggregateRoot
    {
        public string Name { get; set; }
    }

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
                    container.RegisterModule(new MongoDbDataModule());

                    await container.Domain.AddAsync(new Item { Name = "name" });

                    await container.Domain.AddAsync(new Item { Name = "name 2" });
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
