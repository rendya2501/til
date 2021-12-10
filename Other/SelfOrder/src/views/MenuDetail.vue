<template>
  <b-container fluid class="wrapper vh-100 d-flex flex-column">
    <b-row>
      <Header ref="header" />
    </b-row>
    <b-row class="flex-grow-1">
      <b-container fluid class="main d-flex flex-column">
        <b-row class="label d-flex" v-if="this.selectedItem && this.isAccount">
          <b-col type="text" class="comment" ><span v-html=this.DispMenuDetail1></span></b-col>
        </b-row>
        <b-row class="upper d-flex">
          <b-col class="menu-picture">
            <b-img class="img" v-bind:src="'data:image/png;base64,'+this.menuImage" fluid alt="" onerror="this.onerror=null; this.src=''" v-if="this.selectedItem && this.isAccount"></b-img>
          </b-col>
        </b-row>
        <b-row class="middle-0 flex-column" v-if="this.selectedItem && this.isAccount">
          <b-row class="menu-detail flex-row" >
            <b-col class="txt-name">{{this.selectedItem.name}}</b-col>
            <b-col class="txt-price">￥ {{this.selectedItem.price.toLocaleString()}}</b-col>
          </b-row>
        </b-row>
        <b-row class="middle-1 d-flex" v-if="this.selectedItem && this.isAccount">
          <b-col class="select-num">
            <b-form-group
              label="数量"
              label-for="quantity"
              label-cols="6"
              label-align="right"
            >
              <b-form-spinbutton id="quantity" v-model="quantity" min="1" max="99">
              </b-form-spinbutton>
            </b-form-group>
          </b-col>
        </b-row>
        <b-row class="middle-2 d-flex">
          <b-col class="label-request" v-if="this.selectedItem && this.isAccount">
            ご要望
          </b-col>
          <b-col class="dropdown" v-if="this.selectedItem && this.isAccount">
            <b-dropdown class="dropdown-request" id="dropdown-request" text="" right dropup>
              <b-dropdown-item v-for="item in requests" v-bind:key="item.code" v-on:click="onclickRequestItem">
                {{ item.name }}
              </b-dropdown-item>
            </b-dropdown>
          </b-col>
        </b-row>
        <b-row class="bottom flex-fill d-flex">
          <b-col class="btn-back">
            <button type="button" class="btn btn-link" @click="backTo">戻る</button>
          </b-col>
          <b-col class="btn-commit">
            <button type="button" class="btn btn-primary" @click="makeOrder" v-if="this.selectedItem && this.isAccount">選択する</button>
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
import MenuImageService from '@/scripts/services/MenuImageService'
import MasterService from '@/scripts/services/MasterService'
import MasterInfo from '@/types/MasterInfo'
import Environments from '@/constants/Environments'

@Component({
  components: {
    Header
  }
})
export default class MenuDetail extends ViewBase {
  protected menuImageService = new MenuImageService()
  private quantity = 1
  private requests: Array<{'code': number | null, 'name': string}> = []
  private menuList: Array<{'category': string, 'idx': string, 'name': string, 'price': number}> = []
  private selectedIdx = ''
  private selectedItem?: {'category': string, 'idx': string, 'name': string, 'price': number} | null = { category: '', idx: '', name: '', price: 0 }
  private menuImage = ''
  private masterService = new MasterService()
  private masterInfo: MasterInfo = new MasterInfo([], [], [])
  private isAccount = true
  private DispMenuDetail1 = ''

  private async setMaster (): Promise<void> {
    const facirityNo = this.getQueryString(this.$route.query.facilityNo)
    if (facirityNo) {
      this.changeDisabledBottonStatus(true)
      this.masterInfo = await this.masterService.getMasterInfo(facirityNo)
      this.changeDisabledBottonStatus(false)
    } else {
      this.masterInfo = new MasterInfo([], [], [])
    }
  }

