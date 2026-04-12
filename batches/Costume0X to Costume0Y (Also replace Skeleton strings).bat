@echo off
cd /d "%~dp0"
set /p cosID1=Enter Costume ID you want to replace: 
set /p cosID2=Enter Costume ID you want to move files to: 
ArcSysCostumeStringReplacer %1 %cosID2% true %cosID1%
pause