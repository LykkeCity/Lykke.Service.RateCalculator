using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using Lykke.Service.Assets.Client.ReadModels;

namespace Lykke.Service.RateCalculator.Services
{
    internal class NodeInfo
    {
        internal bool Straight { get; set; }
        internal double Bid { get; set; }
        internal double Ask { get; set; }
    }

    // Algorithm is described here - https://lykkex.atlassian.net/wiki/spaces/LKEWALLET/pages/521404481/Cross-pair+conversion
    internal class CrossPairsCalculator
    {
        private readonly ConcurrentDictionary<string, ConcurrentDictionary<string, byte>> _links = new ConcurrentDictionary<string, ConcurrentDictionary<string, byte>>();
        private readonly ConcurrentDictionary<string, Dictionary<string, string>> _bestMoves = new ConcurrentDictionary<string, Dictionary<string, string>>();
        private readonly IAssetPairsReadModelRepository _assetPairsReadModelRepository;

        internal CrossPairsCalculator(IAssetPairsReadModelRepository assetPairsReadModelRepository)
        {
            _assetPairsReadModelRepository = assetPairsReadModelRepository;
        }

        internal Dictionary<string, NodeInfo> PrepareForConversion(Core.Domain.MarketProfile marketProfile)
        {
            var result = new Dictionary<string, NodeInfo>();
            var assetIdsToBuild = new HashSet<string>();

            foreach (var feedData in marketProfile.Profile)
            {
                var pair = _assetPairsReadModelRepository.TryGet(feedData.Asset);
                if (pair == null)
                    continue;

                result[$"{pair.BaseAssetId}_{pair.QuotingAssetId}"] = new NodeInfo
                {
                    Straight = true,
                    Bid = feedData.Bid,
                    Ask = feedData.Ask,
                };
                result[$"{pair.QuotingAssetId}_{pair.BaseAssetId}"] = new NodeInfo
                {
                    Straight = false,
                    Bid = feedData.Bid,
                    Ask = feedData.Ask,
                };

                if (!_links.ContainsKey(pair.BaseAssetId))
                    _links.TryAdd(pair.BaseAssetId, new ConcurrentDictionary<string, byte>());
                if (!_links.ContainsKey(pair.QuotingAssetId))
                    _links.TryAdd(pair.QuotingAssetId, new ConcurrentDictionary<string, byte>());
                bool added = _links[pair.BaseAssetId].TryAdd(pair.QuotingAssetId, 0);
                if (added)
                    assetIdsToBuild.Add(pair.BaseAssetId);
                added = _links[pair.QuotingAssetId].TryAdd(pair.BaseAssetId, 0);
                if (added)
                    assetIdsToBuild.Add(pair.QuotingAssetId);
            }

            foreach (var assetId in assetIdsToBuild)
            {
                BuildBestAssetIdLinks(assetId);
            }

            return result;
        }

        internal double Convert(
            string assetFrom,
            string assetTo,
            double amount,
            Dictionary<string, NodeInfo> pricesData)
        {
            var conversionRate = GetConversionRate(assetFrom, assetTo, pricesData);

            return amount * conversionRate;
        }

        public double GetConversionRate(string assetFrom, string assetTo, Dictionary<string, NodeInfo> pricesData)
        {
            var path = GetPath(assetFrom, assetTo);
            if (path == null)
                return 0;

            var conversionRate = GetPathConversionRate(path, pricesData);
            if (Math.Abs(conversionRate) < double.Epsilon)
                return 0;

            return conversionRate;
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

        private void BuildBestAssetIdLinks(string startAssetId)
        {
            var weightsDict = _links.Keys.ToDictionary(i => i, i => double.MaxValue);
            var usedAssets = new HashSet<string>();

            weightsDict[startAssetId] = 0;

            var bestLinks = new Dictionary<string, string>();
            for (int i = 0; i < weightsDict.Count; ++i)
            {
                string curId = null;
                foreach (var otherAssetId in weightsDict.Keys)
                {
                    if (!usedAssets.Contains(otherAssetId) && (curId == null || weightsDict[otherAssetId] < weightsDict[curId]))
                        curId = otherAssetId;
                }
                if (weightsDict[curId] == double.MaxValue)
                    break;

                usedAssets.Add(curId);
                foreach (var otherAssetId in _links[curId].Keys)
                {
                    var to = otherAssetId;
                    var weight = InitWeight(curId, otherAssetId);
                    if (weightsDict[curId] + weight < weightsDict[to])
                    {
                        weightsDict[to] = weightsDict[curId] + weight;
                        bestLinks[to] = curId;
                    }
                }
            }

            _bestMoves.AddOrUpdate(startAssetId, bestLinks, (k, o) => bestLinks);
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
                result.Insert(0, next);
                if (!path.ContainsKey(next))
                    break;
                next = path[next];
            }
            result.Insert(0, assetFrom);
            return result;
        }

        private double GetPathConversionRate(List<string> path, Dictionary<string, NodeInfo> pricesData)
        {
            double result = 1;

            for (int i = 0; i < path.Count - 1; ++i)
            {
                if (!_links.ContainsKey(path[i]) || !_links[path[i]].ContainsKey(path[i + 1]))
                    return 0;

                string pairPriceKey = $"{path[i]}_{path[i + 1]}";
                if (!pricesData.TryGetValue(pairPriceKey, out var nodeInfo))
                    return 0;

                if (nodeInfo.Straight && Math.Abs(nodeInfo.Bid) > double.Epsilon)
                    result *= nodeInfo.Bid;
                else if (!nodeInfo.Straight && Math.Abs(nodeInfo.Ask) > double.Epsilon)
                    result *= 1 / nodeInfo.Ask;
                else
                    return 0;
            }

            return result;
        }
    }
}
