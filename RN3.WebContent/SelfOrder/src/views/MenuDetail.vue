<template>
  <b-container fluid class="wrapper d-flex flex-column">
    <b-row>
      <Header ref="header" />
    </b-row>
    <b-row class="flex-grow-1">
      <b-container fluid class="main d-flex flex-column">
        <!-- 商品の画像を表示する行 -->
        <b-row class="top d-flex">
          <b-col class="menu-picture">
            <b-container v-if="isLoadingImage" style="height:100%;">
              <b-row align-v="center" style="height:100%;">
                <b-col><b-spinner /></b-col>
              </b-row>
            </b-container>
            <b-img
              class="img"
              v-bind:src="'data:image/png;base64,' + menuImage"
              fluid
              alt=""
              v-else-if="selectedMenu && menuImage"
            />
            <img
              id="barcode-image"
              class="img"
              v-else
              src="@/assets/no_image.png"
            />
          </b-col>
        </b-row>
        <!-- 商品名と金額を表示する行 -->
        <b-row class="menu-info flex-row">
          <b-col
            class="text-left font-weight-bold"
            v-text="selectedMenu.MenuName"
            v-if="selectedMenu"
          />
          <b-col
            class="text-right font-weight-bold"
            v-text="'￥' + selectedMenu.UnitSellingPrice.toLocaleString()"
            v-if="selectedMenu"
          />
        </b-row>
        <b-row class="menu-detail d-flex">
          <b-col v-if="selectedMenu">
            <!-- 親があるリスト -->
            <b-row class="flex-column">
              <b-container>
                <b-row
                  v-for="(parentItem, index) in hasParentList"
                  :key="index"
                >
                  <b-col>
                    <!-- リクエスト名 -->
                    <b-row
                      style=" 
                        text-align: left;
                        padding: 0vh 0vw 0vh 4vw;
                        margin: 0vh 0vw 0vh 0vw;"
                      v-text="parentItem.parent.RequestName"
                    >
                    </b-row>
                    <!-- リクエストの内容 -->
                    <b-row>
                      <b-container>
                        <b-input-group
                          style="
                          -webkit-justify-content:flex-end;
                          -ms-justify-content: flex-end;
                          justify-content: flex-end;
                          "
                        >
                          <b-col
                            cols="4"
                            v-for="(childItem, index) in parentItem.child"
                            :key="index"
                            style="
                          margin-top:1px;
                          margin-bottom:1px;
                          padding-left:1px;
                          padding-right:1px;"
                          >
                            <b-button
                              block
                              class="btn-request"
                              :variant="
                                childItem.isSelected
                                  ? 'primary'
                                  : 'outline-primary bg-white text-primary'
                              "
                              v-bind:disabled="!isTotalEnable"
                              @click="
                                clickHasParentItem(parentItem.child, childItem)
                              "
                            >
                              <span
                                class="btn-txt"
                                v-text="childItem.RequestName"
                              />
                              <span
                                class="btn-txt"
                                v-text="childItem.RequestSuffix"
                              />
                            </b-button>
                          </b-col>
                        </b-input-group>
                      </b-container>
                    </b-row>
                  </b-col>
                </b-row>
              </b-container>
            </b-row>
            <!-- 親がないリスト -->
            <b-row class="flex-column">
              <b-container>
                <b-input-group
                  style="
                  -webkit-justify-content:flex-end;
                  -ms-justify-content: flex-end;
                  justify-content: flex-end;
                  "
                >
                  <b-col
                    cols="4"
                    v-for="(item, index) in noParentList"
                    :key="index"
                    style="
                      margin-top:1px;
                      margin-bottom:1px;
                      padding-left:1px;
                      padding-right:1px;"
                  >
                    <b-button
                      block
                      class="btn-request"
                      :variant="
                        item.isSelected
                          ? 'primary'
                          : 'outline-primary bg-white text-primary'
                      "
                      v-bind:disabled="!isTotalEnable"
                      :pressed.sync="item.isSelected"
                    >
                      <span class="btn-txt" v-text="item.RequestName" />
                      <span class="btn-txt" v-text="item.RequestSuffix" />
                    </b-button>
                  </b-col>
                </b-input-group>
              </b-container>
            </b-row>
          </b-col>
        </b-row>
        <!-- 一番下の「戻る」「選択」ボタン行 -->
        <b-row class="bottom d-flex" v-if="encryptRepreAccountNo">
          <b-col>
            <b-button variant="link" @click="backTo()" v-text="'戻る'" />
          </b-col>
          <b-col>
            <b-button
              block
              variant="primary"
              v-if="selectedMenu"
              v-bind:disabled="!isTotalEnable"
              @click="
                multiMode
                  ? showSelectPlayerDialog()
                  : makeOrder(contextPlayerList)
              "
              v-text="'選択する'"
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
      :message="'お召し上がりになる方を選択してください。'"
      :playerList="contextPlayerList"
      @confirm="makeOrder"
    />
  </b-container>
