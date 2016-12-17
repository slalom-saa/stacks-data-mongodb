using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using ConsoleClient.Commands.AddItem;
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
                var count = 1000;
                using (var container = new ApplicationContainer(this))
                {
                    container.UseMongoDbRepositories();

                    var tasks = new List<Task>(count);
                    watch.Start();
                    Parallel.For(0, count, e =>
                    {
                        tasks.Add(container.Bus.SendAsync(new AddItemCommand(DateTime.Now.Ticks.ToString())));
                    });
                    await Task.WhenAll(tasks);
                }
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"Execution for {count:N0} items completed successfully in {watch.Elapsed} - {Math.Ceiling(count / watch.Elapsed.TotalSeconds):N0} per second.  Press any key to exit...");
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