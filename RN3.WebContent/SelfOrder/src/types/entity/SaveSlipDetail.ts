import { SaveSlipRequest } from '@/types/entity/SaveSlipRequest';

/**
 * 保存用伝票明細
 */
export class SaveSlipDetail {
  /**
   * メニュー分類コード
   * int
   */
  public MenuClassCD!: number;
  /**
   * メニューコード
   * int
   */
  public MenuCD!: number;
  /**
   * 手書備考
   * byte[]
   */
  public HandwritingNote!: number[] | null;
  /**
   * 数量
   * decimal
   */
  public Quantity!: number;
  /**
   * 伝票要望一覧
   */
  public SlipRequestList: SaveSlipRequest[] = [];
}
