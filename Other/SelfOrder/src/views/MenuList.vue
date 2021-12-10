<template>
  <b-container fluid class="wrapper vh-100 d-flex flex-column">
    <b-row>
      <Header ref="header" />
    </b-row>
    <b-row class="flex-grow-1">
      <b-container fluid class="main d-flex flex-column">
        <b-row class="border border-dark" v-if="this.accountInfo">
          <span v-html=this.DispMenuList1></span>
        </b-row>
        <b-row class="category d-flex" v-if="this.accountInfo">
          <b-col class="category-group">
            <li class="category-item" v-for="category in categoryList" :key="category.code">
              <button type="bottun" class="btn btn-link-primary" :id="category.elementId" @click="setCurrentMenuList(category.code)">{{ category.name }}</button>
            </li>
          </b-col>
        </b-row>
        <b-row class="menu-list d-flex" v-if="this.accountInfo">
          <b-col class="group">
            <b-row class="item" v-for="(menu, index) in menuList" :key="`${menu.idx}-${index}`">
              <b-col type="text" class="menu" @click="showMenuDetail(menu.category, menu.idx)">
                <b-row class="menu-upper">
                  <b-col type="text" class="menu-name">{{ menu.name }}</b-col>
                </b-row>
                <b-row class="menu-icon-right">
                  <b-col class="icon-right">
                    <b-icon-chevron-compact-right></b-icon-chevron-compact-right>
                  </b-col>
                </b-row>
                <b-row class="menu-bottom">
                  <b-col type="text" class="menu-price">￥ {{ menu.price.toLocaleString() }}</b-col>
                </b-row>
              </b-col>
            </b-row>
           </b-col>
        </b-row>
        <b-row class="menu-btn d-flex">
          <b-col class="btn-change">
            <button type="button" class="btn btn-link" @click="changeAccountNumber">キャンセル</button>
          </b-col>
          <b-col class="btn-show">
            <button type="button" class="btn btn-link" @click="showOrderedList" v-if="this.accountInfo">注文履歴</button>
          </b-col>
          <b-col class="btn-commit">
            <button type="button" class="btn btn-primary" id="checkOderButton" @click="makeOrders" v-if="this.accountInfo">注文確認</button>
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
import MasterService from '@/scripts/services/MasterService'
import MasterInfo from '@/types/MasterInfo'
import Environments from '@/constants/Environments'

@Component({
  components: {
    Header
  }
})
export default class MenuList extends ViewBase {
  private masterService = new MasterService()
  private masterInfo: MasterInfo = new MasterInfo([], [], [])
  private categoryList: any = []
  private menuList: any = []
  private DispMenuList1 = ''

  private async setMaster (): Promise<void> {
    const facirityNo = this.getQueryString(this.$route.query.facilityNo)
    if (facirityNo) {
      this.masterInfo = await this.masterService.getMasterInfo(facirityNo)
    } else {
      this.masterInfo = new MasterInfo([], [], [])
    }
  }

  private getCategories () {
    this.categoryList.splice(0)
    if (this.masterInfo != null && this.masterInfo.orderMenuClsList != null) {
      this.masterInfo.orderMenuClsList.forEach(orderMenuCls => {
        this.categoryList.push({ code: orderMenuCls.OrderMenuClsCD.toString(), name: orderMenuCls.OrderMenuClsName.toString(), elementId: 'category-btn-' + orderMenuCls.OrderMenuClsCD.toString() })
      })
    }
  }

  private setCurrentMenuList (inputCategory: string | null) {
    this.category = inputCategory
    this.menuList.splice(0)

    if (inputCategory == null || this.masterInfo == null || this.masterInfo.orderMenuList == null) {
      return
    }

    const targetOrderMenuList = this.masterInfo.orderMenuList.filter(orderMenu => orderMenu.OrderMenuClsCD.toString() === inputCategory)

    if (targetOrderMenuList != null) {
      targetOrderMenuList.forEach(orderMenu => {
        this.menuList.push({ category: orderMenu.OrderMenuClsCD, idx: orderMenu.OrderMenuCD, name: orderMenu.OrderMenuName, price: orderMenu.UnitPrice })
      })
    }

    // -- remove class --
    const categoryBtnItems = document.getElementsByClassName('category-item')
    for (let cnt = 0; cnt < categoryBtnItems.length; cnt++) {
      const categoryBtnItem = categoryBtnItems[cnt].firstElementChild
      if (categoryBtnItem) {
        categoryBtnItem.classList.remove('selected')
      }
    }

    const categoryBtn = document.getElementById('category-btn-' + inputCategory)
    if (categoryBtn) {
      categoryBtn.classList.add('selected')
      categoryBtn.scrollIntoView(true)
      window.scrollTo(0, 0)
    }
  }

  async created (): Promise<void> {
    await this.setMaster()
    this.getCategories()
  }

  async mounted (): Promise<void> {
    await super.mounted()

    const setMenuListTimeout = setTimeout(() => {
      this.setCurrentMenuList(null)
      clearInterval(setMenuListInterval)
    }, 3000)

    const setMenuListInterval = setInterval(() => {
      if (this.masterInfo != null && this.masterInfo.orderMenuClsList != null && this.masterInfo.orderMenuClsList.length > 0) {
        this.setCurrentMenuList(this.category)
        clearTimeout(setMenuListTimeout)
        clearInterval(setMenuListInterval)
      }
    }, 100)

    var environments = new Environments(true)
    await environments.loadSetting()
    this.DispMenuList1 = environments.DISP_MENULIST_1

    this.changeDisabledBottonStatus(false)
    this.changeMakeOrderBottonStatus()
  }

