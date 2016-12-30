using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Slalom.Stacks;
using Slalom.Stacks.Configuration;
using Slalom.Stacks.Data.MongoDb;
using Slalom.Stacks.Test.Examples.Actors.Items.Add;
using Slalom.Stacks.Test.Examples.Domain;

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
                using (var container = new ApplicationContainer(typeof(Item)))
                {
                    container.UseMongoDbRepositories();

                    await container.Domain.ClearAsync<Item>();

                    var tasks = new List<Task>(count);
                    watch.Start();
                    Parallel.For(0, count, new ParallelOptions { MaxDegreeOfParallelism = 4 }, e =>
                    {
                        tasks.Add(container.SendAsync(new AddItemCommand(Guid.NewGuid().ToString())));
                    });
                    await Task.WhenAll(tasks);

                    var actual = container.Domain.FindAsync<Item>().Result.Count();
                    if (actual != count)
                    {
                        throw new Exception($"The expected number of items added, {actual}, did not equal the expected, {count}.");
                    }
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