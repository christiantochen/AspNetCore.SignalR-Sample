using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace StockTickr
{
    public class StockTickerHub : Hub
    {
        readonly StockTicker _stockTicker;

        public StockTickerHub(StockTicker stockTicker)
        {
            _stockTicker = stockTicker;
        }

        public IEnumerable<Stock> GetAllStocks() => _stockTicker.GetAllStocks();

        public IObservable<IEnumerable<Stock>> StreamStocks() => _stockTicker.StreamStocks();

        public string GetMarketState() => _stockTicker.MarketState.ToString();

        public async Task OpenMarket() => await _stockTicker.OpenMarket();

        public async Task CloseMarket() => await _stockTicker.CloseMarket();

        public async Task Reset() => await _stockTicker.Reset();
    }
}
