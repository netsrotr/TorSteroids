@echo off
set local
set PATH2SELF=%~d0%~p0

cd "%PATH2SELF%"

IF /i "%1" == "AnyCPU" (
  xcopy /R /E /Y /C "x64\*.*" *.*
) else (
  xcopy /R /E /Y /C "x86\*.*" *.*
)
