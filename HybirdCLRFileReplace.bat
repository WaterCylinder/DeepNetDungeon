@echo off
setlocal enabledelayedexpansion

:: ========== 用户配置区域 ==========
set "sourceFolder=%~dp0HybridCLRData\HotUpdateDlls\StandaloneWindows64"
set "targetFolder=%~dp0Assets\StreamingAssets"

:: 需要替换的文件列表（用空格分隔）
set "filesToReplace=HotUpdate.dll EntityBehaviors.dll Effects.dll"
:: ================================

echo.
echo ██ Start replace
echo ██ source: %sourceFolder%
echo ██ target: %targetFolder%
echo.

for %%F in (%filesToReplace%) do (
    if exist "%sourceFolder%\%%F" (
        set "targetFile=%%F.bytes"
        if exist "%targetFolder%\!targetFile!" (
            echo Y replacing: %%F  !targetFile!
            copy /Y "%sourceFolder%\%%F" "%targetFolder%\!targetFile!" >nul
        ) else (
            echo + adding new: %%F  !targetFile!
            copy /Y "%sourceFolder%\%%F" "%targetFolder%\!targetFile!" >nul
        )
    ) else (
        echo X source not found: %%F
    )
)

echo.
echo ██ done