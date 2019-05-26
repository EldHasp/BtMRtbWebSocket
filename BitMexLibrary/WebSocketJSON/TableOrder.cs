using BitMexLibrary.Enums;
using BitMexLibrary.WebSocketJSON;
using CommLibrary;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BitMexLibrary.WebSocketJSON
{
    public class TableOrder : OnPropertyChangedClass
    {
        private static SynchronizationContext _syncContext;
        private static SynchronizationContext SyncContext => _syncContext ?? SynchronizationContext.Current ?? new SynchronizationContext();

        private long _orderQty;
        private string _symbol;
        private string _orderID;
        private string _ordStatus;
        private SideEnum _side;
        private double _price;
        //private bool _isActive;
        private DateTime _timeStamp;
        //private object side;

        public string Symbol { get => _symbol; set { SetProperty(ref _symbol, value); } }
        public string OrderID { get => _orderID; set { SetProperty(ref _orderID, value); } }
        public long OrderQty { get => _orderQty; set { SetProperty(ref _orderQty, value); } }
        public string OrdStatus { get => _ordStatus; set { SetProperty(ref _ordStatus, value); } }

        public double Price { get => _price; set { SetProperty(ref _price, value); } }
        public SideEnum Side { get => _side; set { SetProperty(ref _side, value); } }

        public DateTime TimeStamp { get => _timeStamp; set { SetProperty(ref _timeStamp, value); } }

        //public bool IsActive { get => _isActive; set { SetProperty(ref _isActive, value); } }

        //protected override void PropertyNewValue<T>(ref T fieldProperty, T newValue, string nameProperty)
        //{
        //    base.PropertyNewValue(ref fieldProperty, newValue, nameProperty);
        //    if (" CurrentQty OpenOrderSellQty OpenOrderBuyQty ".Contains(nameProperty))
        //        IsActive = CurrentQty != 0 || OpenOrderSellQty != 0 || OpenOrderBuyQty != 0;
        //}

        public TableOrder CopyTo()
            => new TableOrder()
            {
                Symbol = Symbol,
                OrderID = OrderID,
                OrderQty = OrderQty,
                OrdStatus = OrdStatus,
                Side = Side,
                TimeStamp = TimeStamp,
                Price = Price
            };

        public void CopyFrom(TableOrder position)
        {
            Symbol = position.Symbol;
            OrderID = position.OrderID;
            OrderQty = position.OrderQty;
            OrdStatus = position.OrdStatus;
            Side = position.Side;
            TimeStamp = position.TimeStamp;
            Price = position.Price;
        }

        public static bool TryFromTable(TableJSON table, ObservableCollection<TableOrder> orders, out ObservableCollection<TableOrder> outOrders)
        {
            if (table?.Table != "order")
            {
                outOrders = orders;
                return false;
            }

            switch (table.Action)
            {
                case "partial":
                    outOrders = new ObservableCollection<TableOrder>
                        (table.Data.Select
                            (order => new TableOrder()
                            {
                                Symbol = order["symbol"].ToString(),
                                OrderID = order["orderID"].ToString(),
                                OrderQty = Convert.ToInt64(order["orderQty"]),
                                OrdStatus = order["ordStatus"].ToString(),
                                Side = order["side"].ToSideEnum(),
                                TimeStamp = Convert.ToDateTime(order["timestamp"]),
                                Price = Convert.ToDouble(order["price"]),
                            }
                            )
                        );
                    break;
                case "insert":
                    if (orders != null)
                    {
                        lock (orders)
                        {
                            foreach (Dictionary<string, object> order in table.Data)
                            {
                                TableOrder newOrder = new TableOrder()
                                {
                                    Symbol = order["symbol"].ToString(),
                                    OrderID = order["orderID"].ToString(),
                                    OrderQty = Convert.ToInt64(order["orderQty"]),
                                    OrdStatus = order["ordStatus"].ToString(),
                                    Side = order["side"].ToSideEnum(),
                                    TimeStamp = Convert.ToDateTime(order["timestamp"]),
                                    Price = Convert.ToDouble(order["price"]),
                                };
                                MainDispatcher.dispatcher.Invoke(() => { orders.Add(newOrder); });
                                //SyncContext.Post(unused => { orders.Add(newOrder); }, null);
                                //orders.Add(new TableOrder()
                                //{
                                //    Symbol = order["symbol"].ToString(),
                                //    OrderID = order["orderID"].ToString(),
                                //    OrderQty = Convert.ToInt64(order["orderQty"]),
                                //    OrdStatus = order["ordStatus"].ToString(),
                                //    Side = order["side"].ToSideEnum(),
                                //    TimeStamp = Convert.ToDateTime(order["timestamp"]),
                                //    Price = Convert.ToDouble(order["price"]),
                                //});
                            }
                        }
                    }
                    outOrders = orders;
                    break;
                case "update":
                    if (orders != null)
                    {
                        lock (orders)
                        {
                            foreach (Dictionary<string, object> order in table.Data)
                            {
                                TableOrder orderFind = default;
                                for (int i = 0; i < 10; i++)
                                {
                                    orderFind = orders.FirstOrDefault(pos => pos.OrderID == order["orderID"].ToString());
                                    if (orderFind != default)
                                        break;
                                    Thread.Sleep(5);
                                }
                                if (orderFind == default)
                                    throw new ArgumentException($"Требуется изменить несуществующий ордер ID={order["orderID"]}!");
                                //{
                                //    orderFind = new TableOrder
                                //    {
                                //        TimeStamp = Convert.ToDateTime(order["timestamp"]),
                                //        OrderID = order["orderID"].ToString()
                                //    };
                                //    if (order.TryGetValue("symbol", out object _val))
                                //        orderFind.Symbol = _val.ToString();
                                //    if (order.TryGetValue("orderQty", out _val))
                                //        orderFind.OrderQty = Convert.ToInt64(_val);
                                //    if (order.TryGetValue("ordStatus", out _val))
                                //        orderFind.OrdStatus = _val.ToString();
                                //    if (order.TryGetValue("side", out _val))
                                //        orderFind.Side = _val.ToSideEnum();
                                //    if (order.TryGetValue("price", out _val))
                                //        orderFind.Price = Convert.ToDouble(_val);
                                //    //orders.Add(orderFind);
                                //    MainDispatcher.dispatcher.Invoke(() => { orders.Add(orderFind); });
                                //    //SyncContext.Post(unused => { orders.Add(orderFind); }, null);
                                //}
                                //else
                                {
                                    DateTime timeStamp = Convert.ToDateTime(order["timestamp"]);

                                    if (timeStamp >= orderFind.TimeStamp)
                                    {
                                        orderFind.TimeStamp = timeStamp;
                                        if (order.TryGetValue("symbol", out object _val))
                                            orderFind.Symbol = _val.ToString();
                                        if (order.TryGetValue("orderID", out _val))
                                            orderFind.OrderID = _val.ToString();
                                        if (order.TryGetValue("orderQty", out _val))
                                            orderFind.OrderQty = Convert.ToInt64(_val);
                                        if (order.TryGetValue("ordStatus", out _val))
                                            orderFind.OrdStatus = _val.ToString();
                                        if (order.TryGetValue("side", out _val))
                                            orderFind.Side = _val.ToSideEnum();
                                        if (order.TryGetValue("price", out _val))
                                            orderFind.Price = Convert.ToDouble(_val);
                                    }
                                }
                            }
                        }
                    }
                    outOrders = orders;
                    break;
                default:
                    throw new ArgumentException();
            }
            return true;
        }
    }
}
