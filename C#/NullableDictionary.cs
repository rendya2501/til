
namespace Test
{

    // https://qiita.com/chocolamint/items/9f13fe7e3c6343f898c2
    // https://qiita.com/RyotaMurohoshi/items/03937297810e7c9aaf8b
    // https://qiita.com/Temarin/items/27614d879e9376421aae
    // https://stackoverflow.com/questions/4632945/dictionary-with-null-key
    // Dictionary key null
    public class Test{
        public Test()
        {
            var aa = new NullableKeyDictionary<string, string>(
                new Dictionary<string, string>
                {
                    {null, "チェックイン処理"},
                    {"001","チェックイン処理"},
                    {"002","アドバンスチェックイン処理"},
                    {"003","スタート入力処理"},
                    {"004","売掛伝票入力・会費伝票入力"},
                    {"005","利用伝票入力"},
                    {"006","現金振替入力"},
                    {"007","伝票一括入力"},
                    {"008","振替伝票入力"},
                    {"009","チェックアウト処理(個人精算)"},
                    {"010","チェックアウト処理"},
                }
            );
        }
    }

    public class NullableKeyDictionary<TKey, TValue> : IReadOnlyDictionary<TKey, TValue>
    {
        private readonly IReadOnlyDictionary<TKey, TValue> source;
        private readonly bool hasNullKey;
        private readonly TValue nullValue;

        public NullableKeyDictionary(IReadOnlyDictionary<TKey, TValue> source)
        {
            if (this.source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            this.source = source;
            this.hasNullKey = false;
            this.nullValue = default(TValue);
        }

        public NullableKeyDictionary(IReadOnlyDictionary<TKey, TValue> source, TValue nullValue)
        {
            if (this.source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            this.source = source;
            this.hasNullKey = true;
            this.nullValue = nullValue;
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            foreach (var it in this.source)
            {
                yield return it;
            }

            if (this.hasNullKey)
            {
                yield return new KeyValuePair<TKey, TValue>(default(TKey), nullValue);
            }
        }

        //IEnumerator<TKey> IEnumerable.Enumerator => (IEnumerator<TKey>)GetEnumerator();

        public int Count => this.hasNullKey ? this.source.Count + 1 : this.source.Count;

        public bool ContainsKey(TKey key)
        {
            if (this.hasNullKey && key == null)
            {
                return true;
            }
            else
            {
                return this.source.ContainsKey(key);
            }
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            if (this.hasNullKey && key == null)
            {
                value = nullValue;
                return true;
            }
            else
            {
                return this.TryGetValue(key, out value);
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public TValue this[TKey key] => key == null && hasNullKey
            ? this.nullValue
            : this.source[key];

        public IEnumerable<TKey> Keys
        {
            get
            {
                foreach (var it in this.source)
                {
                    yield return it.Key;
                }

                if (this.hasNullKey)
                {
                    yield return default(TKey);
                }
            }
        }

        public IEnumerable<TValue> Values
        {
            get
            {
                foreach (var it in this.source)
                {
                    yield return it.Value;
                }

                if (this.hasNullKey)
                {
                    yield return nullValue;
                }
            }
        }
    }
}