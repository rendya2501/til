# 9.管理タスク

- ユーザーとグループの管理  
- ジョブスケジューリング  
- ローカライゼーションと国際化  

``` txt : ユーザーとグループの管理
・/etc/passwd :: ユーザーアカウント情報(※ユーザーのプライマリグループの指定も)が格納されているファイル。パーミッションは「644(-rw-r--r--)」。
・/etc/shadow :: 暗号化されているパスワードや、パスワードの有効期限に関する情報が格納されているファイル。  
                 パーミッション「400(r--------)」、「600(rw-------)」、「000(---------)」など、rootユーザのみが読み書き可能。
                 ユーザーパスワードの最終更新日が記録されるのもこのファイル。
・/etc/skel/  :: 新規ユーザー作成時にコピーする雛形（スケルトン）ファイルを配置するディレクトリ。
・/etc/group  :: グループに関する情報が格納されているファイル。グループのメンバーリストには所属メンバーが記載されていない場合がある。
・/etc/gshadow :: shadowファイルのグループver。暗号化されたグループパスワード等が格納されるファイル。
・/etc/default/useradd :: useraddコマンドでオプションを指定せずにユーザを追加した時のデフォルトの値が記述されたファイル。[useradd -D]で確認できる。
・passwd   :: ユーザーのパスワードを変更するコマンド。
・chage    :: パスワードの有効期限の設定に特化したコマンド
・useradd  :: ユーザーアカウントを新規作成するコマンド。
              [ユーザアカウントを新規作成する際に暗号化されたパスワードを設定 :: -p]
              ※Debian系では、「-m」オプション（ホームディレクトリを作成する）を使用しないとホームディレクトリは作成されない。
・userdel  :: ユーザーアカウントを削除するコマンド。
・usermod  :: 既存のユーザーアカウントを変更するコマンド。[-n :: グループ名の変更]
・groupadd :: グループアカウントを新規作成するコマンド。
・groupdel :: グループアカウントを削除するコマンド。
・groupmod :: 既存のグループアカウントの設定を変更
・id       :: ユーザーのUIDやGIDを表示するコマンド。
・groups   :: ユーザーの所属するグループを表示するコマンド。
・gpasswd  :: 「/etc/group」を管理するコマンド。グループのパスワードやメンバーを設定するできる。
・getent   :: 指定したデータベースからユーザーやグループの情報一覧を表示するコマンド。
              ユーザー名やホームディレクトリなどのユーザー情報の一覧を表示する [getent passwd]

※対話的ログインを禁止
ログインシェルを「/bin/false」 or 「/sbin/nologin」に設定。
「chsh -s /sbin/nologin hoge2」 or 「usermod -s /bin/false hoge2」

※アカウントを使用させないようにする
「/etc/passwd」パスワードフィールドの1文字目に「!」または「*」を追加する
「/etc/shadow」パスワードフィールドの1文字目に「!」または「*」を追加する
「passwd -l user3」
「usermod -L user3」

※あるユーザが3つのグループ（A,B,C)に属しているが、Bグループからそのユーザを削除したい。ただしBグループはユーザのプライマリグループではないものとする。
usermod -Gを使う
```

