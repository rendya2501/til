<template>
  <b-container fluid class="wrapper d-flex flex-column">
    <b-row>
      <Header ref="header" />
    </b-row>
    <b-row class="flex-grow-1">
      <b-container fluid class="main d-flex flex-column">
        <b-row class="border border-dark">
          <span v-html="environments.DISP_MENULIST_1"></span>
        </b-row>
        <b-row class="category d-flex">
          <b-col class="category-group">
            <li
              class="category-item"
              v-for="(menuClass, index) in menuClassList"
              :key="index"
            >
              <button
                class="btn btn-link-primary"
                :id="menuClass.elementId"
                @click="setSelectedMenuList(menuClass.code)"
                v-bind:disabled="!isTotalEnable"
                v-text="menuClass.name"
              />
            </li>
          </b-col>
        </b-row>
        <b-row class="menu-list d-flex">
          <b-col class="group">
            <b-row class="item" v-for="(menu, index) in menuList" :key="index">
              <b-col
                class="menu"
                @click="showMenuDetail(menu.menuClassCD, menu.menuCD)"
                v-bind:disabled="!isTotalEnable"
              >
                <b-row class="p-0 m-0">
                  <b-col class="p-0 m-0">
                    <b-row class="menu-name">
                      <b-col v-text="menu.menuName" />
                    </b-row>
                    <b-row class="menu-price">
                      <b-col v-text="'￥ ' + menu.price.toLocaleString()" />
                    </b-row>
                  </b-col>
                  <b-col align-self="center" cols="auto" class="icon-right">
                    <b-icon-chevron-double-right />
                  </b-col>
                </b-row>
              </b-col>
            </b-row>
          </b-col>
        </b-row>
        <b-row class="bottom d-flex" v-if="encryptRepreAccountNo">
          <b-col class="btn-change">
            <b-button
              variant="link"
              @click="moveToScanQR()"
              v-text="'キャンセル'"
            />
          </b-col>
          <b-col class="btn-show">
            <b-button
              variant="link"
              v-bind:disabled="!isTotalEnable"
              @click="moveToOrderedList()"
              v-text="'注文履歴'"
            />
          </b-col>
          <b-col class="btn-commit">
            <b-button
              variant="primary"
              v-bind:disabled="!isTotalEnable || cartItemList.length == 0"
              @click="moveToConfirmOrder()"
              v-text="'注文確認'"
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
import { MenuResponse } from '@/types/response/MenuResponse';
import { MenuRequest } from '@/types/request/MenuRequest';
import { namespace } from 'vuex-class';
import { OrderMenu } from '@/types/entity/OrderMenu';
import { OrderRequest } from '@/types/entity/OrderRequest';
import { CartItem } from '@/types/entity/CartItem';

const CommonModule = namespace('common');

/**
 * メニュー画面
 */
@Component({
  components: {
    Header
  }
})
export default class MenuList extends ViewBase {
  /** パターンコード */
  @CommonModule.State
  private patternCD!: number;
  /** 状態管理:暗号化された会計No */
  @CommonModule.State
  private encryptRepreAccountNo!: string;
  /** 状態管理:暗号化されたWeb会員CD */
  @CommonModule.State
  private encryptWebMemberCD!: string;
  /** 総合メニュー情報 */
  @CommonModule.State
  private menu!: MenuResponse;
  /** カートのアイテム */
  @CommonModule.State
  private cartItemList!: CartItem[];

  @CommonModule.Action
  private setMenu!: (value: MenuResponse) => void;
  @CommonModule.Action
  private setSelectedMenu!: (value: OrderMenu) => void;
  @CommonModule.Action
  private setTargetRequest!: (value: OrderRequest[]) => void;
  @CommonModule.Action
  private clearSelectedMenu!: () => void;
  @CommonModule.Action
  private clearTargetRequest!: () => void;

  /** メニュー分類一覧 */
  private menuClassList: Array<{
    code: number;
    name: string;
    elementId: string;
  }> = [];
  /** メニュー一覧 */
  private menuList: Array<{
    menuClassCD: number;
    menuCD: number;
    menuName: string;
    price: number;
  }> = [];

  /**
   * ライフサイクルフックcreated
   */
  async created(): Promise<void> {
    // 選択したメニューを初期化する
    this.clearSelectedMenu();
    this.clearTargetRequest();
  }

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

