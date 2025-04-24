using System;
using System.IO;
using MiniProgrammingLanguage.Std;
using MiniProgrammingLanguage.Std.Functions;
using MiniProgrammingLanguage.Std.Types;
using MiniProgrammingLanguage.Std.Variables;

namespace MiniProgrammingLanguage.Tests
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            var filepath = Path.Combine(Directory.GetCurrentDirectory(), "script2.mpl");
            var entryPoint = new EntryPoint(filepath);

            var print = PrintFunction.Create();
            var sleep = SleepFunction.Create();

            var task = TaskType.Create();

            var module = ModuleVariable.Create();
            
            entryPoint.Run(out var exception,
                new [] { task },
                new [] { print, sleep },
                null,
                new [] { module });

            if (exception is not null)
            {
                Console.WriteLine(exception.Message);
            }

            Console.ReadLine();
        }
    }
}