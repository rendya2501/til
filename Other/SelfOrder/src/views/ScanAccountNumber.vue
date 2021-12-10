<template>
  <b-container fluid class="wrapper vh-100 d-flex flex-column">
    <b-row class="header" v-show="this.isCheckedParameter">
      <Header :title="title" />
    </b-row>
    <b-row class="flex-grow-1" v-show="this.isCheckedParameter">
      <b-container fluid class="main d-flex flex-column">
        <b-row class="upper d-flex">
          <b-col class="block">
            <img id="barcode-image" class="img" src="@/assets/holder.png" />
            <b-col class="scan-button" id="startRead">
              <b-button variant="primary" @click="startReadBarcode">[<b-icon-justify aria-hidden="true" rotate="90"></b-icon-justify>] スキャンする</b-button>
              <b-row class="usage-area">
                <span v-html=this.DispBarcode1></span>
              </b-row>
              <b-row class="middle">
                <img class="img" src="@/assets/holder_shot.png" />
              </b-row>
            </b-col>
            <b-col id="interactive" class="viewport" hidden>
            </b-col>
          </b-col>
        </b-row>
        <b-row class="middle d-flex">
        </b-row>
        <b-row class="bottom d-flex">
          <b-col class="return-menu">
            <button type="button" class="btn btn-link" @click="backToRegistTableNumber" v-show="this.qrscan === '1'">戻る</button>
          </b-col>
        </b-row>
      </b-container>
    </b-row>
  </b-container>
</template>

<script lang="ts">
import { Component } from 'vue-property-decorator'
import ViewBase from '@/views/ViewBase.vue'
import Header from '@/components/Header.vue'
import Quagga from '@ericblade/quagga2'
import Environments from '@/constants/Environments'
import AccountService from '@/scripts/services/AccountService'
import AccountInfo from '@/types/AccountInfo'

@Component({
  components: {
    Header
  }
})
export default class ScanAccountNumber extends ViewBase {
  private title = 'ホルダーのバーコードをスキャンしてください。'
  private environments = new Environments(true)
  protected accountService = new AccountService()
  private facilityNo : string | null = null
  private isCheckedParameter = false
  private DispBarcode1 = ''

  async mounted (): Promise<void> {
    await super.mounted()

    if (this.accountInfo) {
      this.moveToMenuList()
      return
    }

    this.isCheckedParameter = true

    this.changeDisabledBottonStatus(false)
    this.facilityNo = this.$route.query.facilityNo ? String(this.$route.query.facilityNo) : null
    const tableNo = this.$route.query.tableNo ? String(this.$route.query.tableNo) : null
    await this.setFacilityTable(this.facilityNo, tableNo)

    var environments = new Environments(true)
    await environments.loadSetting()
    this.DispBarcode1 = environments.DISP_BARCODE_1
  }

  public onWindowResize (): void {
    super.onWindowResize()
    const interactiveElement = document.getElementById('interactive')
    if (interactiveElement instanceof HTMLElement) {
      const videoElements = interactiveElement.getElementsByTagName('video')
      if (videoElements !== null && videoElements.length > 0 && videoElements[0] instanceof HTMLVideoElement) {
        videoElements[0].style.height = this.getCameraHightString()
        videoElements[0].style.width = this.getCameraWidthString()
        this.resizeingCameraAspectRatio(videoElements[0])
      }
    }
  }

  private async onDummyScaned () {
    this.scaned('0051')
  }

  private async scaned (holderNo: string) {
    this.stopQuagga()
    try {
      await this.setAccount(holderNo)
    } catch (error) {
      // APIエラー表示するため不要
      // this.toast.error('アカウント情報が取得できませんでした。')
      setTimeout(() => {
        this.$router.go(0)
      }, 3000)
      return
    }

    if (this.accountInfo) {
      this.accountService.clearCartItems(String(this.facilityNo), this.accountInfo)
      this.moveToMenuList()
    } else {
      // APIエラー表示するため不要
      // this.toast.error('お会計番号が読み込めませんでした。')
    }
  }

  private backToRegistTableNumber () {
    this.stopQuagga()

    this.changeDisabledTemporaryBottonStatus()
    this.moveToRegistTableNumber()
  }

  private startReadBarcode () {
    const myToast = this.toast
    if (this.environments.DEBUG_MODE) {
      this.onDummyScaned()
      return
    }
    if (!('mediaDevices' in navigator) || !('getUserMedia' in navigator.mediaDevices)) {
      this.toast.error(this.environments.CAMERA_ERROR_BC)
      return
    }

    Quagga.init({
      inputStream: {
        type: 'LiveStream',
        constraints: {
          width: 640,
          height: 480,
          facingMode: 'environment'
        }
      },
      locator: {
        patchSize: this.environments.BARCODE_SCAN_PATCH_SIZE,
        halfSample: true
      },
      numOfWorkers: 4,
      decoder: {
        readers: ['code_39_reader']
      },
      locate: true
    }, this.initializedQuagga)

    const scanedCallback = this.scaned
    const barcodeScanDetectedCount = this.environments.BARCODE_SCAN_DETECTED_COUNT
    let detectedCount = 0
    let detectedCode: string | null = null
    Quagga.onDetected(function (result) {
      const code = result.codeResult.code
      if (code === null) {
        detectedCount = 0
        detectedCode = null
      } else if (detectedCode === code) {
        detectedCount++
        if (detectedCount >= barcodeScanDetectedCount) {
          detectedCount = 0
          scanedCallback(detectedCode)
        }
      } else {
        detectedCount = 1
        detectedCode = code
      }
    })
  }

