# Stateパターン

---

[slideshare_Gofのデザインパターン stateパターン編](https://www.slideshare.net/ayumuitou52/gof-state)  

``` mermaid
classDiagram
direction BT

class State {
    <<interface>>
    +methodA()*
    +methodB()*
    +methodC()*
    +methodD()*
}
class Context{
    -state
    +requestX()*
    +requestY()*
    +requestZ()*
}
class ConcreteState1{
    +methodA()*
    +methodB()*
    +methodC()*
    +methodD()*
}
class ConcreteState2{
    +methodA()*
    +methodB()*
    +methodC()*
    +methodD()*
}

Context o--> State
ConcreteState1 ..|> State
ConcreteState2 ..|> State
```
