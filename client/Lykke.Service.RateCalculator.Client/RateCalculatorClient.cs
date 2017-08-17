using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common;
using Common.Log;
using Lykke.Service.RateCalculator.Client.AutorestClient;
using Lykke.Service.RateCalculator.Client.AutorestClient.Models;

namespace Lykke.Service.RateCalculator.Client
{
    public class RateCalculatorClient : IRateCalculatorClient, IDisposable
    {
        private readonly ILog _log;
        private RateCalculatorAPI _service;

        public RateCalculatorClient(string serviceUrl, ILog log)
        {
            _service = new RateCalculatorAPI(new Uri(serviceUrl));
            _log = log;
        }

        public void Dispose()
        {
            if (_service == null)
                return;
            _service.Dispose();
            _service = null;
        }

        public async Task<double> GetRateAsync(string neededAssetId, AssetPair assetPair, double price)
        {
            try
            {
                return await _service.ApiRateCalculatorGetRateByAssetIdByPricePostAsync(neededAssetId, price, assetPair) ?? 0;
            }
            catch (Exception ex)
            {
                await _log.WriteErrorAsync(nameof(RateCalculatorClient), nameof(GetRateAsync), $"assetId = {neededAssetId}, price = {price}, asset pair = {assetPair.Id}", ex);
            }

            return 0;
        }

        public async Task<IEnumerable<BalanceRecordWithBase>> FillBaseAssetDataAsync(IEnumerable<BalanceRecord> balanceRecords, string baseAssetId)
        {
            try
            {
                return await _service.ApiRateCalculatorFillBaseAssetDataListByBaseAssetIdPostAsync(baseAssetId, balanceRecords.ToList());
            }
            catch (Exception ex)
            {
                await _log.WriteErrorAsync(nameof(RateCalculatorClient), nameof(FillBaseAssetDataAsync), $"baseAssetId = {baseAssetId}, balanceRecords = {balanceRecords.ToJson()}", ex);
            }

            return Array.Empty<BalanceRecordWithBase>();
        }

        public async Task<BalanceRecordWithBase> FillBaseAssetDataAsync(BalanceRecord balanceRecord, string baseAssetId)
        {
            try
            {
                return await _service.ApiRateCalculatorFillBaseAssetDataByBaseAssetIdPostAsync(baseAssetId, balanceRecord);
            }
            catch (Exception ex)
            {
                await _log.WriteErrorAsync(nameof(RateCalculatorClient), nameof(FillBaseAssetDataAsync), $"baseAssetId = {baseAssetId}, balanceRecord = {balanceRecord.ToJson()}", ex);
            }

            return null;
        }

        public async Task<IEnumerable<BalanceRecord>> GetAmountInBaseAsync(IEnumerable<BalanceRecord> balanceRecords, string toAssetId)
        {
            try
            {
                return await _service.ApiRateCalculatorGetAmountInBaseListByToAssetIdPostAsync(toAssetId, balanceRecords.ToList());
            }
            catch (Exception ex)
            {
                await _log.WriteErrorAsync(nameof(RateCalculatorClient), nameof(GetAmountInBaseAsync), $"toAssetId = {toAssetId}, balanceRecords = {balanceRecords.ToJson()}", ex);
            }

            return Array.Empty<BalanceRecord>();
        }

        public async Task<double> GetAmountInBaseAsync(string assetFrom, double amount, string assetTo)
        {
            try
            {
                return await _service.ApiRateCalculatorGetAmountInBaseByAssetFromByAssetToByAmountPostAsync(assetFrom, assetTo, amount) ?? 0;
            }
            catch (Exception ex)
            {
                await _log.WriteErrorAsync(nameof(RateCalculatorClient), nameof(GetAmountInBaseAsync), $"assetFrom = {assetFrom}, assetTo = {assetTo}, amount = {amount}", ex);
            }

            return 0;
        }

        public async Task<double> GetAmountInBaseWithProfileAsync(string assetFrom, double amount, string assetTo, MarketProfile marketProfile)
        {
            try
            {
                return await _service.ApiRateCalculatorGetAmountInBaseWithProfileByAssetFromByAssetToByAmountPostAsync(assetFrom, assetTo, amount, marketProfile) ?? 0;
            }
            catch (Exception ex)
            {
                await _log.WriteErrorAsync(nameof(RateCalculatorClient), nameof(GetAmountInBaseAsync), $"assetFrom = {assetFrom}, assetTo = {assetTo}, amount = {amount}", ex);
            }

            return 0;
        }

        public async Task<IEnumerable<ConversionResult>> GetMarketAmountInBaseAsync(IEnumerable<AssetWithAmount> assetsFrom, string assetIdTo, OrderAction orderAction)
        {
            try
            {
                return await _service.ApiRateCalculatorGetMarketAmountInBaseByAssetIdToByOrderActionPostAsync(assetIdTo, orderAction, assetsFrom.ToList());
            }
            catch (Exception ex)
            {
                await _log.WriteErrorAsync(nameof(RateCalculatorClient), nameof(GetMarketAmountInBaseAsync), $"assetIdTo = {assetIdTo}, orderAction = {orderAction}, assetsFrom = {assetsFrom.ToJson()}", ex);
            }

            return null;
        }

        public async Task<MarketProfile> GetMarketProfileAsync()
        {
            try
            {
                return await _service.ApiRateCalculatorGetMarketProfileGetAsync();
            }
            catch (Exception ex)
            {
                await _log.WriteErrorAsync(nameof(RateCalculatorClient), nameof(GetMarketProfileAsync), string.Empty, ex);
            }

            return null;
        }
    }
}
