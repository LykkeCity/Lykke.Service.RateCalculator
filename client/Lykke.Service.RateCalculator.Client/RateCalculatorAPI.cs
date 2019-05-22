using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

// ReSharper disable once CheckNamespace
namespace Lykke.Service.RateCalculator.Client.AutorestClient
{
    internal partial class RateCalculatorAPI
    {
        /// <inheritdoc />
        /// <summary>
        /// Should be used to prevent memory leak in RetryPolicy
        /// </summary>
        public RateCalculatorAPI(Uri baseUri, HttpClient client) : base(client)
        {
            Initialize();

            BaseUri = baseUri ?? throw new ArgumentNullException("baseUri");
        }
    }
}