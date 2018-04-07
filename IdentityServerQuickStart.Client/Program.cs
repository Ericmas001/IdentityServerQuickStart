using System;
using System.Net.Http;
using System.Threading.Tasks;
using IdentityModel.Client;
using Newtonsoft.Json.Linq;

namespace IdentityServerQuickStart.Client
{
    class Program
    {
        public static void Main(string[] args)
        {
            MainAsync().GetAwaiter().GetResult();
            Console.WriteLine("It's the end !! Press ENTER to EXIT");
            Console.ReadLine();
        }

        private static async Task MainAsync()
        {
            var disco = await DiscoveryClient.GetAsync("http://localhost:5000");
            if (disco.IsError)
            {
                Console.WriteLine(disco.Error);
                return;
            }
            Console.WriteLine("Discovery OK");
            Console.WriteLine("=========================================");

            if (!await TestClientToken(disco)) return;

            Console.WriteLine("=========================================");

            if (!await TestUserPasswordToken(disco)) return;

            Console.WriteLine("=========================================");
        }

        private static async Task<bool> TestClientToken(DiscoveryResponse disco)
        {
            // request token
            var tokenClient = new TokenClient(disco.TokenEndpoint, "client", "secret");
            var tokenResponse = await tokenClient.RequestClientCredentialsAsync("api1");

            return await TryCall(tokenResponse);
        }

        private static async Task<bool> TestUserPasswordToken(DiscoveryResponse disco)
        {
            // request token
            var tokenClient = new TokenClient(disco.TokenEndpoint, "ro.client", "secret");
            var tokenResponse = await tokenClient.RequestResourceOwnerPasswordAsync("alice", "password", "api1");

            return await TryCall(tokenResponse);
        }

        private static async Task<bool> TryCall(TokenResponse tokenResponse)
        {
            if (tokenResponse.IsError)
            {
                Console.WriteLine(tokenResponse.Error);
                return false;
            }

            Console.WriteLine(tokenResponse.Json);
            Console.WriteLine("------------------------------------------------");

            // call api
            var client = new HttpClient();
            client.SetBearerToken(tokenResponse.AccessToken);

            var response = await client.GetAsync("http://localhost:5001/identity");
            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine(response.StatusCode);
            }
            else
            {
                var content = await response.Content.ReadAsStringAsync();
                Console.WriteLine(JArray.Parse(content));
            }

            return true;
        }
    }
}
