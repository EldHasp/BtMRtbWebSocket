using BitMexLibrary.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace BitMexLibrary
{
    public partial class STR
    {
        //DispatcherTimer timerCandlesHistory = new DispatcherTimer();
        private int _countQuery;
        private bool? _isReadyCalculate;

        public int CountQuery { get => _countQuery; private set { _countQuery = value; OnPropertyChanged(); } }
        /// <summary>Флаг готовности к очередному расчёту</summary>
        public bool? IsReadyCalculate { get => _isReadyCalculate; private set { _isReadyCalculate = value; OnPropertyChanged(); } }

        public async void TimerCandlesHistory_TickAsync()
        {
            if (IsReadyCalculate == true)
            {
                IsReadyCalculate = false;
                await Task.Run(() => OnCandlesHistoryTick());
                IsReadyCalculate = true;
            }
        }

        private void OnCandlesHistoryTick()
        {
            ReadCandle(); //Чтение свечей
        }

    }
}
