/**
 * スタッフ呼出リクエスト
 */
export class CallStaffRequest {
  /**
   * パターンコード
   */
  PatternCD: number;
  /**
   * テーブルコード
   */
  TableCD: number;
  /**
   * 暗号化された会計No
   */
  EncryptedAccountNo: string;
  /**
   * 呼び出し理由
   */
  Reason: string;

  /**
   * コンストラクタ
   * @param patternCD パターンコード
   * @param tableCD テーブルコード
   * @param encryptedAccountNo 暗号化された会計No
   * @param reason 呼び出し理由
   */
  constructor(
    patternCD: number,
    tableCD: number,
    encryptedAccountNo: string,
    reason: string
  ) {
    this.PatternCD = patternCD;
    this.TableCD = tableCD;
    this.EncryptedAccountNo = encryptedAccountNo;
    this.Reason = reason;
  }
}
