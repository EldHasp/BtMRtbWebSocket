using BitMexLibrary;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;

namespace ViewModel
{
    public partial class ViewModelAuth
    {


        WebSocketBitMexSigned bitMexWebSocket;

        public IReadOnlyList<string> WSProperties => new List<string>() { "IsOpen", "IsClose", "WorkSymbol", "CountMessage", "TimeLastMessage", "InfoDocsList", "Authorization", "Wallet", "Margin" };

        private async void BitMexWebSocket_PropertyChangedAsync(object sender, PropertyChangedEventArgs e)
        {
            string propertyName = e.PropertyName?.Trim();
            await Task.Run(() => BitMexWebSocket_PropertyChanged(propertyName));
        }

        private void BitMexWebSocket_PropertyChanged(string propertyName)
        {
            if (string.IsNullOrEmpty(propertyName))
                foreach (string propName in WSProperties)
                    propertyNameChange(propName);
            else
                propertyNameChange(propertyName);

            void propertyNameChange(string name)
            {
                switch (name)
                {
                    case "IsOpen": IsOpen = bitMexWebSocket?.IsOpen; break;
                    case "IsClose": IsClose = bitMexWebSocket?.IsClose; break;
                    case "WorkSymbol": WorkSymbol = bitMexWebSocket?.WorkSymbol; break;
                    case "CountMessage": CountMessage = bitMexWebSocket?.CountMessage; break;
                    case "TimeLastMessage": TimeLastMessage = bitMexWebSocket?.TimeLastMessage; break;
                    case "InfoDocsList": InfoDocsList = bitMexWebSocket?.InfoDocsList; break;
                    case "Authorization": Authorization = bitMexWebSocket?.Authorization; break;
                    case "Wallet": Wallet = bitMexWebSocket?.Wallet; break;
                    case "Margin": Margin = bitMexWebSocket?.Margin; break;
                }

            }
        }
    }
}
