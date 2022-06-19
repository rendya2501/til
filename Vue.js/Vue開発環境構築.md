
# Vue開発環境構築

VueにはClassStyleとObjectStyleの2つの記法がある模様。  
実務で使っていたのはClassStyle。  
これはプロジェクトを作る時のオプションで指定する必要がある。  
→  
vue typescript class style

---

## Vueの最低限の実装とそこまでのまとめ

1. Node.jsをインストール  
2. vue/cliをインストール  
   - `npm install -g @vue/cli`  
3. vueプロジェクトの生成  

TypeScriptの環境構築
[簡単な例で始めるVue3でTypeScript入門](https://reffect.co.jp/vue/vue3-typescript)  
[Vue CLI 3.0 で TypeScript な Vue.js プロジェクトをつくってみる](https://qiita.com/nunulk/items/7e20d6741637c3416dcd)  
[vue.js + typescript = vue.ts ことはじめ](https://qiita.com/nrslib/items/be90cc19fa3122266fd7)  

インストールして起動するだけならすぐにできるが、そこからtypescriptを使えるようにしたりいろいろやり始めると途端にうまく行かなくなる。  
何が原因で動かないのかすらわからないのに、文献はどれもバラバラ。  

---

## ASP.Net Core API + Vue axios 通信サンプル

[Vue3とaxiosで外部サイトにアクセス](https://akkunblog-happy-life.com/vue3-20/)  

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

[vue-property-decorator + TypeScriptで書かれたVueコンポーネントをscript setupで書き換える](https://zenn.dev/r57ty7/articles/53d189afa27aeb)  

### 後からTypeScriptを足したい

[[Vue3.x]createを使って既存projectにTypeScriptを足してみた](https://zenn.dev/gamin/articles/57d7a1aec6dcb8)  

[はじめてのvue-property-decorator (nuxtにも対応）](https://qiita.com/simochee/items/e5b77af4aa36bd0f32e5)  
