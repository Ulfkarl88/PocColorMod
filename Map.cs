using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace PocColor.Config
{
    // Token: 0x0200000F RID: 15
    public class Map<TKey, TValue> : ConcurrentDictionary<TKey, TValue> where TValue : class
    {
        // Token: 0x1700002B RID: 43
        public new TValue this[TKey key]
        {
            get
            {
                bool flag = key == null || !base.ContainsKey(key);
                TValue result;
                if (flag)
                {
                    result = default(TValue);
                }
                else
                {
                    result = base[key];
                }
                return result;
            }
            set
            {
                bool flag = key == null;
                if (!flag)
                {
                    bool flag2 = !base.ContainsKey(key);
                    if (flag2)
                    {
                        base.TryAdd(key, value);
                    }
                    else
                    {
                        base[key] = value;
                    }
                }
            }
        }

        // Token: 0x0600007B RID: 123 RVA: 0x00006474 File Offset: 0x00004674
        public override string ToString()
        {
            string output = "[";
            foreach (KeyValuePair<TKey, TValue> kvp in this)
            {
                output += string.Format("Key = {0}, Value = {1}", kvp.Key, kvp.Value);
                output += ", ";
            }
            output += "]";
            return output;
        }
    }
}
