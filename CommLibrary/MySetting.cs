using CommLibrary.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace CommLibrary
{
    public static class MySetting
    {
        public static SettingsClass Settings { get => s_settings ?? (Settings = Load()); set { s_settings = value; OnPropertyChanged(); } }

        #region Событие PropertyChanged
        /// <summary>Событие для извещения об изменения свойства</summary>
        private static PropertyChangedEventHandler _propertyChanged;
        public static event PropertyChangedEventHandler PropertyChanged
        {
            add
            {
                if (_propertyChanged == null)
                    _propertyChanged += value;
                else
                    lock (_propertyChanged)
                        _propertyChanged += value;
                value(typeof(SettingsClass), new PropertyChangedEventArgs(null));
            }
            remove { lock (_propertyChanged) { _propertyChanged -= value; } }
        }

        /// <summary>Метод для вызова события извещения об изменении свойства</summary>
        /// <param name="prop">Изменившееся свойство</param>
        public static void OnPropertyChanged([CallerMemberName]string prop = "")
            => _propertyChanged?.Invoke(typeof(MySetting), new PropertyChangedEventArgs(prop));

        #endregion


        //public static event EventHandler ChangeSettings;
        private static SettingsClass s_settings;

        public static SettingsClass Load() => Load(filename);

        public static SettingsClass Load(string filename)
        {
            try
            {
                using (FileStream fs = new FileStream(filename, FileMode.Open))
                {
                    XmlSerializer x = new XmlSerializer(typeof(SettingsClass));
                    Settings = (SettingsClass)x.Deserialize(fs);
                    fs.Close();
                    MySetting.filename = filename;
                }
                return Settings;
            }
            catch (Exception)
            {
                return new SettingsClass();
            }
        }

        static string filename = "Settings.xml";

        public static void Save() => Save(filename);

        public static void Save(string filename)
        {
            XmlSerializer s = new XmlSerializer(typeof(SettingsClass));
            using (TextWriter writer = new StreamWriter(filename))
            {
                s.Serialize(writer, Settings);
                writer.Close();
            }
        }
    }

    [Serializable]
    public class SettingsClass : OnPropertyChangedClass
    {
        private string _aPIKey = new string('*', 50);
        private string _aPISecret = new string('*', 50);
        private bool _realWork = false;
        private int _countCandelesForCalculate = 200;
        private BinSizeEnum _typeCandles = BinSizeEnum.Hour;

        public string APIKey { get => _aPIKey; set { SetProperty(ref _aPIKey, value); } }
        public string APISecret { get => _aPISecret; set { SetProperty(ref _aPISecret, value); } }
        public bool RealWork { get => _realWork; set { SetProperty(ref _realWork, value); } }
        public int CountCandelesForCalculate
        {
            get => _countCandelesForCalculate;
            set
            {
                if (value < 120)
                    value = 120;
                SetProperty(ref _countCandelesForCalculate, value);
            }
        }
        public BinSizeEnum TypeCandles { get => _typeCandles; set { SetProperty(ref _typeCandles, value); } }
    }
}
