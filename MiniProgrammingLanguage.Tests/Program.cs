using System;
using System.IO;
using MiniProgrammingLanguage.Core.Interpreter;
using MiniProgrammingLanguage.SharpKit;
using MiniProgrammingLanguage.Std;

namespace MiniProgrammingLanguage.Tests
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            var filepath = Path.Combine(Directory.GetCurrentDirectory(), "script2.mpl");
            var entryPoint = new EntryPoint(filepath);

            var stdModule = StdModule.Create();
            var sharpKitModule = SharpKitModule.Create();

            entryPoint.Run(out var exception, stdModule, sharpKitModule);

            if (exception is not null)
            {
                Console.WriteLine(exception.Message);
            }

            Console.ReadLine();
        }
    }
}