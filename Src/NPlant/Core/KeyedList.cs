using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace NPlant.Core
{
    public interface IKeyedItem
    {
        string Key { get; }
    }

    public class KeyedList<T> : IEnumerable<T>
        where T : class, IKeyedItem
    {
        private readonly Dictionary<string, T> _innerList = new Dictionary<string, T>();

        public int Count => _innerList.Count;

        public void Add(T item)
        {
            item.CheckForNullArg("item");
            _innerList[item.Key] = item;
        }

        public bool TryGetValue(string key, out T item)
        {
            item = null;

            if (_innerList.ContainsKey(key))
                item = _innerList[key];

            return item != null;
        }

        public bool TryGetValueByIndex(int index, out T item)
        {
            item = null;

            if (index.IsWithin(0, _innerList.Keys.Count - 1))
                item = _innerList.Values.ToArray()[index];

            return item != null;
        }

        public T this[int index, bool throwOnNotFound = true]
        {
            get
            {
                T item;

                if(this.TryGetValueByIndex(index, out item))
                    return item;

                if(throwOnNotFound)
                    throw new NPlantException("Failed to find item at index of {0} in the list of {1} instances".FormatWith(index, typeof(T).FullName));

                return default(T);
            }
        }

        public T this[string key, bool throwOnNotFound = true]
        {
            get
            {
                if (this.TryGetValue(key, out var item))
                    return item;

                if(throwOnNotFound)
                    throw new NPlantException("Failed to find item of key '{0}' in the list of {1} instances".FormatWith(key, typeof(T).FullName));

                return default(T);
            }
            set => Add(value);
        }

        public void AddRange(T item, params T[] others)
        {
            item.CheckForNullArg("item");
            
            this.Add(item);

            if (others != null)
                this.AddRange(others);
        }

        public void AddRange(IEnumerable<T> items)
        {
            // ReSharper disable once PossibleMultipleEnumeration
            items.CheckForNullArg("items");

            // ReSharper disable once PossibleMultipleEnumeration
            foreach (var item in items)
            {
                this.Add(item);
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _innerList.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public bool Contains(T child)
        {
            return _innerList.Values.Contains(child);
        }
    }
}