<template>
  <b-container fluid class="wrapper d-flex flex-column">
    <b-row class="flex-grow-1">
      <b-container fluid class="main d-flex flex-column">
        <b-row class="top">
          <b-col>
            <p
              class="text-left"
              v-text="'テーブル番号、会計番号、氏名をご確認ください。'"
            />
            <div>
              <b-table-simple
                hover
                small
                caption-top
                responsive
                class="tbl-table"
                style="margin-right: 20px; line-height: 1.2;"
              >
                <b-tbody>
                  <b-tr variant="primary" style="border: 2px solid #ffffff;">
                    <!-- <b-td
                      v-if="isMultiSelect"
                      variant="primary"
                      style="
                      color: #fff;
                      background-color: #007bff;
                      border: 2px solid #ffffff;"
                    >
                      <b-form-checkbox />
                    </b-td> -->
                    <b-td
                      colspan="3"
                      style="
                          color: #fff;
                          background-color: #007bff;
                          text-align:left;
                          padding-left:5px;"
                      v-text="'テーブル番号 ' + (tableNo ? tableNo : '')"
                    />
                  </b-tr>
                  <b-tr v-for="item in displayPlayerList" :key="item.AccountNo">
                    <b-td
                      v-if="isMultiSelect"
                      :variant="item.SettlementFlag ? 'danger' : 'primary'"
                      style="border: 2px solid #ffffff; vertical-align:middle;"
                    >
                      <b-form-checkbox
                        v-if="item.AccountNo"
                        v-model="item.IsSelected"
                        v-bind:disabled="item.IsDisable"
                      />
                    </b-td>
                    <b-td
                      :variant="item.SettlementFlag ? 'danger' : 'primary'"
                      style="border: 2px solid #ffffff; vertical-align:middle;"
                      v-text="!item.AccountNo ? '　' : item.AccountNo"
                    />
                    <b-td
                      :variant="item.SettlementFlag ? 'danger' : 'primary'"
                      style="border: 2px solid #ffffff; vertical-align:middle;"
                    >
                      <span
                        class="text-left float-left"
                        v-text="!item.Name ? '　' : item.Name + ' 様'"
                      />
                      <span
                        class="text-right float-right"
                        v-if="item.SettlementFlag"
                        v-text="'精算済'"
                      />
                    </b-td>
                  </b-tr>
                </b-tbody>
              </b-table-simple>
            </div>
            <p
              class="text-left"
              v-text="'※精算済みの方には伝票は付けられません。'"
              v-if="displayPlayerList.some(s => s.SettlementFlag)"
            />
          </b-col>
        </b-row>
        <b-row class="middle">
          <b-col>
            <p
              class="text-left"
              v-text="
                '表示内容に間違いがなければ「注文開始」をタップしてください。'
              "
            />
            <b-button
              block
              class="btn"
              variant="primary"
              @click="goToMenuList()"
              v-bind:disabled="!isTotalEnable"
              v-text="'注文開始'"
            />
          </b-col>
        </b-row>
        <b-row class="bottom">
          <b-col>
            <p
              class="text-left"
              v-text="
                '表示内容に誤りありましたら「スタッフ呼出」をタップしてください。'
              "
            />
            <b-button
              class="btn"
              block
              variant="outline-primary"
              @click="callStaff()"
              v-bind:disabled="isWaiting || tableNo == null"
              v-text="'スタッフ呼出'"
            />
          </b-col>
        </b-row>
      </b-container>
    </b-row>
    <div class="overlay" v-show="isShowContent">
      <div class="content">
        <b-container fluid class="d-flex flex-column">
          <b-row class="menu-description flex-row">
            <b-col class="txt-description">
              <p>
                只今、スタッフを呼び出しております。<br />
                しばらくお待ちください。
              </p>
            </b-col>
          </b-row>
          <b-row class="dialog-bottom flex-fill d-flex">
            <b-col class="btn-commit">
              <b-button
                variant="primary"
                @click="isShowContent = false"
                v-text="'OK'"
              />
            </b-col>
          </b-row>
        </b-container>
      </div>
    </div>
  </b-container>
</template>

<script lang="ts">
import { Component } from 'vue-property-decorator';
import { ViewBase } from '@/views/ViewBase';
import Header from '@/components/Header.vue';
import { CallStaffRequest } from '@/types/request/CallStaffRequest';
import { Player } from '@/types/entity/Player';
import { namespace } from 'vuex-class';

const CommonModule = namespace('common');

/**
 * 来場者確認画面
 */
@Component({
  components: {
    Header
  }
})
export default class ConfirmTable extends ViewBase {
  /** 状態管理:暗号化されたWeb会員CD */
  @CommonModule.State
  private encryptWebMemberCD!: string;
  /** 状態管理:暗号化された会計No */
  @CommonModule.State
  private encryptRepreAccountNo!: string;
  /** 状態管理:テーブルNo */
  @CommonModule.State
  private tableNo!: number;
  /** 状態管理:パターンCD */
  @CommonModule.State
  private patternCD!: number;
  /** 状態管理:プレーヤー一覧 */
  @CommonModule.State
  private playerList!: Player[];
  /** 状態管理:複数選択モード */
  @CommonModule.State
  private multiMode!: boolean;

