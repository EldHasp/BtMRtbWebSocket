using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace CommLibrary
{
    public delegate void SPropertyChangedEventHandler(Type type, PropertyChangedEventArgs e);

    /// <summary>Базовый класс с реализацией INPC </summary>
    public class OnPropertyChangedClass : INotifyPropertyChanged
    {

        #region Событие PropertyChanged
        /// <summary>Событие для извещения об изменения свойства</summary>
        public event PropertyChangedEventHandler PropertyChanged;
        //private PropertyChangedEventHandler _propertyChanged;
        //public event PropertyChangedEventHandler PropertyChanged
        //{
        //    add
        //    {
        //        if (_propertyChanged == null)
        //            _propertyChanged += value;
        //        else
        //            lock (_propertyChanged)
        //                _propertyChanged += value;
        //        value(this, new PropertyChangedEventArgs(null));
        //    }
        //    remove { lock (_propertyChanged) { _propertyChanged -= value; } }
        //}
        #endregion

        #region Методы вызова события PropertyChanged
        /// <summary>Метод для вызова события извещения об изменении свойства</summary>
        /// <param name="prop">Изменившееся свойство</param>
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            if (string.IsNullOrEmpty(prop))
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null));
                return;
            }
            string[] names = prop.Split("\\/\r \n()\"\'-".ToArray(), StringSplitOptions.RemoveEmptyEntries);
            switch (names.Length)
            {
                case 0: break;
                case 1:
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
                    break;
                default:
                    OnPropertyChanged(names);
                    break;
            }

        }

        /// <summary>Метод для вызова события извещения об изменении списка свойств</summary>
        /// <param name="propList">Список имён свойств</param>
        public void OnPropertyChanged(IEnumerable<string> propList)
        {
            foreach (string prop in propList.Where(name => !string.IsNullOrWhiteSpace(name)))
                OnPropertyChanged(prop);
        }

        /// <summary>Метод для вызова события извещения об изменении всех свойств</summary>
        /// <param name="propList">Список свойств</param>
        public void OnAllPropertyChanged()
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null));

        #endregion


        /// <summary>Виртуальный метод определяющий изменения в значении поля значения свойства</summary>
        /// <param name="fieldProperty">Ссылка на поле значения свойства</param>
        /// <param name="newValue">Новое значение</param>
        /// <param name="nameProperty">Название свойства</param>
        protected virtual void SetProperty<T>(ref T fieldProperty, T newValue, [CallerMemberName]string nameProperty = "")
        {
            if ((fieldProperty != null && !fieldProperty.Equals(newValue)) || (fieldProperty == null && newValue != null))
                PropertyNewValue(ref fieldProperty, newValue, nameProperty);
        }

        /// <summary>Виртуальный метод изменяющий значение поля значения свойства</summary>
        /// <param name="fieldProperty">Ссылка на поле значения свойства</param>
        /// <param name="newValue">Новое значение</param>
        /// <param name="nameProperty">Название свойства</param>
        protected virtual void PropertyNewValue<T>(ref T fieldProperty, T newValue, string nameProperty)
        {
            fieldProperty = newValue;
            OnPropertyChanged(nameProperty);
        }
    }
}
