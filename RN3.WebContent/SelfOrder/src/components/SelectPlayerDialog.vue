<template>
  <div class="overlay" v-show="showContent">
    <div class="content">
      <b-container fluid class="main d-flex flex-column">
        <!-- メッセージ行 -->
        <b-row class="middle-0 flex-column">
          <b-row class="menu-description flex-row">
            <b-col class="txt-description">
              {{ message }}
            </b-col>
          </b-row>
        </b-row>
        <!-- プレーヤー行 -->
        <b-row
          class="middle-player d-flex"
          v-for="(item, index) in playerList"
          :key="index"
        >
          <b-col class="label-player">
            <b-button
              :variant="
                item.isSelected
                  ? 'primary'
                  : 'outline-primary bg-white text-primary'
              "
              class="btn btn-player"
              block
              v-text="item.accountNo + ' ' + item.playerName + ' 様'"
              @click="selectPlayer(item)"
            />
          </b-col>
        </b-row>
        <!-- ボタン行 -->
        <b-row class="dialog-bottom flex-fill d-flex">
          <b-col>
            <b-button
              variant="link"
              @click="showContent = false"
              v-text="'戻る'"
            />
          </b-col>
          <b-col>
            <b-button
              block
              variant="primary"
              @click="confirmCommand()"
              v-bind:disabled="playerList.filter(f => f.isSelected).length == 0"
              v-text="'確定'"
            />
          </b-col>
        </b-row>
      </b-container>
    </div>
  </div>
</template>

<script lang="ts">
import { Vue, Component, Prop, Emit } from 'vue-property-decorator';
import { SelectPlayerDialogContext } from '@/types/context/SelectPlayerDialogContext';

@Component
export default class SelectPlayerDialog extends Vue {
  @Prop({ required: true, default: [] })
  private playerList!: Array<SelectPlayerDialogContext>;
  @Prop({ required: true, default: '' })
  private message!: string;
  @Prop({ default: false })
  private SingleSelectMode!: boolean;

  /**
   * 外部公開している確定ボタン押下処理
   */
  @Emit('confirm')
  private confirm(playerList: Array<SelectPlayerDialogContext>): void {}

  /**
   * コンポーネント表示フラグ
   */
  private showContent: boolean = false;

  /**
   * プレーヤー選択時の処理
   */
  private selectPlayer(item: SelectPlayerDialogContext) {
    // 1人選択モードなら他を落として選択した人のみにする。
    if (this.SingleSelectMode) {
      this.playerList.forEach(player => (player.isSelected = false));
      item.isSelected = true;
    } else {
      item.isSelected = !item.isSelected;
    }
  }

  /**
   * 確定ボタン押下処理(デリゲートを実行)
   */
  private confirmCommand(): void {
    this.confirm(this.playerList);
  }

  /**
   * オーバーレイを表示する。
   */
  public show(): void {
    this.showContent = true;
  }
  /**
   * オーバーレイを閉じる。
   */
  public hide(): void {
    this.showContent = false;
  }
}
</script>

<style scoped>
.main {
  height: 100%;
  width: 100%;
  overflow-y: auto;
}

.middle-0 {
  min-height: flex;
  max-height: flex;
  white-space: nowrap;
  padding: 0vh 0vw 0vh 0vw;
  margin: 1vh 0vw 1vh 0vw;
}

.middle-player {
  height: auto;
  width: 100%;
  align-items: center;
  padding: 1vh 0vw 0vh 0vw;
  margin: 0vh 0vw 0vh 0vw;
  flex-shrink: 5;
}

.label-player {
  width: 40%;
  text-align: right;
  padding: 0vh 0vw 0vh 0vw;
  margin: 0vh 0vw 1vh 0vw;
}

.btn-player {
  text-align: left;
  padding: 1vh 10vw 1vh 10vw;
}

.dialog-bottom {
  height: auto;
  width: auto;
  align-items: flex-end;
  padding: 4vh 0vw 1vh 0vw;
}

.overlay {
  /* 要素を重ねた時の順番 */
  z-index: 1;

  /* 画面全体を覆う設定 */
  position: fixed;
  top: 0;
  left: 0;
  min-width: 100%;
  min-height: 100%;
  background-color: rgba(0, 0, 0, 0.5);

  /* 画面の中央に要素を表示させる設定 */
  display: flex;
  align-items: center;
  justify-content: center;
}

.content {
  z-index: 2;
  width: 90%;
  height: auto;
  padding: 1em;
  background: #fff;
  white-space: nowrap;
  box-shadow: 0px 0px 5px;
  border: 1px solid gray;
  border-radius: 10px;
}

.menu-description {
  padding: 0vh 0vw 0vh 0vw;
  margin: 0vh 0vw 0vh 0vw;
}

.txt-description {
  text-align: left;
  white-space: normal;
  position: relative;
  padding: 2vh 0vw 2vh 2vw;
  margin: 0vh 0vw 0vh 0vw;
}
</style>
