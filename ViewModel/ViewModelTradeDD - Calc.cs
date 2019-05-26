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
using System.Windows.Threading;
using CommLibrary.Enums;
using static BitMexLibrary.STR;
using BitMexLibrary.Enums;

namespace ViewModel
{
    public partial class ViewModelTradeDD : OnPropertyChangedClass
    {
        public event SignalHandler SignalEvent;

        /// <summary>Вызов события SignalEvent</summary>
        /// <param name="signal">Значение сигнала</param>
        public virtual void OnSignalEvent(SignalEnum signal) => SignalEvent?.Invoke(signal);

        public TimeSpan? TimeReReadCandle { get => _timeReReadCandle; set { _timeReReadCandle = value; OnPropertyChanged(); OnPropertyChanged("IsTimeReReadCandle"); } }
        public bool IsTimeReReadCandle => TimeReReadCandle == null || TimeReReadCandle >= new TimeSpan();

        private DateTime _timeBitMex = DateTime.UtcNow;
        public DateTime TimeBitMex { get => _timeBitMex; set { SetProperty(ref _timeBitMex, value); } }

        private DGColumns _outColumns = new DGColumns();
        public DGColumns OutColumns { get => _outColumns; set { SetProperty(ref _outColumns, value); } }

        private TimeSpan _deltaTime => BitMEXApi.DeltaTime;

        protected virtual void Timer_Tick(object sender, EventArgs e)
        {
            TimeBitMex = BitMEXApi.RealTime;
            DateTime timeCalc = (FinishCalculationTime == null || FinishCalculationTime.Value > TimeBitMex) ? TimeBitMex : FinishCalculationTime.Value;
            TimeReReadCandle = (LastCandle?.TimeStamp - timeCalc) + TimeSpan.FromMinutes((int)BinSizeSelected);
        }

        DispatcherTimer timer = new DispatcherTimer();
        private int _countQuery;
        private Candle _lastCandle;
        private TimeSpan? _timeReReadCandle;

        public int CountQuery { get => _countQuery; protected set { SetProperty(ref _countQuery, value); } }
        public Candle LastCandle { get => _lastCandle; set { SetProperty(ref _lastCandle , value); } }
        public int CountCandelesForCalculate
        {
            get => Settings.CountCandelesForCalculate;
            set
            {
                if (value < 120)
                    throw new ArgumentOutOfRangeException();
                if (Settings.CountCandelesForCalculate != value)
                {
                    Settings.CountCandelesForCalculate = value;
                    //OnPropertyChanged();
                }
            }
        }
        public BinSizeEnum[] BinSizeList { get; } = new BinSizeEnum[] { BinSizeEnum.Minute, BinSizeEnum.FiveMinutes, BinSizeEnum.SixMinutes, BinSizeEnum.Hour };

        public BinSizeEnum BinSizeSelected
        {
            get => Settings.TypeCandles;
            set
            {
                if (Settings.TypeCandles != value && Array.IndexOf(BinSizeList, value) >= 0)
                {
                    Settings.TypeCandles = value;
                    //OnPropertyChanged();
                }
            }
        }

    }
}
