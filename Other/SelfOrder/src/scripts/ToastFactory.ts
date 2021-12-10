import { Vue } from 'vue-property-decorator'

export default class ToastFactory extends Vue {
  public error (message: string, title = 'エラー') {
    this.show(message, title, 'danger')
  }

  public warning (message: string, title = '警告') {
    this.show(message, title, 'warning')
  }

  public success (message: string, title = '成功') {
    this.show(message, title, 'success')
  }

  public show (message: string, title: string, variant: string) {
    // mainのvmからだとエラーになる場合があるため継承元のVueからtoastを作成
    this.$bvToast.toast(message, {
      title: title,
      variant: variant,
      toaster: 'b-toaster-top-right',
      solid: true
    })
  }
}
