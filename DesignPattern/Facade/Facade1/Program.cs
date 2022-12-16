using System;


namespace Facade1
{
    using FacadePatternConsole.PageMaker;
    /// <summary>
    /// Facadeパターンは、複数のクラス・メソッドからなる複雑な処理をまとめて１つのAPIを提供する。
    /// ・クラス・メソッドの集合（パッケージ）の外部との結合度を下げ、再利用性を高める
    /// ・複数の処理の集合に対し一律の窓口を提供することで、利用者側での実装負担を軽減する
    /// https://qiita.com/aconit96/items/60c924dbc69a059e5746#_reference-9d763076e525fb2324073
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            PageMaker.MakeWelcomePage("Piyo_Taroh", "welcome.html");
        }
    }
}


namespace FacadePatternConsole.PageMaker
{
    using Facade1;
    using System.IO;
    using System.Text;

    /// <summary>
    /// ユーザー名を利用し、リソースファイルに登録されているメールアドレスの検索を行う
    /// </summary>
    public class Database
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        private Database() { }

        /// <summary>
        /// ユーザ名からメールアドレスを検索する
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public static string GetUserMailAddr(string userName)
        {
            return MailData.ResourceManager.GetString(userName);
        }
    }


    /// <summary>
    /// 指定された情報を利用し、HTMLタグを生成する
    /// </summary>
    public class HtmlWriter : IDisposable
    {
        private StreamWriter _writer { get; set; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="writer"></param>
        public HtmlWriter(StreamWriter writer)
        {
            this._writer = writer;
        }

        /// <summary>
        /// HTMLページのタイトル部分を構成するタグ
        /// </summary>
        /// <param name="title"></param>
        public void WriteTitle(string title)
        {
            this._writer.Write("<html>");
            this._writer.Write("<head>");
            this._writer.Write("<title>" + title + "</title>");
            this._writer.Write("</head>");
            this._writer.Write("<body>\n");
            this._writer.Write("<h1>" + title + "<h1>");
        }
        /// <summary>
        /// HTMLページの段落を構成するタグ（p）を生成する
        /// </summary>
        /// <param name="msg"></param>
        public void WriteParagraph(string msg)
        {
            this._writer.Write("<p>" + msg + "</p>\n");
        }
        /// <summary>
        /// HTMLページのリンクを構成するタグ（a href）を生成する
        /// </summary>
        /// <param name="href"></param>
        /// <param name="caption"></param>
        public void WriteLink(string href, string caption)
        {
            this._writer.Write("<a href=\"" + href + "\">" + caption + "</p>\n");
        }
        /// <summary>
        /// メールアドレス部分の文字列を生成する
        /// </summary>
        /// <param name="mailAddr"></param>
        /// <param name="userName"></param>
        public void WriteMailTo(string mailAddr, string userName)
        {
            WriteLink(("mailto:" + mailAddr), userName);
        }
        /// <summary>
        /// body, headの末尾を生成する
        /// </summary>
        public void Close()
        {
            this._writer.Write("</body>");
            this._writer.Write("</html>\n");
        }
        /// <summary>
        /// インターフェース実装メソッド
        /// </summary>
        public void Dispose()
        {
            this._writer.Dispose();
        }
    }

    public class PageMaker
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        private PageMaker() { }

        public static void MakeWelcomePage(string userName, string fileName)
        {
            string mailAddr = Database.GetUserMailAddr(userName);

            using (HtmlWriter writer = new HtmlWriter(new StreamWriter(fileName, false, Encoding.Unicode)))
            {
                writer.WriteTitle("welcome to" + userName + "s' Page");
                writer.WriteParagraph(userName + "のページへようこそ");
                writer.WriteParagraph("メールまってますね。");
                writer.WriteMailTo(mailAddr, userName);
                writer.Close();
            }
            Console.WriteLine(fileName + "@ is created for " + mailAddr + @"(" + userName + @")");
            Console.ReadKey();
        }
    }
}