  private initializedQuagga (err: any) {
    if (err) {
      this.toast.error(this.environments.CAMERA_INIT_BC)
      return
    }

    const barcodeImage = document.getElementById('barcode-image')
    const startReadCol = document.getElementById('startRead')
    const interactiveCol = document.getElementById('interactive')
    if (barcodeImage != null && startReadCol !== null && interactiveCol !== null) {
      barcodeImage.hidden = true
      startReadCol.hidden = true
      interactiveCol.hidden = false
    }

    const interactiveElement = document.getElementById('interactive')
    if (interactiveElement instanceof HTMLElement) {
      const videoElements = interactiveElement.getElementsByTagName('video')
      if (videoElements !== null && videoElements.length > 0 && videoElements[0] instanceof HTMLVideoElement) {
        videoElements[0].style.height = this.getCameraHightString()
        videoElements[0].style.width = this.getCameraWidthString()
        // videoElementsのサイズがb-rowサイズを超えていたら補正する
        this.resizeingCameraAspectRatio(videoElements[0])
      }
      const canvasElements = interactiveElement.getElementsByClassName('drawingBuffer')
      if (canvasElements !== null && canvasElements.length > 0 && canvasElements[0] instanceof HTMLCanvasElement) {
        canvasElements[0].style.width = '160px'
        canvasElements[0].style.position = 'absolute'
        canvasElements[0].style.left = '0px'
        canvasElements[0].style.top = '0px'
      }
    }
    Quagga.start()
  }

  private getCameraHightString () {
    const viewWindow = document.getElementsByClassName('middle')
    if (viewWindow !== null) {
      return String(viewWindow[0].clientHeight) + 'px'
    }
    return '120px'
  }

  private getCameraWidthString () {
    const viewWindow = document.getElementsByClassName('middle')
    if (viewWindow !== null) {
      // 表示範囲のheightからアスペクト比4:3を使ってwidthを返す
      return String((viewWindow[0].clientHeight * 4) / 3) + 'px'
    }
    return '160px'
  }

  private resizeingCameraAspectRatio (videoElement: HTMLVideoElement) {
    const viewWindow = document.getElementsByClassName('middle')
    if (viewWindow !== null) {
      if (viewWindow[0].clientHeight < videoElement.clientHeight) {
        // b-row高をオーバーしているのでリサイズする
        // 高さをKyeにして3:4で再構成
        videoElement.style.height = String(viewWindow[0].clientHeight) + 'px'
        videoElement.style.width = String((viewWindow[0].clientHeight * 4 / 3)) + 'px'
      }
      if (viewWindow[0].clientWidth < videoElement.clientWidth) {
        // b-row幅をオーバーしているのでリサイズする
        // 高さをKeyにして4:3で再構成
        videoElement.style.width = String(viewWindow[0].clientHeight) + 'px'
        videoElement.style.height = String((viewWindow[0].clientHeight * 3 / 4)) + 'px'
      }
    }
  }

  private stopQuagga () {
    const interactiveCol = document.getElementById('interactive')
    if (interactiveCol instanceof HTMLElement && !interactiveCol.hidden) {
      Quagga.stop()
    }
  }
}
</script>

<style scoped>
.header {
  font-size: 2vh;
}
.main .upper {
  min-height: 90%;
  max-height: 90%;
}
.main .upper .block {
  min-height: 100%;
  max-height: 100%;
  padding: 0vh 0vw 0vh 0vw;
  margin: 0vh 0vw 0vh 0vw;
}
.main .upper .block .img {
  padding: 0vh 0vw 0vh 0vw;
  margin: 2vh 0vw 3vh 0vw;
}
.main .bottom {
  min-height: 10%;
  max-height: 10%;
  align-items: center;
}

.main .middle {
  min-height: 60%;
  max-height: 60%;
  align-items: flex-start;
  padding: 0vh 0vw 0vh 0vw;
}

.main .bottom .btn {
  font-size: 2vh;
  padding: 0vh 0vw 0vh 0vw;
  margin: 0vh 0vw 0vh 0vw;
}
#interactive.viewport {
  position: relative;
  width: 100%;
  height: 100%;
  object-fit: scale-down;
}

.usage-area {
  align-items: center;
  padding: 0vh 0vw 0vh 0vw;
  margin: 1vh 0vw 1vh 0vw;
}

.holder-shot {
  width: 100%;
}

#interactive.viewport >>> video,
#interactive.viewport >>> canvas {
  width: 100% !important;
  height: 100% !important;
  object-fit: scale-down;
}
#interactive.viewport > canvas.drawingBuffer {
  position: absolute;
  left: 0px;
  top: 0px;
}
</style>
