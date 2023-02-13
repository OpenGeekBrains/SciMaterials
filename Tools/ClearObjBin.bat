@echo off

echo Cleaning files in Obj and Bin directories

for /f %%f in ('dir /s /b obj') do (del /f /s /q %%f > nul)
for /f %%f in ('dir /s /b bin') do (del /f /s /q %%f > nul)

echo Remove Obj and Bin directories

for /f %%f in ('dir /s /b obj') do (rmdir /s /q %%f > nul)
for /f %%f in ('dir /s /b bin') do (rmdir /s /q %%f > nul)

echo Celaning complete