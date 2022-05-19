<template>
  <b-container fluid class="wrapper d-flex flex-column">
    <b-row>
      <Header ref="header" />
    </b-row>
    <b-row class="flex-grow-1">
      <b-container fluid class="main d-flex flex-column">
        <b-row class="border border-dark border-title">
          <span v-html="environments.DISP_CONFORDER_1"></span>
        </b-row>
        <b-row class="row-menuList">
          <b-col class="group" v-if="cartItemList.length > 0">
            <b-row
              class="row-menu flex-column"
              v-for="(item, index) of cartItemList"
              v-bind:key="index"
            >
              <b-row class="row-menu-detail0 flex-row">
                <b-col class="menu-name">{{ item.menuName }}</b-col>
                <b-col class="menu-func">
                  {{ (item.price * item.quantity).toLocaleString() }}円
                  <b-icon-trash-fill
                    class="icon-trash"
                    scale="1.5"
                    @click="removeMenu(item.id)"
                  >
                  </b-icon-trash-fill>
                </b-col>
              </b-row>
              <b-row class="row-menu-detail1 flex-row">
                <b-col class="menu-request" v-if="item.request"
                  >※{{ item.request }}</b-col
                >
                <b-col class="menu-request" v-else>&nbsp;</b-col>
              </b-row>
              <b-row class="row-menu-detail2 flex-row">
                <b-col class="menu-order" v-if="multiMode">
                  <b-button
                    variant="primary"
                    @click="showSelectPlayerDialog(item)"
                    v-text="'注文者変更'"
                  />
                  &nbsp;
                  {{ '【' + item.accountNo + '】' + item.playerName }}&nbsp;様
                </b-col>
              </b-row>
            </b-row>
          </b-col>
          <b-col v-else>
            <span v-html="environments.DISP_CONFORDER_2"></span>
          </b-col>
        </b-row>
        <b-row class="bottom" v-if="encryptRepreAccountNo">
          <b-col>
            <b-button variant="link" @click="backTo()" v-text="'戻る'" />
          </b-col>
          <b-col>
            <b-button
              block
              variant="primary"
              v-bind:disabled="
                cartItemList.length == 0 || !encryptRepreAccountNo
              "
              @click="makeOrder()"
              v-text="'注文する'"
            />
          </b-col>
        </b-row>
        <b-row v-else>
          <b-col>
            <b-button
              variant="primary"
              block
              @click="moveToScanQR()"
              v-text="'ホームに戻る'"
            />
          </b-col>
        </b-row>
      </b-container>
    </b-row>
    <!-- 選択ボタン押下時のオーバーレイ -->
    <SelectPlayerDialog
      ref="selectPlayerDialog"
      :message="'変更する方を選択してください。'"
      :playerList="contextPlayerList"
      :SingleSelectMode="true"
      @confirm="changeOrderPlayer"
    />
    <b-overlay
      :show="isBusy"
      variant="dark"
      opacity="0.5"
      blur="None"
      no-wrap
    />
  </b-container>
</template>

<script lang="ts">
import { Component } from 'vue-property-decorator';
import { ViewBase } from '@/views/ViewBase';
import Header from '@/components/Header.vue';
import SelectPlayerDialog from '@/components/SelectPlayerDialog.vue';
import { SaveSlipRequest } from '@/types/request/SaveSlipRequest';
import { SaveSlipHeader } from '@/types/entity/SaveSlipHeader';
import { SaveSlipDetail } from '@/types/entity/SaveSlipDetail';
import { CartItem } from '@/types/entity/CartItem';
import { namespace } from 'vuex-class';
import { Player } from '@/types/entity/Player';
import { SelectPlayerDialogContext } from '@/types/context/SelectPlayerDialogContext';

const CommonModule = namespace('common');

/**
 * 注文確認画面
 */
@Component({
  components: {
    Header,
    SelectPlayerDialog
  }
})
export default class ConfirmOrder extends ViewBase {
  @CommonModule.State
  private cartItemList!: CartItem[];
  @CommonModule.State
  private multiMode!: boolean;
  @CommonModule.State
  private patternCD!: number;
  @CommonModule.State
  private tableNo!: number;
  @CommonModule.State
  private businessDate!: Date;
  @CommonModule.State
  private playerList!: Player[];
  @CommonModule.State
  private encryptRepreAccountNo!: string;
  /** 状態管理:暗号化されたWeb会員CD */
  @CommonModule.State
  private encryptWebMemberCD!: string;