  @CommonModule.Action
  private setPlayerList!: (value: Player[]) => void;
  @CommonModule.Action
  private setMultiMode!: (value: boolean) => void;

  /** スタッフ呼び出しオーバーレイ表示フラグ */
  private isShowContent: boolean = false;
  /** スタッフ呼び出し無効フラグ */
  private isWaiting: boolean = false;
  /** 画面上のプレーヤー一覧 */
  private displayPlayerList: {
    EncryptedAccountNo: string;
    AccountNo: string;
    Name: string;
    Kana: string;
    SettlementFlag: boolean;
    IsSelected: boolean;
    IsDisable: boolean;
  }[] = [];
  /** 複数選択可能かどうか */
  private isMultiSelect: boolean = false;

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

    // 表示用のプレーヤー情報をセット。
    this.setDisplayPlayerList();
    // 代表者の会計Noが状態管理に登録されていれば操作可能にする。
    this.isTotalEnable = Boolean(this.encryptRepreAccountNo);
  }

  /**
   * 表示用のプレーヤー情報をセットします。
   */
  private setDisplayPlayerList(): void {
    // QRコード読み取り画面で取得したプレーヤー情報を画面に反映させる
    this.displayPlayerList = this.playerList.map(m => {
      return {
        EncryptedAccountNo: m.EncryptedAccountNo,
        AccountNo: m.AccountNo,
        Name: m.Name,
        Kana: m.Kana,
        SettlementFlag: m.SettlementFlag,
        // 存在しないプレーヤーと精算済みのプレーヤーは選択を解除する
        IsSelected: Boolean(m.EncryptedAccountNo) && !m.SettlementFlag,
        // 代表者は絶対にチェックを外せない or 精算済みも選択できないようにする。
        IsDisable:
          m.EncryptedAccountNo == this.encryptRepreAccountNo || m.SettlementFlag
      };
    });
    // マルチモード∧プレーヤーが2人以上の場合にチェックボックスを表示する
    this.isMultiSelect = this.multiMode && this.playerList.length >= 2;
  }

  /**
   * 注文開始ボタンクリック処理
   * メニュー画面に遷移します。
   */
  private goToMenuList(): void {
    // 選択しているプレーヤーを抜き出す
    const selectedPlayerList = this.displayPlayerList.filter(f => f.IsSelected);
    // 画面上で選択したプレーヤー情報を改めて登録する。
    this.setPlayerList(
      selectedPlayerList.map(m => {
        return {
          EncryptedAccountNo: m.EncryptedAccountNo,
          AccountNo: m.AccountNo,
          Name: m.Name,
          Kana: m.Kana,
          SettlementFlag: m.SettlementFlag
        };
      })
    );
    // 画面上で選択した情報を元に改めて複数モードを状態管理に登録する。
    this.setMultiMode(this.isMultiSelect && selectedPlayerList.length >= 2);
    // メニュー一覧へ遷移する。
    this.moveToMenuList();
  }

  /**
   * スタッフ呼出ボタンクリック処理
   */
  private async callStaff(): Promise<void> {
    this.isShowContent = true;
    this.isWaiting = true;
    // とりあえず5秒無効にする。
    setTimeout(() => (this.isWaiting = false), 5000);
    // スタッフ通知リクエスト生成
    const callStaffRequest = new CallStaffRequest(
      this.patternCD,
      this.tableNo,
      this.encryptRepreAccountNo,
      ''
    );
    // スタッフに通知する
    await this.selfOrderService
      .callStaff(callStaffRequest, this.encryptWebMemberCD)
      .then(
        resolve => {},
        reject => {
          this.toast.error(reject.data);
          this.isWaiting = false;
        }
      )
      .catch(error => {
        console.error(error);
        this.toast.error('エラーが発生しました。スタッフをお呼びください。');
      });
  }
}
</script>

<style scoped>
.main .top {
  min-height: 50%;
  max-height: 50%;
  align-items: center;
}
.main .middle {
  min-height: 25%;
  max-height: 25%;
  align-items: center;
}
.main .bottom {
  min-height: 25%;
  max-height: 25%;
  align-items: center;
}

.txt-description {
  text-align: left;
  white-space: normal;
  position: relative;
  padding: 2vh 0vw 2vh 2vw;
  margin: 0vh 0vw 0vh 0vw;
}
.btn-commit .btn {
  text-align: center;
  padding: 1vh 10vw 1vh 10vw;
  margin: 0vh 0vw 0vh 0vw;
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
.btn-player {
  text-align: left;
  padding: 1vh 10vw 1vh 10vw;
}

.tbl-table >>> input[type='checkbox'] {
  height: 100%;
  width: 100%;
}
</style>
