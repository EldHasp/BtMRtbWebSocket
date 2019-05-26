using BitMexLibrary;
using CommLibrary;
using System;
using System.Windows.Input;
using System.Windows.Threading;
using static BitMexLibrary.STR;

namespace ViewModel
{
    public partial class ViewModel : OnPropertyChangedClass
    {

        // Событие
        public event SignalHandler SignalEvent;


        public DateTime TimeBitMex { get => _timeBitMex; set { _timeBitMex = value; OnPropertyChanged(); } }
        public TimeSpan? TimeReReadCandle { get => _timeReReadCandle; set { _timeReReadCandle = value; OnPropertyChanged(); OnPropertyChanged("TimeReReadCandle"); } }
        public bool IsTimeReReadCandle => TimeReReadCandle == null || TimeReReadCandle >= new TimeSpan();

        private SettingsClass _settings;
        public SettingsClass Settings { get => _settings; set { _settings = value; CountCandelesForCalculate = Settings.CountCandelesForCalculate; OnPropertyChanged(); } }

        private string _linesText;
        public string LinesText { get => _linesText; set { _linesText = value; OnPropertyChanged(); } }

        private ICommand _createComm;
        public ICommand CreateComm => _createComm ?? (_createComm = new RelayCommand(OnCreate, CanCreate));

        public bool IsCreate { get; private set; } = false;
        private bool CanCreate(object arg)
        {
            return !IsCreate;
        }

        public void OnCreate(object param)
        {
            IsCreate = true;
            MySetting.Save();
            if (param is bool realNet)
                CreateREST(Settings.APIKey, Settings.APISecret, realNet);
            else
                CreateREST(Settings.APIKey, Settings.APISecret, Settings.RealWork);
            //LinesText = str.LinesOut;

            //CreateWebSocket(Settings.APIKey, Settings.APISecret, false);
        }

        private ICommand _saveComm;
        private DateTime _timeBitMex = DateTime.UtcNow;

        public ICommand SaveComm => _saveComm ?? (_saveComm = new RelayCommand(OnSave));

        readonly string fileNameTSV = "DataCandles.tsv";
        private void OnSave(object param)
        {
            System.IO.File.WriteAllText(fileNameTSV, LinesText);
        }

        public ViewModel()
        {
            Init(false);
        }

        private void Init(bool slave)
        {

            Settings = MySetting.Settings;
            //OutColumns = new DGColumns();
            //OutColumns.Add(new DGColumn("Один", "1 2 3 4".Split()));
            //OutColumns.Add(new DGColumn("Ldf", "5 6 7 8".Split()));



            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += Timer_Tick;
            (new BitMEXApi()).GetAccountBalance();
            timer.Start();

        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            TimeBitMex = BitMEXApi.RealTime;
            TimeReReadCandle = (LastCandle?.TimeStamp - TimeBitMex) + TimeSpan.FromMinutes((int)BinSizeSelected);

            if (str != null)
            {
                if (TimeReReadCandle == null || TimeReReadCandle < new TimeSpan())
                    str.TimerCandlesHistory_TickAsync();
            }
        }

        DispatcherTimer timer = new DispatcherTimer();
        private TimeSpan? _timeReReadCandle;
    }
}