  @CommonModule.Action
  private updateCart!: (value: CartItem) => void;
  @CommonModule.Action
  private removeCart!: (value: CartItem) => void;

  /** 注文者変更プレーヤーダイアログへのコンテキスト */
  private contextPlayerList: Array<SelectPlayerDialogContext> = [];
  /** 注文者変更ボタンで選択した注文 */
  private selectedCartItem!: CartItem;

  /**
   * ライフサイクルフックmounted
   */
  async mounted(): Promise<void> {
    try {
      await super.mounted();
      // 必要な情報が存在しない場合、処理しない。
      if (!this.encryptRepreAccountNo) {
        this.toast.error('セッションが切れました。');
        return;
      }
      if (this.cartItemList.length == 0) {
        this.toast.error('商品がありません。');
        return;
      }
    } catch (error) {
      console.error(error);
      this.toast.error('エラーが発生しました。スタッフをお呼びください。');
      return;
    }
  }

  /**
   * 注文者変更ボタンクリック処理
   */
  private showSelectPlayerDialog(item: CartItem): void {
    // 選択したメニューを保持する
    this.selectedCartItem = item;
    // オーバーレイコンテキストの生成
    this.contextPlayerList = this.playerList
      .filter(f => f.AccountNo)
      .map(
        player =>
          new SelectPlayerDialogContext({
            encryptedAccountNo: player.EncryptedAccountNo,
            accountNo: player.AccountNo,
            playerName: player.Name,
            isSelected: player.EncryptedAccountNo == item.encryptedAccountNo
          })
      );
    // オーバーレイの表示
    (this.$refs.selectPlayerDialog as SelectPlayerDialog).show();
  }

  /**
   * 注文者を変更します。
   */
  private changeOrderPlayer(
    playerList: Array<SelectPlayerDialogContext>
  ): void {
    // 選択した注文の顧客情報をダイアログで選択した人に変更する。
    const selectedPlayer = playerList.filter(f => f.isSelected)[0];
    this.selectedCartItem.encryptedAccountNo =
      selectedPlayer.encryptedAccountNo;
    this.selectedCartItem.accountNo = selectedPlayer.accountNo;
    this.selectedCartItem.playerName = selectedPlayer.playerName;
    // カートの更新
    this.updateCart(this.selectedCartItem);
    // オーバーレイを閉じる
    (this.$refs.selectPlayerDialog as SelectPlayerDialog).hide();
  }

  /**
   * 注文ボタン押下処理
   * オーダー送信を行う
   */
  private async makeOrder() {
    // カートに1件もなければ警告を出して押せなくする
    if (this.cartItemList.length == 0) {
      this.toast.warning('メニューを選択してください。');
      return;
    }
    // クルクル表示
    this.isBusy = true;
    // リクエストの生成
    const request = this.createSaveSlipRequest(this.cartItemList);
    // 伝票を保存する
    await this.selfOrderService
      .save(request, this.encryptWebMemberCD)
      .then(
        // 成功したら注文完了画面
        response => this.moveToThanksForOrders(),
        reject => this.toast.error(reject.data)
      )
      .catch(error => {
        console.error(error);
        this.toast.error('エラーが発生しました。スタッフをお呼びください。');
      })
      .finally(() => (this.isBusy = false));
  }

  /**
   * リクエストを生成します。(保存する伝票情報の作成)
   */
  private createSaveSlipRequest(cartItemList: CartItem[]): SaveSlipRequest {
    // 端末情報を取得しておく
    let userAgentname = this.getUserAgentname();
    // リクエストの一番上の要素を定義
    let request = new SaveSlipRequest();
    // カートの内容を回す
    cartItemList.forEach(cart => {
      // 一番下の要素
      let saveSlipDetail = new SaveSlipDetail();
      saveSlipDetail.MenuClassCD = cart.menuClassCD;
      saveSlipDetail.MenuCD = cart.menuCD;
      saveSlipDetail.HandwritingNote = null;
      saveSlipDetail.Quantity = cart.quantity;
      saveSlipDetail.SlipRequestList = cart.SlipRequestList;
      // 中間の要素
      let slipHeaderList = new SaveSlipHeader();
      slipHeaderList.EncryptedAccountNo = cart.encryptedAccountNo;
      slipHeaderList.TerminalNumber = userAgentname;
      slipHeaderList.PatternCD = this.patternCD;
      slipHeaderList.MenuLargeClassCD = cart.menuLargeClassCD;
      slipHeaderList.TableCD = this.tableNo;
      slipHeaderList.PersonCount = cart.personCount;
      slipHeaderList.BusinessDate = this.businessDate;
      slipHeaderList.SlipDetailList.push(saveSlipDetail);
      // 一番上の要素に入れる。
      request.SlipHeaderList.push(slipHeaderList);
    });

    return request;
  }

