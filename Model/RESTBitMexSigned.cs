using BitMexLibrary.Enums;
using CommLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace BitMexLibrary
{
    public class RESTBitMexSigned : OnPropertyChangedClass
    {
        BitMEXApi bitMEXApi;
        private bool? _validRest;
        private decimal? _accountBalance;

        /// <summary>Состояние соединения с аккаунтом</summary>
        public bool? ValidRest { get => _validRest; set { SetProperty(ref _validRest, value); } }

        public decimal? AccountBalance { get => _accountBalance; set { SetProperty(ref _accountBalance, value); } }

        public RESTBitMexSigned(string bitmexKey, string bitmexSecret, bool RealWork, int? rateLimit = null)
        {
            if (rateLimit == null)
                bitMEXApi = new BitMEXApi(bitmexKey, bitmexSecret, RealWork);
            else
                bitMEXApi = new BitMEXApi(bitmexKey, bitmexSecret, RealWork, rateLimit.Value);
            AccountBalance = bitMEXApi.GetAccountBalance();
        }

        protected override void SetProperty<T>(ref T fieldProperty, T newValue, [CallerMemberName] string nameProperty = "")
        {
            if (nameProperty == "AccountBalance")
            {
                decimal? newVal = newValue as decimal?;
                if (newVal == null)
                    ValidRest = null;
                else
                {
                    if (newVal.Value < 0)
                    {
                        ValidRest = false;
                        newValue = default;
                    }
                    else
                        ValidRest = true;
                }
            }

            base.SetProperty(ref fieldProperty, newValue, nameProperty);
        }

        protected override void PropertyNewValue<T>(ref T fieldProperty, T newValue, string nameProperty)
        {
            base.PropertyNewValue(ref fieldProperty, newValue, nameProperty);
        }

        public BitMEXOrder LimitNowOrder(string Symbol, SideEnum hand, int Quantity, decimal Price/*, bool ReduceOnly = false, bool PostOnly = false, bool Hidden = false*/)
            => bitMEXApi.LimitNowOrder(Symbol, hand, Quantity, Price);

        public BitMEXOrder LimitNowAmendOrder(string OrderId, decimal? Price = null, int? OrderQty = null)
            => bitMEXApi.LimitNowAmendOrder(OrderId, Price, OrderQty);
        public BitMEXApi GetBitMEXApi() => bitMEXApi;
    }
}
