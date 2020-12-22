using System;

namespace NullableDictionary
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
        }
    }
    
    class NullableDict<K, V> : IDictionary<K, V>
    {

    }
}
