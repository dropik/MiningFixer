cd /D "%~dp0"
sc create MiningFixerService binPath= "%cd%\MiningFixer.exe" start= auto DisplayName= "Mining Fixer Service"
sc start MiningFixerService
pause