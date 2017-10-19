@echo off
setlocal enabledelayedexpansion

if /i "%1" == "--help" call :usage & exit /b 0
if /i "%1" == "/?"     call :usage & exit /b 0

:find_msbuild
set msbuildExe=MSBuild.exe
for %%i in (%msbuildExe%) do if not "%%~$PATH:i"=="" (
	set msbuild=%%~$PATH:i
)
if defined msbuild goto :configure

set msbuildFolder=%SystemRoot%\Microsoft.NET\Framework\v3.5
if not exist "%msbuildFolder%" (
	echo ERROR: .NET Framework v3.5 not found at "%msbuildFolder%".
	exit /b 1
)
if not exist "%msbuildFolder%\%msbuildExe%" (
	echo ERROR: %msbuildExe% not found in "%msbuildFolder%".
	exit /b 1
)
set msbuild=%msbuildFolder%\%msbuildExe%

:configure
set solution=Mechanisms.sln
set configuration=Release

if /i "%1" == "debug" set configuration=Debug
if /i "%1" == "release" set configuration=Release

:build
echo MSBUILD: !msbuild!
"%msbuild%" %solution% /t:Build /p:Configuration=%configuration% /nologo /verbosity:minimal
if %errorlevel% neq 0 exit /b %errorlevel%

:success
exit /b 0

:usage
echo.
echo Usage: %~n0 [debug ^| release]
goto :EOF
