# Vue開発環境構築

VueにはClassStyleとObjectStyleの2つの記法がある模様。  
実務で使っていたのはClassStyle。  
これはプロジェクトを作る時のオプションで指定する必要がある。  

---

## Vueの最低限の実装とそこまでのまとめ

前提条件

1. Node.jsをインストール  
2. vue/cliをインストール  
   - `npm install -g @vue/cli`  

class styleでのプロジェクト作成の仕方  
Use class-style component syntax? をyesにすることで実務で開発した形にできる。
[【2021年版】Vue.js + TypeScriptの開発スタイル](https://tech-blog.rakus.co.jp/entry/20210901/frontend)  

TypeScriptの環境構築
[簡単な例で始めるVue3でTypeScript入門](https://reffect.co.jp/vue/vue3-typescript)  
[Vue CLI 3.0 で TypeScript な Vue.js プロジェクトをつくってみる](https://qiita.com/nunulk/items/7e20d6741637c3416dcd)  
[vue.js + typescript = vue.ts ことはじめ](https://qiita.com/nrslib/items/be90cc19fa3122266fd7)  

インストールして起動するだけならすぐにできるが、そこからtypescriptを使えるようにしたりいろいろやり始めると途端にうまく行かなくなる。  
何が原因で動かないのかすらわからないのに、文献はどれもバラバラ。  

---

## トラブルシューティング

### npmでpermission deniedになった時の対処法

[npmでpermission deniedになった時の対処法[mac]](https://qiita.com/okohs/items/ced3c3de30af1035242d)  

Node.jsとnpmを再インストールする（推奨）  
`npm install -g npm`  

### vue-property-decoratorがない

[Vue.jsでTypeScriptを使う(with vue-property-decorator)](https://qiita.com/paragaki/items/b3fc4b1bd334f54f33e0)  
→  
`npm install --save vue-property-decorator`
`npm i -S vue-property-decorator`  

[vue-property-decorator + TypeScriptで書かれたVueコンポーネントをscript setupで書き換える](https://zenn.dev/r57ty7/articles/53d189afa27aeb)  

どうやらvue3系ではvue-property-decoratorは必要なくなった模様。  
vue2系と違ってここら辺に変更が入ったっぽい？  
なぜvue-property-decoratorに固執するのかと言えば、実務でもそれを使っていたし、ほとんどのサイトでも使っているのを見るから。  
なんの考えもなしに3系で使おうとするとエラーになって動かせない。  
それについて解説しているところもどこにもない。  

vue2であれば[vue-property-decorator]はデフォルトであることを確認した。  

### 後からTypeScriptを足したい

[[Vue3.x]createを使って既存projectにTypeScriptを足してみた](https://zenn.dev/gamin/articles/57d7a1aec6dcb8)  

[はじめてのvue-property-decorator (nuxtにも対応）](https://qiita.com/simochee/items/e5b77af4aa36bd0f32e5)  
