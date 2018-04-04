using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using Xamarin.Forms;

namespace StockTickr
{
    /// <summary>
    /// SignalR Client
    /// </summary>
    public class StockTickerClient
    {
        const string STOCKS_HUB_URL = "http://192.168.1.8:5000/stocks";
        const string STREAM_STOCKS = "StreamStocks";
        const string MARKET_OPENED = "marketOpened";
        const string MARKET_RESET = "marketReset";
        const string MARKET_CLOSED = "marketClosed";
        const string GET_MARKET_STATE = "GetMarketState";
        const string OPEN_MARKET = "OpenMarket";
        const string CLOSE_MARKET = "CloseMarket";
        const string RESET_MARKET = "Reset";

        HubConnection _hub;
        CancellationTokenSource _cts;

        MarketState _marketState = MarketState.Close;
        public MarketState MarketState
        {
            get => _marketState;
            set
            {
                _marketState = value;
                StockMarketStateOnChanged?.Invoke(this, new StockMarketStateEventHandler(_marketState));
            }
        }

        public event EventHandler<StockMarketStateEventHandler> StockMarketStateOnChanged;
        public event EventHandler<StockStreamingEventHandler> StocksStreaming;

        public StockTickerClient()
        {
            _cts = new CancellationTokenSource();

            _hub = new HubConnectionBuilder()
                .WithUrl(STOCKS_HUB_URL)
#if DEBUG
                .WithConsoleLogger()
#endif
                .Build();

            _hub.Closed += e =>
            {
#if DEBUG
                Debug.WriteLine("Connection closed with error: {0}", e);
#endif
                _cts.Cancel();
            };

            InitializeClient();
        }

        public async Task ResetMarket()
        {
            await _hub.InvokeAsync(RESET_MARKET);
        }

        public async Task SetMarketStateAsync(MarketState state)
        {
            switch (state)
            {
                case (MarketState.Open):
                    await _hub.InvokeAsync(OPEN_MARKET);
                    break;
                default:
                    await _hub.InvokeAsync(CLOSE_MARKET);
                    break;
            }
        }

        public async Task<MarketState> GetMarketStateAsync()
        {
            var state = await _hub.InvokeAsync<string>(GET_MARKET_STATE);

            switch (state)
            {
                case ("Open"):
                    MarketState = MarketState.Open;
                    break;
                default:
                    MarketState = MarketState.Close;
                    break;
            }

            return MarketState;
        }

        /// <summary>
        /// Initializes SignalR.
        /// </summary>
        public async void InitializeClient()
        {
            await _hub.StartAsync();

            _hub.On(MARKET_OPENED, async () =>
            {
                if (MarketState != MarketState.Open)
                {
                    MarketState = MarketState.Open;
                    await StartStreaming();
                }
            });
            _hub.On(MARKET_CLOSED, () => MarketState = MarketState.Close);

            // Do an initial check to see if we can start streaming the stocks
            await GetMarketStateAsync();

            if (MarketState == MarketState.Open)
            {
                await StartStreaming();
            }
        }

        async Task StartStreaming()
        {
            var channel = await _hub.StreamAsync<IEnumerable<Stock>>(STREAM_STOCKS, CancellationToken.None);

            while (await channel.WaitToReadAsync() && MarketState == MarketState.Open)
            {
                while (channel.TryRead(out var stock))
                {
                    StocksStreaming?.Invoke(this, new StockStreamingEventHandler(stock));
                }
            }
        }
    }
}

