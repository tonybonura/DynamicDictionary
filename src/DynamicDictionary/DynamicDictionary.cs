/*    
    The MIT License (MIT)

    Copyright (c) 2014 Randy Burden ( http://randyburden.com ) All rights reserved.

    Permission is hereby granted, free of charge, to any person obtaining a copy
    of this software and associated documentation files (the "Software"), to deal
    in the Software without restriction, including without limitation the rights
    to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    copies of the Software, and to permit persons to whom the Software is
    furnished to do so, subject to the following conditions:

    The above copyright notice and this permission notice shall be included in
    all copies or substantial portions of the Software.

    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
    THE SOFTWARE.
*/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace System.Dynamic
{
    /// <summary>
    /// A dynamic dictionary allowing case-insensitive access and returns null when accessing non-existent properties.
    /// </summary>
    /// <example>
    /// // Non-existent properties will return null
    /// dynamic obj = new DynamicDictionary();
    /// var firstName = obj.FirstName;
    /// Assert.Null( firstName );
    /// 
    /// // Allows case-insensitive property access
    /// dynamic obj = new DynamicDictionary();
    /// obj.SuperHeroName = "Superman";
    /// Assert.That( obj.SUPERMAN == "Superman" );
    /// Assert.That( obj.superman == "Superman" );
    /// Assert.That( obj.sUpErMaN == "Superman" );
    /// </example>
    public class DynamicDictionary : DynamicObject, IDictionary<string, object>
    {
        private readonly IDictionary<string, object> dictionary;

        #region [-- Constructors --]

        /// <summary>
        /// Initializes a new instance of the <see cref="DynamicDictionary"/> class using <see cref="StringComparer.InvariantCultureIgnoreCase"/> for comparing keys.
        /// </summary>
        public DynamicDictionary()
        {
            this.dictionary = new DefaultValueDictionary<string, object>(StringComparer.InvariantCultureIgnoreCase);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DynamicDictionary"/> class using the specified <paramref name="comparer"/> for comparing keys.
        /// </summary>
        /// <param name="comparer">The <see cref="StringComparer"/> to use when comparing keys, or null to use <see cref="StringComparer.InvariantCultureIgnoreCase"/>.</param>
        public DynamicDictionary(StringComparer comparer)
        {
            if(comparer == null)
                comparer = StringComparer.OrdinalIgnoreCase;

            this.dictionary = new DefaultValueDictionary<string, object>(comparer);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DynamicDictionary"/> class copying keys and value from <paramref name="dictionary"/> using <see cref="StringComparer.InvariantCultureIgnoreCase"/> for comparing keys.
        /// </summary>
        /// <param name="dictionary">The dictionary to copy keys and values from.</param>
        /// <exception cref="System.ArgumentNullException">dictionary</exception>
        public DynamicDictionary(IDictionary<string, object> dictionary)
        {
            if(dictionary == null)
                throw new ArgumentNullException(nameof(dictionary));

            this.dictionary = new DefaultValueDictionary<string, object>(dictionary, StringComparer.InvariantCultureIgnoreCase);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DynamicDictionary" /> class copying keys and value from <paramref name="dictionary" /> using the specified <paramref name="comparer" /> for comparing keys.
        /// </summary>
        /// <param name="dictionary">The dictionary to copy keys and values from.</param>
        /// <param name="comparer">The <see cref="StringComparer" /> to use when comparing keys, or null to use <see cref="StringComparer.InvariantCultureIgnoreCase" />.</param>
        /// <exception cref="System.ArgumentNullException">dictionary</exception>
        public DynamicDictionary(IDictionary<string, object> dictionary, StringComparer comparer)
        {
            if(dictionary == null)
                throw new ArgumentNullException(nameof(dictionary));

            if(comparer == null)
                comparer = StringComparer.OrdinalIgnoreCase;

            this.dictionary = new DefaultValueDictionary<string, object>(dictionary, comparer);
        }

        #endregion

        #region [-- DynamicObject Overrides --]

        public override bool TryGetMember( GetMemberBinder binder, out object result )
        {
            result = dictionary[ binder.Name ];

            return true;
        }

        public override bool TrySetMember( SetMemberBinder binder, object value )
        {
            if ( dictionary.ContainsKey( binder.Name ) )
            {
                dictionary[ binder.Name ] = value;
            }
            else
            {
                dictionary.Add( binder.Name, value );
            }

            return true;
        }

        public override bool TryInvokeMember( InvokeMemberBinder binder, object[] args, out object result )
        {
            if ( dictionary.ContainsKey( binder.Name ) && dictionary[ binder.Name ] is Delegate )
            {
                var delegateValue = dictionary[ binder.Name ] as Delegate;

                result = delegateValue.DynamicInvoke( args );

                return true;
            }

            return base.TryInvokeMember( binder, args, out result );
        }

        #endregion

        #region [-- IDictionary<string,object> Members --]

        public void Add( string key, object value )
        {
            dictionary.Add( key, value );
        }

        public bool ContainsKey( string key )
        {
            return dictionary.ContainsKey( key );
        }

        public ICollection<string> Keys
        {
            get { return dictionary.Keys; }
        }

        public bool Remove( string key )
        {
            return dictionary.Remove( key );
        }

        public bool TryGetValue( string key, out object value )
        {
            return dictionary.TryGetValue( key, out value );
        }

        public ICollection<object> Values
        {
            get { return dictionary.Values; }
        }

        public object this[ string key ]
        {
            get
            {
                object value = null;

                dictionary.TryGetValue( key, out value );

                return value;
            }
            set { dictionary[ key ] = value; }
        }

        #endregion IDictionary<string,object> Members

        #region [-- ICollection<KeyValuePair<string,object>> Members --]

        public void Add( KeyValuePair<string, object> item )
        {
            dictionary.Add( item );
        }

        public void Clear()
        {
            dictionary.Clear();
        }

        public bool Contains( KeyValuePair<string, object> item )
        {
            return dictionary.Contains( item );
        }

        public void CopyTo( KeyValuePair<string, object>[] array, int arrayIndex )
        {
            dictionary.CopyTo( array, arrayIndex );
        }

        public int Count
        {
            get { return dictionary.Count; }
        }

        public bool IsReadOnly
        {
            get { return dictionary.IsReadOnly; }
        }

        public bool Remove( KeyValuePair<string, object> item )
        {
            return dictionary.Remove( item );
        }

        #endregion

        #region [-- IEnumerable<KeyValuePair<string,object>> Members --]

        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            return dictionary.GetEnumerator();
        }

        #endregion

        #region [-- IEnumerable Members --]

        IEnumerator IEnumerable.GetEnumerator()
        {
            return dictionary.GetEnumerator();
        }

        #endregion
    }
}

namespace System.Collections.Generic
{
    /// <summary>
    /// A dictionary that returns the default value when accessing keys that do not exist in the dictionary.
    /// </summary>
    public class DefaultValueDictionary<TKey, TValue> : IDictionary<TKey, TValue>
    {
        private readonly IDictionary<TKey, TValue> dictionary;

        #region [-- Constructors --]

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultValueDictionary{TKey, TValue}"/> class.
        /// </summary>
        public DefaultValueDictionary()
        {
            dictionary = new Dictionary<TKey, TValue>();
        }

        /// <summary>
        /// Initializes with an existing dictionary.
        /// </summary>
        /// <param name="dictionary"></param>
        public DefaultValueDictionary(IDictionary<TKey, TValue> dictionary)
        {
            this.dictionary = dictionary.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        }

        /// <summary>
        /// Initializes using the given equality comparer.
        /// </summary>
        /// <param name="comparer"></param>
        public DefaultValueDictionary(IEqualityComparer<TKey> comparer)
        {
            dictionary = new Dictionary<TKey, TValue>(comparer);
        }

        /// <summary>
        /// Initializes with an existing dictionary.
        /// </summary>
        /// <param name="dictionary"></param>
        public DefaultValueDictionary(IDictionary<TKey, TValue> dictionary, IEqualityComparer<TKey> comparer)
        {
            this.dictionary = dictionary.ToDictionary(kvp => kvp.Key, kvp => kvp.Value, comparer);
        }

        #endregion

        #region [-- IDictionary<string,TValue> Members --]

        public void Add(TKey key, TValue value)
        {
            dictionary.Add(key, value);
        }

        public bool ContainsKey(TKey key)
        {
            return dictionary.ContainsKey(key);
        }

        public ICollection<TKey> Keys
        {
            get { return dictionary.Keys; }
        }

        public bool Remove(TKey key)
        {
            return dictionary.Remove(key);
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            return dictionary.TryGetValue(key, out value);
        }

        public ICollection<TValue> Values
        {
            get { return dictionary.Values; }
        }

        public TValue this[TKey key]
        {
            get
            {
                TValue value;

                dictionary.TryGetValue(key, out value);

                return value;
            }
            set { dictionary[key] = value; }
        }

        #endregion

        #region [-- ICollection<KeyValuePair<string,TValue>> Members --]

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            dictionary.Add(item);
        }

        public void Clear()
        {
            dictionary.Clear();
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            return dictionary.Contains(item);
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            dictionary.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return dictionary.Count; }
        }

        public bool IsReadOnly
        {
            get { return dictionary.IsReadOnly; }
        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            return dictionary.Remove(item);
        }

        #endregion

        #region [-- IEnumerable<KeyValuePair<TKey,TValue>> Members --]

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return dictionary.GetEnumerator();
        }

        #endregion

        #region [-- IEnumerable Members --]

        IEnumerator IEnumerable.GetEnumerator()
        {
            return dictionary.GetEnumerator();
        }

        #endregion
    }
}
