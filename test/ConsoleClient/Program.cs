using System;
using System.Linq;
using Slalom.Stacks.Data.MongoDb;
using Slalom.Stacks.Test.Examples;

namespace ConsoleClient
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var runner = new ExampleRunner(typeof(MongoDbEntityContext));
            runner.With(e => e.UseMongoDbRepositories());
            runner.Start();

            Console.WriteLine("Running application.  Press any key to halt...");
            Console.ReadKey();
        }
    }
}