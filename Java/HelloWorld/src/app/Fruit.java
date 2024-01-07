package app;

public enum Fruit {
    Orange("オレンジ"),
    Apple("りんご"),
    Cherry("さくらんぼ");

    private String japanese;

    private Fruit(String value) {
        this.japanese = value;
    }
    public String getJapanese(){
        return this.japanese;
    }
}
