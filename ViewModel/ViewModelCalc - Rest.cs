using BitMexLibrary;
using CommLibrary;
using System;
using System.Windows.Input;
using System.Windows.Threading;
using System.Threading.Tasks;
using CommLibrary.Enums;
using static BitMexLibrary.STR;

namespace ViewModel
{
    public partial class ViewModelCalc : OnPropertyChangedClass
    {
        public event SignalHandler SignalEvent;

        STR str; // Модель
        private int _countQuery;
        private Candle _lastCandle;

        public int CountQuery { get => _countQuery; private set { _countQuery = value; OnPropertyChanged(); } }
        public Candle LastCandle { get => _lastCandle; set { _lastCandle = value; OnPropertyChanged(); } }
        public int CountCandelesForCalculate
        {
            get => Settings.CountCandelesForCalculate;
            set
            {
                if (value < 120 || value > 720)
                    throw new ArgumentOutOfRangeException();
                if (Settings.CountCandelesForCalculate != value)
                {
                    Settings.CountCandelesForCalculate = value;
                    OnPropertyChanged();
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
                    OnPropertyChanged();
                }
            }
        }

        private  void CreateREST(bool realWork)
        {
            str = new STR("", "", realWork, BinSizeSelected, CountCandelesForCalculate);
            //STR str = new STR("OzwyJSUylMSgvhsmdihjKvFI", "MgIowV0eYGOyHp5f9PgX49kUtQYy5LMKK7nR6ViDKnE6gvuS");
            //str.ReadCandle();
            str.SignalEvent += Str_SignalEvent;
            str.PropertyChanged += Str_PropertyChanged;
            str.OnAllPropertyChanged();

            //await Task.Run(() => str.OnTimerCandlesHistory());
        }

        private void Str_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            string nameProp = e.PropertyName;
            if (string.IsNullOrEmpty(nameProp) || nameProp == "CountQuery")
                CountQuery = str.CountQuery;
            if (string.IsNullOrEmpty(nameProp) || nameProp == "LastCandle")
                LastCandle = str.LastCandle;
            if (string.IsNullOrEmpty(nameProp) || nameProp == "OutValues")
                OutColumns = str.OutValues;
        }

        private void Str_SignalEvent(BitMexLibrary.Enums.SignalEnum signal) => SignalEvent?.Invoke(signal);


    }
}
