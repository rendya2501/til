/**
 * レスポンスエラー時の例外
 */
export class ResponseError extends Error {
  data: string;
  status: number;
  statusText: string;
  headers: any;

  constructor(error: any) {
    super(error.statusText);
    this.data = error.response.data;
    this.status = error.response.status;
    this.statusText = error.response.statusText;
    this.headers = error.response.headers;
  }
}
