<template>
  <b-container fluid class="wrapper vh-100 d-flex flex-column">
    <b-row>
      <Header ref="header" />
    </b-row>
    <b-row class="flex-grow-1">
      <b-container fluid class="main d-flex flex-column">
        <b-row class="upper">
          <b-col class="name" v-if="this.isAccount">{{accountName}} 様</b-col>
        </b-row>
        <b-row class="middle">
          <b-col class="message" v-if="this.isAccount"><span v-html=this.DispThanksOrder1></span></b-col>
        </b-row>
        <b-row class="bottom">
          <b-col class="back-btn">
            <button type="button" class="btn btn-link" @click="backTo">戻る</button>
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
import CartStorage from '@/scripts/storage/CartStorage'
import Environments from '@/constants/Environments'

@Component({
  components: {
    Header
  }
})

export default class ThansForOrders extends ViewBase {
  private facirityNo = super.getQueryString(this.$route.query.facilityNo)
  private accountNo = super.getQueryString(this.$route.query.holderNo)
  private cartStorage = new CartStorage(this.facirityNo ?? '', this.accountNo ?? '')
  private isAccount = true
  private environments = new Environments()
  private DispThanksOrder1 = ''

  private get accountName () {
    return this.accountInfo?.name ?? ''
  }

  async mounted (): Promise<void> {
    history.pushState(null, '', null)
    window.addEventListener('popstate', function () {
      history.pushState(null, '', null)
    })
    this.cartStorage.clear()
    try {
      await super.mounted()
    } catch (error) {
      window.addEventListener('popstate', function () {
        history.pushState(null, '', null)
      })
      this.isAccount = false
    }

    var environments = new Environments(true)
    await environments.loadSetting()
    this.DispThanksOrder1 = environments.DISP_THANKSORDER_1

    this.changeDisabledBottonStatus(false)
  }

  async onAccountInfoChanged (): Promise<void> {
    const header = this.$refs.header as Header
    header.updateAccount(this.accountInfo ?? null, true)
  }

  private backTo () {
    this.changeDisabledTemporaryBottonStatus()
    this.backToScanAccountNumber()
  }
}
</script>

<style scoped>
.main .upper {
  min-height: 33%;
  max-height: 33%;
  align-items:center;
}
.main .upper .name {
  padding: 0vh 0vw 0vh 0vw;
  margin: 0vh 0vw 0vh 0vw;
}
.main .middle {
  min-height: 34%;
  max-height: 34%;
  align-items:flex-start;
}
.main .middle .message {
  font-size: 2vh;
  padding: 0vh 2vw 0vh 2vw;
  margin: 0vh 0vw 0vh 0vw;
}
.main .bottom {
  min-height: 33%;
  max-height: 33%;
  align-items:flex-end;
}
.main .bottom .btn {
  font-size: 2vh;
  padding: 0vh 0vw 0vh 0vw;
  margin: 0vh 0vw 2vh 0vw;
}
</style>
