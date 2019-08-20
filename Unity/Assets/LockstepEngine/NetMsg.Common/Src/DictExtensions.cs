using System.Collections.Generic;

namespace NetMsg.Common{
    public static class DictExtensions {
        public static TValue GetRefVal<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key)
            where TValue : class{
            if (dict.TryGetValue(key, out TValue val)) {
                return val;
            }

            return null;
        }
    }
}