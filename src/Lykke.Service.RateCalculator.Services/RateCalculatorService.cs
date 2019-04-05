using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common;
using Common.Log;
using Lykke.Service.Assets.Client.Models;
using Lykke.Service.Assets.Client.Models.Extensions;
using Lykke.Service.Assets.Client.ReadModels;
using Lykke.Service.MarketProfile.Client;
using Lykke.Service.RateCalculator.Core.Domain;
using Lykke.Service.RateCalculator.Core.Services;
using MoreLinq;

namespace Lykke.Service.RateCalculator.Services
{
    public class RateCalculatorService : IRateCalculatorService
    {
        private readonly ILog _log;
        private readonly IAssetPairsReadModelRepository _assetPairsReadModelRepository;
        private readonly IAssetsReadModelRepository _assetsReadModelRepository;
        private readonly IOrderBooksService _orderBooksService;
        private readonly ILykkeMarketProfile _marketProfileServiceClient;
        private readonly CrossPairsCalculator _crossPairsCalculator;

        public RateCalculatorService(
            ILog log,
            IAssetPairsReadModelRepository assetPairsReadModelRepository,
            IAssetsReadModelRepository assetsReadModelRepository,
            IOrderBooksService orderBooksService,
            ILykkeMarketProfile marketProfileServiceClient)
        {
            _log = log;
            _assetPairsReadModelRepository = assetPairsReadModelRepository;
            _assetsReadModelRepository = assetsReadModelRepository;
            _orderBooksService = orderBooksService;
            _marketProfileServiceClient = marketProfileServiceClient;

            _assetPairsReadModelRepository.GetAll(); //warming up asset pairs cache

            _crossPairsCalculator = new CrossPairsCalculator(_assetPairsReadModelRepository);
        }

        public double GetRate(string neededAssetId, AssetPair assetPair, double price)
        {
            var inverted = assetPair.IsInverted(neededAssetId);
            int accuracy = inverted ? assetPair.Accuracy : assetPair.InvertedAccuracy;
            var rate = inverted ? price : 1 / price;

            return rate.TruncateDecimalPlaces(accuracy);
        }

        public async Task<IEnumerable<BalanceRecordWithBase>> FillBaseAssetData(
            IEnumerable<IBalanceRecord> balanceRecords, string baseAssetId)
        {
            var result = new List<BalanceRecordWithBase>();

            var marketProfile = await GetMarketProfileRemoteAsync();
            var pricesData = _crossPairsCalculator.PrepareForConversion(marketProfile);

            foreach (var chunk in balanceRecords.Batch(10))
            {
                result.AddRange(
                    chunk.Select(x => new BalanceRecordWithBase
                    {
                        AssetId = x.AssetId,
                        Balance = x.Balance,
                        BaseAssetId = baseAssetId,
                        AmountInBase = GetCrossPairsAmountInBase(
                            x.AssetId,
                            x.Balance,
                            baseAssetId,
                            pricesData),
                    }));
            }

            return result;
        }

        public async Task<IEnumerable<AssetConversionRate>> GetConversionRateForAssets(
            IEnumerable<AssetRequest> balanceRecords, string baseAssetId)
        {
            var result = new List<AssetConversionRate>();

            var marketProfile = await GetMarketProfileRemoteAsync();
            var pricesData = _crossPairsCalculator.PrepareForConversion(marketProfile);

            foreach (var chunk in balanceRecords.Batch(10))
            {
                result.AddRange(
                    chunk.Select(x => new AssetConversionRate
                    {
                        AssetId = x.AssetId,
                        BaseAssetId = baseAssetId,
                        ConversionRate = GetConversionRate(
                            x.AssetId,
                            baseAssetId,
                            pricesData),
                    }));
            }

            return result;
        }

        public async Task<BalanceRecordWithBase> FillBaseAssetData(IBalanceRecord balanceRecord, string baseAssetId)
        {
            return (await FillBaseAssetData(
                new[] {new BalanceRecord {AssetId = balanceRecord.AssetId, Balance = balanceRecord.Balance}},
                baseAssetId)).First();
        }

