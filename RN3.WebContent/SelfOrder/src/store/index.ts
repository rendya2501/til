import Vuex, { StoreOptions } from 'vuex';
import { RootState } from './types';
import { common } from './common'
import Vue from 'vue';
import createPersistedState from 'vuex-persistedstate'

Vue.use(Vuex);

const store: StoreOptions<RootState> = {
    state: {
        version: '1.0.0',
    },
    modules: {
        common,
    },
    plugins: [createPersistedState({ storage: window.sessionStorage })]
};

export default new Vuex.Store<RootState>(store);