using BitMexLibrary.WebSocketJSON;
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
    public partial class ViewModelWSOrderBook10DD : OnPropertyChangedClass
    {

        public ViewModelWSOrderBook10DD()
        {

        }

        private IEnumerable<OrderBook> _orderBook10asks;
        private IEnumerable<OrderBook> _orderBook10bids;
        private long? _countMessage;
        private DateTime? _timeLastMessage;
        private decimal? _minSell;
        private decimal? _maxBuy;
        private string _workSymbol;
        private bool? _isOpen = false;
        private bool? _isClose = true;

        /// <summary>WebSocket открыт</summary>
        public bool? IsOpen { get => _isOpen;  set { SetProperty(ref _isOpen, value); } }

        /// <summary>WebSocket закрыт</summary>
        public bool? IsClose { get => _isClose;  set { SetProperty(ref _isClose, value); } }
        /// <summary>Рабочий Symbol</summary>
        public string WorkSymbol { get => _workSymbol;  set { SetProperty(ref _workSymbol, value); } }

        /// <summary>Топ 10 книги ордеров продажи</summary>
        public IEnumerable<OrderBook> OrderBook10Asks { get => _orderBook10asks;  set { SetProperty(ref _orderBook10asks, value); } }
        /// <summary>Топ 10 книги ордеров покупки</summary>
        public IEnumerable<OrderBook> OrderBook10Bids { get => _orderBook10bids;  set { SetProperty(ref _orderBook10bids, value); } }

        /// <summary>Количество полученных сообщений от сервера</summary>
        public long? CountMessage { get => _countMessage;  set { SetProperty(ref _countMessage, value); } }

        /// <summary>Время получения (локальное) последнего сообщения от сервера</summary>
        public DateTime? TimeLastMessage { get => _timeLastMessage;  set { SetProperty(ref _timeLastMessage, value); } }

        /// <summary>Минимальная цена продажи по Топ 10 книги ордеров</summary>
        public decimal? MinSell { get => _minSell;  set { SetProperty(ref _minSell, value); } }

        /// <summary>Максимальная цена покупки по Топ 10 книги ордеров</summary>
        public decimal? MaxBuy { get => _maxBuy;  set { SetProperty(ref _maxBuy, value); } }

    }
}
