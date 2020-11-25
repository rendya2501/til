
// ①日付で比較したい場合、setHoursで時間を0にしてミリ秒で比較する
DateInstance.setHours(0,0,0,0) === DateInstance.setHours(0,0,0,0)


// ②getTimeで比較する方法
// ミリ秒単位で比較するので、厳密に同じであることを比較したい場合に用いる
DateTime1.getTime() == DateTime2.getTime()
