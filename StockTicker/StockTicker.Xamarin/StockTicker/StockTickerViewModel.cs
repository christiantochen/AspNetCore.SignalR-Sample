using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.AspNetCore.Sockets;
using Xamarin.Forms;
using System.Windows.Input;

namespace StockTickr
{
    public class StockTickerViewModel : BindableObject
    {
        StockTickerClient _client;
        ObservableCollection<Stock> _stock = new ObservableCollection<Stock>();

        public ObservableCollection<Stock> Stocks
        {
            get => _stock;
            set
            {
                _stock = value;
                OnPropertyChanged();
            }
        }

        public MarketState MarketState { get; set; }

        public StockTickerViewModel()
        {
            _client = new StockTickerClient();
            _client.StocksStreaming += StocksStreaming;
            _client.StockMarketStateOnChanged += StockMarketStateOnChanged; ;
        }

        void StocksStreaming(object sender, StockStreamingEventHandler e)
        {
            if (e.Stocks == null)
                return;

            Stocks = new ObservableCollection<Stock>(e.Stocks);
        }

        void StockMarketStateOnChanged(object sender, StockMarketStateEventHandler e)
        {
            MarketState = e.State;
            OnPropertyChanged(nameof(MarketState));
        }

        public ICommand OpenMarketCommand => new Command(OpenMarket);
        public ICommand CloseMarketCommand => new Command(CloseMarket);

        async void OpenMarket() => await _client.SetMarketStateAsync(MarketState.Open);
        async void CloseMarket() => await _client.SetMarketStateAsync(MarketState.Close);
    }
}