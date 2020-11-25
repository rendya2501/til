ユーザー名とパスワードの省略は個人アクセストークンだ。



1. 著者：どんな人が書いているのか
2. 目的：なんのため（誰のため）に書かれた本なのか
3. 構成：どのような内容か
4. 所感：評論や感想
5. まとめ：読者として受け取ったこと、著者の一番言いたかったこと  





	■便利機能

・ALT+PrintScreenでその画面だけのスクリーンショットを取ることができる。

・VSCode 矩形選択
終点を［Shift］＋［Alt］＋左クリック（したままドラッグ）

・整形
Shift + ALT + F

・VSCodeで言語モードの選択を変更するショートカット
Ctrl k + m


・合体して重複をなくす

// 同組、同予約のListを重複無しで合体させ、検索を行う
this.sameGroupList.
    concat(this.sameReservationList).
    filter((x, i, self) => self.indexOf(x) === i).
    some(list => {
        // 名前からVisitListのパラメータを取得
        list.PlayerList.some(player => {
            if (player.Name === changedName || player.Kana === changedName) {
                changedPlayer = player;
                return true;
            }
        })
        if (changedPlayer) {
            return true;
        }
    });



	■F12,Json掃き出し

①一番上のログを右クリックでGlobalなんちゃらで掃き出し。
②JSON.stringify(temp1)でjson掃き出し
③一番右にcopyボタンがあるので、コピー
④cntl+nで新規ファイル作成。
⑤貼り付け
⑥右下プレーンテキストをクリックしてjsonに変更
⑦右クリック、フォーマット



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



	■angular

●hidden ngIf

hidden
T = 非表示
F = 表示

ngIf
T = 表示
F = 非表示

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



