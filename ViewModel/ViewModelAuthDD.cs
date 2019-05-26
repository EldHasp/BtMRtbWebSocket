using BitMexLibrary;
using CommLibrary;
using System;
using System.Windows.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using BitMexLibrary.WebSocketJSON;

namespace ViewModel
{
    public partial class ViewModelAuthDD : OnPropertyChangedClass
    {

        public bool AuthorizationRequest { get => _authorizationRequest; set { SetProperty(ref _authorizationRequest, value); } }
        public bool AuthorizationComplete { get => _authorizationComplete; set { SetProperty(ref _authorizationComplete, value); } }

        private DateTime _timeBitMex = DateTime.UtcNow;
        public DateTime TimeBitMex { get => _timeBitMex; set { SetProperty(ref _timeBitMex, value); } }

        private SettingsClass _settings;
        public SettingsClass Settings { get => _settings; set { SetProperty(ref _settings, value); } }

        private RelayCommand _authorReqComm;
        public RelayCommand AuthorReqComm => _authorReqComm ?? (_authorReqComm = new RelayCommand(OnAuthorReqAsync, (x) => !AuthorizationRequest));

        protected virtual async void OnAuthorReqAsync(object param)
        {
            await Task.Run(() => OnAuthorReq(param));
        }

        protected virtual void OnAuthorReq(object param) { }




        private RelayCommand _reAuthorReqComm;
        public RelayCommand ReAuthorReqComm => _reAuthorReqComm ?? (_reAuthorReqComm = new RelayCommand(OnReAuthorReq, (x) => AuthorizationRequest));
        protected virtual void OnReAuthorReq(object param) { }

        private RelayCommand _continueComm;
        public RelayCommand ContinueComm => _continueComm ?? (_continueComm = new RelayCommand(OnContinue, OnCanContinue));
        protected virtual void OnContinue(object param) { }

        private bool OnCanContinue(object param)
            => AuthorizationRequest && ValidRest == true && IsOpen == true && Authorization == true;

        private void Timer_Tick(object sender, EventArgs e) 
            => TimeBitMex = DateTime.UtcNow - BitMEXApi.DeltaTime;

        DispatcherTimer timer = new DispatcherTimer();

        public  ViewModelAuthDD()
        {
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        private bool _authorizationRequest = false;
        private bool _authorizationComplete = false;
        private bool? _validRest;
        private decimal? _balanceRest;
        private string _workSymbol;
        private long? _countMessage;
        private DateTime? _timeLastMessage;
        private IEnumerable<InfoDocs> _infoDocsList;
        private bool? _authorization;
        private bool? _isOpen;
        private bool? _isClose;
        private DataWallet _wallet;
        private DataMargin _margin;

        protected override void PropertyNewValue<T>(ref T fieldProperty, T newValue, string nameProperty)
        {
            base.PropertyNewValue(ref fieldProperty, newValue, nameProperty);

            if ("AuthorizationComplete" == nameProperty)
            {
                if (AuthorizationComplete)
                    timer.Stop();
                else
                    timer.Start();
            }
                ContinueComm.Invalidate();
            if ("AuthorizationRequest ValidRest IsOpen Authorization".Contains(nameProperty))
                ContinueComm.Invalidate();
            if ("AuthorizationRequest" == nameProperty)
            {
                ReAuthorReqComm.Invalidate();
                AuthorReqComm.Invalidate();
            }
        }


        public bool? ValidRest { get => _validRest; set { SetProperty(ref _validRest, value); } }
        public decimal? BalanceRest { get => _balanceRest; set { SetProperty(ref _balanceRest, value); } }

        /// <summary>Рабочий Symbol</summary>
        public string WorkSymbol { get => _workSymbol; set { SetProperty(ref _workSymbol, value); } }

        /// <summary>Количество полученных сообщений от сервера</summary>
        public long? CountMessage { get => _countMessage; set { SetProperty(ref _countMessage, value); } }

        /// <summary>Время получения (локальное) последнего сообщения от сервера</summary>
        public DateTime? TimeLastMessage { get => _timeLastMessage; set { SetProperty(ref _timeLastMessage, value); } }


        public IEnumerable<InfoDocs> InfoDocsList { get => _infoDocsList; set { SetProperty(ref _infoDocsList, value); } }
        public bool? Authorization { get => _authorization; set { SetProperty(ref _authorization, value); } }
        public bool? IsOpen { get => _isOpen; set { SetProperty(ref _isOpen, value); } }
        public bool? IsClose { get => _isClose; set { SetProperty(ref _isClose, value); } }
        /// <summary>Wallet баланс</summary>
        public DataWallet Wallet { get => _wallet; set { SetProperty(ref _wallet, value); } }
        /// <summary>Margin баланс</summary>
        public DataMargin Margin { get => _margin; set { SetProperty(ref _margin, value); } }

    }
}
