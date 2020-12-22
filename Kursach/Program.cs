using System;
using Kursach.Models;

namespace Kursach
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var model = new Model();
            
            model.Initialize();
            model.Start();
            
            Console.ReadKey();
        }
    }
}
