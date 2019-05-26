using CommLibrary;
using BitMexLibrary;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModel
{
    public partial class ViewModelWSOrderBook10 
    {
        public IReadOnlyList<string> WSProperties => new List<string>() { "IsOpen", "IsClose", "WorkSymbol", "OrderBook10Asks", "OrderBook10Bids", "CountMessage", "TimeLastMessage", "MinSell", "MaxBuy" };

        private async void WSocket_PropertyChangedAsync(object sender, PropertyChangedEventArgs e)
        {

            string propertyName = e.PropertyName?.Trim();
            await Task.Run(() => WSocket_PropertyChanged(propertyName));
        }

        private void WSocket_PropertyChanged(string propertyName)
        {

            if (string.IsNullOrWhiteSpace(propertyName))
                foreach (string propName in WSProperties)
                    propertyNameChange(propName);
            else
                propertyNameChange(propertyName);

            void propertyNameChange(string name)
            {
                switch (name)
                {
                    case "IsOpen": IsOpen = WSocket?.IsOpen; break;
                    case "IsClose": IsClose = WSocket?.IsClose; break;
                    case "WorkSymbol": WorkSymbol = WSocket?.WorkSymbol; break;
                    case "OrderBook10Asks": OrderBook10Asks = WSocket?.OrderBook10Asks; break;
                    case "OrderBook10Bids": OrderBook10Bids = WSocket?.OrderBook10Bids; break;
                    case "CountMessage": CountMessage = WSocket?.CountMessage; break;
                    case "TimeLastMessage": TimeLastMessage = WSocket?.TimeLastMessage; break;
                    case "MinSell": MinSell = WSocket?.MinSell; break;
                    case "MaxBuy": MaxBuy = WSocket?.MaxBuy; break;
                }

            }
        }


    }
}
