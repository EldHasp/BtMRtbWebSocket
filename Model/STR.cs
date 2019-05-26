using CommLibrary;
using CommLibrary.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace BitMexLibrary
{

    public partial class STR : OnPropertyChangedClass
    {

        private string _linesOut;
        public string LinesOut { get => _linesOut; set { _linesOut = value; OnPropertyChanged(); } }

        public DGColumns OutValues { get => _outValues; private set { _outValues = value; OnPropertyChanged(); } }

        public Candle LastCandle { get => _lastCandle; set { _lastCandle = value; OnPropertyChanged(); } }
        /// <summary>Тип свечей для расчёта</summary>
        private  BinSizeEnum typeCandles;
        /// <summary>Количество свечей для расчёта</summary>
        private  int countCandlesForCalculate;
        /// <summary>Период расчёта</summary>
        private  TimeSpan timePeriod;

        public STR(string bitmexKey, string bitmexSecret, bool realWork, BinSizeEnum typeCandles, int countCandlesForCalculate)
        {
            bitMex = new BitMEXApi(bitmexKey, bitmexSecret, realWork);
            bitMex.PropertyChanged += BitMex_PropertyChanged;
            APIValid = GetAPIValidity(bitMex);

            RunWork(typeCandles,countCandlesForCalculate);
        }
        public STR(string bitmexKey, string bitmexSecret, bool realWork)
        {
            bitMex = new BitMEXApi(bitmexKey, bitmexSecret, realWork);
            bitMex.PropertyChanged += BitMex_PropertyChanged;
            APIValid = GetAPIValidity(bitMex);
        }

        public STR(BitMEXApi bitMEXApi)
        {
            bitMex = bitMEXApi;
            bitMex.PropertyChanged += BitMex_PropertyChanged;
            APIValid = GetAPIValidity(bitMex);
        }

        public void RunWork(BinSizeEnum typeCandles, int countCandlesForCalculate)
        {
            OutValues = new DGColumns();
            if ((int)typeCandles < 1)
                this.typeCandles = BinSizeEnum.Minute;
            else if ((int)typeCandles > 60)
                this.typeCandles = BinSizeEnum.Hour;
            else
                this.typeCandles = typeCandles;

            if (countCandlesForCalculate < 120)
                this.countCandlesForCalculate = 120;
            else
                this.countCandlesForCalculate = countCandlesForCalculate;

            timePeriod = TimeSpan.FromMinutes(this.countCandlesForCalculate * (int)this.typeCandles);

            IsReadyCalculate = true;
        }

        private void BitMex_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            string nameProp = e.PropertyName;
            if (string.IsNullOrEmpty(nameProp) || nameProp == "CountQuery")
                CountQuery = bitMex.CountQuery;
        }

        //public STR(string bitmexKey, string bitmexSecret, bool realWork, int rateLimit)
        //{
        //    BitMEXApi bitMex = new BitMEXApi(bitmexKey, bitmexSecret, realWork, rateLimit);
        //}

        readonly BitMEXApi bitMex;
        bool APIValid = false;
        private bool GetAPIValidity(BitMEXApi bitMexApi)
        {
            decimal WalletBalance = 0;
            try // Code is simple, if we get our account balance without an error the API is valid, if not, it will throw an error and API will be marked not valid.
            {

                WalletBalance = bitMexApi.GetAccountBalance();
                return WalletBalance >= 0;
            }
            catch (Exception /*ex*/)
            {
                return false;
            }
        }

        private DGColumns _outValues;
        private Candle _lastCandle;


        /// <summary>Расчёт EMA</summary>
        /// <param name="source">Список данных</param>
        /// <param name="EMAPeriod">"Длина канала"</param>
        /// <returns>Список значений ЕМА</returns>
        public static List<decimal> EMA(List<decimal?> source, int EMAPeriod, decimal? InitialValue = null)
        {
            if (source.IndexOf(null) >= 0)
                throw new ArgumentException("В данных есть null!", "source");
            if (EMAPeriod < 1)
                throw new ArgumentException("Период меньше единицы!", "EMAPeriod");
            if (source.Count < EMAPeriod * 2)
                throw new ArgumentException("Длина последовательности source должна быть более двойного периода EMAPeriod", "source");

            List<decimal> ret = new List<decimal>(); // Массив для выходных значений
            decimal emaPrev;// Расчёт начального значения
            if (InitialValue == null)
                emaPrev = (decimal)source.Take(EMAPeriod).Average();
            else
                emaPrev = (decimal)InitialValue;

            int p1 = EMAPeriod + 1;
            decimal EMAMultiplier = (2.0m / p1);

            foreach (decimal? value in source.Skip(InitialValue == null ? EMAPeriod : 0))
            {
                emaPrev = ((decimal)value - emaPrev) * EMAMultiplier + emaPrev;
                ret.Add(emaPrev);
            }

            return ret;
        }


        public static List<decimal> RSI(List<decimal?> GainOrLoss, int RSIPeriod)
        {
            if (GainOrLoss.IndexOf(null) >= 0)
                throw new ArgumentException("В данных есть null!", "GainOrLoss");
            if (RSIPeriod < 1)
                throw new ArgumentException("Период меньше единицы!", "RSIPeriod");
            if (GainOrLoss.Count < RSIPeriod * 2)
                throw new ArgumentException("Длина последовательности GainOrLoss должна быть более двойного периода RSIPeriod", "GainOrLoss");


            List<decimal> ret = new List<decimal>(); // Массив для выходных значений

            // Расчёт начального значения
            decimal LastAVGGain;
            decimal LastAVGLoss;

            int indexSkip = -1;
            int countPol = 0;
            int countOtr = 0;
            for (int index = 0; index <= GainOrLoss.Count; index++)
            {
                indexSkip = index;
                if (GainOrLoss[index] > 0) countPol++;
                if (GainOrLoss[index] < 0) countOtr++;
                if (countPol >= RSIPeriod && countOtr >= RSIPeriod)
                    break;
            }

            if (indexSkip > GainOrLoss.Count - 2 * RSIPeriod)
                return ret;

            LastAVGGain = (decimal)GainOrLoss.Take(indexSkip + 1).Reverse().Where(a => a > 0).Take(RSIPeriod).Where(a => a > 0).Average();
            LastAVGLoss = -(decimal)GainOrLoss.Take(indexSkip + 1).Reverse().Where(a => a < 0).Take(RSIPeriod).Average();
            /*plot(rsi(close, 7))

                // same on pine, but less efficient
                pine_rsi(x, y) => 
                    u = max(x - x[1], 0) // upward change
                    d = max(x[1] - x, 0) // downward change
                    rs = rma(u, y) / rma(d, y)
                    res = 100 - 100 / (1 + rs)
                    res

                plot(pine_rsi(close, 7))


                            plot(rma(close, 15))

                // same on pine, but much less efficient
                pine_rma(x, y) =>
	                alpha = y
                    sum = 0.0
                    sum := (x + (alpha - 1) * nz(sum[1])) / alpha
                plot(pine_rma(close, 15))
                             */

            foreach (decimal value in GainOrLoss.Skip(indexSkip + 2))
            {

                decimal Gain = 0;
                decimal Loss = 0;

                if (value > 0)
                {
                    Gain = value;
                }
                else if (value < 0)
                {
                    Loss = -value;
                }

                LastAVGGain = (((LastAVGGain * (RSIPeriod - 1)) + Gain) / RSIPeriod);
                LastAVGLoss = (((LastAVGLoss * (RSIPeriod - 1)) + Loss) / RSIPeriod);


                decimal _RSI = (100m - (100m / (1m + (LastAVGGain / LastAVGLoss))));

                ret.Add(_RSI);
            }

            return ret;
        }


        /// <summary>Скользящая сумма</summary>
        /// <param name="source"></param>
        /// <param name="SumPeriod"></param>
        /// <returns></returns>
        public static List<decimal> SumSliding(List<decimal?> source, int SumPeriod)
        {
            if (source.IndexOf(null) >= 0)
                throw new ArgumentException("В данных есть null!", "source");
            if (SumPeriod < 1)
                throw new ArgumentException("Период меньше единицы!", "EMAPeriod");
            if (source.Count < SumPeriod * 2)
                throw new ArgumentException("Длина последовательности source должна быть более двойного периода EMAPeriod", "source");

            List<decimal> ret = new List<decimal>();

            // Подсчёт первой суммы

            Queue<decimal> lastElem = new Queue<decimal>(source.Take(SumPeriod).Select(a => (decimal)a).ToList());

            ret.Add(lastElem.Sum());

            foreach (decimal value in source.Skip(SumPeriod))
            {
                lastElem.Enqueue(value);
                lastElem.Dequeue();
                ret.Add(lastElem.Sum());
            }

            return ret;
        }


    }

    public static class ЕxtensionMetods
    {
        /// <summary>Метод раширения разрезающий последовательность на куски</summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IEnumerable<IEnumerable<T>> Crushing<T>(this IEnumerable<T> sourse, int length)
        {
            if (length < 2)
                return null;

            return
                Enumerable.Range(0, sourse.Count() / length)
                .Select(num =>
                        sourse.Skip(num * length).Take(length));
        }
    }


}
