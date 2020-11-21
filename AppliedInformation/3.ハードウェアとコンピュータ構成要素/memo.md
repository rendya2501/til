# 第3章 ハードウェアとコンピューター構成要素

## 3.1

## 3.2 プロセッサアーキテクチャ

- CISC(Complex Instruction Set Computer) : 複合命令セット
  - 多数の高機能なマイクロ命令
  - パイプラインには不向き
  - ステップ数は少ない

- RISC(Reduced Instruction Set Computer) : 縮小命令セット
  - 少数で単純な命令
  - パイプライン向き
  - ステップ数多い

IC(integrated circuit) 集積回路
ALU(Arithmetic and Logic Unit) 算術論理演算装置

### 命令の実行

1. 命令フェッチ
2. 命令解読(デコード)
3. 処理対象データ(オペランド)のアドレス計算
4. オペランドフェッチ
5. 命令の実行

  aa

パイプライン


