using Strategy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrategyPattern3
{
    /// <summary>
    /// https://qiita.com/a1146234/items/fdecc374b7225d99ab07
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            //if (args.Length != 2)
            //{
            //    Console.WriteLine("Usage: StrategyPattern randomseed1 randomseed2");
            //    Console.WriteLine("Example: StrategyPattern 314 15");
            //    Environment.Exit(0);
            //}
            int seed1 = int.Parse("314");
            int seed2 = int.Parse("15");
            Player player1 = new Player("Taro", new WonStrategy(seed1));
            Player player2 = new Player("Hana", new ProbStrategy(seed2));
            for (int i = 0; i < 10000; i++)
            {
                Hand nextHand1 = player1.NextHand();
                Hand nextHand2 = player2.NextHand();
                if (nextHand1.IsStrongerThan(nextHand2))
                {
                    Console.WriteLine("Winner:" + player1);
                    player1.Win();
                    player2.Lose();
                }
                else if (nextHand2.IsStrongerThan(nextHand1))
                {
                    Console.WriteLine("Winner:" + player2);
                    player1.Lose();
                    player2.Win();
                }
                else
                {
                    Console.WriteLine("Even...");
                    player1.Even();
                    player2.Even();
                }
            }
            Console.WriteLine("Total result:");
            Console.WriteLine(player1);
            Console.WriteLine(player2);
            Console.ReadLine();
        }
    }
}


namespace Strategy
{
    /// <summary>
    /// WonStrategyクラス
    /// </summary>
    /// <remarks>
    /// このクラスのじゃんけん戦略は、前に勝った手をもう一度出し、負けた場合はランダムで出すというものです。
    /// </remarks>
    public class WonStrategy : IStrategy
    {
        private readonly Random _Random;
        private bool _Won = false;
        private Hand _PrevHand;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="seed">シード値</param>
        public WonStrategy(int seed)
        {
            this._Random = new Random(seed);
        }

        /// <summary>
        /// インターフェース実装メソッド
        /// </summary>
        /// <returns></returns>
        public Hand NextHand()
        {
            if (!this._Won)
            {
                this._PrevHand = Hand.GetHand(this._Random.Next(3));
            }
            return this._PrevHand;
        }
        /// <summary>
        /// インターフェース実装メソッド
        /// </summary>
        /// <param name="win"></param>
        public void Study(bool win)
        {
            this._Won = win;
        }
    }

    /// <summary>
    /// Strategyインターフェースを実装するクラスです。
    /// </summary>
    /// <remarks>
    /// このクラスのじゃんけん戦略は、過去の勝ち負けの履歴を記録しそこから確率を出して次の一手を決定するというものです。
    /// </remarks>
    public class ProbStrategy : IStrategy
    {
        private readonly Random _Random;
        private int _PrevHandValue = 0;
        private int _CurrentHandValue = 0;
        private readonly int[][] _History =
        {
            new int[]{1,1,1},
            new int[]{1,1,1},
            new int[]{1,1,1},
        };

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="seed">シード値</param>
        public ProbStrategy(int seed)
        {
            this._Random = new Random(seed);
        }

        /// <summary>
        /// インタフェース実装メソッド
        /// </summary>
        /// <returns></returns>
        public Hand NextHand()
        {
            int bet = this._Random.Next(GetSum(this._CurrentHandValue));
            int handvalue = 0;
            if (bet < this._History[this._CurrentHandValue][0])
            {
                handvalue = 0;
            }
            else if (bet < this._History[this._CurrentHandValue][0] + this._History[this._CurrentHandValue][1])
            {
                handvalue = 1;
            }
            else
            {
                handvalue = 2;
            }
            this._PrevHandValue = this._CurrentHandValue;
            this._CurrentHandValue = handvalue;
            return Hand.GetHand(handvalue);
        }

        /// <summary>
        /// インターフェース実装メソッド
        /// </summary>
        /// <param name="win"></param>
        public void Study(bool win)
        {
            if (win)
            {
                this._History[this._PrevHandValue][this._CurrentHandValue]++;
            }
            else
            {
                this._History[this._PrevHandValue][(this._CurrentHandValue + 1) % 3]++;
                this._History[this._PrevHandValue][(this._CurrentHandValue + 2) % 3]++;
            }
        }

        private int GetSum(int hv)
        {
            int sum = 0;
            for (int i = 0; i < 3; i++)
            {
                sum += this._History[hv][i];
            }
            return sum;
        }
    }

    /// <summary>
    /// じゃんけんを行う人を表現したクラスです。
    /// </summary>
    /// <remarks>
    /// Playerクラスは「名前」と「戦略」を与えられてインスタンスを作成します。
    /// Playerクラスは勝敗結果を保持し、それを利用して次の一手の決定戦略を各Strategy実装クラスに委譲しています。
    /// </remarks>
    public class Player
    {
        private readonly string _Name;
        private readonly IStrategy _Strategy;
        private int _WinCount;
        private int _LoseCount;
        private int _GameCount;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="name">名前</param>
        /// <param name="strategy">戦略</param>
        public Player(string name,IStrategy strategy)
        {
            this._Name = name;
            this._Strategy = strategy;
        }

        public Hand NextHand()
        {
            return this._Strategy.NextHand();
        }

        public void Win()
        {
            this._Strategy.Study(true);
            this._WinCount++;
            this._GameCount++;
        }

        public void Lose()
        {
            this._Strategy.Study(true);
            this._LoseCount++;
            this._GameCount++;
        }

        public void Even()
        {
            this._GameCount++;
        }

        public override string ToString()
        {
            return "[" + this._Name + ":" + this._GameCount + "games, " + this._WinCount + "win, " + this._LoseCount + "  lose" + "]";
        }
    }

    /// <summary>
    /// HandClass
    /// </summary>
    /// <remarks>
    /// Handクラスは、じゃんけんの「手」を表すクラスです。
    /// 手の強さの比較や判定もこのクラスで行います。
    /// </remarks>
    public class Hand
    {
        public const int HANDVALUE_STONE = 0;
        public const int HANDVALUE_SCISSORS = 1;
        public const int HANDVALUE_PAPER = 2;
        public static Hand[] hand = new Hand[] {
            new Hand(HANDVALUE_STONE),
            new Hand(HANDVALUE_SCISSORS),
            new Hand(HANDVALUE_PAPER),
        };
        private static readonly string[] name = new string[] {
            "STONE",
            "SCISSORS",
            "PAPER"
        };

        private int _handvalue;
        public Hand(int handvalue)
        {
            this._handvalue = handvalue;
        }

        public static Hand GetHand(int handvalue)
        {
            return hand[handvalue];
        }

        public bool IsStrongerThan(Hand h)
        {
            return Fight(h) == 1;
        }

        public bool IsWeakerThan(Hand h)
        {
            return Fight(h) == -1;
        }

        private int Fight(Hand h)
        {
            if (this == h)
            {
                return 0;
            }
            else if ((this._handvalue + 1) % 3 == h._handvalue)
            {
                return 1;
            }
            else
            {
                return -1;
            }
        }

        public override string ToString()
        {
            return name[this._handvalue];
        }
    }


    /// <summary>
    /// StrategyInterface
    /// </summary>
    /// <remarks>
    /// じゃんけんの戦略のためのインターフェイスです。
    /// </remarks>
    public interface IStrategy
    {
        /// <summary>
        /// 次の一手を決定するメソッドです。
        /// </summary>
        /// <returns></returns>
        Hand NextHand();
        /// <summary>
        /// study()は前回の対戦結果を記録するメソッドです。
        /// </summary>
        /// <param name="win"></param>
        void Study(bool win);
    }
}