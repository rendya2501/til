import { OrderMenu } from '@/types/entity/OrderMenu';
import { OrderRequest } from '@/types/entity/OrderRequest';
import { Player } from '@/types/entity/Player';
import { MutationTree } from 'vuex';
import { CommonState } from '../types';
import { CartItem } from '@/types/entity/CartItem';
import { MenuResponse } from '@/types/response/MenuResponse';

const mutations: MutationTree<CommonState> = {
  /** 初期化処理 */
  initialize: state => {
    state.patternCD = null;
    state.tableNo = null;
    state.businessDate = null;
    state.encryptRepreAccountNo = null;
    state.playerList = [];
    state.multiMode = false;
    state.menu = null;
    state.selectedMenu = null;
    state.targetRequest = [];
    state.cartItemList = [];
  },
  /** パターンコードをセットします。 */
  setPatternCD: (state, value: number) => {
    state.patternCD = value;
  },
  /** テーブル番号をセットします。 */
  setTableNo: (state, value: number) => {
    state.tableNo = value;
  },
  /** 営業日をセットします。 */
  setBusinessDate: (state, value: Date) => {
    state.businessDate = value;
  },
  /** 暗号化されたWeb会員番号をセットします。 */
  setEncryptWebMemberCD: (state, value: string) => {
    state.encryptWebMemberCD = value;
  },
  /** ログイン顧客の暗号化された会計Noをセットします。 */
  setEncryptRepreAccountNo: (state, value: string) => {
    state.encryptRepreAccountNo = value;
  },
  /** プレーヤー一覧をセットします。 */
  setPlayerList: (state, value: Player[]) => {
    state.playerList = value;
  },
  /** 複数選択モードをセットします。 */
  setMultiMode: (state, value: boolean) => {
    state.multiMode = value;
  },
  /** メニューをセットします。 */
  setMenu: (state, value: MenuResponse) => {
    state.menu = value;
  },
  /** 選択したメニューをセットします。 */
  setSelectedMenu: (state, value: OrderMenu) => {
    state.selectedMenu = value;
  },
  /** 選択したメニューをクリアします。 */
  clearSelectedMenu: state => {
    state.selectedMenu = null;
  },
  /** オーダー要望をセットします。 */
  setTargetRequest: (state, value: OrderRequest[]) => {
    state.targetRequest = value;
  },
  /** オーダー要望をクリアします。 */
  clearTargetRequest: state => {
    state.targetRequest = [];
  },
  /** カートに商品をセットします。 */
  setCart: (state, value: CartItem[]) => {
    state.cartItemList = value;
  },
  /** カートに商品を追加します。 */
  addCart: (state, value: CartItem) => {
    const max = state.cartItemList.length;
    value.id = max + 1;
    state.cartItemList.push(value);
  },
  /** カートの情報を更新します。 */
  updateCart: (state, value: CartItem) => {
    const index = state.cartItemList.findIndex(item => item.id === value.id);
    state.cartItemList[index] = value;
  },
  /** カートの情報を削除します。 */
  removeCart: (state, value: CartItem) => {
    state.cartItemList = state.cartItemList.filter(cart => cart != value);
  },
  /** カートをクリアします。 */
  clearCart: state => {
    state.cartItemList = [];
  }
};

export default mutations;
