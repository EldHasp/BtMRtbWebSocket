using BitMexLibrary.WebSocketJSON;
using CommLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitMexLibrary.WebSocketJSON
{
    public class DataWallet : OnPropertyChangedClass
    {
        private DateTime? _prevTimeStamp;
        private DateTime? _timeStamp;
        private long? _account;
        private long? _amount;
        private long? _prevAmount;

        public DateTime? PrevTimeStamp { get => _prevTimeStamp; set { SetProperty(ref _prevTimeStamp, value); } }
        public DateTime? TimeStamp { get => _timeStamp; set { SetProperty(ref _timeStamp, value); } }
        public long? Account { get => _account; set { SetProperty(ref _account, value); } }
        public long? Amount { get => _amount; set { SetProperty(ref _amount, value); } }
        public long? PrevAmount { get => _prevAmount; set { SetProperty(ref _prevAmount, value); } }

        public static bool TryFromTable(TableJSON table, DataWallet wallet, out DataWallet outWallet)
        {

            if (table?.Table != "wallet")
            {
                outWallet = wallet;
                return false;
            }

            Dictionary<string, object> data = table?.Data[0];
            switch (table.Action)
            {
                case "partial":
                    outWallet = new DataWallet()
                    {
                        PrevTimeStamp = Convert.ToDateTime(data["prevTimestamp"]),
                        TimeStamp = Convert.ToDateTime(data["timestamp"]),
                        Account = Convert.ToInt64(data["account"]),
                        Amount = Convert.ToInt64(data["amount"]),
                        PrevAmount = Convert.ToInt64(data["prevAmount"])
                    };
                    break;
                case "update":
                    if (wallet != null)
                    {
                        if (data.TryGetValue("prevTimestamp", out object _val))
                            wallet.PrevTimeStamp = Convert.ToDateTime(_val);
                        if (data.TryGetValue("timestamp", out _val))
                            wallet.TimeStamp = Convert.ToDateTime(_val);
                        if (data.TryGetValue("account", out _val))
                            wallet.Account = Convert.ToInt64(_val);
                        if (data.TryGetValue("amount", out _val))
                            wallet.Amount = Convert.ToInt64(_val);
                        if (data.TryGetValue("prevAmount", out _val))
                            wallet.PrevAmount = Convert.ToInt64(_val);
                    }
                    outWallet = wallet;
                    break;
                default:
                    throw new ArgumentException();
            }
            return true;
        }

    }
}
