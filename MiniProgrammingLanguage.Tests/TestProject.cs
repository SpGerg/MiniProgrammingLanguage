using System;

namespace MiniProgrammingLanguage.Tests
{
    public class TestProject
    {
        public string Name { get; set; }

        public AnotherTest AnotherTest { get; set; }

        public string Execute(string content)
        {
            Console.WriteLine(content);

            return "123";
        }
    }
}