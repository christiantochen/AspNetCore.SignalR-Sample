using System;
using System.Collections.Generic;

namespace StockTickr
{
    public class StockMarketStateEventHandler : EventArgs
    {
        public MarketState State { get; private set; }

        public StockMarketStateEventHandler(MarketState state)
        {
            State = state;
        }
    }

    public class StockStreamingEventHandler : EventArgs
    {
        public IEnumerable<Stock> Stocks { get; private set; }

        public StockStreamingEventHandler(IEnumerable<Stock> stocks)
        {
            Stocks = stocks;
        }
    }
}
