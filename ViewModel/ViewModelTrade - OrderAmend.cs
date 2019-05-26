using BitMexLibrary.Enums;
using CommLibrary;
using CommLibrary.Enums;
using BitMexLibrary;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Threading;
using BitMexLibrary.WebSocketJSON;

namespace ViewModel
{
    public partial class ViewModelTrade
    {
        /// <summary>Класс для связи REST ордера и WebSocket ордера</summary>
        class OrderRESTWS
        {
            /// <summary>Ордер полученный от REST</summary>
            public BitMEXOrder OrderREST;
            /// <summary>Ордер полученный от WebSocket</summary>
            public TableOrder OrderWS;
            public OrderRESTWS(BitMEXOrder OrderREST, TableOrder OrderWS = null)
            {
                this.OrderREST = OrderREST;
                this.OrderWS = OrderWS;
                if (this.OrderREST != default && this.OrderWS != default && this.OrderREST.orderID != this.OrderWS.OrderID)
                    throw new ArgumentException();
            }
        }

        /// <summary>Класс для исполнения отложенной позиции</summary>
        class DeferredPositionClass
        {
            /// <summary>Размер позиции</summary>
            public int Quantity;
            /// <summary>Символ позиции</summary>
            public string Symbol;

            public DeferredPositionClass(string Symbol, int Value)
            {
                this.Symbol = Symbol;
                this.Quantity = Value;
            }
        }

        /// <summary>Список выставленных ордеров с перемещением</summary>
        readonly List<OrderRESTWS> ListOrderAmend = new List<OrderRESTWS>();

        /// <summary>Отложенная позиция</summary>
        DeferredPositionClass DeferredPosition;

        /// <summary>Создание позиции</summary>
        /// <param name="workSymbol">Рабочий символ</param>
        /// <param name="Size">Размер</param>
        public void CreatePosition(string workSymbol, int Size)
        {
            int positionSize = GetPositionSize(workSymbol);

            if (positionSize == 0) // Если позиции нет
                CreateOrderAmend(workSymbol, Size);
            else
            {
                DeferredPosition = new DeferredPositionClass(workSymbol, Size);
                CreateOrderAmend(workSymbol, -(int)positionSize);
            }
        }
        /// <summary>Получить позицию по указанному символу</summary>
        /// <param name="workSymbol">Символ</param>
        /// <returns>int - размер позиции</returns>
        int GetPositionSize(string workSymbol)
        {
            long positionSize = 0;
            Position position = Positions?.FirstOrDefault(pos => pos.Symbol == workSymbol && pos.CurrentQty != 0);
            if (position != default)
                positionSize = position.CurrentQty;
            return (int)positionSize;
        }

        /// <summary>Создать позицию ордерами с перемещением</summary>
        /// <param name="workSymbol">Рабочий символ</param>
        /// <param name="Size">Размер</param>
        /// <returns><see langword="true"/> - создан успешно</returns>
        public bool CreateOrderAmend(string workSymbol, int Size)
        {
            if (ListOrderAmend.Count > 0)
                return false;

            SideEnum side = Size.GetHand();
            if (side == SideEnum.UnKnown)
                return false;

            BitMEXOrder LimitNowOrderResult = null;
            int count = 0;
            if (IsManualPrice)
            {
                LimitNowOrderResult = bitMexREST.LimitNowOrder(workSymbol, side, Math.Abs(Size), Price);
            }
            else
                do
                {
                    //if (LimitNowOrderResult != null)
                    //    Thread.Sleep(500);
                    decimal priceBest = PriceBest(side);
                    LimitNowOrderResult = bitMexREST.LimitNowOrder(workSymbol, side, Math.Abs(Size), priceBest);
                } while ((LimitNowOrderResult == null || LimitNowOrderResult.ordStatus == "Canceled") && count++ < 10);

            if (LimitNowOrderResult == null || LimitNowOrderResult.ordStatus == "Canceled")
                return false;

            ListOrderAmend.Add(new OrderRESTWS(LimitNowOrderResult));
            OrderAmendCommand.Invalidate();
            return true;
        }

