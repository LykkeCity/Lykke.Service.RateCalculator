using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lykke.Service.RateCalculator.Core.Domain
{
    public enum Blockchain
    {
        None,
        Bitcoin,
        Ethereum
    }

    public interface IAsset
    {
        string Id { get; }
        string BlockChainId { get; }
        string BlockChainAssetId { get; }
        string Name { get; }
        string Symbol { get; }
        string IdIssuer { get; }
        bool IsBase { get; }
        bool HideIfZero { get; }
        int Accuracy { get; }
        [Obsolete("Please use MultiplierPower")]
        double Multiplier { get; }
        int MultiplierPower { get; }
        bool IsDisabled { get; }
        bool HideWithdraw { get; }
        bool HideDeposit { get; }
        int DefaultOrder { get; }
        bool KycNeeded { get; }
        string AssetAddress { get; }
        double DustLimit { get; }
        string CategoryId { get; }
        Blockchain Blockchain { get; }
        string DefinitionUrl { get; }
        string[] PartnerIds { get; }
        bool NotLykkeAsset { get; }
        bool IssueAllowed { get; }
        /// <summary>
        /// Value lower that this property is considered "low volume" and may have some limitations,
        /// e.g. cash out timeout limits
        /// </summary>
        double? LowVolumeAmount { get; set; }
        string DisplayId { get; set; }

        //deposit flags
        bool BankCardsDepositEnabled { get; }
        bool SwiftDepositEnabled { get; }
        bool BlockchainDepositEnabled { get; }
        bool BuyScreen { get; }

        //withdraw flags
        bool SellScreen { get; }
        bool BlockchainWithdrawal { get; }
        bool SwiftWithdrawal { get; }
        bool ForwardWithdrawal { get; }
        bool CrosschainWithdrawal { get; }

        //lock period for forward withdrawal
        int ForwardFrozenDays { get; }
        //base asset for forward withdrawal
        string ForwardBaseAsset { get; }
        string ForwardMemoUrl { get; }

        string IconUrl { get; }
    }

    public class Asset : IAsset
    {
        public string Id { get; set; }
        public string BlockChainId { get; set; }
        public string BlockChainAssetId { get; set; }
        public string Name { get; set; }
        public string Symbol { get; set; }
        public string IdIssuer { get; set; }
        public bool IsBase { get; set; }
        public bool HideIfZero { get; set; }
        public int Accuracy { get; set; }
        public double Multiplier { get; set; }
        public int MultiplierPower { get; set; }
        public bool IsDisabled { get; set; }
        public bool HideWithdraw { get; set; }
        public bool HideDeposit { get; set; }
        public int DefaultOrder { get; set; }
        public bool KycNeeded { get; set; }
        public string AssetAddress { get; set; }
        public double DustLimit { get; set; }
        public string CategoryId { get; set; }
        public Blockchain Blockchain { get; set; }
        public string DefinitionUrl { get; set; }
        public string[] PartnerIds { get; set; }
        public bool NotLykkeAsset { get; set; }
        public bool IssueAllowed { get; set; }
        public double? LowVolumeAmount { get; set; }
        public string DisplayId { get; set; }

        public bool BankCardsDepositEnabled { get; set; }
        public bool SwiftDepositEnabled { get; set; }
        public bool BlockchainDepositEnabled { get; set; }

        public bool BuyScreen { get; set; }
        public bool SellScreen { get; set; }

        public bool BlockchainWithdrawal { get; set; }
        public bool SwiftWithdrawal { get; set; }
        public bool ForwardWithdrawal { get; set; }
        public bool CrosschainWithdrawal { get; set; }

        public int ForwardFrozenDays { get; set; }
        public string ForwardBaseAsset { get; set; }
        public string ForwardMemoUrl { get; set; }

        public string IconUrl { get; set; }

        public static Asset Create(string id, string blockChainId, string categoryId, string name, string symbol, string idIssuer, bool hideIfZero,
            bool bankCardsDeposit, bool swiftEnabled, bool blockChainDepositEnabled, bool isDisabled = false, bool hideWithdraw = false, bool hideDeposit = false,
            int defaultOrder = 999, bool kycNeeded = false)
        {
            return new Asset
            {
                Id = id,
                Name = name,
                Symbol = symbol,
                IdIssuer = idIssuer,
                BlockChainId = blockChainId,
                HideIfZero = hideIfZero,
                Accuracy = 2,
                Multiplier = 1,
                IsDisabled = isDisabled,
                HideDeposit = hideDeposit,
                HideWithdraw = hideWithdraw,
                DefaultOrder = defaultOrder,
                KycNeeded = kycNeeded,
                BankCardsDepositEnabled = bankCardsDeposit,
                SwiftDepositEnabled = swiftEnabled,
                BlockchainDepositEnabled = blockChainDepositEnabled,
                CategoryId = categoryId
            };
        }

        public static Asset CreateDefault()
        {
            return new Asset();
        }
    }

    public interface IAssetsRepository
    {
        Task<IEnumerable<IAsset>> GetAssetsAsync();
        Task AddAssetAsync(IAsset asset);
    }
}
