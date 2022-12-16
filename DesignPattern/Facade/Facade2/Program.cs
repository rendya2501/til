using System;

namespace Facade2
{
    /// <summary>
    /// Visiter Class
    /// </summary>
    class Program
    {
        /// <summary>
        /// Facadeパターン
        /// クラスを組み合わせて、ある機能だけを提供する窓口を作るパターン
        /// https://www.techscore.com/tech/DesignPattern/Facade.html/
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            //窓口の中村くんを作る
            Librarian nakamura = new Librarian();
            //中村くんに昆虫図鑑の場所を聞く
            string location = nakamura.SearchBook("昆虫図鑑");
            if (location == "貸出中です")
            {
                Console.WriteLine("貸出中かよ…");
            }
            else if (location == "その本は所蔵していません")
            {
                Console.WriteLine("なんだ、ないのか");
            }
            else
            {
                Console.WriteLine("サンキュ！");
            }
        }
    }

    /// <summary>
    /// 所蔵本リスト
    /// </summary>
    public class BookList
    {
        public string SearchBook(string bookName)
        {
            string location = null;
            //本の名前から探す
            //あればその場所を、なければnullを返す
            switch (bookName)
            {
                case "昆虫図鑑":
                    location = "g2a1";
                    break;
                default:
                    break;
            }

            return location;
        }
    }

    /// <summary>
    /// 貸出帳
    /// </summary>
    public class LendingList
    {
        public bool Check(string bookName)
        {
            //貸出帳をチェックする
            //貸出中ならtrue、そうでなければfalseを返す
            switch (bookName)
            {
                case "昆虫図鑑":
                    return true;
                default:
                    return true;
            }
            
        }
    }

    /// <summary>
    /// 図書委員の中村くん
    /// Facadeパターンの「窓口」の役割
    /// </summary>
    public class Librarian
    {
        public string SearchBook(string bookName)
        {
            //本を探す
            BookList bookList = new BookList();
            string location = bookList.SearchBook(bookName);
            //本の場所がnullではない(所蔵してる)とき
            if (location != null)
            {
                //貸出中かチェックする
                LendingList lendingList = new LendingList();
                if (lendingList.Check(bookName))
                {
                    //貸出中のとき
                    return "貸出中です";
                }
                else
                {
                    //貸出中ではないとき
                    return location;
                }
                //所蔵してないとき
            }
            else
            {
                return "その本は所蔵していません";
            }
        }
    }
}
