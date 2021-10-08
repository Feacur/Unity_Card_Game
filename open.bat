@echo off
chcp 65001 > nul
setlocal

rem [any]
set project=%1
if [%project%] == [] ( set "project=UCG" )

rem |> PREPARE TOOLS
call "%project%_Project/environment.bat" || ( goto :eof )

rem |> DO
call Unity -version
start Unity -projectPath "%project%"

rem |> FUNCTIONS
goto :eof
