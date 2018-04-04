using Xamarin.Forms;

namespace StockTickr
{
    public partial class StockTickerView : ContentPage
    {
        public StockTickerView()
        {
            InitializeComponent();

            BindingContext = new StockTickerViewModel();
        }
    }
}
