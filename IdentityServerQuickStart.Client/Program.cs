using System;
using System.Threading.Tasks;

namespace IdentityServerQuickStart.Client
{
    class Program
    {
        public static void Main(string[] args) => MainAsync().GetAwaiter().GetResult();

        private static async Task MainAsync()
        {
            Console.WriteLine("Hello World!");
            Console.ReadLine();
        }
    }
}
