import { CartItem } from '@/types/entity/CartItem';
import { OrderMenu } from '@/types/entity/OrderMenu';
import { OrderRequest } from '@/types/entity/OrderRequest';
import { Player } from '@/types/entity/Player';
import { MenuResponse } from '@/types/response/MenuResponse';
import { ActionTree } from 'vuex';
import { CommonState, RootState } from '../types';

const actions: ActionTree<CommonState, RootState> = {
  /** 初期化処理 */
  initialize: ({ commit }) => {
    commit('initialize');
  },
  /** パターンコードをセットします。 */
  setPatternCD: ({ commit }, value: number) => {
    commit('setPatternCD', value);
  },
  /** テーブル番号をセットします。 */
  setTableNo: ({ commit }, value: number) => {
    commit('setTableNo', value);
  },
  /** 営業日をセットします。 */
  setBusinessDate: ({ commit }, value: number) => {
    commit('setBusinessDate', value);
  },
  /** 暗号化されたWeb会員番号をセットします。 */
  setEncryptWebMemberCD: ({ commit }, value: string) => {
    commit('setEncryptWebMemberCD', value);
  },
  /** ログイン顧客の暗号化された会計Noをセットします。 */
  setEncryptRepreAccountNo: ({ commit }, value: string) => {
    commit('setEncryptRepreAccountNo', value);
  },
  /** プレーヤー一覧をセットします。 */
  setPlayerList: ({ commit }, value: Player[]) => {
    commit('setPlayerList', value);
  },
  /** 複数選択モードをセットします。 */
  setMultiMode: ({ commit }, value: boolean) => {
    commit('setMultiMode', value);
  },
  /** メニューをセットします。 */
  setMenu: ({ commit }, value: MenuResponse) => {
    commit('setMenu', value);
  },
  /** 選択したメニューをセットします。 */
  setSelectedMenu: ({ commit }, value: OrderMenu) => {
    commit('setSelectedMenu', value);
  },
  /** 選択したメニューをクリアします。 */
  clearSelectedMenu: ({ commit }) => {
    commit('clearSelectedMenu');
  },
  /** オーダー要望をセットします。 */
  setTargetRequest: ({ commit }, value: OrderRequest[]) => {
    commit('setTargetRequest', value);
  },
  /** オーダー要望をクリアします。 */
  clearTargetRequest: ({ commit }) => {
    commit('clearTargetRequest');
  },
  /** カートに商品をセットします。 */
  setCart: ({ commit }, value: CartItem[]) => {
    commit('setCart', value);
  },
  /** カートに商品を追加します。 */
  addCart: ({ commit }, value: CartItem) => {
    commit('addCart', value);
  },
  /** カートの情報を更新します。 */
  updateCart: ({ commit }, value: CartItem) => {
    commit('updateCart', value);
  },
  /** カートの情報を削除します。 */
  removeCart: ({ commit }, value: CartItem) => {
    commit('removeCart', value);
  },
  /** カートをクリアします。 */
  clearCart: ({ commit }) => {
    commit('clearCart');
  }
};

export default actions;
