using System.Threading.Tasks;

namespace Lykke.Service.RateCalculator.Core.Services
{
    public interface IShutdownManager
    {
        Task StopAsync();
    }
}