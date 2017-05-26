using System;
using System.Linq;
using Autofac;
using Slalom.Stacks;
using Slalom.Stacks.MongoDb;
using Slalom.Stacks.Search;
using Slalom.Stacks.Text;

namespace ConsoleClient
{
    public class Program
    {
        public static void Main(string[] args)
        {
            using (var stack = new Stack())
            {
                stack.UseMongoDb(e =>
                {
                  //  e.WithDatabase("treatment-local");
                });

                stack.Search.Read<User>().Skip(10).Take(10).OutputToJson();
            }

            Console.WriteLine("Running application.  Press any key to halt...");
            Console.ReadKey();
        }
    }
}