        public async Task<IEnumerable<BalanceRecord>> GetAmountInBase(IEnumerable<IBalanceRecord> balanceRecords,
            string toAssetId)
        {
            var result = new List<BalanceRecord>();

            var marketProfile = await GetMarketProfileRemoteAsync();
            var pricesData = _crossPairsCalculator.PrepareForConversion(marketProfile);

            foreach (var record in balanceRecords)
            {
                result.Add(new BalanceRecord
                {
                    AssetId = toAssetId,
                    Balance = GetCrossPairsAmountInBase(
                        record.AssetId,
                        record.Balance,
                        toAssetId,
                        pricesData),
                });
            }

            return result;
        }

        public Task<double> GetAmountInBase(string assetFrom, double amount, string assetTo)
        {
            return GetAmountInBaseWithProfile(
                assetFrom,
                amount,
                assetTo,
                null);
        }

        public async Task<double> GetAmountInBaseWithProfile(
            string assetFrom,
            double amount,
            string assetTo,
            Core.Domain.MarketProfile marketProfile)
        {
            if (assetFrom == assetTo)
                return amount;

            if (Math.Abs(amount) < double.Epsilon)
                return 0;

            var marketProfileData = marketProfile ?? await GetMarketProfileRemoteAsync();
            var pricesData = _crossPairsCalculator.PrepareForConversion(marketProfileData);
            return GetCrossPairsAmountInBase(
                assetFrom,
                amount,
                assetTo,
                pricesData);
        }

        public async Task<IEnumerable<ConversionResult>> GetMarketAmountInBase(
            IEnumerable<AssetWithAmount> assetsFrom,
            string assetIdTo,
            OrderAction orderAction)
        {
            var assetPairsDict = _assetPairsReadModelRepository.GetAll();

            var assetsDict = _assetsReadModelRepository.GetAll().ToDictionary(x => x.Id);
            var marketProfile = await GetMarketProfileRemoteAsync();

            var assetWithAmounts = assetsFrom as AssetWithAmount[] ?? assetsFrom.ToArray();
            var assetPairsToProcess = assetWithAmounts
                .Select(assetFrom => GetPairWithAssets(assetPairsDict, assetFrom.AssetId, assetIdTo))
                .Where(p => p != null);

            var orderBooks = (await _orderBooksService.GetAllAsync(assetPairsToProcess)).ToArray();

            if (!orderBooks.Any())
                return Array.Empty<ConversionResult>();

            return assetWithAmounts.Select(item => GetMarketAmountInBase(
                orderAction,
                orderBooks,
                item,
                assetIdTo,
                assetsDict,
                assetPairsDict,
                marketProfile));
        }

        public async Task<Core.Domain.MarketProfile> GetMarketProfile()
        {
            return await GetMarketProfileRemoteAsync();
        }

        public async Task<double> GetBestPrice(string assetPairId, bool buy)
        {
            var assetPair = _assetPairsReadModelRepository.TryGet(assetPairId);
            if (assetPair == null)
                return 0;

            var orderBooks = (await _orderBooksService.GetAsync(assetPair)).ToArray();

            var price = GetBestPrice(orderBooks, assetPairId, buy);

            if (price > 0)
                return price;

            return GetBestPrice(orderBooks, assetPairId, !buy);
        }

        private double GetConversionRate(
            string assetFrom,
            string assetTo,
            Dictionary<string, NodeInfo> pricesData)
        {
            var convertedAmount = _crossPairsCalculator.GetConversionRate(
                assetFrom,
                assetTo,
                pricesData);

            return convertedAmount;
        }

        private double GetCrossPairsAmountInBase(
            string assetFrom,
            double amount,
            string assetTo,
            Dictionary<string, NodeInfo> pricesData)
        {
            if (assetFrom == assetTo)
                return amount;

            if (Math.Abs(amount) < double.Epsilon)
                return 0;

            var convertedAmount = _crossPairsCalculator.Convert(
                assetFrom,
                assetTo,
                amount,
                pricesData);

            var toAsset = _assetsReadModelRepository.TryGet(assetTo);
            if (toAsset == null)
            {
                _log.WriteWarning(nameof(GetCrossPairsAmountInBase), assetFrom, $"Couldn't find asset by id = {assetTo}");
                return convertedAmount;
            }
            return convertedAmount.TruncateDecimalPlaces(toAsset.Accuracy);
        }

        private Assets.Client.Models.v3.AssetPair GetPairWithAssets(
            IEnumerable<Assets.Client.Models.v3.AssetPair> pairs,
            string assetFrom,
            string assetTo)
        {
            return pairs.FirstOrDefault(p =>
                p.BaseAssetId == assetFrom && p.QuotingAssetId == assetTo
                || p.BaseAssetId == assetTo && p.QuotingAssetId == assetFrom);
        }

