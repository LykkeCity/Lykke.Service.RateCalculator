using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Service.RateCalculator.Core.Domain;

namespace Lykke.Service.RateCalculator.Core.Services
{
    public interface IRateCalculatorService
    {
        double GetRate(string neededAssetId, IAssetPair assetPair, double price);
        Task<IEnumerable<BalanceRecordWithBase>> FillBaseAssetData(IEnumerable<IBalanceRecord> balanceRecords, string baseAssetId);
        Task<BalanceRecordWithBase> FillBaseAssetData(IBalanceRecord balanceRecord, string baseAssetId);
        Task<IEnumerable<BalanceRecord>> GetAmountInBase(IEnumerable<IBalanceRecord> balanceRecords, string toAssetId);
        Task<double> GetAmountInBaseWithProfile(string assetFrom, double amount, string assetTo, MarketProfile marketProfile);
        Task<double> GetAmountInBase(string assetFrom, double amount, string assetTo);
        Task<IEnumerable<ConversionResult>> GetMarketAmountInBase(IEnumerable<AssetWithAmount> assetsFrom, string assetIdTo, OrderAction orderAction);
    }
}
