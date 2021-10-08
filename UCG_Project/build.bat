@echo off
chcp 65001 > nul
setlocal enabledelayedexpansion

rem [any]
set project=%1
if [%project%] == [] ( set "project=UCG" )

rem [Current|Windows64]
set target=%1
if [%target%] == [] ( set "target=Current" )

rem Optimized|Development|Debug
set configuration=%2
if [%configuration%] == [] ( set "configuration=Optimized" )

rem |> PREPARE PROJECT
set project_folder=%cd%
set build_folder=%project%_Build

rem |> PREPARE TOOLS
call "environment.bat" || ( goto :eof )
call taskkill -fi "IMAGENAME eq Unity.exe" > nul

rem |> OPTIONS
set build_options=-batchmode -quit
set build_options=%build_options% -projectPath "%project%"
set build_options=%build_options% -executeMethod Builder.Build_Player_%configuration%
set build_options=%build_options% -logFile "%build_folder%/%target%_%configuration%.log"

if %target% == Windows64 (
	set build_options=%build_options% -buildTarget Win64
)

rem |> BUILD

pushd ..

call Unity -version
call Unity %build_options%

popd

rem |> FUNCTIONS
goto :eof
