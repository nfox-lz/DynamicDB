using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

namespace Compete.DynamicDB.Models
{
    public sealed class Collection : IEnumerable<DynamicEntityFrame.DynamicEntity>
    {
        public IDictionary<Guid, DynamicEntityFrame.DynamicEntity> Items { get; private set; }

        public Collection()
        {
            Items = new Dictionary<Guid, DynamicEntityFrame.DynamicEntity>();
        }

        public DynamicEntityFrame.DynamicEntity this[Guid id]
        {
            get
            {
                return Items.ContainsKey(id) ? Items[id] : null;

            }
            set
            {
                lock (Items)
                    if (Items.ContainsKey(id))
                        Items[id] = value;
                    else
                        Items.Add(id, value);
            }
        }

        public Guid Add(DynamicEntityFrame.DynamicEntity item)
        {
            var id = Guid.NewGuid();
            Items.Add(id, item);
            return id;
        }

        public bool Remove(Guid id)
        {
            if (Items.ContainsKey(id))
                lock (Items)
                    if (Items.ContainsKey(id))
                    {
                        Items.Remove(id);
                        return true;
                    }

            return false;
        }

        public bool Modify(Guid id, DynamicEntityFrame.DynamicEntity item)
        {
            if (Items.ContainsKey(id))
                lock (Items)
                    if (Items.ContainsKey(id))
                    {
                        Items[id] = item;
                        return true;
                    }

            return false;
        }

        public bool Lock()
        {
            bool lockTaken = false;
            Monitor.Enter(Items, ref lockTaken);
            return lockTaken;
        }

        public void Unlock()
        {
            Monitor.Exit(Items);
        }

        public IEnumerator<DynamicEntityFrame.DynamicEntity> GetEnumerator()
        {
            return Items.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Items.GetEnumerator();
        }
    }
}
