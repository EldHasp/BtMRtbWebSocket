using BitMexLibrary;
using CommLibrary;
using System;
using System.Windows.Input;
using System.Windows.Threading;
using BitMexLibrary.WebSocketJSON;
using System.Collections.Generic;
using System.ComponentModel;

namespace ViewModel
{
    public partial class ViewModelCalc
    {
        ///// <summary>Рабочий Symbol</summary>
        //public string WorkSymbol => bitMexWebSocket?.WorkSymbol;

        /////// <summary>Топ 10 книги ордеров продажи</summary>
        ////public IEnumerable<OrderBook> OrderBook10Asks => bitMexWebSocket?.OrderBook10Asks;
        /////// <summary>Топ 10 книги ордеров покупки</summary>
        ////public IEnumerable<OrderBook> OrderBook10Bids => bitMexWebSocket?.OrderBook10Bids;

        ///// <summary>Количество полученных сообщений от сервера</summary>
        //public long? CountMessage => bitMexWebSocket?.CountMessage;

        ///// <summary>Время получения (локальное) последнего сообщения от сервера</summary>
        //public DateTime? TimeLastMessage => bitMexWebSocket?.TimeLastMessage;

        /////// <summary>Минимальная цена продажи по Топ 10 книги ордеров</summary>
        ////public decimal? MinSell => bitMexWebSocket?.MinSell;

        /////// <summary>Максимальная цена покупки по Топ 10 книги ордеров</summary>
        ////public decimal? MaxBuy => bitMexWebSocket?.MaxBuy;


        //WebSocketBitMexSigned bitMexWebSocket;

        //private void CreateWebSocket(string APIKey, string APISecret, bool RealWork)
        //{

        //    bitMexWebSocket = new WebSocketBitMexSigned(APIKey, APISecret, RealWork);
        //    bitMexWebSocket.PropertyChanged += BitMexWebSocket_PropertyChanged;
        //    OnAllPropertyChanged();
        //}

        //private void BitMexWebSocket_PropertyChanged(object sender, PropertyChangedEventArgs e)
        //{
        //    string propertyName = e.PropertyName;
        //    string[] propertiesArr = { "WorkSymbol", "OrderBook10Asks", "OrderBook10Bids", "CountMessage", "TimeLastMessage", "MinSell", "MaxBuy" };
        //    if (Array.IndexOf(propertiesArr, propertyName) >= 0)
        //        OnPropertyChanged(propertyName);
        //}
    }
}
