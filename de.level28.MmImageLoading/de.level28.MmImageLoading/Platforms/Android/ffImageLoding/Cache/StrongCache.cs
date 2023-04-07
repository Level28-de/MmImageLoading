using System.Collections.Generic;
using MmImageLoading.Drawables;
using Java.Util;
using System.Linq;

namespace MmImageLoading.Cache
{
    public class StrongCache<TValue> where TValue : Java.Lang.Object, ISelfDisposingBitmapDrawable
    {
        protected object _monitor = new object();
        private readonly Java.Util.Hashtable _androidCache = new Java.Util.Hashtable();

        public TValue Get(string key)
        {
            lock (_monitor)
            {
                TryGetValue(key, out var outValue);
                return outValue;
            }
        }

        public bool TryGetValue(string key, out TValue value)
        {
            lock (_monitor)
            {
                value = _androidCache.Get(new Java.Lang.String(key)) as TValue;
                return value != null;
            }
        }

        public bool ContainsKey(string key)
        {
            lock (_monitor)
            {
                return (_androidCache.Get(new Java.Lang.String(key))) != null;
            }
        }

        public void Add(string key, TValue value)
        {
            lock (_monitor)
            {
                _androidCache.Put(new Java.Lang.String(key), value);
            }
        }

        public bool Remove(string key)
        {
            lock (_monitor)
            {
                var removed = _androidCache.Remove(new Java.Lang.String(key));
                if (removed != null)
                {
                    return true;
                }

                return false;
            }
        }

        public IEnumerable<string> Keys
        {
            get
            {
                lock (_monitor)
                {
                    var a = _androidCache.KeySet().Cast<Java.Lang.String>().Select(v => v.ToString());
                    return a;
                }
            }
        }

        public void Clear()
        {
            lock (_monitor)
            {
                _androidCache.Clear();
            }
        }
    }
}
