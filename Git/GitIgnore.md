# gitignore

---

## .gitignoreが反映されない

キャッシュを消すべし  

`git rm -r --cached .`

[.gitignoreに記載したのに反映されない件](https://qiita.com/fuwamaki/items/3ed021163e50beab7154)

---

## gitignoreでディレクトリを除外する方法

`*/ディレクトリ名`  

例  
以下のようなフォルダ構成があった時にassetsディレクトリ以下を除外したい場合。  

``` txt : フォルダ構成
src
├─assets ← このディレクトリの中身を対象とさせない
├─components
├─constants
├─router
├─scripts
│  ├─api
│  ├─services
│  └─storage
├─types
│  └─api
└─views
```

特定のディレクトリを無視する場合は先頭と末尾に/を付ける

``` txt : gitignore
/assets/
```

[[Git] .gitignoreの仕様詳解](https://qiita.com/anqooqie/items/110957797b3d5280c44f)  

---

## ignoreのignore

`!`マークで否定する。  

例：  
全てのdmfファイルを追跡対象としないが、`.gitignore`ファイルがあるディレクトリから辿って、`/TekitouCRUD/Shared/Database`ディレクトリにある`Database1.mdf`と`Database1_log.ldf`は追跡対象とする。  

``` bash
# SQL Server files
*.mdf
!/TekitouCRUD/Shared/Database/Database1.mdf
*.ldf
!/TekitouCRUD/Shared/Database/Database1_log.ldf
*.ndf
```

``` txt
TekitouCRUD
├─TekitouCRUD
│  └─Shared
│      └─Database
│          ├─Database1.mdf
│          └─Database1_log.ldf
└.gitignore
```

[.gitignoreで無視フォルダの中の特定のファイルを除外「!（否定文）」を使う場合の注意点](https://blog.s-giken.net/393.html)  
