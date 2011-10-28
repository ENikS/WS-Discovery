using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.ServiceModel.Discovery
{
    public class EventArgs<T> : EventArgs
    {
        public EventArgs(T value)
        {
            _value = value;
        }

        private T _value;

        public T Value
        {
            get { return _value; }
        }
    }

    public class EventArgs<K, T> : EventArgs
    {
        public EventArgs(K key, T value)
        {
            _key = key;
            _value = value;
        }

        private K _key;
        private T _value;

        public K Key
        {
            get { return _key; }
        }
        
        public T Value
        {
            get { return _value; }
        }
    }
}
