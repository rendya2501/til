<template>
  <b-container fluid class="wrapper vh-100 d-flex flex-column">
    <b-row>
      <Header ref="header" />
    </b-row>
    <b-row class="flex-grow-1">
      <b-container fluid class="main d-flex flex-column">
        <b-row class="border border-dark border-title" v-if="this.isAccount">
          <span v-html=this.DispConfOrder1></span>
        </b-row>
        <b-row class="row-menuList">
          <b-col class="group" v-if="this.menuList.length > 0">
            <b-row class="row-menu flex-column" v-for="item of this.menuList" v-bind:key="item.code">
              <b-row class="row-menu-detail0 flex-row">
                <b-col class="menu-name">{{ item.name }}</b-col>
                <b-col class="menu-price-label">単価</b-col>
                <b-col class="menu-price">￥{{ item.price.toLocaleString()}}</b-col>
              </b-row>
              <b-row class="row-menu-detail1 flex-row">
                <b-col class="menu-request" v-if="item.request.length">※{{ item.request }}</b-col>
                <b-col class="menu-request" v-else></b-col>
                <b-col class="menu-quantity">
                  <b-form-spinbutton id="quantity" v-model="item.quantity" min="1" max="99" @change="changeQuantity(item.id, item.quantity)">
              </b-form-spinbutton>
                </b-col>
              </b-row>
              <b-row class="row-menu-detail2 flex-row">
                <b-col class="menu-icon">
                  <b-icon-trash-fill class="icon-trash" scale="1.5" @click="removeMenu(item.id)"></b-icon-trash-fill>
                </b-col>
                <b-col class="menu-total-label">
                  合計
                </b-col>
                <b-col class="menu-total">￥{{ item.total.toLocaleString()}}</b-col>
              </b-row>
            </b-row>
          </b-col>
          <b-col v-else>
            <div v-if="this.isVisible"><span v-html=this.DispConfOrder2></span></div>
          </b-col>
        </b-row>
        <b-row class="bottom">
          <b-col class="cancel">
            <button type="button" class="btn btn-link" @click="backToMenuList">戻る</button>
          </b-col>
          <b-col class="fix">
            <button type="button" class="btn btn-primary" id="makeOderButton" @click="makeOrders" v-if="this.isAccount">注文する</button>
          </b-col>
        </b-row>
      </b-container>
    </b-row>
  </b-container>
</template>

<script lang="ts">
import { Component } from 'vue-property-decorator'
import ViewBase from '@/views/ViewBase.vue'
import Header from '@/components/Header.vue'
import OrderSendService from '@/scripts/services/OrderSendService'
import Environments from '@/constants/Environments'

@Component({
  components: {
    Header
  }
})

export default class ConfirmOrder extends ViewBase {
  private orderSendService = new OrderSendService()
  private menuList: { [key: string]: string | number }[] = [];
  private isAccount = true
  private isVisible = false
  private DispConfOrder1 = ''
  private DispConfOrder2 = ''

  async mounted (): Promise<void> {
    try {
      await super.mounted()
    } catch (error) {
      this.isAccount = false
    }

    var environments = new Environments(true)
    await environments.loadSetting()
    this.DispConfOrder1 = environments.DISP_CONFORDER_1
    this.DispConfOrder2 = environments.DISP_CONFORDER_2

    this.changeDisabledBottonStatus(false)
  }

  private createOrderItemList () {
    this.menuList = []
    const cartItems = this.accountInfo?.getCartItems()
    if (cartItems) {
      cartItems.forEach(item => {
        this.menuList.push({
          id: item.id,
          code: item.menuCode,
          name: item.name,
          price: item.price,
          total: item.subtotal,
          quantity: item.amount,
          request: item.request
        })
      })
    }
  }

  async onAccountInfoChanged (): Promise<void> {
    const header = this.$refs.header as Header
    header.updateAccount(this.accountInfo ?? null, false)
    this.createOrderItemList()
    this.changeMakeOrderBottonStatus()
  }

  private changeMakeOrderBottonStatus () {
    // menuListに1件もない場合、注文確定ボタンを無効化する
    var element = (document.getElementById('makeOderButton')) as HTMLInputElement
    if (!element) {
      return
    }
    if (this.accountInfo && this.accountInfo.cartItems.length) {
      element.disabled = false
      element.classList.remove('btn-secondary')
      element.classList.add('btn-primary')
      this.isVisible = false
    } else {
      element.disabled = true
      element.classList.remove('btn-primary')
      element.classList.add('btn-secondary')
      this.isVisible = true
    }
  }

