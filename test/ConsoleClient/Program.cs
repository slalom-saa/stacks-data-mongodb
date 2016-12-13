using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Slalom.Stacks.Configuration;
using Slalom.Stacks.Data.MongoDb;

// ReSharper disable AccessToDisposedClosure

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
                var watch = new Stopwatch();
                using (var container = new ApplicationContainer(this))
                {
                    container.UseMongoDbRepositories();

                    watch.Start();
                    for (var i = 0; i < 100; i++)
                    {
                        await Task.Run(() => container.Domain.AddAsync(new Item { Name = "name" }, new Item { Name = "name 2" }).ConfigureAwait(false));
                    }
                    watch.Stop();
                }
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"Execution completed successfully in {watch.Elapsed}.  Press any key to exit...");
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