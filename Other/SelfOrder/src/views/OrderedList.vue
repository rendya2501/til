<template>
  <b-container fluid class="wrapper vh-100 d-flex flex-column">
    <b-row>
      <Header ref="header" />
    </b-row>
    <b-row class="flex-grow-1">
      <b-container fluid class="main d-flex flex-column">
        <b-row class="border border-dark border-title" v-if="this.accountInfo">
          <span v-html=this.DispOrderList1></span>
        </b-row>
        <b-row class="ordered" v-if="this.accountInfo">
          <b-col class="group">

              <b-row class="ordered-list flex-column" v-for="ordered in orderedList" :key="ordered.orderedNo" >
                <b-row class="item-detail0 flex-row">
                  <b-col class="item-name">{{ ordered.menuName }}</b-col>
                  <b-col class="item-price-label">単価</b-col>
                  <b-col class="item-price">￥{{ ordered.unitPrice.toLocaleString() }}</b-col>
                </b-row>
                <b-row class="item-detail1 flex-row">
                  <b-col class="item-request" v-if="ordered.request">※{{ ordered.request }}</b-col>
                  <b-col class="item-request" v-else></b-col>
                  <b-col class="item-num-label">数量</b-col>
                  <b-col class="item-num">{{ ordered.num }}</b-col>
                </b-row>
                <b-row class="item-detail2 flex-row">
                  <b-col class="item-padding"></b-col>
                  <b-col class="item-total-label">合計</b-col>
                  <b-col class="item-total">￥{{ ordered.totalPrice.toLocaleString() }}</b-col>
                </b-row>
              </b-row>

          </b-col>
        </b-row>
        <b-row class="bottom">
          <b-col class="return-menu">
            <button type="button" class="btn btn-link" @click="backToMenuList">戻る</button>
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
import Environments from '@/constants/Environments'

@Component({
  components: {
    Header
  }
})
export default class OrderedList extends ViewBase {
  private orderedList: Array<{menuName: string, unitPrice: number, num: number, totalPrice: number, request: string}> = []
  private DispOrderList1 = ''

  async mounted (): Promise<void> {
    await super.mounted()

    var environments = new Environments(true)
    await environments.loadSetting()
    this.DispOrderList1 = environments.DISP_ORDERLIST_1

    this.changeDisabledBottonStatus(false)
  }

  private setOrderedList () {
    this.orderedList = []
    if (this.accountInfo) {
      this.accountInfo.orderedItems.forEach(item => {
        this.orderedList.push({
          menuName: item.name,
          unitPrice: item.price,
          num: item.amount,
          totalPrice: item.subtotal,
          request: item.request
        })
      })
    }
  }

  async onAccountInfoChanged (): Promise<void> {
    const header = this.$refs.header as Header
    header.updateAccount(this.accountInfo ?? null, true)
    this.setOrderedList()
  }

  private backToMenuList () {
    this.changeDisabledTemporaryBottonStatus()
    this.moveToMenuList()
  }
}
</script>

<style scoped>
.main .border {
  min-height: 5%;
  max-height: 5%;
  font-size: 2vh;
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
  white-space:nowrap;
  box-shadow: 0px 0px 3px;
  border: 1px solid gray;
  border-radius: 10px;
  padding: 0vh 0vw 0vh 0vw;
  margin: 0vh 0vw 1vh 0vw;
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
  font-size: 2vh;
  padding: 1vh 0vw 0vh 2vw;
  margin: 0vh 0vw 0vh 0vw;
}
.item-price-label {
  align-content: center;
  text-align: right;
  font-size: 2vh;
  padding: 1vh 2vw 0vh 0vw;
  margin: 0vh 0vw 0vh 0vw;
}
.item-price {
  text-align: right;
  font-size: 2vh;
  padding: 1vh 2vw 0vh 0vw;
  margin: 0vh 0vw 0vh 0vw;
}
.item-detail1 {
  min-height: 6%;
  max-height: 6%;
  padding: 0vh 0vw 0vh 0vw;
  margin: 0vh 0vw 0vh 0vw;
}
.item-request{
  text-align: left;
  font-size: 2vh;
  padding: 0vh 0vw 0vh 2vw;
  margin: 0vh 0vw 0vh 0vw;
}
.item-num-label {
  text-align: right;
  font-size: 2vh;
  padding: 0vh 2vw 0vh 0vw;
  margin: 0vh 0vw 0vh 0vw;
}
.item-num {
  text-align: right;
  font-size: 2vh;
  padding: 0vh 2vw 0vh 0vw;
  margin: 0vh 0vw 0vh 0vw;
}
.item-detail2 {
  min-height: 6%;
  max-height: 6%;
  padding: 0vh 0vw 0vh 0vw;
  margin: 0vh 0vw 0vh 0vw;
}
.item-padding {
  text-align: left;
  font-size: 2vh;
  padding: 0vh 0vw 1vh 2vw;
  margin: 0vh 0vw 0vh 0vw;
}
.item-total-label {
  text-align: right;
  font-size: 2vh;
  padding: 0vh 2vw 1vh 0vw;
  margin: 0vh 0vw 0vh 0vw;
}
.item-total {
  text-align: right;
  font-size: 2vh;
  padding: 0vh 2vw 1vh 0vw;
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
  font-size: 2vh;
  padding: 0vh 0vw 0vh 0vw;
  margin: 0vh 0vw 2vh 0vw;
}
</style>
