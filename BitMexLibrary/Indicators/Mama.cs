using System;
using System.Collections.Generic;
using System.Linq;
using BitMexLibrary;

namespace MachinaTrader.Indicators
{
    public static partial class Extensions
    {
        public static MamaItem Mama(this List<Candle> source, double fastLimit = 0, double slowLimit = 0)
        {
            // int outBegIdx, outNbElement;
            double[] mamaValues = new double[source.Count];
            double[] famaValues = new double[source.Count];
            var closes = source.Select(x => Convert.ToDouble(x.Close)).ToArray();

            var mfi = TicTacTec.TA.Library.Core.Mama(0, source.Count - 1, closes,
                fastLimit, slowLimit, out int outBegIdx, out int outNbElement, mamaValues, famaValues);

            if (mfi == TicTacTec.TA.Library.Core.RetCode.Success)
            {
                return new MamaItem
                {
                    Mama = FixIndicatorOrdering(mamaValues.ToList(), outBegIdx, outNbElement),
                    Fama = FixIndicatorOrdering(famaValues.ToList(), outBegIdx, outNbElement)
                };
            }

            throw new Exception("Could not calculate MAMA!");
        }
    }

    public class MamaItem
    {
        public List<decimal?> Mama { get; set; }
        public List<decimal?> Fama { get; set; }
    }
}
