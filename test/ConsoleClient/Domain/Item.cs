using Slalom.Stacks.Domain;

namespace ConsoleClient
{
    public class Item : Entity, IAggregateRoot
    {
        public string Name { get; set; }
    }
}