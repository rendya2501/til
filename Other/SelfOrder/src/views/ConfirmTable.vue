<template>
  <b-container fluid class="wrapper vh-100 d-flex flex-column">
    <b-row>
      <Header ref="header" />
    </b-row>
    <b-row class="flex-grow-1">
      <b-container fluid class="main d-flex flex-column">
        <b-row class="border border-dark" v-if="this.accountInfo">
          <span v-html="this.DispMenuList1"></span>
        </b-row>
        <b-row class="category d-flex" v-if="this.accountInfo">
          <b-col class="category-group">
            <li
              class="category-item"
              v-for="category in categoryList"
              :key="category.code"
            >
              <button
                type="bottun"
                class="btn btn-link-primary"
                :id="category.elementId"
                @click="setCurrentMenuList(category.code)"
              >
                {{ category.name }}
              </button>
            </li>
          </b-col>
        </b-row>
        <b-row class="menu-list d-flex" v-if="this.accountInfo">
          <b-col class="group">
            <b-row
              class="item"
              v-for="(menu, index) in menuList"
              :key="`${menu.idx}-${index}`"
            >
              <b-col
                type="text"
                class="menu"
                @click="showMenuDetail(menu.category, menu.idx)"
              >
                <b-row class="menu-upper">
                  <b-col type="text" class="menu-name">{{ menu.name }}</b-col>
                </b-row>
                <b-row class="menu-icon-right">
                  <b-col class="icon-right">
                    <b-icon-chevron-compact-right></b-icon-chevron-compact-right>
                  </b-col>
                </b-row>
                <b-row class="menu-bottom">
                  <b-col type="text" class="menu-price"
                    >￥ {{ menu.price.toLocaleString() }}</b-col
                  >
                </b-row>
              </b-col>
            </b-row>
          </b-col>
        </b-row>
        <b-row class="menu-btn d-flex">
          <b-col class="btn-change">
            <button
              type="button"
              class="btn btn-link"
              @click="backToMenuList"
            >
              スタッフ呼出
            </button>
          </b-col>
          <b-col class="btn-show">
            <button
              type="button"
              class="btn btn-link"
              @click="backToMenuList"
              v-if="this.accountInfo"
            >
              はい
            </button>
          </b-col>
        </b-row>
      </b-container>
    </b-row>
  </b-container>
</template>

<script lang="ts">
import { Component } from "vue-property-decorator";
import ViewBase from "@/views/ViewBase.vue";
import Header from "@/components/Header.vue";
import Environments from "@/constants/Environments";

@Component({
  components: {
    Header,
  },
})
export default class ConfirmTable extends ViewBase {
  private backToMenuList() {
    this.changeDisabledTemporaryBottonStatus();
    this.$router.push({ path: "/", query: this.addCategoryParameter() });
  }
}
</script>
