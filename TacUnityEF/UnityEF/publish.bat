@echo off
setlocal enabledelayedexpansion

:: ===== НАСТРОЙКИ =====
set PROJECT_NAME=UnityEF
set PROJECT_FILE=%PROJECT_NAME%.csproj
set PACKAGE_ROOT=P:\_PROGS\_2026\TacBattleAI\Install\TacLibrary\_UnityEF\com.tac.unityef\
set RUNTIME_DIR=%PACKAGE_ROOT%\Runtime

:: Целевая платформа
set TARGET_FRAMEWORK=netstandard2.1

:: ===== ШАГ 1: Очистка RUNTIME_DIR =====
echo Cleaning %RUNTIME_DIR%...
if exist "%RUNTIME_DIR%" (
    rmdir /s /q "%RUNTIME_DIR%" 2>nul
)
mkdir "%RUNTIME_DIR%"

:: ===== ШАГ 2: Публикация прямо в RUNTIME_DIR =====
echo Publishing %PROJECT_NAME% for %TARGET_FRAMEWORK%...
dotnet publish "%PROJECT_FILE%" -c Release -f %TARGET_FRAMEWORK% -o "%RUNTIME_DIR%" --no-self-contained

if %errorlevel% neq 0 (
    echo Publish failed!
    pause
    exit /b %errorlevel%
)


:: ===== ШАГ 3: Удаление лишних и конфликтующих файлов =====
echo Cleaning up extra files...
del "%RUNTIME_DIR%\Microsoft.CSharp.dll" 2>nul 

echo ==========================================
echo Package successfully built at:
echo %PACKAGE_ROOT%
echo ==========================================
pause
