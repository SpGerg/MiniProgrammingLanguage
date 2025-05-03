using System;

namespace MiniProgrammingLanguage.Tests
{
    public abstract class Item
    {
        public abstract ItemType Type { get; }

        public void Destroy()
        {
            Console.WriteLine("Destroyed");
        }

        public static ExecutableItem CreateExecutable()
        {
            return new ExecutableItem();
        }
    }
}