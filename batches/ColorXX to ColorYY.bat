@echo off
cd /d "%~dp0"
echo THIS BATCH IS FOR CHANGING COLOR IDs!
set /p cosID1=Enter Color ID you want to replace: 
set /p cosID2=Enter Color ID you want to move files to: 

ArcSysCostumeStringReplacer %1 %cosID2% false %cosID1% true
pause