---
title: HealthBar
---

Поставляется вместе с визуальным префабом, отображающим простую полоску _жизни_ над персонажем. С помощью простого скрипта _FollowCameraRotation_ всегда поворачивается, так чтобы из основной камеры был виден текст. Сама логика класса __HealtBar__ описан ниже и может также использоваться в другом префабе с другой жудожественной стилистикой.

## Image

Ссылка на юнити canvas-изображение (UnityEngine.UI.Image) визуальной полоски. Его свойство Image.fillAmount используется как отображение оставшегося "количества жизни". 

## Text

Ссылка на юнити canvas-текст (UnityEngine.UI.Text) внутри визуальной полоски. В качестве текста (свойство .text) помещается процент "оставшейся жизни" (если ShowPercentage == true) или абсолютное число.

## ShowText
_bool ShowText_

Отображать ли в качестве текста "оставшиеся количество жизни". По умолчанию = false, а текст может использовать например, для отображения имени агента. 

## CurrentHealth
_MaxValue CurrentHealth_

Позволяет указать текущие значение _Current_, помнит прошлое значение _Previous_, рассчитывает процент _Percentage_ от максимума _Maximum_. 

```csharp
public class MaxValue
{
	public float Current; 
	public float Previous;
	public float Maximum;
	public float Percentage { get { return Current * 100 / Maximum; } }

	public MaxValue(float argCurrentValue, float argPreviousValue, float argMaximumValue)
	{
    Current = argCurrentValue; Previous = argPreviousValue; Maximum = argMaximumValue;
	}
}
```

## ChangeHealth()
_ChangeHealth(MaxValue argCurrentHealth)_

Изменить текущие и прошлое значения "оставшейся жизни". Запускает кароутин, который с заданной скоростью (_AnimationSpeed_) анимирует движение полоски "жизни" при повреждении/лечении. 