</template>

<script lang="ts">
import { Component } from 'vue-property-decorator';
import { ViewBase } from '@/views/ViewBase';
import Header from '@/components/Header.vue';
import SelectPlayerDialog from '@/components/SelectPlayerDialog.vue';
import { OrderMenu } from '@/types/entity/OrderMenu';
import { OrderRequest } from '@/types/entity/OrderRequest';
import { OrderRequestType } from '@/types/enum/OrderRequestType';
import { Player } from '@/types/entity/Player';
import { namespace } from 'vuex-class';
import { CartItem } from '@/types/entity/CartItem';
import { SelectPlayerDialogContext } from '@/types/context/SelectPlayerDialogContext';
import { SaveSlipRequest } from '@/types/entity/SaveSlipRequest';
import { LocalOrderRequest } from '@/types/entity/LocalOrderRequest';

const CommonModule = namespace('common');

/**
 * メニュー詳細(メニュー選択)画面
 */
@Component({
  components: {
    Header,
    SelectPlayerDialog
  }
})
export default class MenuDetail extends ViewBase {
  @CommonModule.State
  private selectedMenu!: OrderMenu;
  @CommonModule.State
  private targetRequest!: OrderRequest[];
  @CommonModule.State
  private playerList!: Player[];
  @CommonModule.State
  private multiMode!: boolean;
  @CommonModule.State
  private encryptRepreAccountNo!: string;
  /** 状態管理:暗号化されたWeb会員CD */
  @CommonModule.State
  private encryptWebMemberCD!: string;

  @CommonModule.Action
  private addCart!: (value: CartItem) => void;