  private getRequests (): void {
    this.requests = [
      { code: null, name: '選択してください' }
    ]

    if (this.masterInfo != null && this.masterInfo.orderRequestList != null) {
      this.masterInfo.orderRequestList.forEach(orderRequest => {
        this.requests.push({ code: orderRequest.OrderRequestCD, name: orderRequest.OrderRequestName })
      })
    }

    this.changeRequest(this.requests[0].name)
  }

  private setCurrentMenuList (inputCategory: string | null) {
    this.menuList.splice(0)

    if (inputCategory == null || this.masterInfo == null || this.masterInfo.orderMenuList == null) {
      return
    }

    const targetOrderMenuList = this.masterInfo.orderMenuList.filter(orderMenu => orderMenu.OrderMenuClsCD.toString() === inputCategory)

    if (targetOrderMenuList != null) {
      targetOrderMenuList.forEach(orderMenu => {
        this.menuList.push({ category: orderMenu.OrderMenuClsCD.toString(), idx: orderMenu.OrderMenuCD.toString(), name: orderMenu.OrderMenuName, price: orderMenu.UnitPrice })
      })
    }
  }

  async mounted (): Promise<void> {
    try {
      await super.mounted()
    } catch (error) {
      this.isAccount = false
    }

    this.changeDisabledBottonStatus(false)

    await this.setMaster()
    this.getRequests()
    if (this.category) {
      this.setCurrentMenuList(this.category)
    }
    const requestIdx = this.getQueryString(this.$route.query.idx)
    if (requestIdx) {
      this.selectedIdx = requestIdx
    }
    try {
      this.selectedItem = this.menuList.find(menu => menu.idx === this.selectedIdx)
    } catch (error) {
      this.selectedItem = null
    }

    const facirityNo = this.getQueryString(this.$route.query.facilityNo)
    if (facirityNo && this.selectedItem) {
      this.menuImage = await this.menuImageService.getMenuImage(facirityNo, this.selectedIdx)
    }

    var environments = new Environments(true)
    await environments.loadSetting()
    this.DispMenuDetail1 = environments.DISP_MENUDETAIL_1
  }

  async onAccountInfoChanged (): Promise<void> {
    const header = this.$refs.header as Header
    header.updateAccount(this.accountInfo ?? null, false)
  }

  private getRequestButtonElement (): HTMLButtonElement | null {
    const requestElement = super.getElementById('dropdown-request') as HTMLDivElement
    if (!requestElement) {
      return null
    }
    return requestElement.firstElementChild as HTMLButtonElement
  }

  private changeRequest (text: string): void {
    const buttonElement = this.getRequestButtonElement()
    if (!buttonElement) {
      return
    }
    buttonElement.innerText = text
  }

  private onclickRequestItem (event: Event): void {
    const selectedRequestItem = event.target as HTMLElement
    if (!selectedRequestItem) {
      return
    }
    this.changeRequest(selectedRequestItem.innerText)
  }

  private getSelectedRequest (): string {
    const buttonElement = this.getRequestButtonElement()
    return !buttonElement || buttonElement.innerText === this.requests[0].name ? '' : buttonElement.innerText
  }

  private backTo (): void {
    this.changeDisabledTemporaryBottonStatus()
    this.moveToMenuList()
  }

  private makeOrder (): void {
    if (!this.selectedItem) {
      return
    }
    if (!this.facilityTableInfo?.facilityNo || !this.accountInfo) {
      return
    }

    this.changeDisabledTemporaryBottonStatus()

    const request = this.getSelectedRequest()
    this.accountService.appendCartItem(this.facilityTableInfo?.facilityNo, this.accountInfo, this.selectedItem.idx, this.selectedItem.name, this.selectedItem.price, this.quantity, request)
    this.moveToMenuList()
  }
}
</script>

<style scoped>
.label {
  min-height: 5%;
  max-height: 7%;
  align-content: center;
  padding: 0vh 0vw 0vh 0vw;
  margin: 0vh 0vw 0vh 0vw;
}
.comment {
  font-size: 2vh;
}

