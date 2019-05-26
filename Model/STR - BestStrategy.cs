using BitMexLibrary.Enums;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace BitMexLibrary
{

    public partial class STR
    {
        readonly double? na = null;

        public void BestStrategy(int countCandles = 720)
        {
            DateTime? lastTimeStamp = LastCandle?.TimeStamp;


            if (lastTimeStamp != null)
            {
                int countNotCalc = allCandles.Count(cand => cand.TimeStamp > lastTimeStamp);
                if (countNotCalc + 150 < countCandles)
                    countCandles = countNotCalc + 150;
            }

            List<Candle> candlesCalc;
            int begInd = allCandles.Count - countCandles;
            if (begInd < 0)
                begInd = 0;
            if (begInd < allCandles.Count)
                candlesCalc = allCandles.GetRange(begInd, allCandles.Count - begInd);
            else
                candlesCalc = new List<Candle>();

            Pine close = candlesCalc.Close().ToPine();
            Pine volume = candlesCalc.Volume().ToPine();
            Pine high = candlesCalc.High().ToPine();
            Pine low = candlesCalc.Low().ToPine();
            Pine hlc3 = (high + low + close) / 3.0;
            IEnumerable<DateTime> timeStamp = candlesCalc.TimeStamp();
            IEnumerable<DateTime> timeOpen = candlesCalc.TimeOpen();

            OutValues.AddRowHeaders(timeOpen);
            OutValues.Add("Close", close);

            Pine change = Change(close);

            Pine fast_ma = Sma(low, 16);
            Pine slow_ma = Sma(low, 29);
            Pine macd = fast_ma - slow_ma;
            Pine signal = Ema(macd, 9);
            Pine hist = macd - signal;

            Pine hist_2 = hist.Drop(2);
            Pine hist_3 = hist.Drop(3);
            Pine close_2 = close.Drop(2);

            PineBool LOL = hist_3 < 3 & hist_3 > -3 & hist_2 < 3 & hist_2 > -3;
            PineBool longCond = hist > 0 & (close - close_2) < 66 & !LOL;
            PineBool shortCond = hist < 0 & (close_2 - close) < 66 & !LOL;


            OutValues.Add("hist_3", hist_3);
            OutValues.Add("hist", hist);
            OutValues.Add("LOL", LOL);

            OutValues.Add("longCond", longCond);
            OutValues.Add("shortCond", shortCond);

            Pine longShortCond = PineBool.ZipEnd(longCond, shortCond, (ln, sh) => ln ? 1.0 : sh ? -1.0 : na).ToPineNA().ToPine(Approximation.Step);

            var handCond = Pine.ZipEnd(longShortCond, longShortCond.Drop(1), (lsc, lscd) => (lsc > 0.5 && lscd < -0.5) ? "Long" : (lsc < 0.5 && lscd > -0.5) ? "Short" : "");

            OutValues.Add("Сигнал", handCond);


            if (longShortCond.Last() == 1.0 && longShortCond.Drop(1).Last() == -1.0)
                OnSignal(SignalEnum.Long);
            if (longShortCond.Last() == -1.0 && longShortCond.Drop(1).Last() == 1.0)
                OnSignal(SignalEnum.Short);

        }
    }
}
