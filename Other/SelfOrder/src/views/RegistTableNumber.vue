<template>
  <b-container fluid class="wrapper vh-100 d-flex flex-column">
    <b-row class="header">
      <Header :title="title" />
    </b-row>
    <b-row class="flex-grow-1">
      <b-container fluid class="main">
        <b-row class="upper">
          <b-col class="message"><span v-html=this.DispQR1></span></b-col>
        </b-row>
        <b-row class="middle">
          <b-col id="startRead">
            <!-- <b-button @click="startReadQr"> -->
            <b-button @click="gotoConfirmTable">
              <img src="@/assets/qr.png" />
            </b-button>
          </b-col>
          <b-col id="reading" hidden>
            <canvas id="canvas"></canvas>
          </b-col>
        </b-row>
      </b-container>
    </b-row>
  </b-container>
</template>

<script lang="ts">
import { Component } from 'vue-property-decorator'
import jsQR from 'jsqr'
import ViewBase from '@/views/ViewBase.vue'
import Header from '@/components/Header.vue'
import Environments from '@/constants/Environments'

@Component({
  components: {
    Header
  }
})
export default class RegistTableNumber extends ViewBase {
  private environments = new Environments(true)
  private title = 'テーブル番号登録'
  private DispQR1 = ''

  async mounted (): Promise<void> {
    await this.setFacilityTable(this.getQueryString(this.$route.query.facilityNo), this.getQueryString(this.$route.query.tableNo))
    await this.setAccount(this.getQueryString(this.$route.query.holderNo))

    if (this.accountInfo) {
      this.moveToMenuList()
    } else if (this.facilityTableInfo) {
      this.moveToScanAccountNumber()
    } else {
      this.qrscan = '1'
    }

    var environments = new Environments(true)
    await environments.loadSetting()
    this.DispQR1 = environments.DISP_QR_1
  }

  private startReadQr () {
    if (this.environments.DEBUG_MODE) {
      this.onDummyScaned()
      return
    }
    if (!('mediaDevices' in navigator) || !('getUserMedia' in navigator.mediaDevices)) {
      this.toast.error(this.environments.CAMERA_ERROR_QR)
      return
    }

    var video = document.createElement('video')
    if (!(video instanceof HTMLVideoElement)) {
      return
    }
    video.id = 'video-id'

    const tick = () => {
      var canvasElement = document.getElementById('canvas')
      if (video instanceof HTMLVideoElement && canvasElement instanceof HTMLCanvasElement) {
        const canvas = canvasElement.getContext('2d')
        if (!(canvas instanceof CanvasRenderingContext2D)) {
          return
        }
        if (video.readyState !== HTMLMediaElement.HAVE_ENOUGH_DATA) {
          window.requestAnimationFrame(tick)
          return
        }

        canvasElement.hidden = false
        canvasElement.height = this.getVideoElementLength()
        canvasElement.width = canvasElement.height
        canvas.drawImage(video, 0, 0, canvasElement.width, canvasElement.height)
        var imageData = canvas.getImageData(0, 0, canvasElement.width, canvasElement.height)
        var code = jsQR(imageData.data, imageData.width, imageData.height, {
          inversionAttempts: 'dontInvert'
        })
        if (code) {
          // this.onDummyScaned()
          let holderNo = this.getQueryString(this.$route.query.holderNo)
          if (holderNo) {
            holderNo = '&holderNo=' + holderNo
          } else {
            holderNo = ''
          }
          window.location.href = code.data + holderNo + '&qrscan=1'
          return
        }
      }
      window.requestAnimationFrame(tick)
    }

    const startReadCol = this.getElementById('startRead')
    const readingCol = this.getElementById('reading')
    if (startReadCol !== null && readingCol !== null) {
      startReadCol.hidden = true
      readingCol.hidden = false
    }

    const elementLength = this.getVideoElementLength()
    const options = {
      video: {
        facingMode: 'environment',
        width: { min: 0, max: elementLength },
        height: { min: 0, max: elementLength },
        aspectRatio: 1.0
      }
    }
    navigator.mediaDevices.getUserMedia(options).then(function (stream) {
      video.srcObject = stream
      video.setAttribute('playsinline', 'true')
      video.play()
      window.requestAnimationFrame(tick)
    })
  }

  private getVideoElementLength (): number {
    const viewWindow = document.getElementsByClassName('middle')
    if (viewWindow !== null && viewWindow.length > 0) {
      return Math.min(viewWindow[0].clientWidth - 30, viewWindow[0].clientHeight)
    }
    return 120
  }

  private async onDummyScaned () {
    this.scaned('1', '11', '0051')
  }

  private async scaned (facilityNo: string, tableNo: string, holderNo: string) {
    await this.setFacilityTable(facilityNo, tableNo)
    await this.setAccount(holderNo)

    if (this.accountInfo) {
      this.moveToMenuList()
    } else if (this.facilityTableInfo) {
      this.moveToScanAccountNumber()
    } else {
      // APIエラー表示するため不要
      // this.toast.error('テーブル番号が読み込めませんでした。')
    }
  }

  private gotoConfirmTable(){
    this.changeDisabledTemporaryBottonStatus();
    this.$router.push({ path: 'ConfirmTable', query: this.addCategoryParameter() });
  }
}
</script>

<style scoped>
.header {
  font-size: 2vh;
}
.main .upper {
  min-height: 30%;
  max-height: 30%;
  align-items:center;
  padding: 0vh 0vw 0vh 0vw;
}
.main .middle {
  min-height: 60%;
  max-height: 60%;
  align-items: flex-start;
  padding: 0vh 0vw 0vh 0vw;
}
#startRead .btn {
  background-color: white;
  border: none;
}
</style>