        private double GetBestPrice(IOrderBook[] orderBooks, string assetPairId, bool buy)
        {
            var orderBook = orderBooks.FirstOrDefault(x => x.AssetPair == assetPairId && x.IsBuy == buy);

            if (orderBook == null)
                return 0;

            orderBook.Order();

            if (orderBook.Prices.Count > 0)
                return orderBook.Prices[0].Price;

            return 0;
        }

        private ConversionResult GetMarketAmountInBase(
            OrderAction orderAction,
            IEnumerable<IOrderBook> orderBooks,
            AssetWithAmount from,
            string assetTo,
            IReadOnlyDictionary<string, Assets.Client.Models.v3.Asset> assetsDict,
            IEnumerable<Assets.Client.Models.v3.AssetPair> assetPairs,
            Core.Domain.MarketProfile marketProfile)
        {
            var result = new ConversionResult();
            var assetPair = GetPairWithAssets(assetPairs, from.AssetId, assetTo);

            if (!IsInputValid(from, assetTo, assetsDict) || assetPair == null)
            {
                result.Result = OperationResult.InvalidInputParameters;
                return result;
            }

            if (from.AssetId == assetTo)
            {
                result.From = result.To = from;
                result.Price = result.VolumePrice = 1;
                result.Result = OperationResult.Ok;
                return result;
            }

            orderAction = IsInverted(assetPair, assetTo) ? orderAction.ViceVersa() : orderAction;
            var isBuy = orderAction == OrderAction.Buy;
            var orderBook = orderBooks.FirstOrDefault(x => x.AssetPair == assetPair.Id && x.IsBuy == isBuy);

            if (orderBook == null)
            {
                result.Result = OperationResult.NoLiquidity;
                return result;
            }

            if (IsInverted(assetPair, assetTo))
            {
                orderBook.Invert();
            }

            orderBook.Order();

            double sum = 0;
            double priceSum = 0;
            int n = 0;

            var neededSum = double.MaxValue;
            foreach (var line in orderBook.Prices)
            {
                if (n != 0 && sum >= neededSum)
                    break;

                sum += Math.Abs(line.Volume);
                priceSum += line.Price;
                n++;
                neededSum = from.Amount * GetRate(assetTo, assetPair, priceSum / n);
            }

            if (n == 0)
            {
                result.Result = OperationResult.NoLiquidity;
                return result;
            }

            var price = priceSum / n;

            result.From = from;
            var rate = GetRate(assetTo, assetPair, price);
            var displayRate = GetRate(from.AssetId, assetPair, price);
            result.To = new AssetWithAmount
            {
                AssetId = assetTo,
                Amount = (rate * from.Amount).TruncateDecimalPlaces(assetsDict[assetTo].Accuracy,
                    orderAction == OrderAction.Buy)
            };
            result.Result = sum < neededSum ? OperationResult.NoLiquidity : OperationResult.Ok;
            result.Price = GetRate(
                from.AssetId,
                assetPair,
                marketProfile.GetPrice(assetPair.Id, orderAction).GetValueOrDefault());
            result.VolumePrice = displayRate;

            return result;
        }

        private bool IsInputValid(
            AssetWithAmount assetFrom,
            string assetTo,
            IReadOnlyDictionary<string, Assets.Client.Models.v3.Asset> assets)
        {
            if (assetFrom.Amount <= 0 || !assets.ContainsKey(assetTo) || !assets.ContainsKey(assetFrom.AssetId))
                return false;

            return true;
        }

        private async Task<Core.Domain.MarketProfile> GetMarketProfileRemoteAsync()
        {
            return (await _marketProfileServiceClient.ApiMarketProfileGetAsync()).ToApiModel();
        }

        private bool IsInverted(Assets.Client.Models.v3.AssetPair assetPair, string targetAsset)
        {
            return assetPair.QuotingAssetId == targetAsset;
        }

        private double GetRate(
            string neededAssetId,
            Assets.Client.Models.v3.AssetPair assetPair,
            double price)
        {
            var inverted = IsInverted(assetPair, neededAssetId);
            int accuracy = inverted ? assetPair.Accuracy : assetPair.InvertedAccuracy;
            var rate = inverted ? price : 1 / price;

            return rate.TruncateDecimalPlaces(accuracy);
        }
    }
}