``` txt : ジョブスケジューリング
・cron  :: スケジュールを定義したコマンドを定期的に実行するプログラム。  
・crond :: 定期的に実行するジョブのスケジュールを管理するデーモン。1分ごとに起動され、crontabファイルなどを調べて設定されたジョブを実行する。
・/usr/sbin/crond :: RedHat系crondデーモン  
・/usr/sbin/cron  :: Debian系crondデーモン  
・crontab         :: ユーザ用のcronの設定を行うコマンド。コマンドの定時実行のスケジュール管理コマンド。  
・crontab書式     :: [分] [時] [日] [月] [曜日] [実行ユーザー名(システム用設定ファイルでのみ指定)] [コマンド]
                     [* :: 全てに一致] , [- :: 範囲指定] , [, :: 複数の値を指定] , [/  :: 間隔の指定]
                     1つの設定を識別するのに必要なもの→「改行」
・/var/spool/cron/ユーザ名  :: ユーザ用のcrontabファイル。エディタで編集不可(cat不可)。crontabからのみ可能。
・/var/spool/cron/crontabs/ :: ユーザ用のcrontabファイルが置かれるディレクトリ。  
・/etc/crontab       :: システムに定期的に実行させたいジョブを設定するファイル。主に以下4つのディレクトリに置かれたスクリプトを実行する。
                        エディタで編集可能。実行ユーザー必要。
・/etc/cron.hourly/  :: 毎時定期的に実行するジョブのスクリプトを格納したディレクトリ。  
・/etc/cron.daily/   :: 毎日定期的に実行するジョブのスクリプトを格納したディレクトリ。  
・/etc/cron.monthly/ :: 毎月定期的に実行するジョブのスクリプトを格納したディレクトリ。  
・/etc/cron.weekly/  :: 毎週定期的に実行するジョブのスクリプトを格納したディレクトリ。  
・/etc/cron.d/       :: サービス個別のジョブ実行を定義した設定ファイルを置くディレクトリ。  

※「hogescript」スクリプトを土曜日と日曜日に2時間ごとに実行したい。
0 */2 * * 6,7 hogescript

・at    :: 1回限りの実行スケジュールを扱うコマンド。※日時は基本的にHH:MMの形式で指定
・batch :: システムの負荷の少ないタイミングで指定した時間に1回だけコマンドを自動実行します。  
・atd   :: atデーモン。atコマンドによるスケジューリングを実施するデーモン。
・atrm  :: atのジョブを削除するコマンド。[at -d]と同じ。  
・atq   :: atのジョブのスケジュールを一覧で表示するコマンド。[at -l]と同じ。  

・/etc/cron.allow :: cronの利用を許可するユーザーを記述する。  
・/etc/cron.deny  :: cronの利用を拒否するユーザーを記述する。  
① /etc/cron.allowファイルがあれば、そこに記述されたユーザーのみがcronを利用できる。/etc/cron.denyファイルは無視される。  
② /etc/cron.allowファイルが無ければ、/etc/cron.denyを参照し、/etc/cron.denyに記述されていないすべてのユーザーがcronを利用できる。  
③ /etc/cron.allowファイル、/etc/cron.denyファイルのどちらもない場合、rootユーザーのみが利用可能。  
・/etc/at.allow :: atの利用を許可するユーザーを記述する。  
・/etc/at.deny  :: atの利用を拒否するユーザーを記述する。  
① /etc/at.allowがあれば、そこに記述されたユーザーのみがatを利用できる。/etc/at.denyファイルは無視される。  
② /etc/at.allowが無ければ、/etc/at.denyを参照し、/etc/at.denyに記述されていないすべてのユーザーがatを利用できる。  
③ どちらのファイルも無ければ、rootユーザーだけがatを利用できる。  

・timerユニット :: systemdにおいて、cronのようにジョブスケジューラとして使用できるユニット。
※「/etc/systemd/system」に拡張子.timerの定義ファイルを作成し、[Timer]セクションに実行タイミングと実行するジョブを指定する。  
・モノトニックタイマー  :: cronのように、起動後はジョブを定期的に実行する。  
・リアルタイムタイマー  :: atのように、指定した実行日時にジョブを1度だけ実行する。  
・systemctl list-timers :: systemdにおいて、全ての有効なtimerユニットを表示するコマンド。  
・systemd-runコマンド   :: systemdで、一時的なユニットを作成してジョブを実行するコマンド。
```

