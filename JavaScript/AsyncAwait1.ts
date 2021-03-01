// Promise,Resolve,Reject,Then,Catch < AsyncAwait
// AsyncAwait使えばいいらしい。

// awaitの中で更にawaitできるのか？
// 無理だな。awaitしたければasyncは必須。
// async,awaitのペアを作らないといけないけど、awaitの中にawaitはどこにasyncを挟める余地があるかって問題になる。

// いや、クロージャーで囲ってごにょごにょすればいけるけど、複雑になる。追えなくなりそう。
// ただでさえ複雑なのに更に複雑になるな。

// うーん。多分頑張ればもっと入れ子にすることはできるけど、複雑になるし、やるべきではないな。
// そもそもそうしないといけない状況は設計から間違っている気がする。


function sleep(sec) {
    return new Promise((resolve) => {
        setTimeout(() => {
            console.log(`wait: ${sec} sec`);
            resolve(sec);
        }, sec * 1000);
    });
}

function raise() {
    return new Promise((_, reject) => {
        reject(new Error("failed"));
    });
}

async function sum(a, b) {
    return a + b;
}

async function main() {
    const sec = await sleep(2);
    console.log(`result: ${sec} sec`);
    try {
        await raise();
    } catch (e) {
        console.log(`error: ${e}`);
    }
    const not_await = sum(1, 2);
    console.log(`not await: ${not_await}`);
    const did_await = await sum(2, 3);
    console.log(`did await: ${did_await}`);
}

main2();



async function main2() {
    await new Promise(
        resolve => {
            setTimeout(
                () => {
                    console.log('Calculating_1');
                    resolve("done!_1")
                },
                2000
            );
        }).then(data1 => {
            let innerProcess = async () => await new Promise(
                resolve => {
                    setTimeout(
                        () => {
                            console.log('Calculating_2');
                            resolve("done!_2");
                        },
                        2000
                    );
                });
            innerProcess().then(data2 => {
                console.log(data2)
                let innerinnerProcess = async () => await new Promise(
                    resolve => {
                        setTimeout(
                            () => {
                                console.log('Calculating_3');
                                resolve("done!_3");
                            },
                            2000
                        );
                    }
                );
                innerinnerProcess().then(data3 => console.log(data3));
            });
            console.log(data1);
        });
    console.log("done");
}
