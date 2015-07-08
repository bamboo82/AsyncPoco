using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsyncPoco
{
    public class HashCodeCombiner
    {
        private long _combinedHash = 5381L;

        internal void AddObject(object o)
        {
            AddInt(o.GetHashCode());
        }

        internal void AddInt(int i)
        {
            _combinedHash = ((_combinedHash << 5) + _combinedHash) ^ i;
        }

        internal void AddCaseInsensitiveString(string s)
        {
            if (s != null)
                AddInt((StringComparer.InvariantCultureIgnoreCase).GetHashCode(s));
        }

        /// <summary>
        /// Returns the hex code of the combined hash code
        /// </summary>
        /// <returns></returns>
        internal string GetCombinedHashCode()
        {
            return _combinedHash.ToString("x", CultureInfo.InvariantCulture);
        }
    }
}