    if (!this.menu) {
      this.isBusy = true;
    }
    // メニュー情報をロードします。
    await this.loadMenu()
      .then(() => {
        // メニュー分類をセット(上のバー)
        this.setMenuClass();
        const setMenuListTimeout = setTimeout(() => {
          this.setSelectedMenuList(null);
          clearInterval(setMenuListInterval);
        }, 3000);

        const setMenuListInterval = setInterval(() => {
          if (
            this.menu != null &&
            this.menu.MenuClassList != null &&
            this.menu.MenuClassList.length > 0
          ) {
            this.setSelectedMenuList(this.menu.MenuClassList[0].MenuClassCD);
            clearTimeout(setMenuListTimeout);
            clearInterval(setMenuListInterval);
          }
        }, 100);
      })
      // 操作可能にする
      .then(() => (this.isTotalEnable = true))
      .catch(error => {
        if (typeof error == 'string') {
          this.toast.error(error);
        } else {
          console.error(error);
          this.toast.error('エラーが発生しました。スタッフをお呼びください。');
        }
      })
      .finally(() => (this.isBusy = false));
  }

  /**
   * メニュー情報をロードします。
   */
  private async loadMenu(): Promise<void> {
    // メニュー取得済みなら再取得しない。
    if (this.menu) {
      return Promise.resolve();
    }
    // メニュー取得リクエスト生成
    const request = new MenuRequest(this.patternCD);
    // メニュー情報取得
    await this.selfOrderService.getMenu(request, this.encryptWebMemberCD).then(
      resolve => this.setMenu(resolve),
      reject => Promise.reject(reject.data)
    );
  }

  /**
   * メニュー分類情報をセットします。
   * TOd_MenuClassのMenuClassCDとNameが上部に並びます。
   */
  private setMenuClass() {
    this.menuClassList.splice(0);
    if (this.menu != null && this.menu.MenuClassList != null) {
      this.menu.MenuClassList.forEach(menuClass => {
        this.menuClassList.push({
          code: menuClass.MenuClassCD,
          name: menuClass.MenuClassName,
          elementId: 'category-btn-' + menuClass.MenuClassCD.toString()
        });
      });
    }
  }

  /**
   * 選択しているMenuClassCDのメニューをセットします。
   * TOd_Menuの情報がセットされます。
   */
  private setSelectedMenuList(menuClassCD: number | null) {
    this.menuList.splice(0);

    if (
      menuClassCD == null ||
      this.menu == null ||
      this.menu.MenuList == null
    ) {
      return;
    }

    const targetMenuList = this.menu.MenuList.filter(
      orderMenu => orderMenu.MenuClassCD == menuClassCD
    );

    if (targetMenuList != null) {
      // ParentMenuCDだけ抜き出して、nullではないコードを持つメニューcdは削除する。
      // 親子関係を実装する時はこの処理は削除する。
      const hasParentList = targetMenuList
        .filter(f => f.ParentMenuCD != null)
        .map(m => m.ParentMenuCD);
      targetMenuList.forEach(menuList => {
        if (!hasParentList.includes(menuList.MenuCD)) {
          this.menuList.push({
            menuClassCD: menuList.MenuClassCD,
            menuCD: menuList.MenuCD,
            menuName: menuList.MenuName,
            price: menuList.UnitSellingPrice ?? 0
          });
        }
      });
    }

    // -- remove class --
    const categoryBtnItems = document.getElementsByClassName('category-item');
    for (let cnt = 0; cnt < categoryBtnItems.length; cnt++) {
      const categoryBtnItem = categoryBtnItems[cnt].firstElementChild;
      if (categoryBtnItem) {
        categoryBtnItem.classList.remove('selected');
      }
    }

    const categoryBtn = document.getElementById(
      'category-btn-' + menuClassCD.toString()
    );
    if (categoryBtn) {
      categoryBtn.classList.add('selected');
      categoryBtn.scrollIntoView(true);
      window.scrollTo(0, 0);
    }
  }

  /**
   * メニューボタンクリック処理。
   * メニュー詳細画面を開きます。
   */
  private showMenuDetail(menuClassCD: number, menuCD: number) {
    this.setTargetRequest(
      this.menu.RequestList.filter(
        request =>
          request.MenuClassCD == menuClassCD && request.MenuCD == menuCD
      )
    );
    this.setSelectedMenu(this.menu.MenuList.filter(x => x.MenuCD == menuCD)[0]);
    this.moveToMenuDetail();
  }
}
</script>

<style scoped>
.border {
  min-height: 5%;
  max-height: 5%;
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
  overflow-y: auto;
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
.main .menu-list .group .item .menu .menu-name {
  text-align: left;
  padding: 0vh 0vw 0vh 0vw;
  margin: 0vh 0vw 0vh 0vw;
}
.main .menu-list .group .item .menu .menu-price {
  text-align: right;
  padding: 0vh 0vw 0vh 0vw;
  margin: 0vh 0vw 0vh 0vw;
}
.main .menu-list .group .item .menu .icon-right {
  padding: 0vh 3vw 0vh 0vw;
  margin: 0vh 0vw 0vh 0vw;
}

.main .bottom {
  min-height: 10%;
  max-height: 10%;
  align-content: flex-end;
}
.main .btn-change {
  border-style: none;
  background-color: white;
  align-self: center;
  padding: 0vh 0vw 0vh 0vw;
  margin: 0vh 0vw 1vh 0vw;
}
.main .btn-change .btn {
  white-space: nowrap;
  padding: 0vh 0vw 0vh 0vw;
  margin: 0vh 0vw 0vh 0vw;
}
.main .btn-show {
  border-style: none;
  background-color: white;
  align-self: center;
  padding: 0vh 0vw 0vh 0vw;
  margin: 0vh 0vw 1vh 0vw;
}
.main .btn-show .btn {
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
