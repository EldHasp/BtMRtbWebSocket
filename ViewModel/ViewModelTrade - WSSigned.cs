using BitMexLibrary.WebSocketJSON;
using CommLibrary;
using BitMexLibrary;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace ViewModel
{
    public partial class ViewModelTrade
    {

        WebSocketBitMexSigned WebSocketSigned;


        ///// <summary>Рабочий Symbol</summary>
        //public string WorkSymbol => WebSocketSigned.WorkSymbol;

        ///// <summary>Количество полученных сообщений от сервера</summary>
        //public long CountMessage => WebSocketSigned.CountMessage;

        ///// <summary>Время получения (локальное) последнего сообщения от сервера</summary>
        //public DateTime TimeLastMessage => WebSocketSigned.TimeLastMessage;

        ///// <summary>Wallet баланс</summary>
        //public DataWallet Wallet => WebSocketSigned.Wallet;
        ///// <summary>Margin баланс</summary>
        //public DataMargin Margin => WebSocketSigned.Margin;

        ///// <summary>Список текущих позиций</summary>
        //public IEnumerable<Position> Positions => WebSocketSigned.Positions;



        public IReadOnlyList<string> WSProperties
            => "IsOpen WorkSymbol CountMessage TimeLastMessage Wallet Margin Positions MinSell MaxBuy Orders"
            .Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
        private async void WebSocketSigned_PropertyChangedAsync(object sender, PropertyChangedEventArgs e)
        {
            string propertyName = e.PropertyName?.Trim();
            await Task.Run(() => WebSocketSigned_PropertyChanged(propertyName));
        }

        private void WebSocketSigned_PropertyChanged(string propertyName)
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
                    case "IsOpen": IsOpen = WebSocketSigned?.IsOpen; break;
                    //case "IsClose": IsClose = WebSocketSigned?.IsClose; break;
                    case "WorkSymbol": WorkSymbol = WebSocketSigned?.WorkSymbol; break;
                    case "CountMessage": CountMessage = WebSocketSigned?.CountMessage; break;
                    case "TimeLastMessage": TimeLastMessage = WebSocketSigned?.TimeLastMessage; break;
                    //case "InfoDocsList": InfoDocsList = WebSocketSigned?.InfoDocsList; break;
                    //case "Authorization": Authorization = WebSocketSigned?.Authorization; break;
                    case "Wallet": Wallet = WebSocketSigned?.Wallet; break;
                    case "Margin": Margin = WebSocketSigned?.Margin; break;
                    case "Positions": Positions = WebSocketSigned?.Positions; break;
                    case "ChangedPositions": ChangedPositions(); break;
                    case "MinSell":
                        ChangeMinSell(WebSocketSigned.MinSell);
                        MinSell = WebSocketSigned.MinSell;
                        break;
                    case "MaxBuy":
                        ChangeMaxBuy(WebSocketSigned.MaxBuy);
                        MaxBuy = WebSocketSigned.MaxBuy;
                        break;
                    case "Orders":
                        {
                            if (Orders != default)
                                Orders.CollectionChanged -= Orders_CollectionChanged;
                            Orders = WebSocketSigned.Orders;
                            if (Orders != default)
                            {
                                Orders.CollectionChanged += Orders_CollectionChanged;
                                Orders_CollectionChanged
                                    (
                                        Orders,
                                        new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, Orders)
                                    );
                            }
                        }
                        break;
                }

            }
        }

        /// <summary>Обработчик измений коллекции Ордеров</summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Orders_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    addElements();
                    break;
                case NotifyCollectionChangedAction.Remove:
                case NotifyCollectionChangedAction.Reset:
                    delElements();
                    break;
                case NotifyCollectionChangedAction.Replace:
                    addElements();
                    delElements();
                    break;
            }
            void addElements()
            {
                foreach (TableOrder order in e.NewItems.Cast<TableOrder>())
                {
                    if (ListOrderAmend.Count > 0)
                    {
                        OrderRESTWS orderRESTWS = ListOrderAmend
                            .FirstOrDefault(ord => ord.OrderREST.orderID == order.OrderID);
                        if (orderRESTWS != default)
                            orderRESTWS.OrderWS = order;
                    }
                    order.PropertyChanged += Order_PropertyChanged;
                }

            }
            void delElements()
            {
                foreach (TableOrder order in e.OldItems.Cast<TableOrder>())
                {
                    if (ListOrderAmend.Count > 0)
                    {
                        OrderRESTWS orderRESTWS = ListOrderAmend
                             .FirstOrDefault(ord => ord.OrderREST.orderID == order.OrderID);
                        if (orderRESTWS != default)
                        {
                            orderRESTWS.OrderWS = default;
                            ListOrderAmend.Remove(orderRESTWS);
                            OrderAmendCommand.Invalidate();
                        }
                    }
                    order.PropertyChanged -= Order_PropertyChanged;
                }
            }
        }

        private void Order_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            string nameProperty = e.PropertyName;
            TableOrder order = (TableOrder)sender;
            if ((string.IsNullOrEmpty(nameProperty) || nameProperty == "OrdStatus")
                && (order.OrdStatus == "Filled" || order.OrdStatus == "Canceled"))
                MainDispatcher.dispatcher.Invoke(() => { Orders.Remove(order); });
        }
    }
}
