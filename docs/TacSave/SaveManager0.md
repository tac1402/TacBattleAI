---
title: SaveManager0
---

Это абстрактный класс от оторого вам нужно наследовать свою реализацию [SaveManager](../../TacSave/SaveManager). Он реализует базовое управление сохранениями, которое вы должны расширить и настроить для своего проекта.

## InEditor
_bool InEditor = false_

Запись происходит в отладочном режиме в редакторе Юнити (=true) или в откомпилированной игре (=false, по умолчанию). Во время отладки лучше переключить на =true, тогда сохранения будут записываться в Assets\SaveTmp (создайте соответствующую директорию), иначе они будут сохраняться исходя из настроек компьютера Application.persistentDataPath, что менее удобно во время отладки.

## IsDebugMode
_bool IsDebugMode = true_

Режим отладки, по умолчанию включен. Система сохарнение DirectConvert будет вести лог загрузки (_LoadLog.txt_) в корне игры (на дирректорию выше ../Assets).

## version
_string version = "v0.01"_

Если изменяется набор данных которые вы сохраняете увеличивайте номер версии, чтобы следить за совместимостью версий.

## LoadError
_event Change LoadError_

Подписка на событие позволяет отловить момент, когда во время загрузки игры произошла непредвиденная ошибка. [SaveCatalog](../../TacSave/SaveCatalog) обрабатывает это событие по умолчанию в UI загрузки.

## ILoadGet()
_virtual ILoadManager ILoadGet()_

Обязателен для переопределения в наследнике, чтобы предоставить интерфейс [ILoadManager](../../TacSave/ILoadManager). Подробности см. в [SaveManager](../../TacSave/SaveManager).

## SaveBin
_virtual void SaveBin(string argDirName, string argFileName)_

Переопределяется в наследнике, чтобы управлять ходом сохранения. Подробности см. в [SaveManager](../../TacSave/SaveManager).

## LoadBin
_virtual void LoadBin(string argDirName, string argFileName)_

Переопределяется в наследнике, чтобы управлять ходом загрузки. Подробности см. в [SaveManager](../../TacSave/SaveManager).





