import Vue from 'vue';
import App from './App.vue';
import {
  BootstrapVue,
  BIconTrashFill,
  BIconJustify,
  BootstrapVueIcons
} from 'bootstrap-vue';
import router from './router';
import store from './store';

import 'bootstrap/dist/css/bootstrap.css';
import 'bootstrap-vue/dist/bootstrap-vue.css';

Vue.config.productionTip = false;

Vue.use(BootstrapVue);
Vue.use(BootstrapVueIcons);
Vue.component('BIconTrashFill', BIconTrashFill);
Vue.component('BIconJustify', BIconJustify);

const vm = new Vue({
  router,
  render: h => h(App),
  store
}).$mount('#app');

export default vm;
