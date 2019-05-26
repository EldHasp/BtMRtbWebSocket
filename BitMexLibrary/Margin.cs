using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitMexLibrary
{
    public class Margin // For account balance
    {
        public double? WalletBalance { get; set; }
        public double? AvailableMargin { get; set; }
        public double? UsefulBalance
        {
            get { return (WalletBalance / 100000000) ?? 0; }
        }
    }
}
