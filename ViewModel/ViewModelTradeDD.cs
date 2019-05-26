using BitMexLibrary.WebSocketJSON;
using CommLibrary;
using BitMexLibrary;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace ViewModel
{
    public partial class ViewModelTradeDD : OnPropertyChangedClass
    {

        private SettingsClass _settings;
        private bool _authorizationComplete = false;
        private string _workSymbol;
        private long? _countMessage;
        private DateTime? _timeLastMessage;
        private DataWallet _wallet;
        private DataMargin _margin;
        private ObservableCollection<Position> _positions = new ObservableCollection<Position>();
        private decimal _minSell;
        private decimal _maxBuy;
        private bool? _isOpen;
        private uint _sizeOrder;
        private RelayCommand _createOrderAmendCommand;
        private ObservableCollection<TableOrder> _orders;
        private bool _isSideBuy;
        private RelayCommand _runWorkCommand;
        private bool _workStarted;
        private bool _isManualPrice;
        private decimal _price;
        private RelayCommand _endWorkCommand;
        private DateTime? _finishCalculationTime;
        private RelayCommand _addTimeCommand;
        private bool _isTestTime;
        private bool _isPosition;
        private int? _sizePositionAutoWork;
        private RelayCommand _orderAmendCommand;
        private RelayCommand _giveSignalCommand;
        private bool _isAmend;
        private decimal prevPrice;
        private bool prevIsAmend;
        private bool prevIsManualPrice;
        private bool _isShortSignal;

        public SettingsClass Settings { get => _settings ?? (Settings = MySetting.Settings); set { SetProperty(ref _settings, value); } }

        public bool AuthorizationComplete { get => _authorizationComplete; set { SetProperty(ref _authorizationComplete, value); } }

        //public static WebSocketBitMexSigned WSocket { get; private set; }
        /// <summary>Рабочий Symbol</summary>
        public string WorkSymbol { get => _workSymbol; set { SetProperty(ref _workSymbol, value); } }

        /// <summary>Количество полученных сообщений от сервера</summary>
        public long? CountMessage { get => _countMessage; set { SetProperty(ref _countMessage, value); } }

        /// <summary>Время получения (локальное) последнего сообщения от сервера</summary>
        public DateTime? TimeLastMessage { get => _timeLastMessage; set { SetProperty(ref _timeLastMessage, value); } }

        /// <summary>Wallet баланс</summary>
        public DataWallet Wallet { get => _wallet; set { SetProperty(ref _wallet, value); } }
        /// <summary>Margin баланс</summary>
        public DataMargin Margin { get => _margin; set { SetProperty(ref _margin, value); } }

        /// <summary>Список текущих позиций</summary>
        public ObservableCollection<Position> Positions { get => _positions; set { SetProperty(ref _positions, value); } }

        /// <summary>Список текущих ненулевых позиций</summary>
        public ObservableCollection<TableOrder> Orders { get => _orders; set { SetProperty(ref _orders, value); } }

        /// <summary>Минимальная цена продажи по Топ 10 книги ордеров</summary>
        public decimal MinSell { get => _minSell; set { SetProperty(ref _minSell, value); } }
        /// <summary>Максимальная цена покупки по Топ 10 книги ордеров</summary>
        public decimal MaxBuy { get => _maxBuy; set { SetProperty(ref _maxBuy, value); } }

        /// <summary>Сокет открыт</summary>
        public bool? IsOpen { get => _isOpen; set { SetProperty(ref _isOpen, value); } }

        /// <summary>Размер выставляемого ордера</summary>
        public uint SizeOrder { get => _sizeOrder; set { SetProperty(ref _sizeOrder, value);/* CreateOrderAmendCommand.Invalidate(); */} }

        /// <summary>Направление (строна) сделки: true - Buy, false - Sell</summary>
        public bool IsSideBuy { get => _isSideBuy; set { SetProperty(ref _isSideBuy, value); } }

        /// <summary>Позиция или ордер: true - создать позицию, false - выставить ордер</summary>
        public bool IsPosition { get => _isPosition; set { SetProperty(ref _isPosition, value); } }

        /// <summary>Ордер с улучшаемой ценой : true - улучшать цену, false - постоянная цена</summary>
        public bool IsAmend { get => _isAmend; set { SetProperty(ref _isAmend, value); } }

        /// <summary>Позиция или ордер: true - создать позицию, false - выставить ордер</summary>
        public int? SizePositionAutoWork { get => _sizePositionAutoWork; set { SetProperty(ref _sizePositionAutoWork, value); } }

        /// <summary>Задать цену: true - в ручную, false - взять лучшую </summary>
        public bool IsManualPrice { get => _isManualPrice; set { SetProperty(ref _isManualPrice, value); } }

        /// <summary>Цена</summary>
        public decimal Price { get => _price; set { SetProperty(ref _price, value); } }

        /// <summary>Вид сигнала: <see langword="true"/> - Short, <see langword="false"/> - Long</summary>
        public bool IsShortSignal { get => _isShortSignal; set => _isShortSignal = value; }

        public RelayCommand CreateOrderAmendCommand => _createOrderAmendCommand
            ?? (_createOrderAmendCommand = new RelayCommand(OnCreateOrderAmend, CanCreateOrderAmend));

        public RelayCommand OrderAmendCommand => _orderAmendCommand
            ?? (_orderAmendCommand = new RelayCommand(OnOrderAmend, CanOrderAmend));

        protected virtual bool CanCreateOrderAmend(object parameter) => Price > 0 || SizeOrder != 0;
        protected virtual void OnCreateOrderAmend(object parameter) { }

        protected virtual bool CanOrderAmend(object parameter) => Price > 0 || SizeOrder != 0;
        protected virtual void OnOrderAmend(object parameter) { }


        public RelayCommand RunWorkCommand => _runWorkCommand ?? (_runWorkCommand = new RelayCommand(OnRunWork, CanRunWork));

        protected virtual bool CanRunWork(object parameter) => !IsWorkStarted && SizePositionAutoWork > 0;

        protected virtual void OnRunWork(object parameter) => IsWorkStarted = true;

        public RelayCommand EndWorkCommand => _endWorkCommand ?? (_endWorkCommand = new RelayCommand(OnEndWork, CanEndWork));

        protected virtual bool CanEndWork(object parameter) => IsWorkStarted;

        protected virtual void OnEndWork(object parameter) => IsWorkStarted = false;

        public RelayCommand GiveSignalCommand => _giveSignalCommand ?? (_giveSignalCommand = new RelayCommand(OnGiveSignal, CanGiveSignal));

        protected virtual bool CanGiveSignal(object arg) => IsWorkStarted;

        protected virtual void OnGiveSignal(object obj) { }

        public RelayCommand AddTimeCommand => _addTimeCommand ?? (_addTimeCommand = new RelayCommand(OnAddTime, CanAddTime));

        protected bool CanAddTime(object parameter)
            => FinishCalculationTime != null;

        /// <summary>Добавление к установленному времени периода одной свечи</summary>
        /// <param name="parameter">Не используется</param>
        protected virtual void OnAddTime(object parameter)
        {
            if (FinishCalculationTime != null)
                FinishCalculationTime = FinishCalculationTime.Value.AddMinutes((int)BinSizeSelected);
        }

        public DateTime? FinishCalculationTime { get => _finishCalculationTime; set { SetProperty(ref _finishCalculationTime, value); } }

        public bool IsTestTime { get => _isTestTime; set { SetProperty(ref _isTestTime, value); } }

        public bool IsWorkStarted { get => _workStarted; set { SetProperty(ref _workStarted, value); } }

        protected override void PropertyNewValue<T>(ref T fieldProperty, T newValue, string nameProperty)
        {
            base.PropertyNewValue(ref fieldProperty, newValue, nameProperty);

            switch (nameProperty)
            {
                case "IsWorkStarted":
                    if (IsWorkStarted)
                        timer.Start();
                    else
                        timer.Stop();
                    break;
                case "MinSell":
                case "MaxBuy":
                case "IsSideBuy":
                case "IsManualPrice":
                case "IsPrice":
                    if (!IsManualPrice)
                        Price = IsSideBuy ? MaxBuy : MinSell;
                    break;
                case "IsTestTime":
                    if (!IsTestTime)
                        FinishCalculationTime = null;
                    break;
                case "IsPosition":
                    if (IsPosition)
                    {
                        prevPrice = Price;
                        prevIsAmend = IsAmend;
                        prevIsManualPrice = IsManualPrice;
                        Price = IsSideBuy ? MaxBuy : MinSell;
                        IsAmend = true;
                        IsManualPrice = false;
                    }
                    else
                    {
                        IsManualPrice = prevIsManualPrice;
                        IsAmend = prevIsAmend;
                        Price = IsManualPrice ? prevPrice : IsSideBuy ? MaxBuy : MinSell;
                    }
                    break;
            }

        }

        public ViewModelTradeDD()
        {
            timer.Interval = TimeSpan.FromMilliseconds(500);
            timer.Tick += Timer_Tick;
        }

    }
}
