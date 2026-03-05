---
title: RandomUnique
---

Обеспечивает возврат уникальных индексов

# Конструктор 
_RandomUnique(System.Random argRnd)_

При создании нужно передать объект System.Random.

# Свойство
## int UsedCount

Сколько индексов уже расходавано.

## IdKey
_Dictionary<int, string> IdKey_

Словарь соответствий индекс-ключ.

## KeyId
_Dictionary<string, int> KeyId_

Словарь соответствий ключ-индекс.

# Методы

## Get
_int Get(int argMax, GetInt getInt = null)_

Получить новый неиспользованный индекс, с максимально возможным (argMax). По умолчанию, для получения нового индекса используется _rnd.Next(argMax)_. Можно переопределить функцию получения нового индекса getInt.

Возврат: -1 - нет свободных индексов, >=0 неиспользованный индекс

## AddKey()
_AddKey(string argKey)_

Добавить текстовый ключ, для которого будет автоматически выделен индекс (следующий, еще не использованный).

## MarkUsed()
_MarkUsed(int argIndex)_
_MarkUsed(string argKey)_

Пометить индекс (argIndex) как использованый. В случае передачи текстового ключа (argKey) будет найден, соответствующий индекс.

## MarkUnUsed()
_MarkUnUsed(int argIndex)_
_MarkUnUsed(string argKey)_

Пометить индекс (argIndex) снова как не использованый. В случае передачи текстового ключа (argKey) будет найден, соответствующий индекс.

