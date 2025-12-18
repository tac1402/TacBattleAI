---
title: Item2
---

Сущность второго поколения, которая наследуется от [Item](../Item) и расширяется различными компонентами. 


# Tac.Wireframe

```csharp
namespace Tac
{
    public partial class Item2 : Item 
    {
		public Wireframe Wireframe; // В любую сущность встраиваться возможно показать сетку
		public void InitWireframe() { ... } // Перед использованием требует инициализации, ищет в том же объекте компонент Wireframe
		public void ShowError(bool argIsError) { ... } // При наличии ошибки показывает красную сетку, иначе зеленую 
		public void WireframeShow(WireframeMode argMode)  { ... } // Показывает сетку, ту которая указана как WireframeMode
	}
}
```

# Tac.ItemTurn

```csharp
namespace Tac
{
	public partial class Item2 : Item
	{
		public GameObject Pivot; // Точка вокруг которой будет происходить вращение. GameObject определяется пустым в префабе устанавливая только координаты.
		public GameObject View; // Уровень в префабе, который отображается в сцене
		public [TurnInfo](../../TacItemMove/TurnInfo) Turn; // Описание возможных поворотов 
		public bool DefaultTurn = true; // По умолчанию обечпечивает поворот по горизонтали на 0, 90, 180, 270 градусов, если нужно переопределить, установите = false
		public bool AllowTurn = true; // Можно ли поварачивать объект

		public void TurnNext() { ... } // Осуществить следующий поворот
		public void SetTurn() { ... } // Осуществить текущий поворот, заданный в Turn.CurrentTurn
		public virtual void SetTurn(TurnType argRotateIndex) { ... } // Осуществить произвольный поворот, передав соответствующий индекс argRotateIndex из списка возможны поворотов TurnType
	}
}
```