        /// <summary>Определение лучшей цены</summary>
        /// <param name="side">Направление: Buy,Sell </param>
        /// <returns>decimal с лучшей ценой</returns>
        public decimal PriceBest(SideEnum side)
        {
            switch (side)
            {
                case SideEnum.Buy: return WebSocketSigned.MaxBuy;
                case SideEnum.Sell: return WebSocketSigned.MinSell;
                case SideEnum.UnKnown: return 0;
            }
            throw new ArgumentException();
        }

        protected override bool CanCreateOrderAmend(object parameter)
        {
            bool ret = ListOrderAmend.Count == 0 && SizeOrder >= 10;
            return ret && base.CanCreateOrderAmend(parameter);
        }

        /// <summary>Создать ордер или позицию</summary>
        /// <param name="parameter"></param>
        protected override void OnCreateOrderAmend(object parameter)
        {
            if (SizeOrder >= 10)
            {
                if (IsPosition)
                {
                    CreatePosition(WorkSymbol, IsSideBuy ? (int)SizeOrder : -(int)SizeOrder);
                }
                else
                {
                    if (CreateOrderAmend(WorkSymbol, IsSideBuy ? (int)SizeOrder : -(int)SizeOrder))
                    {
                    }
                    else
                        MessageBox.Show("Ошибка выставления ордера!");
                }
            }
        }

        /// <summary>Обработка изменения минимальной цены продажи</summary>
        /// <param name="newValue">Размер ордера</param>
        private void ChangeMinSell(decimal newValue)
        {
            if (IsAmend)
            {
                foreach (OrderRESTWS orderRW
                    in ListOrderAmend
                    .Where(ord => ord.OrderWS != default 
                                    && ord.OrderWS.Side == SideEnum.Sell 
                                    && !"Filled Canceled ".Contains(ord.OrderWS.OrdStatus))
                    .ToList()
                    )
                {
                    decimal priceBest = PriceBest(orderRW.OrderWS.Side);
                    BitMEXOrder resultOrder;
                    if (priceBest != (decimal)orderRW.OrderWS.Price)
                        resultOrder = OrderAmend(orderRW.OrderWS.OrderID, priceBest);
                }
            }

        }


        /// <summary>Обработка изменения максимальной цены покупки</summary>
        /// <param name="newValue">Размер ордера</param>
        private void ChangeMaxBuy(decimal newValue)
        {
            if (IsAmend)
            {
                foreach (OrderRESTWS orderRW
                    in ListOrderAmend
                    .Where(ord => ord.OrderWS != default
                                    && ord.OrderWS.Side == SideEnum.Buy 
                                    && !"Filled Canceled ".Contains(ord.OrderWS.OrdStatus))
                    .ToList()
                    )
                {
                    decimal priceBest = PriceBest(orderRW.OrderWS.Side);
                    BitMEXOrder resultOrder;
                    if (priceBest != (decimal)orderRW.OrderWS.Price)
                        resultOrder = OrderAmend(orderRW.OrderWS.OrderID, priceBest);
                }
            }
        }


        private BitMEXOrder OrderAmend(string OrderID, decimal Price)
            => bitMexREST.LimitNowAmendOrder(OrderID, Price);

        /// <summary>Проверка позиции после изменений</summary>
        private void ChangedPositions()
        {
            // Если нет отложенной позиции, то выход
            if (DeferredPosition == null)
                return;

            lock (DeferredPosition)
            {
                // Выход, если не установлен символ в отложенной позиции
                if (string.IsNullOrWhiteSpace(DeferredPosition.Symbol))
                    return;

                // Если позиция ещё не нулевая, то выход
                if (GetPositionSize(DeferredPosition.Symbol) != 0)
                    return;

                if (CreateOrderAmend(DeferredPosition.Symbol, DeferredPosition.Quantity))
                    DeferredPosition.Symbol = null;
            }
        }


        protected override bool CanOrderAmend(object parameter)
            => ListOrderAmend != null && ListOrderAmend.Count > 0 && base.CanOrderAmend(parameter);

        /// <summary>Изменить цену по выставленному ордеру</summary>
        /// <param name="parameter"></param>
        protected override void OnOrderAmend(object parameter)
        {
            if (CanOrderAmend(parameter))
            {
                TableOrder oldOrder = ListOrderAmend.First().OrderWS;
                OrderAmend(oldOrder.OrderID, Price);
            }
            base.OnOrderAmend(parameter);
        }


    }
}
