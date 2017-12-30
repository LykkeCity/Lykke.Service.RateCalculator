using System;
using Lykke.Service.RateCalculator.Client;

namespace TestClient
{
    class Program
    {
        static void Main(string[] args)
        {
            var client = new RateCalculatorClient("http://rate-calculator.lykke-service.svc.cluster.local", null);

            var amount = client.GetAmountInBaseAsync("USD", 10d, "LKK").Result;

            client.GetMarketAmountInBaseAsync()

            Console.WriteLine(amount);

            Console.ReadKey();
        }
    }
}
