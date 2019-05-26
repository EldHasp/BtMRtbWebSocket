using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitMexLibrary
{
    public class RESTUnSigned
    {
        private static BitMEXApi _bitMexTestREST;
        public static BitMEXApi BitMexTestREST => _bitMexTestREST ?? (_bitMexTestREST = new BitMEXApi("", "", false));

        private static BitMEXApi _bitMexRealREST;
        public static BitMEXApi BitMexRealREST => _bitMexRealREST ?? (_bitMexRealREST = new BitMEXApi("", "", true));
    }
}
