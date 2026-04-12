@echo off
cd /d "%~dp0"
echo THIS BATCH IS FOR CHANGING COSTUME IDs!
set /p cosID1=Enter Costume ID you want to replace: 
set /p cosID2=Enter Costume ID you want to move files to: 
ArcSysCostumeStringReplacer %1 %cosID2% false %cosID1%