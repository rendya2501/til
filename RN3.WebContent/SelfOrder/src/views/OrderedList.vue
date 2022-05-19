<template>
  <b-container fluid class="wrapper d-flex flex-column">
    <b-row>
      <Header ref="header" />
    </b-row>
    <b-row class="flex-grow-1">
      <b-container fluid class="main d-flex flex-column">
        <b-row class="border border-dark border-title">
          <span v-html="environments.DISP_ORDERLIST_1"></span>
        </b-row>
        <b-row class="ordered">
          <b-col class="group">
            <b-row
              class="ordered-list flex-column"
              v-for="(ordered, index) in orderedList"
              :key="index"
            >
              <b-row class="item-detail0 flex-row">
                <b-col class="item-name">{{ ordered.menuName }}</b-col>
                <b-col class="item-total">
                  {{ ordered.totalPrice.toLocaleString() }}円
                </b-col>
              </b-row>
              <b-row class="item-detail1 flex-row">
                <b-col class="item-request" v-if="ordered.request">
                  ※{{ ordered.request }}
                </b-col>
                <b-col class="item-request" v-else>&nbsp;</b-col>
              </b-row>
              <b-row class="item-detail2 flex-row">
                <b-col class="menu-order" v-if="multiMode">
                  注文者:&nbsp;
                  {{
                    '【' + ordered.accountNo + '】' + ordered.playerName
                  }}&nbsp;様
                </b-col>
              </b-row>
            </b-row>
          </b-col>
        </b-row>
        <b-row class="bottom" v-if="encryptRepreAccountNo">
          <b-col class="return-menu">
            <b-button variant="link" @click="backTo()" v-text="'戻る'" />
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
import { SlipDetailListRequest } from '@/types/request/SlipDetailListRequest';
import { OrderRequestType } from '@/types/enum/OrderRequestType';
import { namespace } from 'vuex-class';
import { Player } from '@/types/entity/Player';

const CommonModule = namespace('common');
/**
 * 注文履歴画面
 */
@Component({
  components: {
    Header
  }
})
export default class OrderedList extends ViewBase {
  /** 状態管理:暗号化された会計No */
  @CommonModule.State
  private encryptRepreAccountNo!: string;
  /** 状態管理:暗号化されたWeb会員CD */
  @CommonModule.State
  private encryptWebMemberCD!: string;
  @CommonModule.State
  private playerList!: Player[];
  @CommonModule.State
  private multiMode!: boolean;

  /** 画面上の注文一覧情報 */
  private orderedList: Array<{
    accountNo: string;
    playerName: string;
    menuName: string;
    unitPrice: number;
    quantity: number;
    totalPrice: number;
    request: string;
  }> = [];

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
    } catch (error) {
      console.error(error);
      this.toast.error('エラーが発生しました。スタッフをお呼びください。');
      return;
    }

    this.isBusy = true;
    // 注文明細ロード
    await this.loadSlipDetailList()
      .catch(error => this.toast.error(error))
      .finally(() => (this.isBusy = false));
  }

  /**
   * 注文明細をロードします。
   */
  private async loadSlipDetailList(): Promise<void> {
    // 注文明細取得リクエストの取得
    const request = new SlipDetailListRequest(
      this.playerList
        .filter(f => f.EncryptedAccountNo)
        .map(m => m.EncryptedAccountNo)
    );
    // 注文明細の取得
    await this.selfOrderService
      .GetSlipDetailList(request, this.encryptWebMemberCD)
      .then(
        resolve => {
          resolve.SlipDetailList.forEach(slipD => {
            // リクエスト抽出
            let filterdRequest = resolve.SlipRequestList.filter(
              slipR => slipR.OrderSlipID == slipD.OrderSlipID
            );
            // 金額計算
            let totalPrice =
              (slipD?.Quantity ?? 0) * (slipD?.UnitPrice ?? 0) +
              filterdRequest
                .filter(req => req.Type == OrderRequestType.Price)
                .reduce((sum, s) => {
                  return sum + (s?.UnitPrice ?? 0);
                }, 0);
            // リクエスト文字列
            let requestString = filterdRequest
              .map(req => {
                if (req.Type == OrderRequestType.Price) {
                  req.RequestName += '(￥' + req?.UnitPrice + ')';
                }
                return req.RequestName;
              })
              .join(',');
            // プレーヤー名
            let playerName = this.playerList.find(
              player => player.AccountNo == slipD.AccountNo
            )?.Name;
            // 画面上の注文一覧にプッシュ
            this.orderedList.push({
              accountNo: String(slipD.AccountNo),
              playerName: String(playerName),
              menuName: String(slipD.MenuName),
              totalPrice: totalPrice,
              unitPrice: Number(slipD.UnitPrice),
              quantity: 1,
              request: requestString
            });
          });
        },
        reject => {
          console.error(reject);
          return Promise.reject(reject.data);
        }
      );
  }
}
</script>

