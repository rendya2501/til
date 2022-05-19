import { Vue } from 'vue-property-decorator';

/**
 * VueのToastダイアログ
 */
export class ToastFactory extends Vue {
  /**
   * エラーダイアログ表示
   * @param message
   * @param title
   */
  public error(message: string, title = 'エラー') {
    this.show(message, title, 'danger');
  }
  /**
   * 警告ダイアログ表示
   * @param message
   * @param title
   */
  public warning(message: string, title = '警告') {
    this.show(message, title, 'warning');
  }
  /**
   * 成功ダイアログ表示
   * @param message
   * @param title
   */
  public success(message: string, title = '成功') {
    this.show(message, title, 'success');
  }

  /**
   * ダイアログを表示するラップ処理
   * @param message
   * @param title
   * @param variant
   */
  public show(message: string, title: string, variant: string) {
    // mainのvmからだとエラーになる場合があるため継承元のVueからtoastを作成
    this.$bvToast.toast(message, {
      title: title,
      variant: variant,
      toaster: 'b-toaster-top-right',
      solid: true
    });
  }
}
