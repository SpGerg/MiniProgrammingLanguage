using System;

namespace MiniProgrammingLanguage.Tests
{
    public class AnotherTest
    {
        public string Tg { get; set; }

        public string Testificate(TestProject testProject)
        {
            Console.WriteLine(testProject.Name);

            return "523";
        }
        
        public string Testificate2(TestProject testProject, bool checkerBecker, bool trylala)
        {
            Console.WriteLine(testProject.Name);
            Console.WriteLine(checkerBecker);
            Console.WriteLine(trylala);

            return "523";
        }
    }
}