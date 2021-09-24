# データベース

---

## 多対多の概念

<https://qiita.com/ramuneru/items/db43589551dd0c00fef9>  

---

## 正規化

<https://www.momoyama-usagi.com/entry/info-database-seikika>  
[うさぎでもわかる](https://rikulogger.com/db/nomalization/)  
[](https://rikulogger.com/db/nomalization/)

---

## 主キーはアップデート可能か？

<https://urashita.com/archives/33098>

可能。  
萬君に質問された。  
Teelaか何かでやったことがあるような気がしたが、まとめていなかったので自信を持って答えられなかったのでまとめ。  

<https://atmarkit.itmedia.co.jp/bbs/phpBB/viewtopic.php?topic=45369&forum=26>  
更新は可能だが、1レコードを特定するための情報が変更されてしまうので、運用が難しくなると思われる。  
後、そうしなきゃいけない地点で設計が悪い。  
マスターの主キーが更新された日には他の関連するテーブル全てを更新しなきゃいけなくなるわけだからね。  
