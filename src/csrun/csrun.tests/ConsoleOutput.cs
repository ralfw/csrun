using System;
using System.IO;

namespace csrun.tests
{
    internal class ConsoleOutput
    {
        public static string Capture(Action run) {
            var sw = new StringWriter();
            var currStdOut = Console.Out;
            Console.SetOut(sw);

            run();

            Console.SetOut(currStdOut); // Wichtig! Zuerst zurücksetzen, bevor auf sw zugegriffen wird.

            return sw.ToString();
        }
    }
}