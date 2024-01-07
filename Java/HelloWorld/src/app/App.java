package app;

public class App {
    public static void main(String[] args) {
        System.out.println("Start.");
        Singleton obj1 = Singleton.getInstance();
        Singleton obj2 = Singleton.getInstance();
        if (obj1 == obj2) {
            System.out.println("obj1とobj2は同じインスタンスです。");
        } else {
            System.out.println("obj1とobj2は同じインスタンスではありません。");
        }
        System.out.println("End.");
        Fruit hoge = Fruit.Apple;
        System.out.println(hoge.getJapanese());
    }
}


enum Hoge {
    HOGE,
    FUGA,
    PIYO
}
