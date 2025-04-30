using System;

namespace MiniProgrammingLanguage.Tests
{
    public class Player
    {
        public string Name { get; set; }

        public void Broadcast(object target)
        {
            Console.WriteLine(target);
        }
    }
}