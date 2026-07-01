DECLARE @sql NVARCHAR(MAX) = N'';

-- 1. Удаляем все внешние ключи
SELECT @sql += N'ALTER TABLE ' 
    + QUOTENAME(SCHEMA_NAME(schema_id)) + '.' + QUOTENAME(OBJECT_NAME(parent_object_id)) 
    + ' DROP CONSTRAINT ' + QUOTENAME(name) + ';' + CHAR(13)
FROM sys.foreign_keys;

-- 2. Удаляем все пользовательские таблицы (is_ms_shipped = 0 – не системные)
SELECT @sql += N'DROP TABLE ' 
    + QUOTENAME(SCHEMA_NAME(schema_id)) + '.' + QUOTENAME(name) + ';' + CHAR(13)
FROM sys.tables
WHERE is_ms_shipped = 0;   -- пользовательские таблицы (исключаем системные)

-- 3. Выполняем сгенерированный скрипт
EXEC sp_executesql @sql;