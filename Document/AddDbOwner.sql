USE [master];
GO

IF NOT EXISTS (SELECT * FROM sys.server_principals WHERE name = 'NT AUTHORITY\СИСТЕМА')
BEGIN
    CREATE LOGIN [NT AUTHORITY\СИСТЕМА] FROM WINDOWS;
END
GO

USE [Rotark];
GO

-- Добавляем пользователя в роль db_owner, если его там нет
IF ISNULL(IS_ROLEMEMBER('db_owner', 'NT AUTHORITY\СИСТЕМА'), 0) = 0
BEGIN
    EXEC sp_addrolemember 'db_owner', 'NT AUTHORITY\СИСТЕМА';
END
GO