/**
 * 保存用伝票要望
 */
export class SaveSlipRequest {
  /**
   * 要望コード
   * int
   */
  public RequestCD!: number;
  /**
   * 数量
   * decimal?
   */
  public Quantity!: number;
}
