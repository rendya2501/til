<template>
  <b-container fluid class="header">
    <b-row class="upper half" v-if="account">
      <b-col class="col-account-info">【{{account.accountNo}}】&nbsp;{{account.name}}&nbsp;様</b-col>
    </b-row>
    <b-row class="lower half" v-if="account">
      <b-col>
        <label class="lbl-account-total">合計金額&nbsp;￥{{total.price.toLocaleString()}}</label>
        <label class="lbl-account-numberOfOrders">({{total.amount}}品)</label>
      </b-col>
    </b-row>
    <b-row class="full" align-v="center" v-if="!account">
      <b-col class="title text-left">{{title}}</b-col>
    </b-row>
  </b-container>
</template>

<script lang="ts">
import { Vue, Component, Prop } from 'vue-property-decorator'
import AccountInfo from '@/types/AccountInfo'
import TotalInfo from '@/types/TotalInfo'

@Component
export default class Header extends Vue {
  @Prop({ required: false })
  public title?: string

  private account: AccountInfo | null = null
  private total: TotalInfo | null = null

  public updateAccount (account: AccountInfo | null, showOrdered: boolean): void {
    this.account = account
    if (this.account) {
      this.total = showOrdered ? this.account.getOrderedTotal() : this.account.getCartTotal()
    }
  }
}
</script>

<style scoped>
.header {
  background-color: rgb(50, 147, 223);
  color: white !important;
  height: var(--header-height);
}
.header .full {
  height: 100%;
}
.header .half {
  height: 50%;
  position: relative;
}
.header .title {
  padding-left: 4px;
}

.upper {
  position: absolute;
  font-size: 18px;
}

/* お客様氏名 と 会計番号 */
.col-account-info {
  position: absolute;
  text-align: left;
}

.lower {
  font-size: 18px;
  text-align: right;
  white-space:nowrap;
  position: absolute;
}

/* 合計金額 */
.lbl-account-total {
  margin: 0 15px;
}
</style>
