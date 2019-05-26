using BitMexLibrary.WebSocketJSON;
using CommLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Threading;
using WebSocketSharp;

namespace BitMexLibrary
{
    public partial class WebSocketBitMexUnSigned : OnPropertyChangedClass
    {
        private IEnumerable<OrderBook> _orderBook10asks;
        private IEnumerable<OrderBook> _orderBook10bids;
        private long _countMessage;
        private DateTime _timeLastMessage;
        private decimal _minSell;
        private decimal _maxBuy;
        private bool _isOpen = false;
        private bool _isClose = true;

        /// <summary>WebSocket открыт</summary>
        public bool IsOpen { get => _isOpen; private set { SetProperty(ref _isOpen, value); } }
        /// <summary>WebSocket закрыт</summary>
        public bool IsClose { get => _isClose; private set { SetProperty(ref _isClose, value); } }

        /// <summary>Рабочий Symbol</summary>
        public string WorkSymbol => "XBTUSD";

        /// <summary>Топ 10 книги ордеров продажи</summary>
        public IEnumerable<OrderBook> OrderBook10Asks { get => _orderBook10asks; private set { SetProperty(ref _orderBook10asks, value); } }
        /// <summary>Топ 10 книги ордеров покупки</summary>
        public IEnumerable<OrderBook> OrderBook10Bids { get => _orderBook10bids; private set { SetProperty(ref _orderBook10bids, value); } }

        DispatcherTimer _timerPing;
        DispatcherTimer TimerPing
        {
            get
            {
                if (_timerPing == null)
                {
                    _timerPing = new DispatcherTimer();
                    _timerPing.Interval = TimeSpan.FromSeconds(5);
                    _timerPing.Tick += TimerPing_Tick; ;
                }
                return _timerPing;
            }
        }

        private void TimerPing_Tick(object sender, EventArgs e)
        {
            WS.Ping();
        }

        /// <summary>Количество полученных сообщений от сервера</summary>
        public long CountMessage
        {
            get => _countMessage;
            private set
            {
                TimerPing.Stop();
                TimerPing.Start();
                SetProperty(ref _countMessage, value);
            }
        }

        /// <summary>Время получения (локальное) последнего сообщения от сервера</summary>
        public DateTime TimeLastMessage { get => _timeLastMessage; private set { SetProperty(ref _timeLastMessage, value); } }

        /// <summary>Минимальная цена продажи по Топ 10 книги ордеров</summary>
        public decimal MinSell { get => _minSell; private set { SetProperty(ref _minSell, value); } }

        /// <summary>Максимальная цена покупки по Топ 10 книги ордеров</summary>
        public decimal MaxBuy { get => _maxBuy; private set { SetProperty(ref _maxBuy, value); } }

    }

}
