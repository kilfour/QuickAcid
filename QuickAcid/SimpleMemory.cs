using System;
using System.Collections.Generic;

namespace QuickAcid
{
    public class SimpleMemory
    {
        private Dictionary<object, object> Dictionary { get; set; }

        public SimpleMemory()
        {
            Dictionary = new Dictionary<object, object>();
        }

        public T Get<T>(object key, T newValue)
        {
            if (!Dictionary.ContainsKey(key))
                Dictionary[key] = newValue;
            return (T)Dictionary[key];
        }

        public T Get<T>(object key, Func<T> newValue)
        {
            if (!Dictionary.ContainsKey(key))
                Dictionary[key] = newValue();
            return (T)Dictionary[key];
        }

        public T Get<T>(object key)
        {
            return (T)Dictionary[key];
        }

        public void Set<T>(object key, T value)
        {
            Dictionary[key] = value;
        }
    }
}