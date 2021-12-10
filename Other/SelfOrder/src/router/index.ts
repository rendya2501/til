import Vue from 'vue'
import VueRouter, { RouteConfig } from 'vue-router'
import RegistTableNumber from '../views/RegistTableNumber.vue'
import ConfirmTable from '../views/ConfirmTable.vue'
import ScanAccountNumber from '../views/ScanAccountNumber.vue'
import MenuList from '../views/MenuList.vue'
import MenuDetail from '../views/MenuDetail.vue'
import ConfirmOrder from '../views/ConfirmOrder.vue'
import OrderedList from '../views/OrderedList.vue'
import ThanksForOrders from '../views/ThanksForOrders.vue'

Vue.use(VueRouter)

const routes: Array<RouteConfig> = [
  {
    path: '/',
    name: 'RegistTableNumber',
    component: RegistTableNumber
  },
  { 
    path: '/ConfirmTable',
    name: 'ConfirmTable',
    component: ConfirmTable
  },
  {
    path: '/ScanAccountNumber',
    name: 'ScanAccountNumber',
    component: ScanAccountNumber
    // route level code-splitting
    // this generates a separate chunk (about.[hash].js) for this route
    // which is lazy-loaded when the route is visited.
    // component: () => import(/* webpackChunkName: "about" */ '../views/About.vue')
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
]

const router = new VueRouter({
  mode: 'history',
  base: process.env.BASE_URL,
  routes
})

export default router
