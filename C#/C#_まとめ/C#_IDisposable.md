# IDisposable

---

■IDisposableインターフェース  
「使い終わったらリソースを開放する必要がある」ということを表現するためにあります。  
で、このIDisposableインターフェースには、「使い終わったリソースを開放する」ためのDispose()というメソッドが1つあるだけです。  

マネージリソース：ガベージコレクションがメモリを管理してくれるリソース  
アンマネージリソース：ガベージコレクションは何もしてくれない。開発者が責任を持ってメモリを管理する必要があるリソース。  
<http://divakk.co.jp/aoyagi/csharp_tips_using.html>  

■using  
>自動的にDispose()を呼び出してくれ、しかも例外にも対応してくれる便利な構文があります。それがusingです。  
<https://code.msdn.microsoft.com/windowsdesktop/11-Using-7483c4a0>  

https://anderson02.com/cs/cskiso/cskisoniwari-24/
