import { SaveSlipRequest } from "./SaveSlipRequest";

/**
 * カートアイテム
 */
export class CartItem {
  id!: number;
  encryptedAccountNo!: string;
  accountNo!: string;
  playerName!: string;
  menuLargeClassCD!: number;
  menuClassCD!: number;
  menuCD!: number;
  menuName!: string;
  price!: number;
  quantity!: number;
  personCount!: number;
  request!: string;
  SlipRequestList: SaveSlipRequest[] = [];
  /**
   * コンストラクタ
   * @param init
   */
  constructor(init?: Partial<CartItem>) {
    Object.assign(this, init);
  }
}
