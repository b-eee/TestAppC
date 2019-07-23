using System;
using System.Net;
using System.Threading;

namespace TestAppC
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            var t = new Thread(delegate ()
            {
                var myServer = new Server("0.0.0.0", 9999);
            });

            t.Start();

            Console.WriteLine($"server started...");
        }
    }
}
