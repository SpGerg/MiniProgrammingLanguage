using System;

namespace MiniProgrammingLanguage.Tests
{
    public class OtherItem : Item
    {
        public override ItemType Type => ItemType.Other;

        public void Pepe()
        {
            Console.WriteLine("peper");
        }
    }
}