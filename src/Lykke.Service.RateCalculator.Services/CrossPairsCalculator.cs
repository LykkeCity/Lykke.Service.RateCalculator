using System;
using System.Collections.Generic;
using System.Linq;
using Lykke.Service.Assets.Client.ReadModels;

namespace Lykke.Service.RateCalculator.Services
{
    internal class NodeInfo
    {
        internal double Weight { get; set; }
        internal string AssetPairId { get; set; }
        internal bool Staright { get; set; }
        internal double Bid { get; set; }
        internal double Ask { get; set; }
    }

    // Algorithm is described here - https://lykkex.atlassian.net/wiki/spaces/LKEWALLET/pages/521404481/Cross-pair+conversion
    internal class CrossPairsCalculator
    {
        private readonly Dictionary<string, Dictionary<string, NodeInfo>> _graph = new Dictionary<string, Dictionary<string, NodeInfo>>();
        private readonly Dictionary<string, Dictionary<string, string>> _bestMoves = new Dictionary<string, Dictionary<string, string>>();

        internal CrossPairsCalculator(Core.Domain.MarketProfile marketProfile, IAssetPairsReadModelRepository assetPairsReadModelRepository)
        {
            foreach (var feedData in marketProfile.Profile)
            {
                var pair = assetPairsReadModelRepository.TryGet(feedData.Asset);
                if (pair == null)
                    continue;

                if (!_graph.ContainsKey(pair.BaseAssetId))
                    _graph.Add(pair.BaseAssetId, new Dictionary<string, NodeInfo>());
                if (!_graph.ContainsKey(pair.QuotingAssetId))
                    _graph.Add(pair.QuotingAssetId, new Dictionary<string, NodeInfo>());
                var weight = InitWeight(pair.BaseAssetId, pair.QuotingAssetId);
                _graph[pair.BaseAssetId][pair.QuotingAssetId] = new NodeInfo
                {
                    Weight = weight,
                    AssetPairId = pair.Id,
                    Staright = true,
                    Bid = feedData.Bid,
                    Ask = feedData.Ask,
                };
                _graph[pair.QuotingAssetId][pair.BaseAssetId] = new NodeInfo
                {
                    Weight = weight,
                    AssetPairId = pair.Id,
                    Staright = false,
                    Bid = feedData.Bid,
                    Ask = feedData.Ask,
                };
            }

            foreach (var assetId in _graph.Keys)
            {
                _bestMoves[assetId] = BuildGraphData(assetId);
            }
        }

        internal double Convert(
            string assetFrom,
            string assetTo,
            double amount)
        {
            var path = GetPath(assetFrom, assetTo);
            if (path == null)
                return 0;

            var conversionRate = GetPathConversionRate(path);
            if (Math.Abs(conversionRate) < double.Epsilon)
                return 0;

            return amount * conversionRate;
        }

        private double InitWeight(string asset1, string asset2)
        {
            if (asset1 == "BTC" || asset2 == "BTC")
                return 1.1;
            if (asset1 == "ETH" || asset2 == "ETH")
                return 1.11;
            if (asset1 == "USD" || asset2 == "USD")
                return 1.111;
            return 1.1111;
        }

        private Dictionary<string, string> BuildGraphData(string startAssetId)
        {
            var weightsDict = _graph.Keys.ToDictionary(i => i, i => double.MaxValue);
            var usedAssets = new HashSet<string>();

            weightsDict[startAssetId] = 0;

            var result = new Dictionary<string, string>();
            for (int i = 0; i < _graph.Keys.Count; ++i)
            {
                string curId = null;
                foreach (var otherAssetId in _graph.Keys)
                {
                    if (!usedAssets.Contains(otherAssetId) && (curId == null || weightsDict[otherAssetId] < weightsDict[curId]))
                        curId = otherAssetId;
                }
                if (weightsDict[curId] == double.MaxValue)
                    break;
                usedAssets.Add(curId);
                foreach (var otherAssetId in _graph[curId].Keys)
                {
                    var to = otherAssetId;
                    var weight = _graph[curId][otherAssetId].Weight;
                    if (weightsDict[curId] + weight < weightsDict[to])
                    {
                        weightsDict[to] = weightsDict[curId] + weight;
                        result[to] = curId;
                    }
                }
            }
            return result;
        }

        private List<string> GetPath(string assetFrom, string assetTo)
        {
            if (!_bestMoves.ContainsKey(assetFrom))
                return null;

            var path = _bestMoves[assetFrom];
            var result = new List<string>();
            var next = assetTo;
            while (next != assetFrom)
            {
                result.Add(next);
                if (!path.ContainsKey(next))
                    break;
                next = path[next];
            }
            result.Add(assetFrom);
            result.Reverse();
            return result;
        }

        private double GetPathConversionRate(List<string> path)
        {
            double result = 1;

            for (int i = 0; i < path.Count - 1; ++i)
            {
                if (!_graph.ContainsKey(path[i]) || !_graph[path[i]].ContainsKey(path[i + 1]))
                    return 0;

                var nodeInfo = _graph[path[i]][path[i + 1]];

                if (nodeInfo.Staright && Math.Abs(nodeInfo.Bid) > double.Epsilon)
                    result *= nodeInfo.Bid;
                else if (!nodeInfo.Staright && Math.Abs(nodeInfo.Ask) > double.Epsilon)
                    result *= 1 / nodeInfo.Ask;
                else
                    return 0;
            }

            return result;
        }
    }
}
