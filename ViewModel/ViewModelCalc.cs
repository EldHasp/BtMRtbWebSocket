using BitMexLibrary;
using CommLibrary;
using System;
using System.Windows.Input;
using System.Windows.Threading;

namespace ViewModel
{
    public partial class ViewModelCalc : OnPropertyChangedClass
    {

        private DateTime _timeBitMex = DateTime.UtcNow;
        public DateTime TimeBitMex { get => _timeBitMex; set { _timeBitMex = value; OnPropertyChanged(); } }

        private SettingsClass _settings;
        public SettingsClass Settings { get => _settings; set { _settings = value; OnPropertyChanged(); } }

        private DGColumns _outColumns = new DGColumns();
        public DGColumns OutColumns { get => _outColumns; set { _outColumns = value; OnPropertyChanged(); } }

        private string _linesText;
        public string LinesText { get => _linesText; set { _linesText = value; OnPropertyChanged(); } }

        private ICommand _createComm;
        public ICommand CreateComm => _createComm ?? (_createComm = new RelayCommand(OnCreate));


        private void OnCreate(object param)
        {
            MySetting.Save();
            CreateREST(Settings.RealWork);
            //LinesText = str.LinesOut;

            //CreateWebSocket(Settings.APIKey, Settings.APISecret, false);
        }

        private ICommand _saveComm;

        public ICommand SaveComm => _saveComm ?? (_saveComm = new RelayCommand(OnSave));

        readonly string fileNameTSV = "DataCandles.tsv";
        private void OnSave(object param)
        {
            System.IO.File.WriteAllText(fileNameTSV, LinesText);
        }

        public ViewModelCalc()
        {
            Settings = MySetting.Settings;
            OutColumns = new DGColumns();
            //OutColumns.Add(new DGColumn("Один", "1 2 3 4".Split()));
            //OutColumns.Add(new DGColumn("Ldf", "5 6 7 8".Split()));



            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += Timer_Tick;
            //(new BitMEXApi()).GetAccountBalance();
            timer.Start();
        }

        private TimeSpan _deltaTime => BitMEXApi.DeltaTime;

        private void Timer_Tick(object sender, EventArgs e)
        {
            TimeBitMex = DateTime.UtcNow - _deltaTime;
        }

        DispatcherTimer timer = new DispatcherTimer();

    }
}