<style scoped>
.main .border {
  min-height: 5%;
  max-height: 5%;
  text-align: left;
  align-items: center;
  padding: 0vh 0vw 0vh 2vw;
}
.main .ordered {
  min-height: 85%;
  max-height: 85%;
  overflow-y: auto;
}

.main .ordered .group {
  padding: 0vh 0vw 0vh 0vw;
  margin: 1vh 3vw 1vh 3vw;
}

.main .ordered-list {
  list-style: none;
  white-space: nowrap;
  box-shadow: 0px 0px 3px;
  border: 1px solid gray;
  border-radius: 10px;
  padding: 1vh 2vw 1vh 2vw;
  margin: 0vh 0vw 1vh 0vw;
  max-width: 100%;
}

.item-detail0 {
  min-height: 6%;
  max-height: 6%;
  align-content: center;
  padding: 0vh 0vw 0vh 0vw;
  margin: 0vh 0vw 0vh 0vw;
}
.item-name {
  align-content: center;
  text-align: left;
  padding: 0vh 0vw 0vh 0vw;
  margin: 0vh 0vw 0vh 0vw;
}
.item-price-label {
  align-content: center;
  text-align: right;
  padding: 1vh 2vw 0vh 0vw;
  margin: 0vh 0vw 0vh 0vw;
}
.item-price {
  text-align: right;
  padding: 1vh 2vw 0vh 0vw;
  margin: 0vh 0vw 0vh 0vw;
}
.item-detail1 {
  min-height: 6%;
  max-height: 6%;
  padding: 0vh 0vw 0vh 0vw;
  margin: 0vh 0vw 0vh 0vw;
}
.item-request {
  text-align: left;
  padding: 0vh 0vw 0vh 0vw;
  margin: 1vh 0vw 0vh 0vw;
  white-space: normal;
  word-break: keep-all;
}
.item-num-label {
  text-align: right;
  padding: 0vh 2vw 0vh 0vw;
  margin: 0vh 0vw 0vh 0vw;
}
.item-num {
  text-align: right;
  padding: 0vh 2vw 0vh 0vw;
  margin: 0vh 0vw 0vh 0vw;
}
.item-detail2 {
  min-height: 6%;
  max-height: 6%;
  padding: 0vh 0vw 0vh 0vw;
  margin: 0vh 0vw 0vh 0vw;
}
.menu-order {
  text-align: left;
  position: relative;
  padding: 0vh 0vw 0vh 0vw;
  margin: 1vh 0vw 0vh 0vw;
}
.item-total {
  text-align: right;
  padding: 0vh 0vw 0vh 5vw;
  margin: 0vh 0vw 0vh 0vw;
}

.main .bottom {
  min-height: 10%;
  max-height: 10%;
}
.main .return-menu {
  align-self: flex-end;
}
.main .return-menu .btn {
  padding: 0vh 0vw 0vh 0vw;
  margin: 0vh 0vw 2vh 0vw;
}
</style>
