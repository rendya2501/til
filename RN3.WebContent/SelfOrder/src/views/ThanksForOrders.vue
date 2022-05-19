<template>
  <b-container fluid class="wrapper d-flex flex-column">
    <b-row>
      <Header ref="header" :context="context" />
    </b-row>
    <b-row class="flex-grow-1">
      <b-container fluid class="main d-flex flex-column">
        <b-row class="middle">
          <b-col><span v-html="environments.DISP_THANKSORDER_1"></span></b-col>
        </b-row>
        <b-row class="bottom">
          <b-col class="back-btn">
            <b-button
              variant="link"
              @click="
                clearCart();
                moveToMenuList();
              "
              v-text="'戻る'"
            />
          </b-col>
        </b-row>
      </b-container>
    </b-row>
  </b-container>
</template>

<script lang="ts">
import { Component } from 'vue-property-decorator';
import { ViewBase } from '@/views/ViewBase';
import Header from '@/components/Header.vue';
import { namespace } from 'vuex-class';
import { HeaderContext } from '@/types/context/HeaderContext';

const CommonModule = namespace('common');

/**
 * 注文完了画面
 */
@Component({
  components: {
    Header
  }
})
export default class ThansForOrders extends ViewBase {
  @CommonModule.Action
  private clearCart!: () => void;

  @CommonModule.Getter
  private getHeaderContext!: HeaderContext;

  private context: HeaderContext = new HeaderContext();

  /**
   * ライフサイクルcreated
   */
  async created(): Promise<void> {
    this.context = this.getHeaderContext;
  }

  /**
   * ライフサイクルフックmounted
   */
  async mounted(): Promise<void> {
    try {
      await super.mounted();
      // iOSだと勝手に画面が遷移してしまうことがあるので少しディレイする。
      setTimeout(() => this.clearCart(), 100);
    } catch (error) {
      console.error(error);
      this.toast.error('エラーが発生しました。スタッフをお呼びください。');
    }
  }
}
</script>

<style scoped>
.main .middle {
  min-height: 95%;
  max-height: 95%;
  align-items: center;
}
.main .bottom {
  min-height: 5%;
  max-height: 5%;
  align-items: flex-end;
  padding: 0vh 0vw 2vh 0vw;
}
</style>
