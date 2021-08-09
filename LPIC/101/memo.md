
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

cp
-f
-i
-p
-r,-R
-d
-a


mv
-f
-i


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
[] []の文字いずれか
{} ,で区切られた文字列にマッチする。test{1,2}はtest1,test2

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


3章のファイル操作系

4章のコマンドすべてを再復習

→こいつらやってから紙の模試をやってみる。




ls
-a
-A
-i
-l
-d
-R
-F
-t
-h


cp
-i
-f
-a = rpd
-r,-R
-p
-d


mv
-i
-f


mkdir
-p 
-m 

rm
-i
-f
-r,-R

rmdir
-p


touch
-a
-m
-t

cat
-n

nl
-b 本文に行番号付加
-h 
-f
n nonumber 行番号の表示を停止
a 全ての行
t 空行以外


od
-t
o
x
c

head
-行数
-n 行数
-c バイト数

tail
-n 行数
-行数
-f
-c バイト数

cut
-c 文字位置
-d デリミタ指定
-f フィールド指定

paste
-d

tr
-d マッチした文字列の削除
-s 連続するパターンを1文字に置き換える
文字クラス

sort
-b 行頭の空白は無視する
-f 大文字小文字を区別しない
-r 降順
-n 数値として処理
-k フィールド指定
-t デリミタ指定

split
デフォは1000
- 行数

uniq
-d 重複した行を表示
-u 重複していない行を表示
デフォ 重複した行を1行にする

wc
-w 単語数
-l 行数
-c バイト数(文字数)

xargs

. 任意の1文字
\* 直前の文字0回以上の繰り返し
[]
^
$
\
+
?
|
｛n｝
｛n,m｝

grep
-c マッチした行数を表示
-f パターンをファイルから取得
-i 大文字小文字を区別しない
-n 行番号を表示
-v
-E
-F


sed
s///g
y//
//d
-e


tar
-c
-x
-f
-t
-z
-j
-J


cpio
-i
-o

dd
if
of
count
bs

gzip
-c
-d
-k

bzip2
-c
-d

xz
-l
-d
-k

ps
a
u
x
f
-e
-l
-f
-p
-C
-w
