using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrategyPattern2
{
    /// <summary>
    /// ストラテジーパターン2
    /// https://takachan.hatenablog.com/entry/2017/12/11/070000
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
        }
    }
}


/// <summary>
/// 関数テーブル化
/// 長所
///     クラスを増やさない
///     メソッドの複雑度を下げることができる
///     個々の処理に分割されるので可読性が上がる
/// 短所
///     コードが少し長くなる
/// </summary>
namespace Pattern1
{
    public class Hoge
    {

        public string Func(int mode, string str)
        {
            //if (mode == 1)
            //{
            //    // 処理１
            //}
            //else if (mode == 2)
            //{
            //    // 処理２
            //}
            //else if (mode == 3)
            //{
            //    // 処理３
            //}
            //else
            //{
            //    throw new NotSupportedException($"This mode is not suported. mode={mode}");
            //}
            // ↑上記処理を以下の通り置き換える↓
            IDictionary<int, Func<string, string>> funcTable = this.Table;

            if (!funcTable.ContainsKey(mode))
            {
                throw new NotSupportedException($"This mode is not suported. mode={mode}");
            }

            return funcTable[mode](str);
        }
        // ★関数テーブルを作成
        public IDictionary<int, Func<string, string>> Table =>
            new Dictionary<int, Func<string, string>>()
            {
                { 1, this.FuncMode1 },
                { 2, this.FuncMode2 },
                { 3, this.FuncMode3 },
            };

        // ★Modeに対応した個別の処理を追加
        public string FuncMode1(string str)
        {
            return "ok";
        }

        public string FuncMode2(string str)
        {
            return "ng";
        }

        public string FuncMode3(string str)
        {
            return "ok";
        }
    }
}


/// <summary>
/// 分岐ごとの処理をクラス化
/// 長所
///     もともとの呼び出し箇所がシンプル化する
///     クラス・メソッドの複雑度を更に下げることができる
///     個々の処理に分割されるので可読性が上がる
///     変更の影響の局所化の説明がしやすい
/// 短所
///     クラスが増える
///     コードが少し長くなる
///     コード追跡時に若干追いにくくなる
/// </summary>
namespace Pattern2
{

    public class Hoge
    {
        // メイン処理
        public string Func(int mode, string str)
        {
            return SimpleIHogeFuncFactory.Create(mode).InvokeMode(str); // これだけ
        }
    }

    // Modeごとの処理クラスを取得するためのファクトリ
    public static class SimpleIHogeFuncFactory
    {
        private static readonly IDictionary<int, Func<IHogeFunc>> table =
            new Dictionary<int, Func<IHogeFunc>>()
            {
                { 1, () => new Func1() },
                { 1, () => new Func2() },
                { 1, () => new Func3() },
            };

        public static IHogeFunc Create(int mode)
        {
            if (!table.ContainsKey(mode))
            {
                throw new NotSupportedException($"This mode is not suported. mode={mode}");
            }

            return table[mode]();
        }
    }
    public class Func1 : IHogeFunc
    {
        public string InvokeMode(string str) { return ""; }
    }

    public class Func2 : IHogeFunc
    {
        public string InvokeMode(string str) { return ""; }
    }

    public class Func3 : IHogeFunc
    {
        public string InvokeMode(string str) { return ""; }
    }
    // Modeごとの処理を表すインターフェース
    public interface IHogeFunc
    {
        string InvokeMode(string str);
    }
}


/// <summary>
/// ストラテジーパターンを適用する
/// 長所
///     世間で一般的とされるリファクタリングが実施できる
///     リリース後にモードが増えても呼び出し元には影響が出ない
/// 短所
///     呼び出し側コードを変更する必要がある
/// </summary>
namespace Pattern3
{
    // Modeごとの処理クラスを取得するためのファクトリ
    public static class SimpleIHogeFactory
    {
        private static readonly IDictionary<int, Func<IHoge>> table = new Dictionary<int, Func<IHoge>>()
            {
                { 1, () => new Hoge1() },
                { 2, () => new Hoge2() },
                { 3, () => new Hoge3() },
            };

        public static IHoge Create(int mode)
        {
            if (!table.ContainsKey(mode))
            {
                throw new NotSupportedException($"This mode is not suported. mode={mode}");
            }

            return table[mode]();
        }
    }
    // Modeごとの処理クラス
    internal class Hoge1 : IHoge
    {
        public string Func(string str) { return "ok"; }
    }

    internal class Hoge2 : IHoge
    {
        public string Func(string str) { return "ng"; }
    }

    internal class Hoge3 : IHoge
    {
        public string Func(string str) { return ""; }
    }
    // Modeごとの処理を表すインターフェース
    public interface IHoge
    {
        string Func(string str);
    }
}