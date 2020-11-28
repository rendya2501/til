
# IISを使ってLaravelのプロジェクトをwebとして公開するまで

1. IISマネージャーを立ち上げる。  
   フォルダを右クリック、アプリケーションへの変換を選択。  

2. プログラムと機能のWindowsの機能の有効化または無効化より、Webに関係しそうな機能を選択してダウンロード  

3. 権限を設定。アプリケーション側の権限とphpフォルダの権限。  
   おもにIIS系とゲスト系。  
   ③、④はここを参照：<http://create-something.hatenadiary.jp/entry/2014/06/11/194808>

4. php.iniの設定。ネットを参考に2箇所のステータスを開放。  

5. URL書き換えツールをインストール  
   <https://www.microsoft.com/ja-jp/download/details.aspx?id=7435>
