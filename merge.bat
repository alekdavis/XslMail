@echo off
setlocal enabledelayedexpansion

set Config=%1
if "%Config%"=="" set Config=Release

set OutputDir=DIST
set BuildDir=bin\%Config%

echo Checking for ILRepack...
where ILRepack.exe >nul 2>&1
if errorlevel 1 (
    echo ERROR: ILRepack not found. Run: dotnet tool install -g dotnet-ilrepack
    cd ..\..
    exit /b 1
)

if exist %OutputDir% rd /s /q %OutputDir%
mkdir %OutputDir%

cd %BuildDir%
if errorlevel 1 (
    echo ERROR: Build directory not found: %BuildDir%
    cd ..\..
    exit /b 1
)

echo Merging assemblies...
ILRepack.exe /out:"..\..\%OutputDir%\XslMail.exe" "XslMail.exe" "AngleSharp.dll" "C5.dll" "CsQuery.dll" "Newtonsoft.Json.dll" "Plossum CommandLine.dll" "PreMailer.Net.dll" "System.Buffers.dll" "System.Memory.dll" "System.Numerics.Vectors.dll" "System.Runtime.CompilerServices.Unsafe.dll" "System.Text.Encoding.CodePages.dll" "TidyHTML5Managed.dll"
if errorlevel 1 (
    echo ERROR: Assembly merge failed
    cd ..\..
    exit /b 1
)

echo Copying files to ..\..\%OutputDir%
copy "tidy.*.dll" "..\..\%OutputDir%" /Y
if errorlevel 1 (
    echo ERROR: Cannot copy tidy DLLs
    cd ..\..
    exit /b 1
)
copy "*.exe.config" "..\..\%OutputDir%" /Y
if errorlevel 1 (
    echo ERROR: Cannot copy config file
    cd ..\..
    exit /b 1
)

cd ..\..

echo Creating ZIP archive
set ZipFile=%OutputDir%\XslMail.zip
if exist %ZipFile% del /q %ZipFile%
powershell -NoProfile -Command "Compress-Archive -Path '%OutputDir%\*' -DestinationPath '%ZipFile%' -CompressionLevel Optimal"
if errorlevel 1 (
    echo WARNING: Failed to create ZIP archive
) else (
    echo ZIP archive created in %ZipFile%
)

echo Build completed successfully in %OutputDir%