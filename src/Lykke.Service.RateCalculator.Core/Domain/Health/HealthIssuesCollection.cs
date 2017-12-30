using System.Collections.Generic;
using System.Collections;

namespace Lykke.Service.RateCalculator.Core.Domain.Health
{
    public class HealthIssuesCollection : IReadOnlyCollection<HealthIssue>
    {
        public int Count => _list.Count;

        private readonly List<HealthIssue> _list;

        public HealthIssuesCollection()
        {
            _list = new List<HealthIssue>();
        }

        public void Add(string type, string value)
        {
            _list.Add(HealthIssue.Create(type, value));
        }

        public IEnumerator<HealthIssue> GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}