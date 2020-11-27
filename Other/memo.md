ユーザー名とパスワードの省略は個人アクセストークンだ。


1. 著者：どんな人が書いているのか
2. 目的：なんのため（誰のため）に書かれた本なのか
3. 構成：どのような内容か
4. 所感：評論や感想
5. まとめ：読者として受け取ったこと、著者の一番言いたかったこと  


	■便利機能

	■git pullのあれ

git push pgmweb fix-366
git pull pgmweb fix-366

git pull [リポジトリ] [ブランチ]
         [リモート]   [ローカル]

必要とあらばpgmwebをpullしてもかまわない。

425をやっているうちにbug-fix-develop-alpに適応された変更を425にマージしておきたい。
そのためのコマンド
git pull origin bug-fix-develop-alp

	■gitlabからブランチを削除する方法

リポジトリ→ブランチ→ゴミ箱マーク


・bug-fex-develop-alpを消したい場合

2020/03/22に間違ってbug-fix-develop-alpにマージしてしまったときの対処法


・リポジトリ、ブランチ、alpで検索

・右上の歯車
　∟プロジェクトを編集
　　∟Defaultブランチを適当なものに変更して変更を保存
　∟保護ブランチ
　　∟Unprotectに変更

・ブランチを消す

・東さんにごにょごにょしてもらう

・右上の歯車から逆のことをする



	■vim

:wq 保存して閉じる

	■内税

内税価格 - 内税 = 本体価格
内税価格 / 消費税率 = 本体価格

① : 1000 / 1.1 = 909.0909
② : 1000 * 10 / 110 = 90.9

切り上げの場合
1000 - 91 = 909

切り捨ての場合
1000 - 90 = 910

	■150.41 開発環境

CentOS 7
CentOSはRedHat系

・OS確認コマンド
$ cat /etc/redhat-release

・chrome update コマンド
$ sudo yum install https://dl.google.com/linux/direct/google-chrome-stable_current_x86_64.rpm

・chrome version 確認コマンド
$ google-chrome --version

参考サイト：
http://www.ajisaba.net/develop/chrome/install_centos7.html


●VisualStudioUpdate手順

・rpmをインポート
　ここはVSCodeのアップデートをしようとすると飛ばされた先のrmpダウンロードを選択した先に書いてあるコマンドをコピペして実行する
①$ sudo rpm --import [指定されたURL]
②$ sudo sh -c 'echo -e [なんかとても長い呪文]

・アップデートを確認してyumでインストール
③$ sudo yum check-update
④$ sudo yum -y install code



●管理者権限でフォルダを消す
rm : remove
-r : 配下のデータも削除する
-f : 確認なしで削除する

普通はこれでいける↓
rm -rf opt

でも、optフォルダは鍵マークがついていて、明らかに普通では削除できない雰囲気が漂っていた。
実際、権限がないといわれ削除できなかったので、管理者権限でやるしかない。
それに対応したコマンドがこちら。

sudo rm -rf opt

頭にsudo をつけただけだが、これだけで警告もなしに一瞬で消える。
気をつけないとマジでやばいな。

	●SQL SERVER のトランザクション

BEGIN TRANSACTION
COMMIT TRANSACTION
ROLLBACK TRANSACTION

-------------------------------------------------------------------------------------------------------------------------------------------

	●対象カラムが存在するかどうかをチェックする

   ,CASE
		WHEN EXISTS(
			SELECT *
			FROM   [Eco21_Otaru].sys.columns
			WHERE  Name = N'訂正区分'
			AND    Object_ID = OBJECT_ID(N'[Eco21_Otaru].[dbo].[TS_請求]')
		) THEN ISNULL([A].[訂正区分],0)
		ELSE 0 --0:請求 小樽以外は0:請求しかありえない。 1:訂正
	END AS [CorrectClassification]



-------------------------------------------------------------------------------------------------------------------------------------------
-------------------------------------------------------------------------------------------------------------------------------------------
		■TypeScript
-------------------------------------------------------------------------------------------------------------------------------------------
-------------------------------------------------------------------------------------------------------------------------------------------


●全角の長さを取得する

    /**
     * 全角の長さを取得する
     * 「あ」   = 全角1文字
     * 「あa」  = 全角2文字
     * 「あaa」 = 全角2文字
     * 「あaab」= 全角3文字
     * @param str 判定文字列
     */
    private getZenkakuLength(str: string): number {
        // 半角0.5 は繰り上げる
        return Math.ceil(this.zenkakuCount(str));
    }

    /**
     * 全角数を判定する処理
     * 全角:1
     * 半角:0.5
     * @param str 判定する文字列
     */
    private zenkakuCount(str: string) {
        let length = 0;
        for (let i = 0; i < str.length; i++) {
            if (str[i].match(/[ -~]/)) {
                length += 0.5;
            } else {
                length += 1;
            }
        }
        return length;
    }
