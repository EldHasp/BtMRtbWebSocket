﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitMexLibrary
{
    public class PineBool : List<bool>
    {
        //#region переопределение сложения с PineBool с числом
        //public static PineBool operator &(PineBool pine1, PineBool pine2) => ZipEnd(pine1, pine2, (p1, p2) => p1 & p2);
        //public static Pine operator +(double number, Pine pine) => pine + number;
        //public static Pine operator +(decimal number, Pine pine) => pine + (double)number;
        //public static Pine operator +(Pine pine, decimal number) => pine + (double)number;
        //public static Pine operator +(int number, Pine pine) => pine + (double)number;
        //public static Pine operator +(Pine pine, int number) => pine + (double)number;
        //#endregion

        //#region переопределение умножения с Pine с числом
        //public static Pine operator *(Pine pine, double number)
        //{
        //    Pine ret = new Pine();
        //    foreach (double num in pine)
        //        ret.Add(num * number);
        //    return ret;
        //}
        //public static Pine operator *(double number, Pine pine) => pine * number;
        //public static Pine operator *(decimal number, Pine pine) => pine * (double)number;
        //public static Pine operator *(Pine pine, decimal number) => pine * (double)number;
        //public static Pine operator *(int number, Pine pine) => pine * (double)number;
        //public static Pine operator *(Pine pine, int number) => pine * (double)number;
        //#endregion

        //#region переопределение вычитания с Pine с числом
        //public static Pine operator -(Pine pine, double number)
        //{
        //    Pine ret = new Pine();
        //    foreach (double num in pine)
        //        ret.Add(num - number);
        //    return ret;
        //}

        //public static Pine operator -(double number, Pine pine)
        //{
        //    Pine ret = new Pine();
        //    foreach (double num in pine)
        //        ret.Add(number - num);
        //    return ret;
        //}

        //public static Pine operator -(decimal number, Pine pine) => (double)number - pine;
        //public static Pine operator -(Pine pine, decimal number) => pine - (double)number;
        //public static Pine operator -(int number, Pine pine) => (double)number - pine;
        //public static Pine operator -(Pine pine, int number) => pine - (double)number;
        //#endregion

        //#region переопределение деления с Pine с числом
        //public static Pine operator /(Pine pine, double number)
        //{
        //    Pine ret = new Pine();
        //    foreach (double num in pine)
        //        ret.Add(num / number);
        //    return ret;
        //}
        //public static Pine operator /(double number, Pine pine)
        //{
        //    Pine ret = new Pine();
        //    foreach (double num in pine)
        //        ret.Add(number / num);
        //    return ret;
        //}

        //public static Pine operator /(Pine pine, decimal number) => pine / (double)number;
        //public static Pine operator /(decimal number, Pine pine) => (double)number / pine;
        //public static Pine operator /(Pine pine, int number) => pine / (double)number;
        //public static Pine operator /(int number, Pine pine) => (double)number / pine;
        //#endregion

        #region Переопределение логических операторов с PineBool
        public static PineBool operator &(PineBool pine1, PineBool pine2) => ZipEnd(pine1, pine2, (p1, p2) => p1 & p2);
        public static PineBool operator |(PineBool pine1, PineBool pine2) => ZipEnd(pine1, pine2, (p1, p2) => p1 | p2);
        public static PineBool operator !(PineBool pine) => pine.Select(p => !p).ToPineBool();

        public static PineBool operator &(PineBool pine1, PineNA pine2) => pine1 & pine2.ToPineBool();
        public static PineBool operator &(PineNA pine2, PineBool pine1) => pine1 & pine2.ToPineBool();
        public static PineBool operator |(PineBool pine1, PineNA pine2) => pine1 | pine2.ToPineBool();
        public static PineBool operator |(PineNA pine2, PineBool pine1) => pine1 | pine2.ToPineBool();
        #endregion

        #region Методы соединения Pine с выравниванием по последнему элементу
        //public static Pine ZipEnd(Pine first, Pine second, Pine third, Func<double, double, double, double> result)
        //{
        //    var fR = ((IEnumerable<double>)first).Reverse();
        //    var sR = ((IEnumerable<double>)second).Reverse();
        //    var tR = ((IEnumerable<double>)third).Reverse();

        //    var fsr = fR.Zip(sR, (f, s) => (f, s)).Zip(tR, (fs, t) => (fs.f, fs.s, t));

        //    return fsr.Reverse().Select(x => result(x.f, x.s, x.t)).ToPine();
        //}

        //public static Pine ZipEnd(Pine first, Pine second, Pine third, Pine fourth, Func<double, double, double, double, double> result)
        //{
        //    var fR = ((IEnumerable<double>)first).Reverse();
        //    var sR = ((IEnumerable<double>)second).Reverse();
        //    var tR = ((IEnumerable<double>)third).Reverse();
        //    var fH = ((IEnumerable<double>)fourth).Reverse();


        //    var fs = fR.Zip(sR, (f, s) => (s1: f, s2: s));
        //    var tf = tR.Zip(fH, (t, f4) => (s3: t, s4: f4));

        //    var fsrf = fs.Zip(tf, (f, s) => (f.s1, f.s2, s.s3, s.s4));

        //    return fsrf.Reverse().Select(x => result(x.s1, x.s2, x.s3, x.s4)).ToPine();
        //}
        //public static Pine ZipEnd(
        //    Pine first, Pine second, Pine third, Pine fourth,
        //    Pine fifth, Pine sixth, Pine seventh, Pine eighth,
        //    Func<double, double, double, double, double, double, double, double, double> result)
        //{
        //    var fR = ((IEnumerable<double>)first).Reverse();
        //    var sR = ((IEnumerable<double>)second).Reverse();
        //    var tR = ((IEnumerable<double>)third).Reverse();
        //    var fH = ((IEnumerable<double>)fourth).Reverse();
        //    var fF = ((IEnumerable<double>)fifth).Reverse();
        //    var sX = ((IEnumerable<double>)sixth).Reverse();
        //    var sV = ((IEnumerable<double>)seventh).Reverse();
        //    var eG = ((IEnumerable<double>)eighth).Reverse();


        //    var fs = fR.Zip(sR, (s1, s2) => (s1, s2));
        //    var tf = tR.Zip(fH, (s3, s4) => (s3, s4));
        //    var f6 = fF.Zip(sX, (s5, s6) => (s5, s6));
        //    var sE = sV.Zip(eG, (s7, s8) => (s7, s8));

        //    var fstf = fs.Zip(tf, (f, s) => (f.s1, f.s2, s.s3, s.s4));
        //    var fsse = f6.Zip(sE, (f, s) => (f.s5, f.s6, s.s7, s.s8));

        //    var ret = fstf.Zip(fsse, (f, ff) => (f.s1, f.s2, f.s3, f.s4, ff.s5, ff.s6, ff.s7, ff.s8));

        //    return ret.Reverse().Select(x => result(x.s1, x.s2, x.s3, x.s4, x.s5, x.s6, x.s7, x.s8)).ToPine();
        //}
        //public static Pine ZipEnd(Pine first, Pine second, Pine third, Pine fourth, Pine fifth, Pine sixth, Pine seventh,
        //    Func<double, double, double, double, double, double, double, double> result)
        //{
        //    var fR = ((IEnumerable<double>)first).Reverse();
        //    var sR = ((IEnumerable<double>)second).Reverse();
        //    var tR = ((IEnumerable<double>)third).Reverse();
        //    var fH = ((IEnumerable<double>)fourth).Reverse();
        //    var fF = ((IEnumerable<double>)fifth).Reverse();
        //    var sX = ((IEnumerable<double>)sixth).Reverse();
        //    var sV = ((IEnumerable<double>)seventh).Reverse();


        //    var fs = fR.Zip(sR, (s1, s2) => (s1, s2));
        //    var tf = tR.Zip(fH, (s3, s4) => (s3, s4));
        //    var f6 = fF.Zip(sX, (s5, s6) => (s5, s6)).Zip(sV, (f, s7) => (f.s5, f.s6, s7));

        //    var fsrf = fs.Zip(tf, (f, s) => (f.s1, f.s2, s.s3, s.s4)).Zip(f6, (f, ff) => (f.s1, f.s2, f.s3, f.s4, ff.s5, ff.s6, ff.s7));

        //    return fsrf.Reverse().Select(x => result(x.s1, x.s2, x.s3, x.s4, x.s5, x.s6, x.s7)).ToPine();
        //}
        //public static Pine ZipEnd<T>(Pine first, Pine second, Pine third, Pine fourth, Pine fifth, Pine sixth,
        //    Func<double, double, double, double, double, double, double> result)
        //{
        //    var fR = ((IEnumerable<double>)first).Reverse();
        //    var sR = ((IEnumerable<double>)second).Reverse();
        //    var tR = ((IEnumerable<double>)third).Reverse();
        //    var fH = ((IEnumerable<double>)fourth).Reverse();
        //    var fF = ((IEnumerable<double>)fifth).Reverse();
        //    var sX = ((IEnumerable<double>)sixth).Reverse();


        //    var fs = fR.Zip(sR, (f, s) => (s1: f, s2: s));
        //    var tf = tR.Zip(fH, (t, f4) => (s3: t, s4: f4));
        //    var f6 = fF.Zip(sX, (f5, s6) => (s5: f5, s6));

        //    var fsrf = fs.Zip(tf, (f, s) => (f.s1, f.s2, s.s3, s.s4)).Zip(f6, (f, ff) => (f.s1, f.s2, f.s3, f.s4, ff.s5, ff.s6));

        //    return fsrf.Reverse().Select(x => result(x.s1, x.s2, x.s3, x.s4, x.s5, x.s6)).ToPine();
        //}

        //public static Pine ZipEnd(Pine first, Pine second, Pine third, Pine fourth, Pine fifth, Func<double, double, double, double, double, double> result)
        //{
        //    var fR = ((IEnumerable<double>)first).Reverse();
        //    var sR = ((IEnumerable<double>)second).Reverse();
        //    var tR = ((IEnumerable<double>)third).Reverse();
        //    var fH = ((IEnumerable<double>)fourth).Reverse();
        //    var fF = ((IEnumerable<double>)fifth).Reverse();


        //    var fs = fR.Zip(sR, (f, s) => (s1: f, s2: s));
        //    var tf = tR.Zip(fH, (t, f4) => (s3: t, s4: f4));

        //    var fsrff = fs.Zip(tf, (f, s) => (f.s1, f.s2, s.s3, s.s4)).Zip(fF, (f, ff) => (f.s1, f.s2, f.s3, f.s4, s5: ff));

        //    return fsrff.Reverse().Select(x => result(x.s1, x.s2, x.s3, x.s4, x.s5)).ToPine();
        //}

        public static PineBool ZipEnd(PineBool first, PineBool second, Func<bool, bool, bool> result)
        {
            var fR = ((IEnumerable<bool>)first).Reverse();
            var sR = ((IEnumerable<bool>)second).Reverse();

            var fs = fR.Zip(sR, (f, s) => (f, s));

            return fs.Reverse().Select(x => result(x.f, x.s)).ToPineBool();
        }
        public static IEnumerable<T> ZipEnd<T>(PineBool first, PineBool second, Func<bool, bool, T> result)
        {
            var fR = ((IEnumerable<bool>)first).Reverse();
            var sR = ((IEnumerable<bool>)second).Reverse();

            var fs = fR.Zip(sR, (f, s) => (f, s));

            return fs.Reverse().Select(x => result(x.f, x.s));
        }
        //public static PineNA ZipEnd(Pine first, Pine second, Func<double, double, double?> result)
        //{
        //    var fR = ((IEnumerable<double>)first).Reverse();
        //    var sR = ((IEnumerable<double>)second).Reverse();

        //    var fs = fR.Zip(sR, (f, s) => (f, s));

        //    return fs.Reverse().Select(x => result(x.f, x.s)).ToPineNA();
        //}
        #endregion
    }
}

