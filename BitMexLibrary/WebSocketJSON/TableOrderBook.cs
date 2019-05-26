using CommLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace BitMexLibrary.WebSocketJSON
{
    [DataContract]
    public class OrderBook : OnPropertyChangedClass
    {
        private decimal _price;
        private int _size;

        [DataMember(IsRequired = false)]
        public decimal Price { get => _price; set { SetProperty(ref _price, value); } }
        [DataMember(IsRequired = false)]
        public int Size { get => _size; set { SetProperty(ref _size, value); } }


        public void CopyFrom(OrderBook orderBook)
        {
            Price = orderBook.Price;
            Size = orderBook.Size;
        }

        public static OrderBook FromArr(decimal[] value)
             => new OrderBook() { Price = value[0], Size = Convert.ToInt32(value[1]) };
        public static List<OrderBook> FromArr(decimal[][] value)
             => value.Select(ord => FromArr(ord)).ToList();

    }
    [DataContract]
    public class OrderBookList
    {
        [DataMember(IsRequired = false)]
        public string Symbol { get; set; }
        [DataMember(IsRequired = false)]
        public DateTime TimeStamp { get; set; }
        [DataMember(IsRequired = false)]
        public List<OrderBook> Bids { get; set; }
        [DataMember(IsRequired = false)]
        public List<OrderBook> Asks { get; set; }

        public static OrderBookList FromArr(OrderBookArr value)
            => new OrderBookList()
            {
                Symbol = value.Symbol,
                TimeStamp = value.TimeStamp,
                Asks = OrderBook.FromArr(value.Asks),
                Bids = OrderBook.FromArr(value.Bids)
            };

        public static List<OrderBookList> FromArr(IEnumerable<OrderBookArr> value)
            => value.Select(ord => FromArr(ord)).ToList();
    }

    [DataContract]
    public class TableOrderBookList
    {
        [DataMember(IsRequired = false)]
        public string Table { get; set; }
        [DataMember(IsRequired = false)]
        public string Action { get; set; }
        [DataMember(IsRequired = false)]
        public List<OrderBookList> Data { get; set; }

        public static TableOrderBookList FormArr(TableOrderBookArr value)
            => new TableOrderBookList()
            {
                Table = value.Table,
                Action = value.Action,
                Data = OrderBookList.FromArr(value.Data)
            };
    }

    [DataContract]
    public class OrderBookArr
    {
        [DataMember(Name = "symbol", IsRequired = true)]
        public string Symbol { get; set; }
        [DataMember(Name = "timestamp", IsRequired = true)]
        public DateTime TimeStamp { get; set; }
        [DataMember(Name = "bids", IsRequired = true)]
        public decimal[][] Bids { get; set; }
        [DataMember(Name = "asks", IsRequired = true)]
        public decimal[][] Asks { get; set; }
    }

    [DataContract]
    public class TableOrderBookArr
    {
        [DataMember(Name = "table", IsRequired = true)]
        public string Table { get; set; }
        [DataMember(Name = "action", IsRequired = true)]
        public string Action { get; set; }
        [DataMember(Name = "data", IsRequired = true)]
        public OrderBookArr[] Data { get; set; }
    }
}
