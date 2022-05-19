<template>
  <b-container fluid class="wrapper d-flex flex-column">
    <b-row class="flex-grow-1">
      <b-container fluid class="main d-flex flex-column">
        <b-row class="upper">
          <b-col>
            <h1 class="title">SelfOrder</h1>
          </b-col>
        </b-row>
        <b-row class="middle">
          <b-col id="startRead">
            <div class="middle-div">
              <p class="description">
                「カメラ起動」をタップしてください。<br />
                カメラが起動しましたら、テーブルに配置されているQRコードをスキャンしてください。
              </p>
            </div>
          </b-col>
          <b-col id="reading" hidden>
            <canvas id="canvas" hidden></canvas>
          </b-col>
        </b-row>
        <b-row class="bottom">
          <b-col>
            <b-button
              block
              variant="primary"
              v-bind:disabled="
                !encryptWebMemberCD || !isEnableCamera || !isTotalEnable
              "
              @click="activateCamera()"
            >
              <img src="@/assets/qr.png" width="20px" height="20px" />
              カメラ起動
            </b-button>
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
import jsQR from 'jsqr';
import { ViewBase } from '@/views/ViewBase';
import { FramePlayerListRequest } from '@/types/request/FramePlayerListRequest';
import { Player } from '@/types/entity/Player';
import Header from '@/components/Header.vue';
import { namespace } from 'vuex-class';

const CommonModule = namespace('common');

/**
 * QRコード読み込み画面
 */
@Component({
  components: {
    Header
  }
})
export default class ScanQRCode extends ViewBase {
  /** 状態管理:暗号化されたWeb会員CD */
  @CommonModule.State
  private encryptWebMemberCD!: string;
  /** 状態管理:暗号化された会計No */
  @CommonModule.State
  private encryptRepreAccountNo!: string;

  @CommonModule.Action
  private setEncryptWebMemberCD!: (value: string | null) => void;
  @CommonModule.Action
  private setEncryptRepreAccountNo!: (value: string) => void;
  @CommonModule.Action
  private setPatternCD!: (value: number) => void;
  @CommonModule.Action
  private setTableNo!: (value: number) => void;
  @CommonModule.Action
  private setBusinessDate!:(value:Date) => void;
  @CommonModule.Action
  private initialize!: () => void;
  @CommonModule.Action
  private setPlayerList!: (value: Player[]) => void;
  @CommonModule.Action
  private setMultiMode!: (value: boolean) => void;

  /** カメラ有効フラグ */
  private isEnableCamera: boolean = true;

  /**
   * ライフサイクルフックcreated
   */
  async created(): Promise<void> {
    // 状態管理初期化
    this.initialize();
  }

  /**
   * ライフサイクルフックmounted
   */
  async mounted(): Promise<void> {
    try {
      await super.mounted();
    } catch (error) {
      console.error(error);
      this.toast.error('エラーが発生しました。スタッフをお呼びください。');
      return;
    }

    // RNWebからログインした場合、クエリパラメータに暗号化されたWeb会員Noが来るので、取得して状態管理に登録する。
    let encryptWebMemberCD = this.getQueryString(this.$route.query.WebMemberCD);
    // // デバッグモードでは読み込めたことにする。
    // if (this.environments.DEBUG_MODE) {
    //   encryptWebMemberCD = 'GoBdNWnb87CfHAw89MoIow==';
    // }
    // 読み込めなかった場合、どちらにせよnullが入って以降の操作はできなくなる。
    this.setEncryptWebMemberCD(encryptWebMemberCD);
    // 万が一Web会員Noがなければ、それ以降の処理はできない。
    if (!encryptWebMemberCD) {
      this.toast.error('ログインに失敗しました。スタッフをお呼びください。');
      return;
    }

    // クルクル表示
    this.isBusy = true;
    // プレーヤー一覧をロード
    await this.loadFramePlayerList()
      // 代表者の会計Noが状態管理に登録されていれば操作可能にする。
      .then(() => (this.isTotalEnable = Boolean(this.encryptRepreAccountNo)))
      .catch(error => this.toast.error(error))
      .finally(() => (this.isBusy = false));
  }

  /**
   * プレーヤー一覧をロードします。
   */
  private async loadFramePlayerList(): Promise<void> {
    // プレーヤー一覧取得リクエストの生成
    const framePlayerListRequest = new FramePlayerListRequest(
      this.encryptWebMemberCD
    );
    // プレーヤー一覧の取得
    await this.selfOrderService
      .getFramePlayerList(framePlayerListRequest, this.encryptWebMemberCD)
      .then(
        resolve => {
          // 代表者が精算済みか判定する。
          const repreSettledFlag =
            resolve?.PlayerList?.find(
              f => f.EncryptedAccountNo == resolve.EncryptedAccountNo
            )?.SettlementFlag ?? false;
          // 代表者が精算済みの場合、注文はできないのでアナウンスして塞ぐ。
          if (repreSettledFlag) {
            return Promise.reject(
              '精算済みのため、サービスをご利用いただくことができません。'
            );
          }
          // 営業日をセットする。
          this.setBusinessDate(resolve.BusinessDate);
          // 暗号化された代表者の会計Noを登録する。
          this.setEncryptRepreAccountNo(resolve.EncryptedAccountNo);
          // モードを状態管理に登録する。
          this.setMultiMode(resolve.IsMultiSelect);
          // プレーヤー一覧を状態管理に登録する。
          this.setPlayerList(resolve.PlayerList);

          // if (this.environments.DEBUG_MODE) {
          //   this.setMultiMode(true);
          //   this.setPlayerList(
          //     resolve.PlayerList.map((m, index) => {
          //       return {
          //         EncryptedAccountNo: m.EncryptedAccountNo,
          //         AccountNo: m.AccountNo,
          //         Name: m.Name,
          //         Kana: m.Kana,
          //         SettlementFlag: index <= 2 ? false : true
          //       };
          //     })
          //   );
          // }
        },
        reject => {
          console.error(reject);
          return Promise.reject(reject.data);
        }
      );
  }

