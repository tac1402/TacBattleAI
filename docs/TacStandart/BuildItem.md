---
title: BuildItem
---

Т.н. сущность второго поколения, которая наследуется от [Item](../Item) и расширяется различными компонентами. Суть в том, что эту сущность можно не только создать в сцене (это только Item), но и пользователь может выполнять над ней различные действия по 'постройке' (это уже BuildItem) - поворачивать, выбирать в каком месте разместить и т.п.


# Tac.Wireframe

```csharp
namespace Tac
{
    public partial class BuildItem : Item 
    {
		public Wireframe Wireframe; // В любую сущность встраиваться возможно показать сетку
		public void InitWireframe() { ... } // Перед использованием требует инициализации, ищет в том же объекте компонент Wireframe
		public void ShowError(bool argIsError) { ... } // При наличии ошибки показывает красную сетку, иначе зеленую 
		public void WireframeShow(WireframeMode argMode)  { ... } // Показывает сетку, ту которая указана как WireframeMode
	}
}
```

# Tac.ItemTurn

Встраивает список возможных поворотов [TurnInfo](../../TacItemMove/TurnInfo) в сущность. Перечисление можно изменять под ваш проект.

```csharp
namespace Tac
{
	public partial class BuildItem : Item
	{
		public GameObject Pivot; // Точка вокруг которой будет происходить вращение. GameObject определяется пустым в префабе устанавливая только координаты.
		public GameObject View; // Уровень в префабе, который отображается в сцене
		public TurnInfo Turn; // Описание возможных поворотов 
		public bool DefaultTurn = true; // По умолчанию обечпечивает поворот по горизонтали на 0, 90, 180, 270 градусов, если нужно переопределить, установите = false
		public bool AllowTurn = true; // Можно ли поварачивать объект

		public void TurnNext() { ... } // Осуществить следующий поворот
		public void SetTurn() { ... } // Осуществить текущий поворот, заданный в Turn.CurrentTurn
		public virtual void SetTurn(TurnType argRotateIndex) { ... } // Осуществить произвольный поворот, передав соответствующий индекс argRotateIndex из списка возможны поворотов TurnType
	}
}
```

# Tac.ItemMove

## ItemCollision

```csharp
namespace Tac
{
	public partial class BuildItem : Item
	{
		public bool AllowMove = true; // Можно ли перемещать объект
		public Collider[] Colliders; // Список всех коллайдеро объекта, вызвав InitColliders() вы заполните их автоматически
		public int GhostId; // Идентификатор, еще не построенного/ не существующего объекта в мире

		public void InitColliders() // Требует инициализации, чтобы собрать в массив Colliders все коллайдеры объекта (включая дочерние объекты)
		public static BuildItem GetItem(GameObject go) // находит сущность в объекте go, если он не имеет компонента BuildItem ищет компонент BuildPart, который указывает на главный объект, который содержит BuildItem
	}
}
```

## ItemBuild

```csharp
namespace Tac
{
	public partial class BuildItem : Item
	{
		public Vector3 DiscreteStep = Vector3.one; // Определяет шаг дискретности по всем осям, по умолчанию =1, используется при строительстве, позволяя размещать строимые объекты только учитывая дискретную сетку
		public Vector3 GetDiscrete(Vector3 argValue, XYZ argXYZ) { ... } // Дискретизирует вектор argValue в соответствии с указанными осями argXYZ для которых нужно выполнить дискретизацию
	}
}
```