  async onAccountInfoChanged (): Promise<void> {
    const header = this.$refs.header as Header
    header.updateAccount(this.accountInfo ?? null, false)
    this.changeMakeOrderBottonStatus()
  }

  private changeAccountNumber () {
    this.changeDisabledTemporaryBottonStatus()
    this.backToScanAccountNumber()
  }

  private showOrderedList () {
    this.changeDisabledTemporaryBottonStatus()
    this.moveToOrderedList()
  }

  private makeOrders () {
    this.changeDisabledTemporaryBottonStatus()
    this.moveToConfirmOrder()
  }

  private changeMakeOrderBottonStatus () {
    // accountInfoのcartItemsに1件もない場合、注文確定ボタンを無効化する
    var element = (document.getElementById('checkOderButton')) as HTMLInputElement
    if (!element) {
      return
    }
    if (this.accountInfo && this.accountInfo.cartItems.length) {
      element.disabled = false
      element.classList.remove('btn-secondary')
      element.classList.add('btn-primary')
    } else {
      element.disabled = true
      element.classList.remove('btn-primary')
      element.classList.add('btn-secondary')
    }
  }

  private showMenuDetail (category: string, idx: string) {
    this.moveToMenuDetail(category, idx)
  }
}
</script>

<style scoped>
.border {
  min-height: 5%;
  max-height: 5%;
  font-size: 2vh;
  align-content: center;
  padding: 0vh 0vw 0vh 2vw;
  border-color: black;
}
.main .category {
  min-height: 7%;
  max-height: 7%;
  align-items: center;
  overflow-x: auto;
  white-space: nowrap;
}
.main .category-group {
  list-style: none;
  padding: 0vh 0vw 0vh 0vw;
  margin: 0vh 2vw 0vh 2vw;
}
.main .category-item {
  display: inline-block;
  padding: 0vh 0vw 0vh 0vw;
  margin: 0vh 0vw 0vh 0vw;
}
.main .category-group .btn {
  font-size: 2.5vh;
  text-align: center;
  text-decoration: underline;
  padding: 0vh 2vw 0vh 2vw;
}
.main .category-group .btn.selected {
  color: rgb(0, 128, 255);
}

.main .menu-list {
  min-height: 78%;
  max-height: 78%;
  overflow-y:auto;
}
.main .menu-list .group {
  padding: 0vh 0vw 0vh 0vw;
  margin: 1vh 3vw 1vh 3vw;
}
.main .menu-list .group .item {
  list-style: none;
  padding: 0vh 0vw 0vh 0vw;
  margin: 0vh 0vw 1vh 0vw;
  box-shadow: 0px 0px 3px;
  border: 1px solid gray;
  border-radius: 10px;
}
.main .menu-list .group .item .menu {
  min-height: flex;
  max-height: flex;
  padding: 0vh 0vw 0vh 0vw;
  margin: 0vh 0vw 0vh 0vw;
}
.main .menu-list .group .item .menu .menu-upper {
  padding: 0vh 0vw 0vh 2vw;
  margin: 0vh 0vw 0vh 0vw;
}
.main .menu-list .group .item .menu .menu-upper .menu-name {
  text-align: left;
  padding: 0vh 0vw 0vh 2vw;
  margin: 0vh 0vw 0vh 0vw;
}
.main .menu-list .group .item .menu .menu-icon-right {
  text-align: right;
  padding: 0vh 0vw 0vh 0vw;
  margin: 0vh 0vw 0vh 0vw;
}
.main .menu-list .group .item .menu .menu-icon-right .icon-right {
  padding: 0vh 2vw 0vh 0vw;
  margin: 0vh 0vw 0vh 0vw;
}
.main .menu-list .group .item .menu .menu-bottom {
  padding: 0vh 2vw 0vh 0vw;
  margin: 0vh 0vw 0vh 0vw;
}
.main .menu-list .group .item .menu .menu-bottom .menu-price {
  text-align: right;
  padding: 0vh 10vw 0vh 0vw;
  margin: 0vh 0vw 0vh 0vw;
}

.main .menu-btn {
  min-height: 10%;
  max-height: 10%;
  align-content: flex-end;
}
.main .btn-change {
  border-style: none;
  background-color:white;
  align-self: center;
  padding: 0vh 0vw 0vh 0vw;
  margin: 0vh 0vw 1vh 0vw;
}
.main .btn-change .btn {
  font-size: auto;
  white-space: nowrap;
   padding: 0vh 0vw 0vh 0vw;
  margin: 0vh 0vw 0vh 0vw;
}
.main .btn-show {
  border-style: none;
  background-color:white;
  align-self: center;
  padding: 0vh 0vw 0vh 0vw;
  margin: 0vh 0vw 1vh 0vw;
}
.main .btn-show .btn {
  font-size: auto;
  white-space: nowrap;
  padding: 0vh 0vw 0vh 0vw;
  margin: 0vh 0vw 0vh 0vw;
}
.main .btn-commit {
  align-self: center;
  padding: 0vh 1vw 0vh 0vw;
  margin: 0vh 0vw 1vh 0vw;
}
.main .btn-commit .btn {
  text-align: center;
  white-space: nowrap;
  padding: 1vh 8vw 1vh 8vw;
  margin: 0vh 0vw 0vh 0vw;
}
</style>
