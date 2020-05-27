echo @ON

cd ..\bin\Debug\netcoreapp3.1
dir

sc.exe create ApiCall binpath="%cd%\MonitorService.exe"

pause