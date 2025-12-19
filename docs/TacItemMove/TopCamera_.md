---
title: TopCamera_
---

Расширяет поведение компонента [TopCamera](../../TacCamera/TopCamera) методами, которые рейкастят нажатие под курсором мыши и определяют для чего пригодно та или инач точка в сцене.

```csharp
namespace Tac.Camera
{
	public partial class TopCamera
	{
		public LayerMask TerrainLayer; // Слой поверхности 
		public LayerMask BuildingsLayer; // Слой строений
		public EventSystem EventSystem; // Нужно задать в редакторе Unity, используется для определения находится ли курсор над UI
		public float MaxHeight = 1000; // Максимальная высота с которой начинается рейкаст
		public Dictionary<string, bool> MoveError; // Словарь флагов, какие ошибки произошли 

		public bool IsUsingUI() // используется для определения находится ли курсор над UI
		public GameObject GetBuilding(Vector2 touch) // Возвращает объект на слое строений в точке нажатия мышью
		public (Vector3, GameObject) GetTerrain(Vector2 touch) //Возвращает координаты и объект с компонентом Item2 на слое поверхности в точке нажатия мышью

    // Определяет разрешено ли строить в точке нажатия мышью (логику поведения см. ниже)
		public (Vector3, GameObject) GetAllowBuildPoint(Vector2 touch, List<Bounds> argBounds)
		void OnDrawGizmos() // в режиме редактора Unity показывает зелеными линиями направление лучей рейкаста (debugRays) и точку цель (debugPoint)
	}
}
```

# GetAllowBuildPoint

Осуществляет рейкаст на поверхность (TerrainLayer), если находит объект на котором можно строить, а сам объект, который строится имеет колайдеры (argBounds - размеры колайдеров) происходит проверка пересечений. Каждый колайдер не должен пересекаться с другими объектами на слое строений (BuildingsLayer). Допустимо в качестве TerrainLayer использовать не только непосредственно слой поверхностей, но и слой строений. Тогда более сложными проверками занимается компонент [ItemBuild](../../TacItemMove/ItemBuild) с помощью [ItemCollision](../../TacItemMove/ItemCollision).


