/**
 * メニュー取得リクエスト
 */
export class MenuRequest {
  /**
   * パターンコード
   */
  public PatternCD?: number;
  
  /**
   * コンストラクタ
   * @param patternCD
   */
  constructor(patternCD: number) {
    this.PatternCD = patternCD;
  }
}
