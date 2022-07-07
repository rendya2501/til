using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace NullableDictionary
{
  class NullableDict<K, V> : IDictionary<K, V>
  {
    public V this[K key] { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    public ICollection<K> Keys => throw new NotImplementedException();

    public ICollection<V> Values => throw new NotImplementedException();

    public int Count => throw new NotImplementedException();

    public bool IsReadOnly => throw new NotImplementedException();

    public void Add(K key, V value)
    {
      throw new NotImplementedException();
    }

    public void Add(KeyValuePair<K, V> item)
    {
      throw new NotImplementedException();
    }

    public void Clear()
    {
      throw new NotImplementedException();
    }

    public bool Contains(KeyValuePair<K, V> item)
    {
      throw new NotImplementedException();
    }

    public bool ContainsKey(K key)
    {
      throw new NotImplementedException();
    }

    public void CopyTo(KeyValuePair<K, V>[] array, int arrayIndex)
    {
      throw new NotImplementedException();
    }

    public IEnumerator<KeyValuePair<K, V>> GetEnumerator()
    {
      throw new NotImplementedException();
    }

    public bool Remove(K key)
    {
      throw new NotImplementedException();
    }

    public bool Remove(KeyValuePair<K, V> item)
    {
      throw new NotImplementedException();
    }

    public bool TryGetValue(K key, [MaybeNullWhen(false)] out V value)
    {
      throw new NotImplementedException();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      throw new NotImplementedException();
    }
  }
}
