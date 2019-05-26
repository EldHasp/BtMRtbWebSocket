using BitMexLibrary.WebSocketJSON;
using CommLibrary;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitMexLibrary.WebSocketJSON
{
    public class Position : OnPropertyChangedClass, IEditableObject
    {
        private long _currentQty;
        private string _symbol;
        private long _openOrderBuyQty;
        private long _openOrderSellQty;
        private bool _isActive;

        public string Symbol { get => _symbol; set { SetProperty(ref _symbol, value); } }
        public long CurrentQty { get => _currentQty; set { SetProperty(ref _currentQty, value); } }

        public long OpenOrderSellQty { get => _openOrderSellQty; set { SetProperty(ref _openOrderSellQty, value); } }
        public long OpenOrderBuyQty { get => _openOrderBuyQty; set { SetProperty(ref _openOrderBuyQty, value); } }

        public DateTime TimeStamp { get => _timeStamp; set { SetProperty(ref _timeStamp, value); } }

        public bool IsActive { get => _isActive; set { SetProperty(ref _isActive, value); } }

        protected override void PropertyNewValue<T>(ref T fieldProperty, T newValue, string nameProperty)
        {
            base.PropertyNewValue(ref fieldProperty, newValue, nameProperty);
            if (" CurrentQty OpenOrderSellQty OpenOrderBuyQty ".Contains(nameProperty))
                IsActive = CurrentQty != 0 || OpenOrderSellQty != 0 || OpenOrderBuyQty != 0;
        }

        public Position CopyTo()
            => new Position()
            {
                Symbol = Symbol,
                CurrentQty = CurrentQty,
                OpenOrderSellQty = OpenOrderSellQty,
                OpenOrderBuyQty = OpenOrderBuyQty
            };

        public void CopyFrom(Position position)
        {
            Symbol = position.Symbol;
            CurrentQty = position.CurrentQty;
            OpenOrderSellQty = position.OpenOrderSellQty;
            OpenOrderBuyQty = position.OpenOrderBuyQty;
        }

        public static bool TryFromTable(TableJSON table, ObservableCollection<Position> positions, out ObservableCollection<Position> outPositions)
        {
            if (table?.Table != "position")
            {
                outPositions = positions;
                return false;
            }

            switch (table.Action)
            {
                case "partial":
                    outPositions = new ObservableCollection<Position>
                        (table.Data.Select
                            (pos => new Position()
                            {
                                Symbol = pos["symbol"].ToString(),
                                CurrentQty = Convert.ToInt64(pos["currentQty"]),
                                OpenOrderSellQty = Convert.ToInt64(pos["openOrderSellQty"]),
                                OpenOrderBuyQty = Convert.ToInt64(pos["openOrderBuyQty"]),
                                TimeStamp = Convert.ToDateTime(pos["timestamp"])
                            }
                            )
                        );
                    break;
                case "update":
                    if (positions != null)
                    {
                        lock (positions)
                        {
                            foreach (Dictionary<string, object> data in table.Data)
                            {
                                Position position;
                                if ((position = positions.FirstOrDefault(pos => pos.Symbol == data["symbol"].ToString())) == default)
                                {
                                    position = new Position();
                                    position.Symbol = data["symbol"].ToString();
                                    position.TimeStamp = Convert.ToDateTime(data["timestamp"]);
                                    if (data.TryGetValue("currentQty", out object _val))
                                        position.CurrentQty = Convert.ToInt64(_val);
                                    if (data.TryGetValue("openOrderSellQty", out _val))
                                        position.OpenOrderSellQty = Convert.ToInt64(_val);
                                    if (data.TryGetValue("openOrderBuyQty", out _val))
                                        position.OpenOrderBuyQty = Convert.ToInt64(_val);
                                    positions.Add(position);
                                }
                                else
                                {
                                    DateTime timeStamp = Convert.ToDateTime(data["timestamp"]);

                                    if (timeStamp > position.TimeStamp)
                                    {
                                        position.TimeStamp = timeStamp;
                                        if (data.TryGetValue("currentQty", out object _val))
                                            position.CurrentQty = Convert.ToInt64(_val);
                                        if (data.TryGetValue("openOrderSellQty", out _val))
                                            position.OpenOrderSellQty = Convert.ToInt64(_val);
                                        if (data.TryGetValue("openOrderBuyQty", out _val))
                                            position.OpenOrderBuyQty = Convert.ToInt64(_val);
                                    }
                                }
                            }
                        }
                    }
                    outPositions = positions;
                    break;
                default:
                    throw new ArgumentException();
            }
            return true;
        }

        private Position backupPosition = default;
        private DateTime _timeStamp;

        public void BeginEdit()
        {
            if (backupPosition == default)
                backupPosition = CopyTo();
        }

        public void EndEdit()
        {
            if (backupPosition != default)
            {
                CopyFrom(backupPosition);
                backupPosition = default;
            }
        }

        public void CancelEdit()
        {
            if (backupPosition != default)
                backupPosition = default;
        }
    }
}
