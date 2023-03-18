# VSCode関係

## コンソールプロジェクトでVSCodeのデバッグでコンソールの入力を有効にする方法

`.vscode`ディレクトリ配下にある`launch.json`の`"console":"internalConsole"`の部分を`"console":"externalTerminal"`に書き換える。  
削除してから`ctrl + space`で候補が出てくるので、覚えていれば入力に迷うことはないと思われる。  

[Visual Studio Code で Console.ReadLineを使う方法](https://qiita.com/link_to_someone/items/2b7cb8747a34165b8c8e)  
