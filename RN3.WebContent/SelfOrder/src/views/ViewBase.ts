import { Vue } from 'vue-property-decorator';
import { ToastFactory } from '@/views/ToastFactory';
import { SelfOrderService } from '@/scripts/services/SelfOrderService';
import { Environments } from '@/constants/Environments';

/**
 *
 */
export class ViewBase extends Vue {
  /** Vueダイアログ */
  protected toast = new ToastFactory();
  /** APIサービス */
  protected selfOrderService = new SelfOrderService();
  /** 環境情報 */
  protected environments: Environments = new Environments(true);
  /** クルクル表示 */
  protected isBusy: boolean = false;
  /** 全体の項目の有効無効制御 */
  protected isTotalEnable: boolean = false;

  /**
   *
   */
  async mounted(): Promise<void> {
    // 環境変数ロード
    this.environments.loadSetting();
    // ロード時画面サイズ最適化
    window.addEventListener('load', this.onWindowResize);
    // リサイズ時画面サイズ最適化
    window.addEventListener('resize', this.onWindowResize);
    // ピンチズームを無効
    window.addEventListener('touchstart', this.preventMultipleTouches, {
      passive: false
    });
    // safari対策でdocumentも
    document.addEventListener('touchstart', this.preventMultipleTouches, {
      passive: false
    });
    // ダブルタップでブラウザが拡大するのを防ぐ
    window.addEventListener(
      'dblclick',
      function(e) {
        e.preventDefault();
      },
      { passive: false }
    );
    // safari対策でdocumentも
    document.addEventListener(
      'dblclick',
      function(e) {
        e.preventDefault();
      },
      { passive: false }
    );
    // スクロール禁止
    window.addEventListener('touchmove', this.preventMultipleTouches, {
      passive: false
    });
    // safari対策でdocumentも
    document.addEventListener('touchmove', this.preventMultipleTouches, {
      passive: false
    });
    // 画面サイズ最適化
    this.onWindowResize();
  }

  /**
   * 画面サイズ変更イベント時の処理
   * 画面サイズを最適化します。
   */
  public onWindowResize(): void {
    // アドレスバーの高さを除いたブラウザの高さを取得
    const vh = window.innerHeight;
    // 全ての画面の大本を実サイズに合わせる
    const appDiv = document.getElementById('app');
    if (appDiv) {
      appDiv.style.height = `${vh}px`;
    }
    // mainコンテンツの高さを実サイズに合わせる
    const mainDivs = document.getElementsByClassName('main');
    if (mainDivs && mainDivs.length > 0 && mainDivs[0] instanceof HTMLElement) {
      // ヘッダーがあればヘッダー分の高さを引く。
      const headerDiv = document.getElementsByClassName('header');
      const headerHeight =
        headerDiv && headerDiv.length > 0 && headerDiv[0] instanceof HTMLElement
          ? headerDiv[0].clientHeight
          : 0;
      (mainDivs[0] as HTMLElement).style.height = `${vh - headerHeight}px`;
    }
  }

  /**
   * 複数タッチによる動作を防止する
   * @param e
   */
  public preventMultipleTouches(e: TouchEvent): void {
    if (e.touches.length >= 2 || e.targetTouches.length >= 2) {
      e.preventDefault();
    }
  }

  /**
   *クエリパラメータを取得します。
   */
  protected getQueryString(param: string | (string | null)[]): string | null {
    return param ? String(param) : null;
  }

  /**
   *
   */
  protected getElementById(id: string): HTMLElement {
    return document.getElementById(id) as HTMLElement;
  }

  /**
   *
   */
  protected focusById(id: string): void {
    this.getElementById(id).focus();
  }

  /**
   * 1つ前の画面に戻ります。
   */
  protected backTo(): void {
    this.$router.go(-1);
  }

  /**
   * QRコード読み込み画面に遷移します。
   */
  protected moveToScanQR(): void {
    this.$router.push({
      path: '/',
      query: Object.assign({}, this.$route.query)
    });
  }
  /**
   * 来場者確認画面に遷移します。
   */
  protected moveToConfirmTable(): void {
    this.$router.push({
      path: 'ConfirmTable',
      query: Object.assign({}, this.$route.query)
    });
  }
  /**
   * メニュー画面に遷移します。
   */
  protected moveToMenuList(): void {
    this.$router.push({
      path: 'MenuList',
      query: Object.assign({}, this.$route.query)
    });
  }
  /**
   * メニュー選択画面に遷移します。
   */
  protected moveToMenuDetail(): void {
    this.$router.push({
      path: 'MenuDetail',
      query: Object.assign({}, this.$route.query)
    });
  }
  /**
   * 注文履歴画面に遷移します。
   */
  protected moveToOrderedList(): void {
    this.$router.push({
      path: 'OrderedList',
      query: Object.assign({}, this.$route.query)
    });
  }
  /**
   * 注文確認画面に遷移します。
   */
  protected moveToConfirmOrder(): void {
    this.$router.push({
      path: 'ConfirmOrder',
      query: Object.assign({}, this.$route.query)
    });
  }
  /**
   * 注文完了画面に遷移します。
   */
  protected moveToThanksForOrders(): void {
    this.$router.push({
      path: 'ThanksForOrders',
      query: Object.assign({}, this.$route.query)
    });
  }
}
