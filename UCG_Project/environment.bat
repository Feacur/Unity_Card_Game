@echo off
chcp 65001 > nul

rem enable ANSI escape codes for CMD: set `HKEY_CURRENT_USER\Console\VirtualTerminalLevel` to `0x00000001`
rem enable UTF-8 by default for CMD: set `HKEY_LOCAL_MACHINE\Software\Microsoft\Command Processor\Autorun` to `chcp 65001 > nul`

rem @note: `if errorlevel n` means `if %errorlevel% geq n`
rem        you can chain those with `&&` when true and `||` when false

if not [%environment_is_ready%] == [] (
	goto :eof
)

rem |> Unity
set "PATH=%PATH%;C:/Program Files/Unity/2020.3.19f1/Editor/"
call :check_unity || (
	echo.can't find Clang's compiler/linker
	goto :eof
)

set environment_is_ready=true

rem |> FUNCTIONS
goto :eof

:check_unity
	where -q "Unity.exe"
	rem return: `errorlevel`
goto :eof
