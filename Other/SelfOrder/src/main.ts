import Vue from 'vue'
import App from './App.vue'
import { BootstrapVue, BIconTrashFill, BIconChevronCompactRight, BIconJustify } from 'bootstrap-vue'
import router from './router'

import 'bootstrap/dist/css/bootstrap.css'
import 'bootstrap-vue/dist/bootstrap-vue.css'

Vue.config.productionTip = false

Vue.use(BootstrapVue)
Vue.component('BIconTrashFill', BIconTrashFill)
Vue.component('BIconChevronCompactRight', BIconChevronCompactRight)
Vue.component('BIconJustify', BIconJustify)

const vm = new Vue({
  router,
  render: h => h(App)
}).$mount('#app')

export default vm
