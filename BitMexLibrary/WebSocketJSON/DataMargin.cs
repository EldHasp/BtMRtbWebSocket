using BitMexLibrary.WebSocketJSON;
using CommLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitMexLibrary.WebSocketJSON
{
    public class DataMargin : OnPropertyChangedClass
    {
        private DateTime? _timeStamp;
        private long? _account;
        private long? _amount;
        private long? _walletBalance;
        private long? _marginBalance;

        public DateTime? TimeStamp { get => _timeStamp; set { SetProperty(ref _timeStamp, value); } }
        public long? Account { get => _account; set { SetProperty(ref _account, value); } }
        public long? Amount { get => _amount; set { SetProperty(ref _amount, value); } }
        public long? WalletBalance { get => _walletBalance; set { SetProperty(ref _walletBalance, value); } }
        public long? MarginBalance { get => _marginBalance; set { SetProperty(ref _marginBalance, value); } }

        public static bool TryFromTable(TableJSON table, DataMargin margin, out DataMargin outMargin)
        {

            if (table?.Table != "margin")
            {
                outMargin = margin;
                return false;
            }

            Dictionary<string, object> data = table?.Data[0];
            switch (table.Action)
            {
                case "partial":
                    outMargin = new DataMargin()
                    {
                        TimeStamp = Convert.ToDateTime(data["timestamp"]),
                        Account = Convert.ToInt64(data["account"]),
                        Amount = Convert.ToInt64(data["amount"]),
                        WalletBalance = Convert.ToInt64(data["walletBalance"]),
                        MarginBalance = Convert.ToInt64(data["marginBalance"])
                    };
                    break; 
                case "update":
                    if (margin != null)
                    {
                        if (data.TryGetValue("timestamp", out object _val))
                            margin.TimeStamp = Convert.ToDateTime(_val);
                        if (data.TryGetValue("account", out _val))
                            margin.Account = Convert.ToInt64(_val);
                        if (data.TryGetValue("amount", out _val))
                            margin.Amount = Convert.ToInt64(_val);
                        if (data.TryGetValue("walletBalance", out _val))
                            margin.WalletBalance = Convert.ToInt64(_val);
                        if (data.TryGetValue("marginBalance", out _val))
                            margin.MarginBalance = Convert.ToInt64(_val);
                    }
                    outMargin = margin;
                    break; 
                default:
                    throw new ArgumentException();
            }
            return true;
        }
    }
}
