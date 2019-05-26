using BitMexLibrary.WebSocketJSON;
using CommLibrary;
using BitMexLibrary;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommLibrary.Enums;
using BitMexLibrary.Enums;
using System.Windows;

namespace ViewModel
{
    public partial class ViewModelTrade
    {
        STR str;

        protected override void OnRunWork(object parameter)
        {
            if (SizePositionAutoWork == null || Math.Abs(SizePositionAutoWork.Value) < 10)
            {
                MessageBox.Show("Введите количествов (>9) ордеров для автоматической работы!", "Некорректные данные");
                return;
            }
            base.OnRunWork(parameter);
            str = new STR(bitMexREST.GetBitMEXApi());
            str.RunWork(BinSizeSelected, CountCandelesForCalculate);
            str.SetFinishCalculationTime(FinishCalculationTime);
            str.SignalEvent += Str_SignalEvent;
            str.PropertyChanged += Str_PropertyChanged;
            str.OnAllPropertyChanged();

        }

        protected override bool CanRunWork(object parameter)
            => str == null && base.CanRunWork(parameter);

        protected override bool CanEndWork(object parameter)
            => str != null && base.CanEndWork(parameter);

        protected override void OnEndWork(object parameter)
        {
            if (str != null)
            {
                str.PropertyChanged -= Str_PropertyChanged;
                str.SignalEvent -= Str_SignalEvent;
                str = null;
            }

            base.OnEndWork(parameter);
        }

        private void Str_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            string nameProp = e.PropertyName;
            if (string.IsNullOrEmpty(nameProp) || nameProp == "CountQuery")
                CountQuery = str.CountQuery;
            if (string.IsNullOrEmpty(nameProp) || nameProp == "LastCandle")
                LastCandle = str.LastCandle;
            if (string.IsNullOrEmpty(nameProp) || nameProp == "OutValues")
                OutColumns = str.OutValues;
            //if (string.IsNullOrEmpty(nameProp) || nameProp == "FinishCalculationTime")
            //    FinishCalculationTime = str.FinishCalculationTime;
        }

        private void Str_SignalEvent(SignalEnum signal)
        {
            OnSignalEvent(signal);

            switch (signal)
            {
                case SignalEnum.Long: CreateOrderAmend(WorkSymbol, (int)SizePositionAutoWork); break;
                case SignalEnum.Short: CreateOrderAmend(WorkSymbol, -(int)SizePositionAutoWork); break;
            }
        }

        protected override void Timer_Tick(object sender, EventArgs e)
        {
            base.Timer_Tick(sender, e);
            if (str != null)
            {
                if (TimeReReadCandle == null || TimeReReadCandle <= new TimeSpan())
                    str.TimerCandlesHistory_TickAsync();
            }

        }

        protected override void PropertyNewValue<T>(ref T fieldProperty, T newValue, string nameProperty)
        {
            base.PropertyNewValue(ref fieldProperty, newValue, nameProperty);
            if (nameProperty == "FinishCalculationTime")
                str?.SetFinishCalculationTime(FinishCalculationTime);
        }

        protected override bool CanGiveSignal(object parameter)
        {
            return SizePositionAutoWork != null && Math.Abs(SizePositionAutoWork.Value) >= 10 && base.CanGiveSignal(parameter);
        }

        protected override void OnGiveSignal(object parameter)
        {
            if (parameter is bool isLong)
                Str_SignalEvent(isLong ? SignalEnum.Long: SignalEnum.Short);
            base.OnGiveSignal(parameter);
        }

        protected override void OnAddTime(object parameter)
        {
            base.OnAddTime(parameter);
        }

        public override void OnSignalEvent(SignalEnum signal)
        {
            base.OnSignalEvent(signal);
        }
    }
}
