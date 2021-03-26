using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

namespace PokemonCardCatalogue.Helpers
{
    public class ObservableList<T> : IList<T>, INotifyCollectionChanged
    {
        private readonly List<T> _internalList = new List<T>();

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public ObservableList(IEnumerable<T> items)
        {
            AddRange(items);
        }

        public T this[int index] { get => _internalList[index]; set => _internalList[index] = value; }

        public int Count => _internalList.Count;

        public bool IsReadOnly => false;

        public void Add(T item)
        {
            _internalList.Add(item);
            InvokeCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item));
        }

        public void Clear()
        {
            _internalList.Clear();
            InvokeCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        public bool Contains(T item)
        {
            return _internalList.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            _internalList.CopyTo(array, arrayIndex);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _internalList.GetEnumerator();
        }

        public int IndexOf(T item)
        {
            return _internalList.IndexOf(item);
        }

        public void Insert(int index, T item)
        {
            _internalList.Insert(index, item);
            InvokeCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, index));
        }

        public bool Remove(T item)
        {
            var result = _internalList.Remove(item);
            if (result)
            {
                InvokeCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item));
            }

            return result;
        }

        public bool Replace(T oldItem, T newItem)
        {
            var index = _internalList.IndexOf(oldItem);
            _internalList.RemoveAt(index);
            _internalList.Insert(index, newItem);
            InvokeCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, newItem, oldItem));

            return true;
        }

        public void RemoveAt(int index)
        {
            InvokeCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, _internalList[index]));
            _internalList.RemoveAt(index);
        }

        public void AddRange(IEnumerable<T> items)
        {
            var itemsToAdd = items
                .ToList();

            var count = itemsToAdd.Count;
            _internalList.AddRange(itemsToAdd);

            InvokeCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, itemsToAdd, count));
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _internalList.GetEnumerator();
        }

        private void InvokeCollectionChanged(NotifyCollectionChangedEventArgs args)
        {
            CollectionChanged?.Invoke(this, args);
        }
    }
}
