using System;

namespace MiniProgrammingLanguage.Tests
{
    public class ExecutableItem : Item
    {
        public override ItemType Type => ItemType.Executable;
        
        public void Execute()
        {
            Console.WriteLine("Doer");
        }
    }
}