``` txt : ローカライゼーションと国際化
・locale :: 現在のロケールの設定を確認するコマンド。「-a」オプションを使用すると現在のシステムで使用できるロケールが表示される。
・/etc/locale.conf :: systemdが動作するシステムでのシステム全体のロケール設定ファイル
・LC_CTYPE     :: 文字の種類やその比較・分類の規定  
・LC_NUMERIC   :: 数値の書式に関する規定  
・LC_TIME      :: 日付や時刻の書式に関する規定  
・LC_MESSAGES  :: メッセージ表示に使用する言語  
・LC_MONETARY  :: 通貨に関する規定  
・LC_NAME      :: 名前の書式
・LC_ADDRESS   :: アドレス、ロケーションの書式
・LC_TELEPHONE :: 電話番号の書式
・LC_COLLATE   :: 文字の照合や整列に関する規定  
・LC_ALL       :: ロケールの環境変数すべて  
                  指定した値が全てのカテゴリを上書きし、カテゴリごとの設定が出来ない。
・LANG         :: 全てのカテゴリで必ずその値が使われるが、個々のカテゴリ毎に個別の設定が可能。
※LC_ALLが設定されていない場合、指定した値がデフォルトとして使用される。
※LC_ALLは全てのカテゴリを上書きするので「/etc/locale.conf」に設定することは推奨されません。
※「LANG=C」を設定すると、ロケールが異なる環境でも英語で共通の出力が得られる
※LC_ALLはlocale.confに記述しない
※環境変数LANGの指定をコマンドの前に置くと、そのコマンドのみ指定された環境で実行される。
例) LANG=ja_JP.eucJP myapp

・iconv :: 文字コードを変換するコマンド  
・ASCII       :: 7ビットで表される基本的な128種類の文字(英数字+α)  
・ISO-8859    :: ASCIIを拡張した8ビットの文字コードで256種類の文字を表現する文字コード  
・UTF-8       :: Unicodeを使った文字コードで、1文字を1バイトから6バイトで表す。Linux標準。
・EUC-JP      :: 日本語EUC。UNIX環境で標準的に利用されていた日本語の文字コード  
・ShiftJIS    :: Windowsで利用される日本語の文字コード  
・ISO-2022-JP :: 電子メールなどで利用される日本語の文字コード(JISコード)  
・Unicode     :: 多言語を扱うために作成された符号化文字集合。符号化方式には、UTF-8,UTF-16,UTF-32等がある。

・UTC(Coordinated Universal Time) :: 協定世界時。各地の時間を決める際の基準となるもの。
・JST(Japan Standard Time)        :: 日本標準時。UTCより9時間進んでいます。
・/etc/localtime :: システムクロックの時刻からローカルタイムへ変換するための時差情報が格納されている。  
・/etc/timezone  :: システムで使用するタイムゾーンを指定するテキストファイル。  
・/usr/share/zoneinfo/ :: タイムゾーンの情報がバイナリファイルとして格納されているディレクトリ。
※システムで使用したいファイルを「/etc/localtime」へコピーまたはシンボリックリンクを作成することで反映される。
・環境変数TZ :: タイムゾーンの設定に使用される環境変数。
・tzselect :: 一覧から表示される情報をもとにタイムゾーンを設定できるコマンド。  
              環境変数「TZ」や「/etc/timezone」ファイルで指定するタイムゾーンの値を確認できる。
・tzconfig :: タイムゾーンを設定できるコマンド。「/etc/localtime」と「/etc/timezone」の値をまとめて変更する。  

※エポック(epoch)時間 :: UTC 1970年1月1日午前0時0分0秒からの経過秒数。
※タイムゾーンの設定はエポック時間からローカルの時間に変換するときに参照される。
```

---

## 小豆本問題

---

### 1 ×

ユーザーアカウント[jupiter]を新しく作成したい。  
ホームディレクトリは[/home/plane]、デフォルトシェルは[/bin/zsh]として作成する場合のコマンド

→  
× `useradd -m /home/planet -s /bin/zsh jupiter`  
○ `useradd -d /home/planet -s /bin/zsh jupiter`  

