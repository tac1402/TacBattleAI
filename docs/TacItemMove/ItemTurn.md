---
title: ItemTurn
---

Обеспечивает поворот объекта, содержащего [Item2](../../TacStandart/Item2).

```csharp

namespace Tac.ItemMove
{
	public class ItemTurn : MonoBehaviour
	{
		public KeyCode RotateKey = KeyCode.Mouse1; // По умолчанию клик правой кнопкой мыши используется для поворот
		private Item2 selectedItem; // сущность которая выбрана для поворота, через метод OnItemTap

		private void Update() // Обеспечивает поворот selectedItem при нажатии RotateKey
		public void OnItemTap(Item2 argItem) { selectedItem = argItem; } // Задает сущность для поворота
	}
}
```
