---
title: ItemBuild
---

Обеспечивает режим "строительства" на сцене, позволяя размещать выбраные из UI объекты на сцене. Требует задания трех компонентов для обеспечения своей работы: 

1. [TopCamera](../../TacItemMove/TopCameraExt) - управление камерой для получения сигналов о том, что пользовател строит
2. [GhostCache](../../TacItemCreate/GhostCache) - кэш, через который будут создаваться объекты и их "призраки" (временные объекты, которые мышь передвигает по сцене во время выбора места, где разместить объект)
3. [ItemCollision](../../TacItemMove/ItemCollision) - обеспечивает проверку того ,чтобы размещаемые объекты не пересекались бы


```csharp
	[Component(typeof(TopCamera), typeof(GhostCache), typeof(ItemCollision))]
	public partial class ItemBuild : MonoBehaviour
	{
		public GameObject Grid; // Сетка над поверхностью, если задана будет отображена во время режима строительства
		public XYZ DiscreteType = XYZ.XYZ; // Какие оси нужно дискретизировать при размещении объекта
		public bool IsBuildMode; // Находится ли компонент в режиме строительства

    // Автоматически вызывается метод сброса режима строительства (ResetObjectToPlace()) при нажатии на Escape
    // Если курсор мыши не находится над UI и была выбрана модель для строительства обеспечивает движение "призрака объекта" под мышью,
    // позволяет поварачивать "призрак" или размещает объект если нет пересечений, в сцене левой кнопкой мыши.
		void Update() 

		public void SelectEntity(string argModelName) // Выбрать модель объекта, которую нужно построить
		public void ResetObjectToPlace() // Отменить режим строительства

    // Обеспечивает передвижение "призрака" объекта в режиме строительства каждых 0.3 сек.
		protected IEnumerator TaskMoveGhost()
		{
			while (true)
			{
				if (IsBuildMode == true) { MoveGhost(); }
				yield return new WaitForSeconds(0.3f);
			}
		}
    
	}
}
```
