import { OrderMenuClass } from '@/types/entity/OrderMenuCls';
import { OrderMenu } from '@/types/entity/OrderMenu';
import { OrderRequest } from '@/types/entity/OrderRequest';

/**
 * メニュー取得レスポンス
 */
export class MenuResponse {
  /**
   * メニュー分類一覧
   */
  public MenuClassList: OrderMenuClass[] = [];
  /**
   * メニュー一覧
   */
  public MenuList: OrderMenu[] = [];
  /**
   * 要望一覧
   */
  public RequestList: OrderRequest[] = [];
}
