using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommLibrary
{
    public static class JsonConvertExtension
    {
        public static bool TryDeserializeObject<T> (string value, out T ret) where T: class
        {
            try
            {
                string val = value;
                ret = JsonConvert.DeserializeObject<T>(val);
                return true;
            }
            catch (Exception)
            {
                ret = null;
                return false;
            }
        }
    }
}
