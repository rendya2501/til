# LRU (Least Recentry Used)

置き換え対象の中で最も長い時間参照されていないものを置き換え対象とするアルゴリズム。  

この置換アルゴリズムは、「最近使用されたページは再び近い将来に参照される可能性が高く、長い間参照されていないページは今後も参照される可能性が低い」ということを根拠としていて、キャッシュメモリや仮想記憶におけるデータの置き換えを決定するアルゴリズムとして使われています。  

※Least : 最小  

[LRUキャッシュとLFUキャッシュをけっこう丁寧に実装します(Python)](https://qiita.com/grouse324/items/8c7c48b17c4fbf246f44)  
lru cache implementation c#  

---

## 例題

- ページ枠数 : 3  
- プログラム大きさ : 5  
- 参照ページ番号順 : 0→1→2→3→0→3→4→2→3→2→1→3  

``` txt
ページフォールト | *  *  *  *  *     *  *        * 
-----------------+-----------------------------------
参照ページ       | 0  1  2  3  0  3  4  2  3  2  1  3
-----------------+-----------------------------------
ページ枠の内容   | 0  1  2  3  0  3  4  2  3  2  1  3
                 |    0  1  2  3  0  3  4  2  3  2  1
                 |       0  1  2  2  0  3  4  4  3  2
-----------------+-----------------------------------
                 |          ↡  ↡     ↡  ↡        ↡  
ページアウト     |          0  1     2  0        4 
```

---

## 実装

[Fast, Short And Clean O1 LRU Cache Algorithm Implementation In C#](https://www.c-sharpcorner.com/article/fast-and-clean-o1-lru-cache-implementation/)  

``` C#
using System.Collections.Generic;

namespace LRUCache
{
    public class LRUCache
    {
        private int _capacity;
        private Dictionary<int, (LinkedListNode<int> node, int value)> _cache;
        private LinkedList<int> _list;

        public LRUCache(int capacity)
        {
            _capacity = capacity;
            _cache = new Dictionary<int, (LinkedListNode<int> node, int value)>(capacity);
            _list = new LinkedList<int>();
        }

        public int Get(int key)
        {
            if (!_cache.ContainsKey(key))
                return -1;

            var node = _cache[key];
            _list.Remove(node.node);
            _list.AddFirst(node.node);

            return node.value;
        }

        public void Put(int key, int value)
        {
            if (_cache.ContainsKey(key))
            {
                var node = _cache[key];
                _list.Remove(node.node);
                _list.AddFirst(node.node);

                _cache[key] = (node.node, value);
            }
            else
            {
                if (_cache.Count >= _capacity)
                {
                    var removeKey = _list.Last.Value;
                    _cache.Remove(removeKey);
                    _list.RemoveLast();
                }

                // add cache
                _cache.Add(key, (_list.AddFirst(key), value));
            }
        }
    }
}
```
