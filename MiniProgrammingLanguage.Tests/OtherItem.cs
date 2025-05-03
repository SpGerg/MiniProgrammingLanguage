using System;

namespace MiniProgrammingLanguage.Tests
{
    public class OtherItem : Item
    {
        public override ItemType Type => ItemType.Other;

        private int[] asd = new[] { 1, 2, 3, 4, 5 };
        
        public void Pepe()
        {
            Console.WriteLine("peper");
        }

        public void ICooler()
        {
            asd[4] = 1;
        }
        
        public void ShowMeSomething(int[] array)
        {
            asd = array;
        }

        public int[] GiveMeIt()
        {
            return asd;
        }
    }
}