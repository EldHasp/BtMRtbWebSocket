using System.Collections.Generic;
using System.Linq;
using TA = BitMexLibrary.TradingAlgos;

namespace BitMexLibrary
{

    public partial class STR
    {
        Pine TCI(Pine pine)
        {
            //Pine emaSrc35, src_emaSrc35, abs_src_emaSrc35, emaAbs, comm, ret;
            Pine
            emaSrc35 = TA.Ema(pine, 35);
            Pine src_emaSrc35 = pine - emaSrc35;
            Pine abs_src_emaSrc35 = TA.Abs(src_emaSrc35);
            Pine emaAbs = TA.Ema(abs_src_emaSrc35, 35);
            Pine comm = src_emaSrc35 / (0.025 * emaAbs);
            Pine ret = TA.Ema(comm, 5) + 50;
            return ret;
        }
        Pine MF(Pine pine, Pine pVolume)
        {
            var pChange = TA.Change(pine);
            var chnM = TA.ZipEnd(pChange, pine, (ch, cl) => ch <= 0 ? 0 : cl).ToPine();
            var sumChnM = TA.Sum(pVolume * chnM, 3);

            var chnL = TA.ZipEnd(pChange, pine, (ch, cl) => ch >= 0 ? 0 : cl).ToPine();
            var sumChnL = TA.Sum(pVolume * chnL, 3);

            //mf(src) => rsi(sum(volume * (change(src) <= 0 ? 0 : src), 3), sum(volume * (change(src) >= 0 ? 0 : src), 3))

            return TA.RSI(sumChnM, sumChnL);

            //var result = TradingAlgos.Alroritm(candles6, 35, 5, 3);
        }

        Pine Willy(Pine pine)
        {
            var highest = TA.Highest(pine, 5);
            var lowest = TA.Lowest(pine, 5);
            return 60.0 * (pine - highest) / (highest - lowest) + 80;
        }
        Pine Tradition(Pine pine, Pine pVolume)
        {

            var tci = TCI(pine);
            var mf = MF(pine, pVolume);
            var rsi = TA.RSI(pine, 3);

            OutValues.Add("tci", tci);
            OutValues.Add("mf", mf);
            OutValues.Add("rsi", rsi);
            return Pine.ZipEnd(tci, mf, rsi, (t, m, r) => (t + r + m) / 3.0);
        }

        Pine f_fractalize(Pine pine)
        {
            double minMaxFract(IEnumerable<double> segm)
            {
                int index = segm.Count() / 2;
                double elm = segm.ElementAt(index);
                IEnumerable<double> items = segm.Take(index).Concat(segm.Skip(index + 1));

                if (items.Max() < elm) return 1.0;
                if (items.Min() > elm) return -1.0;
                return 0.0;
            }

            return pine.SplitSegments(5).Select(x => minMaxFract(x)).ToPine();
        }

        #region Переименование TA функций
        Pine Change(Pine pine) => TA.Change(pine);
        Pine Sma(Pine pine, int period) => TA.Sma(pine, period);
        Pine Linreg(Pine source, int period, int offset) => TA.Linreg(source, period, offset);
        Pine Ema(Pine source, int period) => TA.Ema(source, period);
        Pine ValueWhen(PineNA condition, Pine source, uint occurrence) => TA.ValueWhen(condition, source, occurrence).ToPine(Enums.Approximation.Step);
        Pine BarsSince(PineBool source) => TA.BarsSince(source);
        PineBool CrossUnder(Pine xSource, Pine ySource) => TA.CrossUnder(xSource, ySource);
        PineBool CrossOver(Pine xSource, Pine ySource) => TA.CrossOver(xSource, ySource);
        #endregion

        #region Переименование TA функций

        #endregion

        //int countAdd = -1;

        //void plot<T>(string Header, IEnumerable<T> Items)
        //{
        //    Header = Header.Trim();
        //    if (Header == "Время" && OutValues.ColumnHeaders.Contains("Время"))
        //    {
        //        var timeCol = OutValues.Columns["Время"];
        //        countAdd = timeCol.Items.Count(x => DateTime.Parse(x,"t"))
        //    }
        //}
        //OutValues.Add("Close", close);
    }
}
