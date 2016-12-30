using System;
using System.Collections;
using System.Collections.Generic;

namespace Compete.DynamicDB.Models
{
    public abstract class ItemDictionary<T> : IEnumerable<T>
    {
        public IDictionary<string, T> Items { get; protected set; }

        public T this[string key]
        {
            get
            {
                T item;
                return Items.TryGetValue(key, out item) ? item : default(T);

            }
            set
            {
                if (Items.ContainsKey(key))
                    Items[key] = value;
                else
                    Items.Add(key, value);
            }
        }

        public ItemDictionary()
        {
            Items = new Dictionary<string, T>();
        }

        public bool ContainsKey(string key)
        {
            return Items.ContainsKey(key);
        }

        public bool Add(string name)
        {
            if (AddItem(name))
                return true;

            return Added(name);
        }

        private bool AddItem(string name)
        {
            if (!Items.ContainsKey(name))
            {
                var item = Activator.CreateInstance<T>();
                lock (Items)
                    if (!Items.ContainsKey(name))
                    {
                        Items.Add(name, item);
                        return false;
                    }
            }

            return true;
        }

        protected virtual bool Added(string name)
        {
            if (this[name] is IChlid)
                (this[name] as IChlid).Parent = this;

            return false;
        }

        public bool Remove(string name)
        {
            if (Items.ContainsKey(name))
                lock (Items)
                    if (Items.ContainsKey(name))
                    {
                        Items.Remove(name);
                        return false;
                    }

            return true;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return Items.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Items.Values.GetEnumerator();
        }
    }
}
