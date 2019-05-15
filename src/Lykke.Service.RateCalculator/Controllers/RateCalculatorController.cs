using Lykke.Service.Assets.Client.Models;
using Lykke.Service.RateCalculator.Core.Domain;
using Lykke.Service.RateCalculator.Core.Services;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lykke.Service.RateCalculator.Controllers
{
    [Route("api/[controller]")]
    public class RateCalculatorController : Controller
    {
        private readonly IRateCalculatorService _rateCalculatorService;

        public RateCalculatorController(IRateCalculatorService rateCalculatorService)
        {
            _rateCalculatorService = rateCalculatorService;
        }

        [HttpPost]
        [Route("GetRate/{assetId}/{price}")]
        public Task<double> GetRate(string assetId, double price, [FromBody]AssetPair assetPair)
        {
            var rate = _rateCalculatorService.GetRate(
                assetId,
                new AssetPairRateModel
                {
                    QuotingAssetId = assetPair.QuotingAssetId,
                    Accuracy = assetPair.Accuracy,
                    InvertedAccuracy = assetPair.InvertedAccuracy,
                },
                price);
            return Task.FromResult(rate);
        }

        [HttpPost]
        [Route("GetAmountInBase/{assetFrom}/{assetTo}/{amount}")]
        public async Task<double> GetAmountInBase(string assetFrom, string assetTo, double amount)
        {
            return await _rateCalculatorService.GetAmountInBase(assetFrom, amount, assetTo);
        }

        [HttpPost]
        [Route("GetAmountInBaseWithProfile/{assetFrom}/{assetTo}/{amount}")]
        public async Task<double> GetAmountInBaseWithProfile(string assetFrom, string assetTo, double amount, [FromBody] Core.Domain.MarketProfile marketProfile)
        {
            return await _rateCalculatorService.GetAmountInBaseWithProfile(assetFrom, amount, assetTo, marketProfile);
        }

        [HttpPost]
        [Route("GetAmountInBaseList/{toAssetId}")]
        public async Task<IEnumerable<BalanceRecord>> GetAmountInBaseList(string toAssetId, [FromBody]IEnumerable<BalanceRecord> balanceRecords)
        {
            return await _rateCalculatorService.GetAmountInBase(balanceRecords, toAssetId);
        }

        [HttpPost]
        [Route("FillBaseAssetData/{baseAssetId}")]
        public async Task<BalanceRecordWithBase> FillBaseAssetData(string baseAssetId, [FromBody]BalanceRecord balanceRecord)
        {
            return await _rateCalculatorService.FillBaseAssetData(balanceRecord, baseAssetId);
        }

        [HttpPost]
        [Route("FillBaseAssetDataList/{baseAssetId}")]
        public async Task<IEnumerable<BalanceRecordWithBase>> FillBaseAssetDataList(string baseAssetId, [FromBody]IEnumerable<BalanceRecord> balanceRecords)
        {
            return await _rateCalculatorService.FillBaseAssetData(balanceRecords, baseAssetId);
        }

        [HttpPost]
        [Route("GetMarketAmountInBase/{assetIdTo}/{orderAction}")]
        public async Task<IEnumerable<ConversionResult>> GetMarketAmountInBase(string assetIdTo, OrderAction orderAction, [FromBody] IEnumerable<AssetWithAmount> assetsFrom)
        {
            return await _rateCalculatorService.GetMarketAmountInBase(assetsFrom, assetIdTo, orderAction);
        }

        [HttpGet]
        [Route("GetMarketProfile")]
        public async Task<Core.Domain.MarketProfile> GetMarketProfile()
        {
            return await _rateCalculatorService.GetMarketProfile();
        }

        [HttpGet]
        [Route("GetBestPrice/{assetPair}/{isBuy}")]
        public async Task<double> GetBestPrice(string assetPair, bool isBuy)
        {
            return await _rateCalculatorService.GetBestPrice(assetPair, isBuy);
        }

        [HttpPost]
        [Route("GetConversionRatesForAssets/{baseAssetId}")]
        public async Task<IEnumerable<AssetConversionRate>> GetConversionRatesForAssets(
            [FromRoute]string baseAssetId, 
            [FromBody]IEnumerable<AssetRequest> assetIds)
        {
            var assetIdsList = assetIds?.ToList();

            return await _rateCalculatorService.GetConversionRateForAssets(assetIdsList, baseAssetId);
        }
    }
}
