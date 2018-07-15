using System;

namespace testchild
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            /*
            Console.WriteLine("  Child running...");
            Console.Write("  You name: ");
            var name = Console.ReadLine();
            Console.WriteLine("  Welcome, {0}", name);
            */
            for(var i=0; i<10; i++)
                Console.WriteLine(i);
        }
    }
}