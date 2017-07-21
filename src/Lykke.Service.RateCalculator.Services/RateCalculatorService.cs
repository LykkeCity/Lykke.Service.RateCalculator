using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common;
using Lykke.Service.RateCalculator.Core.Domain;
using Lykke.Service.RateCalculator.Core.Services;

namespace Lykke.Service.RateCalculator.Services
{
    public class RateCalculatorService : IRateCalculatorService
    {
        private readonly CachedDataDictionary<string, IAsset> _assetsDict;
        private readonly CachedDataDictionary<string, IAssetPair> _assetPairsDict;
        private readonly IAssetPairBestPriceRepository _bestPriceRepository;
        private readonly IOrderBooksService _orderBooksService;

        public RateCalculatorService(
            CachedDataDictionary<string, IAsset> assetsDict,
            CachedDataDictionary<string, IAssetPair> assetPairsDict,
            IAssetPairBestPriceRepository bestPriceRepository,
            IOrderBooksService orderBooksService)
        {
            _assetsDict = assetsDict;
            _assetPairsDict = assetPairsDict;
            _bestPriceRepository = bestPriceRepository;
            _orderBooksService = orderBooksService;
        }

        public double GetRate(string neededAssetId, IAssetPair assetPair, double price)
        {
            var inverted = assetPair.IsInverted(neededAssetId);
            int accuracy = inverted ? assetPair.Accuracy : assetPair.InvertedAccuracy;
            var rate = inverted ? price : 1 / price;

            return rate.TruncateDecimalPlaces(accuracy);
        }

        public async Task<IEnumerable<BalanceRecordWithBase>> FillBaseAssetData(IEnumerable<IBalanceRecord> balanceRecords, string baseAssetId)
        {
            List<BalanceRecordWithBase> result = new List<BalanceRecordWithBase>();
            var marketProfile = await _bestPriceRepository.GetAsync();

            foreach (var record in balanceRecords)
            {
                result.Add(new BalanceRecordWithBase
                {
                    AssetId = record.AssetId,
                    Balance = record.Balance,
                    BaseAssetId = baseAssetId,
                    AmountInBase = await GetAmountInBase(record.AssetId, record.Balance, baseAssetId, marketProfile)
                });
            }

            return result;
        }

        public async Task<BalanceRecordWithBase> FillBaseAssetData(IBalanceRecord balanceRecord, string baseAssetId)
        {
            return (await FillBaseAssetData(
                new[] { new BalanceRecord { AssetId = balanceRecord.AssetId, Balance = balanceRecord.Balance } },
                baseAssetId)).First();
        }

        public async Task<IEnumerable<BalanceRecord>> GetAmountInBase(IEnumerable<IBalanceRecord> balanceRecords, string toAssetId)
        {
            var result = new List<BalanceRecord>();

            var marketProfile = await _bestPriceRepository.GetAsync();

            foreach (var record in balanceRecords)
            {
                result.Add(new BalanceRecord{AssetId = toAssetId, Balance = await GetAmountInBase(record.AssetId, record.Balance, toAssetId, marketProfile)});
            }

            return result;
        }

        public async Task<double> GetAmountInBase(string assetFrom, double amount, string assetTo, MarketProfile marketProfile = default(MarketProfile))
        {
            var marketProfileData = marketProfile ?? await _bestPriceRepository.GetAsync();

            if (assetFrom == assetTo)
                return amount;

            var assetPair = (await _assetPairsDict.Values()).PairWithAssets(assetFrom, assetTo);

            if (Math.Abs(amount) < double.Epsilon || assetPair == null ||
                marketProfileData.Profile.All(x => x.Asset != assetPair.Id))
                return 0;

            var profile = marketProfileData.Profile.First(x => x.Asset == assetPair.Id);
            var bestPrice = assetPair.IsInverted(assetTo) ? profile.Bid : profile.Ask;

            if (Math.Abs(bestPrice) < double.Epsilon)
                return 0;

            var toAsset = await _assetsDict.GetItemAsync(assetTo);

            var price = assetPair.BaseAssetId == assetFrom ? bestPrice : 1 / bestPrice;

            return (price * amount).TruncateDecimalPlaces(toAsset.Accuracy);
        }

        public async Task<IEnumerable<ConversionResult>> GetMarketAmountInBase(IEnumerable<AssetWithAmount> assetsFrom, string assetIdTo, OrderAction orderAction)
        {
            var orderBooksTask = _orderBooksService.GetAllAsync();
            var assetsDictTask = _assetsDict.GetDictionaryAsync();
            var assetPairsTask = _assetPairsDict.Values();
            var marketProfileTask = _bestPriceRepository.GetAsync();

            var orderBooks = await orderBooksTask;
            var assetsDict = await assetsDictTask;
            var assetPairs = await assetPairsTask;
            var marketProfile = await marketProfileTask;

            return assetsFrom.Select(item => GetMarketAmountInBase(orderAction, orderBooks, item, assetIdTo, assetsDict, assetPairs, marketProfile));
        }

        private ConversionResult GetMarketAmountInBase(OrderAction orderAction, IEnumerable<IOrderBook> orderBooks, AssetWithAmount from,
            string assetTo, IDictionary<string, IAsset> assetsDict, IEnumerable<IAssetPair> assetPairs, MarketProfile marketProfile)
        {
            var result = new ConversionResult();
            var assetPair = assetPairs.PairWithAssets(from.AssetId, assetTo);

            if (!IsInputValid(from, assetTo, assetsDict) || assetPair == null)
            {
                result.SetResult(OperationResult.InvalidInputParameters);
                return result;
            }

            if (from.AssetId == assetTo)
            {
                result.From = result.To = from;
                result.Price = result.VolumePrice = 1;
                result.SetResult(OperationResult.Ok);
                return result;
            }

            orderAction = assetPair.IsInverted(assetTo) ? orderAction.ViceVersa() : orderAction;
            var isBuy = orderAction == OrderAction.Buy;
            var orderBook = orderBooks.FirstOrDefault(x => x.AssetPair == assetPair.Id && x.IsBuy == isBuy);

            if (orderBook == null)
            {
                result.SetResult(OperationResult.NoLiquidity);
                return result;
            }

            if (assetPair.IsInverted(assetTo))
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
                result.SetResult(OperationResult.NoLiquidity);
                return result;
            }

            var price = priceSum / n;

            result.From = from;
            var rate = GetRate(assetTo, assetPair, price);
            var displayRate = GetRate(from.AssetId, assetPair, price);
            result.To = new AssetWithAmount
            {
                AssetId = assetTo,
                Amount = (rate * from.Amount).TruncateDecimalPlaces(assetsDict[assetTo].Accuracy, orderAction == OrderAction.Buy)
            };
            result.SetResult(sum < neededSum ? OperationResult.NoLiquidity : OperationResult.Ok);
            result.Price = GetRate(from.AssetId, assetPair, marketProfile.GetPrice(assetPair.Id, orderAction).GetValueOrDefault());
            result.VolumePrice = displayRate;

            return result;
        }

        private bool IsInputValid(AssetWithAmount assetFrom, string assetTo, IDictionary<string, IAsset> assets)
        {
            if (assetFrom.Amount <= 0 || !assets.ContainsKey(assetTo) || !assets.ContainsKey(assetFrom.AssetId))
                return false;

            return true;
        }
    }
}
