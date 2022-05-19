import { OrderMenu } from '@/types/entity/OrderMenu';
import { OrderRequest } from '@/types/entity/OrderRequest';
import { Player } from '@/types/entity/Player';
import { CartItem } from '@/types/entity/CartItem';
import { MenuResponse } from '@/types/response/MenuResponse';

export interface RootState {
  version: string;
}

export interface CommonState {
  /** パターンコード */
  patternCD: number | null;
  /** テーブル番号 */
  tableNo: number | null;
  /** 営業日 */
  businessDate: Date | null;
  /** 暗号化されたWeb会員番号 */
  encryptWebMemberCD: string | null;
  /** ログイン顧客の暗号化された会計No */
  encryptRepreAccountNo: string | null;
  /** プレーヤー一覧 */
  playerList: Player[];
  /** 複数選択モード */
  multiMode: boolean;
  /** 統合メニュー情報 */
  menu: MenuResponse | null;
  /** 選択したメニュー */
  selectedMenu: OrderMenu | null;
  /** オーダー要望 */
  targetRequest: OrderRequest[];
  /** カートのアイテム */
  cartItemList: CartItem[];
}