.main {
  height: 100%;
  width: 100%;
  overflow-y:auto;
}

.upper {
  width: auto;
  align-content: center;
  padding: 0vh 0vw 0vh 0vw;
  margin: 0vh 0vw 0vh 0vw;
}
.menu-picture {
  height: 100%;
  width: auto;
  object-fit: scale-down;
  padding: 0vh 0vw 0vh 0vw;
  margin: 0vh 0vw 0vh 0vw;
}
.img {
  height: 75vw;
  width: auto;
  object-fit: scale-down;
}

.middle-0 {
 min-height: flex;
  max-height: flex;
  white-space:nowrap;
  box-shadow: 0px 0px 5px;
  border: 1px solid gray;
  border-radius: 10px;
  padding: 0vh 0vw 0vh 0vw;
  margin: 1vh 0vw 1vh 0vw;
}

.menu-detail {
  min-height: 6%;
  max-height: 12%;
  min-width: 100%;
  max-width: 100%;
  padding: 0vh 0vw 0vh 0vw;
  margin: 0vh 0vw 0vh 0vw;
}

.txt-name {
  font-size: 2vh;
  text-align: left;
  position: relative;
  padding: 2vh 0vw 2vh 2vw;
  margin: 0vh 0vw 0vh 0vw;
}

.menu-detail {
  height: 48%;
  width: auto;
  font-size: auto;
  text-align: right;
  text-align: end;
  padding: 0vh 0vw 0vh 0vw;
  margin: 0vh 0vw 0vh 0vw;
}

.txt-price {
  font-size: 2vh;
  text-align: right;
  position: relative;
  padding: 2vh 2vw 2vh 0vw;
  margin: 0vh 0vw 0vh 0vw;
}

.middle-1 {
  height: auto;
  width: auto;
  align-items: center;
  padding: 1vh 0vw 0vh 0vw;
  margin: 0vh 0vw 0vh 0vw;
  flex-shrink: 5;
}
.select-num {
  padding: 0vh 0vw 0vh 0vw;
  margin: 0vh 0vw 0vh 0vw;
}
.select-num .form-group {
  font-size: auto;
  padding: 0vh 0vw 0vh 0vw;
  margin: 0vh 0vw 0vh 0vw;
}
.b-form-spinbutton >>> .btn {
  font-size: auto;
  color: white;
  background-color:#007BFF;
}

.middle-2 {
  height: auto;
  width: 100%;
  align-items: center;
  padding: 1vh 0vw 0vh 0vw;
  margin: 0vh 0vw 0vh 0vw;
  flex-shrink: 5;
}
.label-request {
  width: 40%;
  font-size: auto;
  text-align: right;
  padding: 0vh 3vw 0vh 0vw;
  margin: 0vh 0vw 0vh 0vw;
}
.dropdown {
  width: 60%;
  font-size: auto;
  padding: 0vh 0vw 0vh 0vw;
  margin: 0vh 0vw 0vh 0vw;
}
.dropdown-request {
  width: 100%;
  text-align: center;
  padding: 0vh 0vw 0vh 0vw;
  margin: 0vh 0vw 0vh 0vw;
}
.dropdown-request >>> .btn {
  min-height: auto;
  max-height: auto;
  font-size: auto;
  inline-size: 100%;
  overflow: hidden;
  color: black;
  background-color:white;
}

.bottom {
  height: auto;
  width: auto;
  align-items: flex-end;
  padding: 0vh 0vw 1vh 0vw;
  margin: 0vh 0vw 0vh 0vw;
}
.main .btn-back .btn {
  font-size: 2vh;
  text-align: center;
  padding: 0vh 0vw 0vh 0vw;
  margin: 0vh 0vw 0vh 0vw;
}
.main .btn-commit .btn {
  font-size: 2vh;
  text-align: center;
  padding: 1vh 10vw 1vh 10vw;
  margin: 0vh 0vw 0vh 0vw;
}
</style>