  /**
   * カメラを起動します。
   */
  private activateCamera(): void {
    // // 開発用
    // if (this.environments.DEBUG_MODE) {
    //   this.setPatternCD(4);
    //   this.setTableNo(11);
    //   this.moveToConfirmTable();
    //   return;
    // }

    // カメラを使用できるデバイスか判定する。
    if (
      !('mediaDevices' in navigator) ||
      !('getUserMedia' in navigator.mediaDevices)
    ) {
      this.toast.error(this.environments.CAMERA_ERROR_QR);
      return;
    }
    // ビデオ要素の取得
    const video = document.createElement('video');
    if (!(video instanceof HTMLVideoElement)) {
      return;
    }
    video.id = 'video-id';
    // キャンバス要素の取得
    const canvasElement = document.getElementById('canvas');
    if (!(canvasElement instanceof HTMLCanvasElement)) {
      return;
    }
    const canvas = canvasElement.getContext('2d');
    if (!(canvas instanceof CanvasRenderingContext2D)) {
      return;
    }

    const startReadCol = this.getElementById('startRead');
    const readingCol = this.getElementById('reading');
    if (startReadCol !== null && readingCol !== null) {
      startReadCol.hidden = true;
      readingCol.hidden = false;
    } else {
      return;
    }

    const tick = () => {
      if (video.readyState !== HTMLMediaElement.HAVE_ENOUGH_DATA) {
        window.requestAnimationFrame(tick);
        return;
      }
      canvasElement.hidden = false;
      canvasElement.height = this.getVideoElementLength();
      canvasElement.width = canvasElement.height;
      canvas.drawImage(video, 0, 0, canvasElement.width, canvasElement.height);
      const imageData = canvas.getImageData(
        0,
        0,
        canvasElement.width,
        canvasElement.height
      );
      const code = jsQR(imageData.data, imageData.width, imageData.height, {
        inversionAttempts: 'dontInvert'
      });
      if (code) {
        try {
          // カメラ止める
          video.pause();
          video.src = '';
          startReadCol.hidden = false;
          readingCol.hidden = true;
          // 「patternCD=**&tableNo=**」の形式で来る
          const firstSplit = code.data.split('&');
          if (firstSplit.length <= 1) {
            throw new Error();
          }
          // patternCDの取得
          const patternCD = firstSplit[0].split('=');
          if (
            patternCD.length <= 1 ||
            patternCD[0] != 'patternCD' ||
            !Number(patternCD[1])
          ) {
            throw new Error();
          }
          // tableNoの取得
          const tableNo = firstSplit[1].split('=');
          if (
            tableNo.length <= 1 ||
            tableNo[0] != 'tableNo' ||
            !Number(tableNo[1])
          ) {
            throw new Error();
          }
          // 正常に取得できたらセットする
          this.setPatternCD(Number(patternCD[1]));
          this.setTableNo(Number(tableNo[1]));
          // 来場者確認画面に移動
          this.moveToConfirmTable();
        } catch (error) {
          video.pause();
          this.toast.error(
            '読み取りに失敗しました。画面を更新して再度お試しください。'
          );
        } finally {
          return;
        }
      }
      window.requestAnimationFrame(tick);
    };

    const elementLength = this.getVideoElementLength();
    const options = {
      video: {
        facingMode: 'environment',
        width: { min: 0, max: elementLength },
        height: { min: 0, max: elementLength },
        aspectRatio: 1.0
      }
    };
    navigator.mediaDevices
      .getUserMedia(options)
      .then(function(stream) {
        video.srcObject = stream;
        video.setAttribute('playsinline', 'true');
        video.play();
        window.requestAnimationFrame(tick);
      })
      .catch(error => {
        this.toast.error(
          'カメラの起動に失敗しました。画面を更新して再度お試しください。'
        );
        return;
      });
    // カメラボタンを押せなくする
    this.isEnableCamera = false;
  }

  /**
   * ビデオ要素の長さを取得します。
   */
  private getVideoElementLength(): number {
    const viewWindow = document.getElementsByClassName('middle');
    if (viewWindow !== null && viewWindow.length > 0) {
      return Math.min(
        viewWindow[0].clientWidth - 30,
        viewWindow[0].clientHeight
      );
    }
    return 120;
  }
}
</script>

<style scoped>
.main .title {
  background-color: #007bff;
  color: white !important;
  height: 100px;
  width: 90%;
  display: table;
  font-weight: bold;
  font-size: 3rem;
  margin: auto;
  padding: 20px 0;
}

.main .upper {
  min-height: 30%;
  max-height: 30%;
  align-items: center;
}
.main .middle {
  min-height: 40%;
  max-height: 40%;
  align-items: center;
}
.middle-div {
  text-align: center;
  display: block;
}
.middle-div .description {
  text-align: left;
  display: inline-block;
  margin: 0px 10px;
}
.main .bottom {
  min-height: 30%;
  max-height: 30%;
  align-items: center;
}
#startRead .btn {
  background-color: white;
  border: none;
}
</style>