  /** プレーヤー選択オーバーレイ用コンテキスト */
  private contextPlayerList: Array<SelectPlayerDialogContext> = [];
  /** 画像を表すbase64文字列 */
  private menuImage: string = '';
  /** 画像読み込み中のクルクル表示フラグ */
  private isLoadingImage: boolean = false;
  /** 親がある一覧 */
  private hasParentList: Array<{
    parent: OrderRequest;
    child: LocalOrderRequest[];
  }> = [];
  /** 親がない一覧 */
  private noParentList: Array<LocalOrderRequest> = [];

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
      if (!this.selectedMenu) {
        this.toast.error('商品が選択されていません。');
        return;
      }
    } catch (error) {
      console.error(error);
      this.toast.error('エラーが発生しました。スタッフをお呼びください。');
      return;
    }
    // 商品画像をロード
    // 画像でエラーになっても動作に影響はないので続ける。
    await this.loadImage().catch(error => this.toast.error(error));
    // 選択した商品情報を画面項目にセットする
    this.setParam();

    // 操作可能にする
    this.isTotalEnable = true;
  }

  /**
   * 選択した商品情報を画面項目にセットする
   */
  private setParam(): void {
    // コンテキストの生成
    this.contextPlayerList = this.playerList
      .filter(f => f.AccountNo)
      .map(
        player =>
          new SelectPlayerDialogContext({
            encryptedAccountNo: player.EncryptedAccountNo,
            accountNo: player.AccountNo,
            playerName: player.Name,
            isSelected: player.EncryptedAccountNo == this.encryptRepreAccountNo
          })
      );

    // ParentRequestがNULLでかつTypeが3ではないものを抽出して並べる。
    this.noParentList = this.targetRequest
      .filter(
        x =>
          x.ParentRequestCD == null &&
          x.Type != OrderRequestType.Level &&
          x.Type != OrderRequestType.Quantity
      )
      .map(req => {
        let requestSuffix =
          req.Type == OrderRequestType.Price
            ? '￥' + req.Price.toLocaleString()
            : '';
        return new LocalOrderRequest({
          RequestCD: req.RequestCD,
          RequestName: req.RequestName,
          RequestSuffix: requestSuffix,
          Price: req.Price,
          Qunantity: 0,
          isSelected: false
        });
      });

    // ParentRequestがあり、かつそのCDと同じTypeを抜き出し、それぞれのParentCDでソートし、親子関係を構築する。
    this.targetRequest
      .filter(
        x => x.ParentRequestCD == null && x.Type == OrderRequestType.Level
      )
      .forEach(parent => {
        // 子供を抽出
        let child = this.targetRequest
          .filter(
            x =>
              x.ParentRequestCD == parent.RequestCD &&
              x.Type != OrderRequestType.Quantity
          )
          .map(req => {
            let requestSuffix =
              req.Type == OrderRequestType.Price
                ? '￥' + req.Price.toLocaleString()
                : '';
            return new LocalOrderRequest({
              RequestCD: req.RequestCD,
              RequestName: req.RequestName,
              RequestSuffix: requestSuffix,
              Price: req.Price,
              isSelected: false
            });
          });
        // 追加
        this.hasParentList.push({
          parent: parent,
          child: child
        });
      });
  }

  /**
   * 商品の画像をロードします。
   */
  private async loadImage(): Promise<void> {
    this.isLoadingImage = true;
    await this.selfOrderService
      .getMenuImage(
        {
          PatternCD: this.selectedMenu?.PatternCD,
          MenuLargeClassCD: this.selectedMenu?.MenuLargeClassCD,
          MenuClassCD: this.selectedMenu?.MenuClassCD,
          MenuCD: this.selectedMenu?.MenuCD
        },
        this.encryptWebMemberCD
      )
      .then(
        resolve => (this.menuImage = resolve.toString()),
        reject => {
          console.error(reject);
          return Promise.reject(reject.data);
        }
      )
      .finally(() => (this.isLoadingImage = false));
  }

  /**
   * 選択ボタンクリック処理
   * オーバーレイを表示する
   */
  private showSelectPlayerDialog(): void {
    (this.$refs.selectPlayerDialog as SelectPlayerDialog).show();
  }

  /**
   * 親の子要素を選択した時の処理
   * その子要素を排他する。
   */
  private clickHasParentItem(
    allChild: LocalOrderRequest[],
    SelectChild: LocalOrderRequest
  ) {
    SelectChild.isSelected = !SelectChild.isSelected;
    // 選択した要素以外のチェックを全て外す
    allChild.filter(f => f != SelectChild).forEach(f => (f.isSelected = false));
  }

  /**
   * 注文確定ボタンクリック処理
   */
  private makeOrder(playerList: Array<SelectPlayerDialogContext>): void {
    let filterdPlayerList = playerList.filter(f => f.isSelected);
    // チェックしたプレーヤー数を取得
    const playerCount = filterdPlayerList.length;
    // 選択されているプレーヤーをループしてカートに入れる
    filterdPlayerList.forEach(player => {
      let request: string[] = [];
      let slipRequestList: SaveSlipRequest[] = [];
      let optionPrice: number = 0;
      // 親のあるやつのループ
      this.hasParentList.forEach(hasParent => {
        hasParent.child
          .filter(x => x.isSelected)
          .forEach(req => {
            if (req.RequestCD) {
              slipRequestList.push({
                RequestCD: req.RequestCD,
                Quantity: req.Qunantity
              });
            }
            request.push(req.RequestName + req.RequestSuffix);
            optionPrice += req?.Price ?? 0;
          });
      });
      // 親のない奴のループ
      this.noParentList
        .filter(x => x.isSelected)
        .forEach(req => {
          if (req.RequestCD) {
            slipRequestList.push({
              RequestCD: req.RequestCD,
              Quantity: req.Qunantity
            });
          }
          request.push(req.RequestName + req.RequestSuffix);
          optionPrice += req?.Price ?? 0;
        });
      // カートアイテム作成
      const cartItem = new CartItem({
        encryptedAccountNo: player.encryptedAccountNo,
        accountNo: player.accountNo,
        playerName: player.playerName,
        menuLargeClassCD: this.selectedMenu.MenuLargeClassCD,
        menuClassCD: this.selectedMenu.MenuClassCD,
        menuCD: this.selectedMenu.MenuCD,
        menuName: this.selectedMenu.MenuName,
        price: this.selectedMenu.UnitSellingPrice + optionPrice,
        quantity: 1,
        personCount: playerCount,
        request: request.join(','),
        SlipRequestList: slipRequestList
      });
      // カートに追加
      this.addCart(cartItem);
    });
    // メニュー一覧に戻る
    this.backTo();
  }
}
</script>

<style scoped>
.main .top {
  width: auto;
  align-content: center;
}

.menu-picture {
  height: 30vh;
  width: auto;
  object-fit: scale-down;
}

.menu-picture .spinner-border {
  width: 3rem;
  height: 3rem;
}

.img {
  height: 100%;
  width: auto;
  object-fit: scale-down;
}

.menu-info {
  min-height: 6%;
  max-height: 12%;
  align-items: center;
  padding: 0vh 0vw 0vh 0vw;
  margin: 0vh 0vw 0vh 0vw;
}

.main .menu-detail {
  min-height: 48%;
  max-height: 48%;
  overflow-y: auto;
}

.main .bottom {
  min-height: 10%;
  max-height: 10%;
  align-items: flex-end;
  padding: 0vh 0vw 1vh 0vw;
}

.btn-request {
  white-space: normal;
  min-height: 60px;
  height: auto;
  padding-left: 1px;
  padding-right: 1px;
  display: flex;
  flex-direction: column;
  /* 左右中央揃え */
  justify-content: center;
  /*上下中央揃え*/
  align-items: center;
}

.btn-txt {
  overflow-wrap: break-word;
}
</style>
