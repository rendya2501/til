# OSのバージョン確認方法

1. /etc/**-release ファイルを確認する。  

    各種Linuxのバージョンによって`**`の部分が違う。  

    - CentOSの場合

    ``` bash
    cat /etc/redhat-release
    ```

    - AlmaLinuxの場合

    ``` bash
    cat /etc/os-release
    ```

2. lsb_releaseコマンドを使用する

    ``` bash
    lsb_release -a
    ```
