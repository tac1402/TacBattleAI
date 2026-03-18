---
title: Agent
---
Абстракция нижнего уровня, решающая математические задачи ИИ, для персонажа. Использует NavMeshAgent агент Юнити.

## Name

Имя агента, используется системное поле gameobject.name, тем самым видно в редакторе Юнити, а так же дублируется в полоске жизни [HealthBar](../../TacAgent/HealthBar).

## TargetPoint
_Vector3 TargetPoint_

Точка на карте куда движется агент.

## WalkDistance
_float WalkDistance (only get)_

Какое расстояние в юнити метрах пройдено по пути к цели WalkTarget;

## PathStatus
_int PathStatus_

0 - путь не задан, 1 - нужно рассчитать, 2 - путь расчитан

## PathPoints
_List\<Vector3> PathPoints_

Рассчитанный путь агента к цели (TargetPoint).

## OnCheckDistance
_event Send OnCheckDistance_

Возникает, когда агент расчитывает какую дистанцию он прошел по назначенному пути.

## OnWalkEnd
_event Send OnWalkEnd_

Возникает, когда агент заканчивает движение к заданной цели

## Init()
Перед использование агента его нужно инициализировать.

## Walk()
_Walk(Vector3 argTarget, float stoppingDistance = 0.1f)_

Дать задание агенту двигаться к точке argTarget, с точностью её достижения stoppingDistance.

## Tick()
Init() запускает короутин, который раз в 0.1 сек. выполняет проверку, какая дистанция пройдена (CheckDistance()) и контролирует точки на пути движения и окончание пути (CheckWalkEnd()). Сами методы закрытые (private), но можно использовать события OnCheckDistance и OnWalkEnd для расширения логики.

## CancelTarget()
Отменить движение к цели и остановиться.

## CheckPosition()
Найти ближайшую доступную позицию на NavMesh карте и поместить в неё агента

## DrawPath()
Рисует линией путь, куда движется агент
