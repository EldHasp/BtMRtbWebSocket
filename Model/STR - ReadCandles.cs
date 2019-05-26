using CommLibrary.Enums;
using System;
using System.Linq;

namespace BitMexLibrary
{

    public partial class STR
    {

        /// <summary>Поле для хранения свечей</summary>
        Candles allCandles;
        //string fileNameCandles = "Candles.json";
        /// <summary>Флаг получения новых свечей</summary>
        bool readNewCandle = false;
        private DateTime? _finishCalculationTime;

        /// <summary>Время окончания расчётов, если оно задано
        /// Если оно <see langword="null"/>, то берётся реальное время</summary>
        public DateTime? FinishCalculationTime { get => _finishCalculationTime; private set { SetProperty(ref _finishCalculationTime, value); } }

        /// <summary>Установка времени окончания расчётов</summary>
        /// <param name="finishTime">Новое значение времени. 
        /// Если оно больше реального, то установка <see langword="null"/>.
        /// <see langword="null"/> - реальное время</param>
        public void SetFinishCalculationTime(DateTime? finishTime) => FinishCalculationTime = finishTime <= BitMEXApi.RealTime ? finishTime : null;

        /// <summary>Получение времени окончания расчётов.
        /// Если оно больше реального или <see langword="null"/>, то берётся реальное время</summary>
        public DateTime GetFinishCalculationTime()
            => (FinishCalculationTime == null || FinishCalculationTime >= BitMEXApi.RealTime) ? BitMEXApi.RealTime : FinishCalculationTime.Value;

        ///// <summary>Чтение часовых свечей</summary>
        ///// <param name="timeStart">С какого времени считывать</param>
        ///// <returns>Candles с часовыми свечами</returns>
        //Candles ReadCandlesHour()
        //{
        //    readNewCandle = true;
        //    Candles candles = bitMex.GetCandleHistory("XBTUSD", countCandlesForCalculate, typeCandles);
        //    return candles;
        //}


        /// <summary>Чтение часовых свечей </summary>
        /// <param name="timeStart">С какого времени считывать</param>
        /// <returns>Candles с часовыми свечами</returns>
        Candles ReadCandlesHour(DateTime timeStart, DateTime timeEnd)
        {
            readNewCandle = true;
            Candles candles = bitMex.GetCandleHistory("XBTUSD", 720, typeCandles, startTime: timeStart, endTime: timeEnd);
            return candles;
        }

        /// <summary>Чтение одноминутнух свечей и их перевод в шестиминутные</summary>
        /// <param name="timeStart">С какого времени считывать</param>
        /// <returns>Candles с сформированными шестиминутными свечами</returns>
        Candles ReadCandlesSix(DateTime timeStart)
        {
            readNewCandle = true;
            Candles candles = bitMex.GetCandleHistory("XBTUSD", 720, BinSizeEnum.Minute, startTime: timeStart);
            var кккк = candles.Last().TimeStamp;
            Candles ret = new Candles();
            int countMinut = (int)BinSizeEnum.SixMinutes;
            int skipMinut = (countMinut + 1 - (candles.First().TimeStamp.Minute % countMinut)) % countMinut;
            int candleCount = (candles.Count() - skipMinut) / countMinut;
            ret.AddRange
                (
                    candles.Skip(skipMinut).SplitSegments(countMinut, Enums.SplitSegmentsOptions.Step).Select
                        (
                            cnds => new Candle()
                            {
                                Open = cnds.First().Open,
                                Close = cnds.Last().Close,
                                TimeStamp = cnds.Last().TimeStamp,
                                High = cnds.Max(cn => cn.High),
                                Low = cnds.Min(cn => cn.Low),
                                Volume = cnds.Sum(cn => cn.Volume),
                                ID = cnds.First().ID,

                            }
                        ).ToList()
                );
            ret.BinSize = BinSizeEnum.SixMinutes;
            return ret;
        }

        public void ReadCandle()
        {
            // Сброс флага чтения новых свечей
            readNewCandle = false;

            DateTime calcTime = GetFinishCalculationTime();

            // Проверка времени после чтения последней свечи
            if (allCandles != null)
            {
                if (
                    allCandles.Count == 0
                    || (calcTime - allCandles.Last().TimeStamp) > timePeriod
                    || (calcTime - allCandles.First().TimeOpen) < timePeriod
                    )
                    allCandles = null;
            }

            if (allCandles == null) // Если накопленных данных нет, то новое чтение
                allCandles = ReadCandlesHour(calcTime.AddSeconds(1) - timePeriod, calcTime);

            // Чтение и проверка последней свечи на бирже
            Candle lastCandleBitMex;
            while ( // Цикл если выполняется условие
                (lastCandleBitMex = bitMex.GetCandleLast("XBTUSD", BinSizeEnum.Hour, endTime: calcTime)) != null  // Последняя свеча прочитана
                && (lastCandleBitMex.TimeStamp - allCandles.Last().TimeStamp) > new TimeSpan(0, (int)typeCandles, -1)) // Время последней свечи отличается больше чем на период свечи
            {
                Candles newCandles = ReadCandlesHour(allCandles.Last().TimeStamp.AddSeconds(1), calcTime);
                newCandles.RemoveAll(candle => allCandles.FindIndex(cand => cand.TimeStamp == candle.TimeStamp) >= 0);
                allCandles.AddRange(newCandles);
            }

            if (allCandles.Count > countCandlesForCalculate)
                allCandles.RemoveRange(0, allCandles.Count - countCandlesForCalculate);

            //File.WriteAllText(fileNameCandles, JsonConvert.SerializeObject(candles.GetRange(candles.Count - 720, 720)));
            //allCandles.Remove(allCandles.Last());

            if (readNewCandle)
                BestStrategy(countCandlesForCalculate);
            LastCandle = allCandles.Last();
        }

    }


}
