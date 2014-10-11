using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace RainmeterStudio.Core.Utils
{
    public class ObservableDictionary<TKey, TValue> : IDictionary<TKey, TValue>, INotifyCollectionChanged, INotifyPropertyChanged
    {
        private Dictionary<TKey, TValue> _dict;

        #region Events

        /// <summary>
        /// Triggered when the items of the collection change
        /// </summary>
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        /// <summary>
        /// Triggered when a property changes
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the System.Collections.Generic.Dictionary&lt;TKey,TValue&gt;
        /// class that is empty, has the default initial capacity, and uses the default
        /// equality comparer for the key type.
        /// </summary>
        public ObservableDictionary()
        {
            _dict = new Dictionary<TKey,TValue>();
        }

        /// <summary>
        /// Initializes a new instance of the System.Collections.Generic.Dictionary&lt;TKey,TValue&gt;
        /// class that contains elements copied from the specified System.Collections.Generic.IDictionary&lt;TKey,TValue&gt;
        /// and uses the default equality comparer for the key type.
        /// </summary>
        /// <param name="dictionary">
        /// The System.Collections.Generic.IDictionary&lt;TKey,TValue&gt; whose elements are
        /// copied to the new System.Collections.Generic.Dictionary&lt;TKey,TValue&gt;.
        /// </param>
        /// <exception cref="System.ArgumentNullException">dictionary is null</exception>
        /// <exception cref="System.ArgumentException">dictionary contains one or more duplicate keys</exception>
        public ObservableDictionary(IDictionary<TKey, TValue> dictionary)
        {
            _dict = new Dictionary<TKey,TValue>(dictionary);
        }

        /// <summary>
        /// Initializes a new instance of the System.Collections.Generic.Dictionary&lt;TKey,TValue&gt;
        /// class that is empty, has the default initial capacity, and uses the specified
        /// System.Collections.Generic.IEqualityComparer&lt;TKey&gt;.
        /// </summary>
        /// <param name="comparer">
        /// The System.Collections.Generic.IEqualityComparer&lt;TKey&gt; implementation to use
        /// when comparing keys, or null to use the default System.Collections.Generic.EqualityComparer&lt;TKey&gt;
        /// for the type of the key
        /// </param>
        public ObservableDictionary(IEqualityComparer<TKey> comparer)
        {
            _dict = new Dictionary<TKey, TValue>(comparer);
        }

        /// <summary>
        /// Initializes a new instance of the System.Collections.Generic.Dictionary&lt;TKey,TValue&gt;
        /// class that is empty, has the specified initial capacity, and uses the default
        /// equality comparer for the key type.
        /// </summary>
        /// <param name="capacity">
        /// The initial number of elements that the System.Collections.Generic.Dictionary&lt;TKey,TValue&gt;
        /// can contain.
        /// </param>
        /// <exception cref="System.ArgumentOutOfRangeException">capacity is less than 0</exception>
        public ObservableDictionary(int capacity)
        {
            _dict = new Dictionary<TKey, TValue>(capacity);
        }

        /// <summary>
        /// Initializes a new instance of the System.Collections.Generic.Dictionary&lt;TKey,TValue&gt;
        /// class that contains elements copied from the specified System.Collections.Generic.IDictionary&lt;TKey,TValue&gt;
        /// and uses the specified System.Collections.Generic.IEqualityComparer&lt;TKey&gt;.
        /// </summary>
        /// <param name="dictionary">
        /// The System.Collections.Generic.IDictionary&lt;TKey,TValue&gt; whose elements are
        /// copied to the new System.Collections.Generic.Dictionary&lt;TKey,TValue&gt;.
        /// </param>
        /// <param name="comparer">
        /// The System.Collections.Generic.IEqualityComparer&lt;TKey&gt; implementation to use
        /// when comparing keys, or null to use the default System.Collections.Generic.EqualityComparer&lt;TKey&gt;
        /// for the type of the key.
        /// </param>
        /// <exception cref="System.ArgumentNullException">dictionary is null</exception>
        /// <exception cref="System.ArgumentException">dictionary contains one or more duplicate keys</exception>
        public ObservableDictionary(IDictionary<TKey, TValue> dictionary, IEqualityComparer<TKey> comparer)
        {
            _dict = new Dictionary<TKey, TValue>(dictionary, comparer);
        }

        /// <summary>
        /// Initializes a new instance of the System.Collections.Generic.Dictionary&lt;TKey,TValue&gt;
        /// class that is empty, has the specified initial capacity, and uses the specified
        /// System.Collections.Generic.IEqualityComparer&lt;TKey&gt;.
        /// </summary>
        /// <param name="capacity">
        /// The initial number of elements that the System.Collections.Generic.Dictionary&lt;TKey,TValue&gt;
        /// can contain.
        /// </param>
        /// <param name="comparer">
        /// The System.Collections.Generic.IEqualityComparer&lt;TKey&gt; implementation to use
        /// when comparing keys, or null to use the default System.Collections.Generic.EqualityComparer&lt;TKey&gt;
        /// for the type of the key.
        /// </param>
        /// <exception cref="System.ArgumentOutOfRangeException">capacity is less than 0</exception>
        public ObservableDictionary(int capacity, IEqualityComparer<TKey> comparer)
        {
            _dict = new Dictionary<TKey, TValue>(capacity, comparer);
        }

        #endregion

        #region IDictionary<TKey, TValue>

        /// <summary>
        /// Gets the number of key/value pairs contained in the dictionary.
        /// </summary>
        public int Count
        {
            get { return _dict.Count; }
        }

        /// <summary>
        /// Gets a collection containing the keys in the dictionary.
        /// </summary>
        public ICollection<TKey> Keys
        {
            get { return _dict.Keys; }
        }

        /// <summary>
        /// Gets a collection containing the values in the dictionary.
        /// </summary>
        public ICollection<TValue> Values
        {
            get { return _dict.Values; }
        }

        /// <summary>
        /// Gets or sets the value associated with the specified key.
        /// </summary>
        /// <param name="key">The key of the value to get or set</param>
        /// <returns>
        /// The value associated with the specified key. If the specified key is not
        /// found, a get operation throws a <see cref="System.Collections.Generic.KeyNotFoundException"/>,
        /// and a set operation creates a new element with the specified key.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">key is null</exception>
        /// <exception cref="System.Collections.Generic.KeyNotFoundException">The property is retrieved and key does not exist in the collection.</exception>
        public TValue this[TKey key]
        {
            get
            {
                return _dict[key];
            }
            set
            {
                TValue oldValue;
                TryGetValue(key, out oldValue);

                _dict[key] = value;

                if (CollectionChanged != null)
                    CollectionChanged(this, new NotifyCollectionChangedEventArgs(
                        NotifyCollectionChangedAction.Replace, 
                        new KeyValuePair<TKey, TValue>(key, oldValue), 
                        new KeyValuePair<TKey, TValue>(key, value)));
            }
        }

        /// <summary>
        /// Adds the specified key and value to the dictionary.
        /// </summary>
        /// <param name="key">The key of the element to add</param>
        /// <param name="value">The value of the element to add. The value can be null for reference types.</param>
        /// <exception cref="System.ArgumentNullException">key is null</exception>
        /// <exception cref="System.ArgumentException">An element with the same key already exists in the dictionary.</exception>
        public void Add(TKey key, TValue value)
        {
            _dict.Add(key, value);

            if (CollectionChanged != null)
                CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, new KeyValuePair<TKey, TValue>(key, value), null));
        }

        /// <summary>
        /// Removes all keys and values from the dictionary.
        /// </summary>
        public void Clear()
        {
            _dict.Clear();

            if (CollectionChanged != null)
                CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        /// <summary>
        /// Determines whether the dictionary contains the specified key.
        /// </summary>
        /// <param name="key">The key to locate in the dictionary</param>
        /// <returns>true if the dictionary contains an element with the specified key; otherwise, false.</returns>
        /// <exception cref="System.ArgumentNullException">key is null</exception>
        public bool ContainsKey(TKey key)
        {
            return _dict.ContainsKey(key);
        }

        /// <summary>
        /// Removes the value with the specified key from the dictionary.
        /// </summary>
        /// <param name="key">The key of the element to remove</param>
        /// <returns>
        /// true if the element is successfully found and removed; otherwise, false.
        /// This method returns false if key is not found in the dictionary.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">key is null</exception>
        public bool Remove(TKey key)
        {
            TValue oldValue;
            _dict.TryGetValue(key, out oldValue);

            bool res = _dict.Remove(key); 

            if (res && CollectionChanged != null)
                CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, new KeyValuePair<TKey, TValue>(key, oldValue)));

            return res;
        }

        /// <summary>
        /// Gets the value associated with the specified key.
        /// </summary>
        /// <param name="key">The key of the value to get</param>
        /// <param name="value">
        /// When this method returns, contains the value associated with the specified
        /// key, if the key is found; otherwise, the default value for the type of the
        /// value parameter. This parameter is passed uninitialized.
        /// </param>
        /// <returns>
        /// true if the dictionary contains an element with the specified key;
        /// otherwise, false.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">key is null</exception>
        public bool TryGetValue(TKey key, out TValue value)
        {
            return _dict.TryGetValue(key, out value);
        }

        #endregion

        #region ICollection<KeyValuePair<TKey, TValue>>

        void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item)
        {
            ((ICollection<KeyValuePair<TKey, TValue>>)_dict).Add(item);

            if (CollectionChanged != null)
                CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, null));
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> item)
        {
            return ((ICollection<KeyValuePair<TKey, TValue>>)_dict).Contains(item);
        }

        void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            ((ICollection<KeyValuePair<TKey, TValue>>)_dict).CopyTo(array, arrayIndex);
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly
        {
            get { return ((ICollection<KeyValuePair<TKey, TValue>>)_dict).IsReadOnly; }
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item)
        {
            bool res = ((ICollection<KeyValuePair<TKey, TValue>>)_dict).Remove(item);

            if (res && CollectionChanged != null)
                CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item));

            return res;
        }

        #endregion

        #region IEnumerable

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return _dict.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)_dict).GetEnumerator();
        }

        #endregion
    }
}
