---
title: Component
---
Часто компоненты основываются на функциональности других компонентов. Чтобы явно указать, от каких компонентов зависит компонент, нужно воспользоваться аттрибутом Component. Это позволит переложить на компилятор проблему отслживания зависимостей. В примере, ниже компонент ItemBuild зависит от трех компонентов, которые должны быть в вашем проекте TopCamera, GhostCache, ItemCollision. 

```csharp
  [Component(typeof(TopCamera), typeof(GhostCache), typeof(ItemCollision))]
  public partial class ItemBuild : MonoBehaviour
  { ... }
```
