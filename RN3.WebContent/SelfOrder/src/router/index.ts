import Vue from 'vue';
import VueRouter, { RouteConfig } from 'vue-router';
import ScanQRCode from '../views/ScanQRCode.vue';
import ConfirmTable from '../views/ConfirmTable.vue';
import MenuList from '../views/MenuList.vue';
import MenuDetail from '../views/MenuDetail.vue';
import ConfirmOrder from '../views/ConfirmOrder.vue';
import OrderedList from '../views/OrderedList.vue';
import ThanksForOrders from '../views/ThanksForOrders.vue';

Vue.use(VueRouter);

const routes: Array<RouteConfig> = [
  {
    path: '/',
    name: 'ScanQRCode',
    component: ScanQRCode
  },
  {
    path: '/ConfirmTable',
    name: 'ConfirmTable',
    component: ConfirmTable
  },
  {
    path: '/MenuList',
    name: 'MenuList',
    component: MenuList
  },
  {
    path: '/MenuDetail',
    name: 'MenuDetail',
    component: MenuDetail
  },
  {
    path: '/ConfirmOrder',
    name: 'ConfirmOrder',
    component: ConfirmOrder
  },
  {
    path: '/OrderedList',
    name: 'OrderedList',
    component: OrderedList
  },
  {
    path: '/ThanksForOrders',
    name: 'ThanksForOrders',
    component: ThanksForOrders
  }
];

const router = new VueRouter({
  // サーバー構成がわかるまで、historyからhashでの運用に変更します。
  mode: 'hash',
  base: process.env.BASE_URL,
  routes: routes
});
export default router;
