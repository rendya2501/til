



public abstract class Item
{
    protected string caption;
    public Item(string caption)
    {
        this.caption = caption;
    }
    public abstract string MakeHTML();
}

public abstract class Link : Item
{
    protected string url;
    public Link(string caption, string url) : base(caption)
    {
        this.url = url;
    }
}


public abstract class Tray : Item
{
    protected List<Item> tray = new List<Item>();

    //コンストラクタ
    public Tray(String caption) : base(caption) { }

    public void Add(Item item) => tray.Add(item);
}
