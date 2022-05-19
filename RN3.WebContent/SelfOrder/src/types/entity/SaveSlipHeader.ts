import { SaveSlipDetail } from '@/types/entity/SaveSlipDetail';

/**
 * 保存用伝票ヘッダー
 */
export class SaveSlipHeader {
  /**
   * 暗号化された会計No
   */
  public EncryptedAccountNo!: string;
  /**
   * 端末番号
   * Required,StringLength(15)
   */
  public TerminalNumber!: string | null;
  /**
   * パターンコード
   * int
   */
  public PatternCD!: number;
  /**
   * メニュー大分類コード
   * int
   */
  public MenuLargeClassCD!: number;
  /**
   * テーブルコード
   * int
   */
  public TableCD!: number;
  /**
   * 人数
   * decimal
   */
  public PersonCount!: number;
  /**
   * 営業日
   */
  public BusinessDate!: Date;
  /**
   * 伝票明細一覧
   */
  public SlipDetailList: SaveSlipDetail[] = [];
}
