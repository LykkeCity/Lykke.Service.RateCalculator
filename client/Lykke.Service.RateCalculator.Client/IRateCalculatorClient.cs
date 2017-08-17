using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Service.RateCalculator.Client.AutorestClient.Models;

namespace Lykke.Service.RateCalculator.Client
{
    public interface IRateCalculatorClient
    {
        Task<double> GetRateAsync(string neededAssetId, AssetPair assetPair, double price);
        Task<IEnumerable<BalanceRecordWithBase>> FillBaseAssetDataAsync(IEnumerable<BalanceRecord> balanceRecords, string baseAssetId);
        Task<BalanceRecordWithBase> FillBaseAssetDataAsync(BalanceRecord balanceRecord, string baseAssetId);
        Task<IEnumerable<BalanceRecord>> GetAmountInBaseAsync(IEnumerable<BalanceRecord> balanceRecords, string toAssetId);
        Task<double> GetAmountInBaseAsync(string assetFrom, double amount, string assetTo);
        Task<double> GetAmountInBaseWithProfileAsync(string assetFrom, double amount, string assetTo, MarketProfile marketProfile);
        Task<IEnumerable<ConversionResult>> GetMarketAmountInBaseAsync(IEnumerable<AssetWithAmount> assetsFrom, string assetIdTo, OrderAction orderAction);
        Task<MarketProfile> GetMarketProfileAsync();
    }
}