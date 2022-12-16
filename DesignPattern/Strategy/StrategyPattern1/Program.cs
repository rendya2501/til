using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrategyPattern1
{
    /// <summary>
    /// ストラテジーパターン1
    /// https://yoshinorin.net/2016/06/08/strategy-pattern/
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            var dynamics = Console.ReadLine();

            Strategy.Piano _piano = new Strategy.Piano();
            _piano.Play(dynamics);

            Console.ReadLine();
        }
    }
}

namespace Strategy.Dynamics
{
    /// <summary>
    /// インターフェースの実装クラス
    /// ピアニッシモの時の演奏を実装します。
    /// </summary>
    public class Pianissimo : IPlay
    {
        public void Play()
        {
            Console.WriteLine("チャーン....");
        }
    }
    /// <summary>
    /// ピアノの時の演奏を実装します。
    /// </summary>
    public class Piano : IPlay
    {
        public void Play()
        {
            Console.WriteLine("ポロローン....");
        }
    }
    /// <summary>
    /// フォルテのときの演奏を実装します。
    /// </summary>
    public class Forte : IPlay
    {
        public void Play()
        {
            Console.WriteLine("ジャーン！");
        }
    }
    /// <summary>
    /// フォルティシモのときの演奏を実装します。
    /// </summary>
    public class Fortessimo : IPlay
    {
        public void Play()
        {
            Console.WriteLine("ジャジャーン！");
        }
    }

}

namespace Strategy
{
    /// <summary>
    /// ピアノクラス
    /// </summary>
    public class Piano
    {
        private static IPlay _currentPlay;

        /// <summary>
        /// 演奏します
        /// </summary>
        /// <param name="dynamics"></param>
        public void Play(string dynamics)
        {
            switch (dynamics)
            {
                case "pp":
                    _currentPlay = new Dynamics.Pianissimo();
                    break;
                case "p":
                    _currentPlay = new Dynamics.Piano();
                    break;
                case "f":
                    _currentPlay = new Dynamics.Forte();
                    break;
                case "ff":
                    _currentPlay = new Dynamics.Fortessimo();
                    break;
                default:
                    break;
            }
            // 実行はインターフェースを通じて行う
            _currentPlay.Play();
        }
    }

    /// <summary>
    /// 演奏のインターフェース
    /// </summary>
    public interface IPlay
    {
        /// <summary>
        /// 演奏
        /// </summary>
        void Play();
    }
}
