
USE [master];
GO

-- Создаём логин, если не существует
IF NOT EXISTS (SELECT * FROM sys.server_principals WHERE name = 'NT AUTHORITY\СИСТЕМА')
BEGIN
    CREATE LOGIN [NT AUTHORITY\СИСТЕМА] FROM WINDOWS;
END
GO

-- Даём права на создание баз данных (роль dbcreator)
USE [master];
GO
ALTER SERVER ROLE [dbcreator] ADD MEMBER [NT AUTHORITY\СИСТЕМА];
GO

-- Для основной базы Rotark даём права db_owner (как и раньше)
USE [Rotark];
GO

-- Добавляем пользователя в роль db_owner, если его там нет
IF ISNULL(IS_ROLEMEMBER('db_owner', 'NT AUTHORITY\СИСТЕМА'), 0) = 0
BEGIN
    EXEC sp_addrolemember 'db_owner', 'NT AUTHORITY\СИСТЕМА';
END
GO