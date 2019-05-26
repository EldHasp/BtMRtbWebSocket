using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommLibrary
{
    public class DGColumn
    {
        public string Header { get; private set; }
        public IEnumerable<string> Items { get; private set; }
        public DGColumn(string Header, IEnumerable<object> Items)
        {
            this.Header = Header;
            this.Items = Items.Select(item => item.ToString());
        }
        public DGColumn(string Header, IEnumerable<string> Items)
        {
            this.Header = Header;
            this.Items = Items;
        }
        public static DGColumn Create<T>(string Header, IEnumerable<T> Items)
            => new DGColumn(Header, Items.Select(item => item.ToString()));
        public static DGColumn Create(string Header, IEnumerable<double> Items)
            => new DGColumn(Header, Items.Select(item => item.ToString("f2")));
        public static DGColumn Create(string Header, IEnumerable<double?> Items)
            => new DGColumn(Header, Items.Select(item => item == null ? "n/a" : item.Value.ToString("f2")));
        public static DGColumn Create(string Header, IEnumerable<bool> Items)
            => new DGColumn(Header, Items.Select(item => item ? "Yes!" : ""));
    }
}
