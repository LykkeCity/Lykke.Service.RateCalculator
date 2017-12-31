using Lykke.Service.RateCalculator.Core.Domain.Health;
using System.Collections.Generic;

namespace Lykke.Service.RateCalculator.Core.Services
{
    public interface IHealthService
    {
        string GetHealthViolationMessage();
        IEnumerable<HealthIssue> GetHealthIssues();
    }
}