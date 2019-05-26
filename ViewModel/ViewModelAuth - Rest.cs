using BitMexLibrary;

namespace ViewModel
{
    public partial class ViewModelAuth
    {

        private RESTBitMexSigned BitMexREST { get; set; }

        private void BitMexREST_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            string nameProperty = e.PropertyName;
            if (IsNameProperty("ValidRest"))
                ValidRest=BitMexREST.ValidRest;
            if (IsNameProperty("AccountBalance"))
                BalanceRest = BitMexREST.AccountBalance;

            bool IsNameProperty(string NameProperty)
                => string.IsNullOrWhiteSpace(nameProperty) || nameProperty.Trim() == NameProperty.Trim();
        }

    }
}
