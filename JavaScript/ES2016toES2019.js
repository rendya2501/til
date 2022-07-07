/**
 * 最近追加されたJavaScript構文まとめ
 * https://dev.to/albertomontalesi/javascript-everything-from-es2016-to-es2019-246c
 */


(() => {
    let array = [1, 2, 4, 5];
    // true
    console.log(array.includes(2));
    // false
    console.log(array.includes(3));
})();


(() => {
    // 4
    Math.pow(2, 2);
    2 ** 2;
    // 8
    Math.pow(2, 3);
    2 ** 3;
    // 16
    Math.pow(Math.pow(2, 2), 2);
    2 ** 2 ** 2;
})();


(() => {
    // " hello"
    console.log("hello".padStart(6));
    // "hello "
    console.log("hello".padEnd(6));
    // 10-2 = 8 "        hi"
    console.log("hi".padStart(10));
    // 10-6 = 4 "    welcome"
    console.log("welcome".padStart(10));
})();


(() => {
    const strings = ["short", "medium length", "very long string"];
    const longestString = strings.sort(str => str.length).map(str => str.length)[0];
    strings.forEach(str => console.log(str.padStart(longestString)));
    // very long string
    //    mediam length
    //            short
})();