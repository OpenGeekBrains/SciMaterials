@echo off

echo dotnet clean...

dotnet clean

echo dotnet clean completed

call ClearObjBin.bat

echo dotnet build...

dotnet build

echo dotnet build completed

pause