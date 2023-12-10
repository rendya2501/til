# サンプル1

``` cs
public interface IMediator {
    void Interaction();
}

public class Mediator : IMediator {
    public Piece piece;
    public Part part;
    public Thing thing;

    public Mediator() {
        // setup components
    }

    public void Interaction() {

    }
}

public class Piece {
    IMediator mediator;

    public Piece(IMediator mediator) => this.mediator = mediator;

    public void Invoke() {
        mediator.Interaction();
    }
}

public class Part {
    IMediator mediator;
    public Part(IMediator mediator) => this.mediator = mediator;
}

public class Thing {
    IMediator mediator;
    public Thing(IMediator mediator) => this.mediator = mediator;
}
```

[Mediator Design Pattern (C#) - YouTube](https://www.youtube.com/watch?v=VYLD75sU1rw&t=21s)  
