/**
 * プレーヤーエンティティ
 */
export class Player {
  /**
   * 暗号化された会計No
   */
  EncryptedAccountNo: string = '';
  /**
   * 会計No
   */
  AccountNo: string = '';
  /**
   * 来場者名
   */
  Name: string = '';
  /**
   * 来場者カナ
   */
  Kana: string = '';
  /**
   * 精算フラグ
   */
  SettlementFlag: boolean = false;

  /**
   * コンストラクタ
   * @param init
   */
  constructor(init?: Partial<Player>) {
    Object.assign(this, init);
  }
}