  private backToMenuList () {
    this.changeDisabledTemporaryBottonStatus()
    this.moveToMenuList()
  }

  // オーダー送信を行う
  private async makeOrders () {
    this.changeDisabledTemporaryBottonStatus()

    const facirityNo = this.facilityTableInfo?.facilityNo
    const accountInfo = this.accountInfo
    const accountNo = this.accountInfo?.accountNo
    const tableNo = this.facilityTableInfo?.tableNo
    if (facirityNo && accountNo && tableNo && accountInfo) {
      const ret = await this.orderSendService.sendOrder(facirityNo, accountNo, tableNo, accountInfo)
      if (!ret) {
        this.moveToThanksForOrders()
      }
    }
  }

  // メニューリストから削除する
  private removeMenu (targetCode: string) {
    // 表示部の削除
    const targetIndex = this.menuList.findIndex(m => m.id === targetCode)
    this.menuList.splice(targetIndex, 1)
    // OrderItemsからの削除
    if (!this.facilityTableInfo || !this.accountInfo) {
      return
    }
    this.accountService.deleteCartItemById(this.facilityTableInfo.facilityNo ?? '', this.accountInfo, Number(targetCode))
    this.onAccountInfoChanged()
  }

  private changeQuantity (targetCode: string, quantity: number) {
    const targetMenu = this.menuList.find(m => m.id === targetCode)
    // 最小値を1とする
    if (targetMenu && typeof (targetMenu.id) === 'number') {
      targetMenu.quantity = quantity
      if (!this.facilityTableInfo || !this.accountInfo) {
        return
      }
      this.accountService.updateCartItemById(this.facilityTableInfo.facilityNo ?? '', this.accountInfo, targetMenu.id, targetMenu.quantity)
      this.onAccountInfoChanged()
    }
  }
}

</script>

<style scoped>
.border-title {
  min-height: 5%;
  max-height: 5%;
  font-size: 2vh;
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
}
.row-menu {
  min-height: flex;
  max-height: flex;
  white-space:nowrap;
  box-shadow: 0px 0px 5px;
  border: 1px solid gray;
  border-radius: 10px;
  padding: 0vh 0vw 0vh 0vw;
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
/* メニュー名 */
.menu-name {
  font-size: 2vh;
  text-align: left;
  position: relative;
  padding: 1vh 0vw 0vh 2vw;
  margin: 0vh 0vw 0vh 0vw;
}
/* 単価ラベル */
.menu-price-label {
  font-size: 2vh;
  text-align: right;
  position: relative;
  padding: 1vh 0vw 0vh 2vw;
  margin: 0vh 0vw 0vh 0vw;
}
/* 単価 */
.menu-price {
  font-size: 2vh;
  text-align: right;
  position: relative;
  padding: 1vh 2vw 0vh 0vw;
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
  font-size: 2vh;
  text-align: left;
  position: relative;
  padding: 1vh 0vw 0vh 2vw;
  margin: 0vh 0vw 0vh 0vw;
}
/* 数量変更ボタン */
.b-form-spinbutton >>> .btn {
  font-size: auto;
  color: white;
  background-color:#007BFF;
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
.menu-icon {
  align-self: center;
  position: relative;
  text-align: left;
  padding: 0vh 0vw 1vh 5vw;
  margin: 0vh 0vw 0vh 0vw;
}
/* 合計のラベル */
.menu-total-label {
  align-self: center;
  position: relative;
  text-align: right;
  padding: 0vh 0vw 1vh 0vw;
  margin: 0vh 0vw 0vh 0vw;
}
/* 金額(合計) */
.menu-total {
  font-size: 2vh;
  position: relative;
  text-align: right;
  align-self: center;
  padding: 0vh 2vw 1vh 0vw;
  margin: 0vh 0vw 0vh 0vw;
}

.bottom {
  min-height: 10%;
  max-height: 10%;
}
.bottom .cancel {
  align-self: flex-end;
  padding: 0vh 0vw 0vh 0vw;
  margin: 0vh 0vw 0vh 0vw;
}
.bottom .cancel .btn {
  font-size: 2vh;
  padding: 0vh 0vw 0vh 0vw;
  margin: 0vh 0vw 2vh 0vw;
}
.bottom .fix {
  align-self: flex-end;
  padding: 0vh 0vw 0vh 0vw;
  margin: 0vh 0vw 0vh 0vw;
}
.bottom .fix .btn {
  font-size: 2vh;
  padding: 1vh 8vw 1vh 8vw;
  margin: 0vh 0vw 1vh 0vw;
}
</style>
