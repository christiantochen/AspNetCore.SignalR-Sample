using Xamarin.Forms;

namespace StockTickr
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new StockTickerView();
        }
    }
}