  /**
   * メニューリストから削除する
   */
  private removeMenu(id: number) {
    // 表示部の削除
    const removeTarget = this.cartItemList.filter(m => m.id === id)[0];
    // カートの削除
    this.removeCart(removeTarget);
  }

  /**
   * ユーザーエージェント名を取得します。
   */
  private getUserAgentname(): string {
    const userAgent = window.navigator.userAgent.toLowerCase();

    if (userAgent.indexOf('iphone') != -1) {
      return 'iPhone';
    } else if (userAgent.indexOf('ipad') != -1) {
      return 'iPad';
    } else if (userAgent.indexOf('android') != -1) {
      return 'Android';
    } else {
      //OSの種類
      return navigator.platform;
    }
  }
}
</script>

<style scoped>
.border-title {
  min-height: 5%;
  max-height: 5%;
  text-align: left;
  align-items: center;
  padding: 0vh 0vw 0vh 2vw;
}
.row-menuList {
  min-height: 85%;
  max-height: 85%;
  overflow-y: auto;
}
.row-menuList .group {
  padding: 0vh 0vw 0vh 0vw;
  margin: 1vh 3vw 1vh 3vw;
  max-width: calc(100% - 6vw);
}
.row-menu {
  min-height: flex;
  max-height: flex;
  white-space: nowrap;
  box-shadow: 0px 0px 5px;
  border: 1px solid gray;
  border-radius: 10px;
  padding: 1vh 2vw;
  margin: 0vh 0vw 1vh 0vw;
}
.row-menu-detail0 {
  min-height: 6%;
  max-height: 12%;
  min-width: 100%;
  max-width: 100%;
  padding: 0vh 0vw 0vh 0vw;
  margin: 0vh 0vw 0vh 0vw;
}

.lbl-menu-name {
  text-align: left;
  white-space: nowrap;
  position: relative;
}

/* メニュー名 */
.menu-name {
  text-align: left;
  position: relative;
  padding: 0vh 0vw 0vh 0vw;
  margin: 0vh 0vw 0vh 0vw;
}

.row-menu-detail1 {
  min-height: 7%;
  max-height: 14%;
  max-width: 100%;
  padding: 0vh 0vw 0vh 0vw;
  margin: 0vh 0vw 0vh 0vw;
}
/* ご要望 */
.menu-request {
  text-align: left;
  position: relative;
  padding: 0vh 0vw 0vh 0vw;
  margin: 1vh 0vw 0vh 0vw;
  white-space: normal;
  word-break: keep-all;
}
/* 数量変更ボタン */
.b-form-spinbutton >>> .btn {
  color: white;
  background-color: #007bff;
}
.row-menu-detail2 {
  min-height: 7%;
  max-height: 7%;
  min-width: 100%;
  max-width: 100%;
  padding: 0vh 0vw 0vh 0vw;
  margin: 0vh 0vw 0vh 0vw;
}
/* 注文削除アイコン */
.icon-trash {
  text-align: right;
  margin: 0vh 0vw 0vh 2vw;
}
/* 合計のラベル */
.menu-order {
  align-self: center;
  position: relative;
  text-align: left;
  padding: 0vh 0vw 0vh 3vw;
  margin: 1vh 0vw 0vh 0vw;
}
/* 金額(合計) */
.menu-func {
  position: relative;
  text-align: right;
  align-self: center;
  padding: 0vh 0vw 0vh 5vw;
  margin: 0vh 0vw 0vh 0vw;
}

.bottom {
  min-height: 10%;
  max-height: 10%;
  align-content: flex-end;
  padding: 0vh 0vw 1vh 0vw;
}
</style>
