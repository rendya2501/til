import { Module } from 'vuex';
import { CommonState, RootState } from '../types';
import state from './state';
import actions from './actions';
import mutations from './mutations';
import { HeaderContext } from '@/types/context/HeaderContext';

export const common: Module<CommonState, RootState> = {
  namespaced: true,
  state,
  actions,
  mutations,
  getters: {
    getHeaderContext: state => {
      let accountNo = '';
      let playerName = '';
      let totalPrice = 0;
      let totalQuantity = 0;
      // シングルモードであればログインしたプレーヤーが必要。
      if (!(state?.multiMode ?? true)) {
        const reprePlayer = state?.playerList?.find(
          f => f.EncryptedAccountNo == state?.encryptRepreAccountNo
        );
        accountNo = reprePlayer?.AccountNo ?? '';
        playerName = reprePlayer?.Name ?? '';
      }
      // カート情報の取得
      state?.cartItemList?.forEach(cart => {
        totalPrice += cart.quantity * cart.price;
        totalQuantity += cart.quantity;
      });
      return new HeaderContext({
        AccountNo: accountNo,
        PlayerName: playerName,
        TotalPrice: totalPrice,
        TotalQuantity: totalQuantity
      });
    }
  }
};
