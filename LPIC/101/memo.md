gzip
-d decompress
-c output
-r recursive

bzip2
-d decompress
-c output

xz
-d decompress
-l list
-k keep


ls
-a
-A
-d
-i
-l
-F
-t
-h
-R


sort
-b 
-f 
-r 
-n 
-k 
-t 


grep
-c 
-i 
-f 
-n 
-v 
-E 
-F 



tar
-c
-x
-t
-v
-f

-z
-j
-J

cpio
-i
-o

cpio -o < backup.cpio

dd
if
of
byte
count

ps
a 全てのユーザー
u プロセスの実行ユーザー名も表示する
x デーモンも含める
f 親子関係をツリー上に表示
-e 全てのプロセスを表示
-f 完全フォーマット表示
-l 詳細表示
-p 指定したpidプロセス情報のみ表示
-C プロセス名 指定したプロセス名のプロセスのみ表示
-w 長い行は折り返して表示



メタキャラクタ
\* 0文字以上
? 任意の1文字

viエディタを読み取り専用で開くオプション
-R

whatisデータベースを作成するコマンド
makewhatis

tailやwcなどバイトサイズの指定オプションは大抵cである。




mount
-a /etc/fstabに記述のあるすべてのファイルシステムをアンマウントする
-t ファイルシステムの指定
-o マウントオプションの指定

  ディレクトリ「dir1」をディレクトリ「dir2」にマウントしたい。下線部に入るオプションは？
  mount __________ dir1 dir2
-- bind 

umount
-a /etc/mtabに記述のあるすべてのファイルシステムをアンマウントする
-t 指定したファイルシステムだけ餡マウント


マウントオプション
async ファイルシステムの非同期入出力を設定
auto -aで自動マウント
noauto -aで自動マウントしない
defaults async,auto,dev,exec,nouser,rw,suid
exec バイナリの実行を許可
noexec バイナリの実行を不許可
ro 読み取り専用でマウント
rw 書き込み許可でマウント
unhide 隠しファイルも表示
suid SUIDとGUIDを有効にする
user 一般ユーザーでもマウント可能
nouser 一般ユーザーのマウント不許可
users マウントしたユーザー以外のユーザーもアンマウント可能

defaultはnouser以外は肯定的。

