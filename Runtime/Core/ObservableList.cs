using System;
using System.Collections;
using System.Collections.Generic;

namespace UIBinding
{
    /// <summary>
    /// 可观察列表
    /// </summary>
    public class ObservableList<T> : IObservableList, IList<T> where T : ViewModelBase
    {
        private readonly List<T> items;

        public ObservableList()
        {
            items = new List<T>();
        }

        public T this[int index]
        {
            get { return items[index]; }
            set { Set(index, value); }
        }

        public int Count => items.Count;

        public bool IsReadOnly => false;

        public event CollectionChangedEventHandler CollectionChanged;

        public object Get(int index)
        {
            return items[index];
        }

        public void Add(T item)
        {
            items.Add(item);
            int index = items.Count - 1;
            OnCollectionAdd(item, index);
        }

        private void Set(int index, T item)
        {
            items[index] = item;
            OnCollectionUpdate(item);
        }

        public void Insert(int index, T item)
        {
            items.Insert(index, item);
            OnCollectionAdd(item, index);
        }

        public bool Remove(T item)
        {
            var index = IndexOf(item);
            var isRemoved = items.Remove(item);
            if (isRemoved)
            {
                OnCollectionRemove(item, index);
            }
            return isRemoved;
        }

        public void RemoveAt(int index)
        {
            var item = items[index];
            items.RemoveAt(index);
            OnCollectionRemove(item, index);
        }

        public void Clear()
        {
            items.Clear();
            OnCollectionReset();
        }

        public bool Contains(T item)
        {
            return items.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            items.CopyTo(array, arrayIndex);
        }

        public int IndexOf(T item)
        {
            return items.IndexOf(item);
        }

        public virtual void Reset()
        {
            CollectionChanged = null;
            items.Clear();
        }


        public int FindIndex(Predicate<T> match)
        {
            return items.FindIndex(match);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)items).GetEnumerator();
        }

        private void OnCollectionAdd(T item, int index)
        {
            CollectionChanged?.Invoke(this, CollectionChangedAction.Add, item, index);
        }

        private void OnCollectionRemove(T item, int index)
        {
            CollectionChanged?.Invoke(this, CollectionChangedAction.Remove, item, index);
        }

        private void OnCollectionUpdate(T item)
        {
            var index = IndexOf(item);
            CollectionChanged?.Invoke(this, CollectionChangedAction.Update, item, index);
        }

        private void OnCollectionReset()
        {
            CollectionChanged?.Invoke(this, CollectionChangedAction.Reset);
        }
    }
}
