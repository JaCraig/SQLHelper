@echo off
setlocal

if not exist ".config\dotnet-tools.json" (
	dotnet new tool-manifest || goto :error
)

call :EnsureLocalTool Husky
call :EnsureLocalTool Versionize

dotnet tool update -g docfx >nul 2>&1 || dotnet tool install -g docfx || goto :error

if not exist "docfx_project\docfx.json" (
	pushd docfx_project
	docfx init --quiet || goto :error
	popd
)

dotnet tool run husky install || goto :error
echo Setup completed successfully.
exit /b 0

:EnsureLocalTool
dotnet tool update %~1 >nul 2>&1 || dotnet tool install %~1 || goto :error
goto :eof

:error
echo Setup failed.
exit /b 1