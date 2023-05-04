# Apacheメモ

- 設定ファイルの一覧の取得  

    ``` bash
    httpd -t -D DUMP_CONFIG 2>/dev/null | grep '# In' | awk '{print $4}'
    ```

[Apache の情報をコマンドラインで取得する - Qiita](https://qiita.com/bezeklik/items/7bbfcbcfeb05c1d57fec)  