ホームディレクトリの指定は-dでした。  
-mはホームディレクトリを自動的に生成するオプションです。  
でも、全体的にいい線は言っていたのではないでしょうか。  

---

### 2 ×

ユーザーuser07は現在group07とgroup08の2つのグループに所属している。  
更に既存のグループgroup09にも所属させたい場合、実行すべきコマンドは？  

→  
-gか-Gか。  
どちらかはメインを変更せず、追加するだけだった気がする。  
最後にユーザーを指定する構文なので、選択肢的に`usermod -g group09 user07`しかないなぁ。  

→  
まさかの選択肢C`usermod -G group07,group08,group09 user07`だった。  
[-G]はプライマリグループは変更しないで所属するグループを変更するオプションだった。  
複数のグループにいる状態で更に追加したい場合は、今いるグループもすべて記述しないといけないっぽい。  

---

### 3 ○

ユーザーアカウントsaturnを削除すると同時に、そのユーザーが使っていたホームディレクトリも削除したい。  
適切なオプションとコマンドを記述せよ。  

→  
`userdel -r saturn`  

これはあたり。  
削除はremoveっぽいので-rで正解。  
ホームディレクトリの引数は特に必要ない。  

---

### 4 ×

ユーザーを新規に作成する際、新たに作成されるホームディレクトリの中に、デフォルトで必要とされるいくつかのファイルをコピーする必要がある。  
ユーザーの作成と同時に自動的にそれらのファイルがユーザーのホームディレクトリ内にコピーされるようにするには、ひな形となるファイルをどのディレクトリに配置すれば良いか？  
ディレクトリ名を絶対パスで記述せよ。  

→  
`/etc/skld`  

あー。おしい。
sk~~なんだっけ？ってところまではいい線言ってた。  

skelの意味はどうやらskeltonという意味らしい。  
skeltonは骸骨のイメージがあるが、それ以外にも骨組みや必要最小限という意味もあるみたいね。  

---

### 5 ○

crontabコマンドを用いて、毎月10日の午後5時10分に/home/myhome/myscriptが実行されるように死体。  

→  
D. `10 17 10 * * /home/myhome/myscript`  

当たった。  
秒は指定出来ず、分から徐々に大きくなって行くのは覚えていた。  
月まではいいとして、最後は曜日だったか？  
うん、曜日だった。  

---

### 6 ×

atコマンドを利用しようとしている。  
/etc/at.allowファイルは存在せず、/etc/at.denyファイルは存在下が、中には何も記述されていない。  
この時の説明は？すべて選択。  

→  
B. rootユーザーだけが使える。  

それ以外にもA. すべての一般ユーザーはatコマンドを利用できる も正解らしい。  
あれ？そうだったっけ？  
あー。すべてのファイルがない場合の話だ。  
denyがあって何も記述が無かったら、確かに全員使えるわな。  

---

### 7

日付や時刻の表記を規定する変数  

→  
あれー。何だっけ。  
普通に考えて`LC_FORMAT`  

はい。`LC_TIME`だそうです。  
LC_FORMATなるものは存在しません。  

---

### 8 ○  

システムに設定されているロケールを表示するコマンド  

→  
locale  

まぁ、ロケール言ってるんだからこれしかないよね。  
引数なしでlocaleコマンドを実行すると、システムに設定されているロケールが表示されるらしい。  

---

### 9 ○

タイムゾーンを日本に設定しようとしている。下線部に当てはまるコマンド。  
`# ___ -s /usr/share/zoneinfo/Asia/Tokyo /etc/localtime`  

→  
忘れた。環境変数はTZと覚えているのだが。  
タイムゾーンだから普通にtimezoneってやりたいけどそんなコマンドあったっけ？  
いや、違う。シンボリックリンクを通すかコピーするかって話だった気がする。
だったらリンクを作るコマンドなので `ln`か？  

いやー、覚えてるもんだね。  
因みにタイムゾーンを設定するコマンドはlocaltimeだそうです。  
全然覚えてない。  
