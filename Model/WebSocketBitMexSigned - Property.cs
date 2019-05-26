using BitMexLibrary.WebSocketJSON;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Threading;

namespace BitMexLibrary
{
    public partial class WebSocketBitMexSigned
    {
        private long _countMessage;
        private DateTime _timeLastMessage;
        private bool _isOpen = false;
        private bool _isClose = true;
        private DataMargin _margin = new DataMargin();
        private DataWallet _wallet = new DataWallet();
        private bool? _authorization;
        private bool _invalidAPIKey;
        private bool _signatureNotValid = false;
        private IEnumerable<InfoDocs> _infoDocsList = Enumerable.Empty<InfoDocs>();
        private ObservableCollection<Position> _positions;
        private ObservableCollection<TableOrder> _orders;

        /// <summary>Рабочий Symbol</summary>
        public string WorkSymbol => "XBTUSD";


        /// <summary>WebSocket открыт</summary>
        public bool IsOpen { get => _isOpen; private set { SetProperty(ref _isOpen, value); } }
        /// <summary>WebSocket закрыт</summary>
        public bool IsClose { get => _isClose; private set { SetProperty(ref _isClose, value); } }

        DispatcherTimer _timerPing;
        private decimal _minSell;
        private decimal _maxBuy;

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

        /// <summary>Wallet баланс</summary>
        public DataWallet Wallet { get => _wallet; private set { SetProperty(ref _wallet, value); } }
        /// <summary>Margin баланс</summary>
        public DataMargin Margin { get => _margin; private set { SetProperty(ref _margin, value); } }

        /// <summary>Авторизация</summary>
        public bool? Authorization { get => _authorization; private set { SetProperty(ref _authorization, value); } }
        /// <summary>Ошибка APIKey</summary>
        public bool InvalidAPIKey { get => _invalidAPIKey; private set { SetProperty(ref _invalidAPIKey, value); } }
        /// <summary>Ошибка API Signature</summary>
        public bool SignatureNotValid { get => _signatureNotValid; private set { SetProperty(ref _signatureNotValid, value); } }

        /// <summary>Список сообщений типа InfoDoc</summary>
        public IEnumerable<InfoDocs> InfoDocsList { get => _infoDocsList; private set { SetProperty(ref _infoDocsList, value); } }

        /// <summary>Список текущих позиций</summary>
        public ObservableCollection<Position> Positions { get => _positions; private set { SetProperty(ref _positions, value); } }

        /// <summary>Список текущих ордеров</summary>
        public ObservableCollection<TableOrder> Orders { get => _orders; private set { SetProperty(ref _orders, value); } }

        /// <summary>Минимальная цена продажи по Топ 10 книги ордеров</summary>
        public decimal MinSell { get => _minSell; private set { SetProperty(ref _minSell, value); } }

        /// <summary>Максимальная цена покупки по Топ 10 книги ордеров</summary>
        public decimal MaxBuy { get => _maxBuy; private set { SetProperty(ref _maxBuy, value); } }

    }
}
