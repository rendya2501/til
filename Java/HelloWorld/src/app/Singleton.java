package app;

public class Singleton {
    private static Singleton singleton = new Singleton();
    /**
     * コンストラクタ
     */
    private Singleton() {
        System.out.println("インスタンスを生成しました。");
    }
    /**
     * getter
     * @return Singleton instance
     */
    public static Singleton getInstance() {
        return singleton;
    }
}