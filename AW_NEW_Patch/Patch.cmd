if "%1"=="" %0 Debug Release Done 

@SETLOCAL 
@set path=%path%;"C:\Program Files\Microsoft Platform SDK\Samples\SysMgmt\Msi\Patching"
@set PatchTmp=C:\VSTemp

:loop 
if "%1"=="Done" goto end 

if not exist %1\*.msi goto nopatch 
if not exist v840\*.msi goto nopatch 
if not exist v841\*.msi goto nopatch
if not exist v844\*.msi goto nopatch

:ok 
rmdir /s /q %PatchTmp% 
mkdir %PatchTmp% 
mkdir %PatchTmp%\v840
mkdir %PatchTmp%\v841
mkdir %PatchTmp%\v844
mkdir %PatchTmp%\UpgradedImage 
mkdir %PatchTmp%\Patch 

for %%a in (v840\*.msi) do copy %%a %PatchTmp%\setup.msi 
msiexec /qb /a %PatchTmp%\setup.msi TARGETDIR=%PatchTmp%\v840 /L*v %PatchTmp%\v840\setup.log 
del %PatchTmp%\setup.msi 

for %%a in (v841\*.msi) do copy %%a %PatchTmp%\setup.msi 
msiexec /qb /a %PatchTmp%\setup.msi TARGETDIR=%PatchTmp%\v841 /L*v %PatchTmp%\v841\setup.log 
del %PatchTmp%\setup.msi 

for %%a in (v844\*.msi) do copy %%a %PatchTmp%\setup.msi 
msiexec /qb /a %PatchTmp%\setup.msi TARGETDIR=%PatchTmp%\v844 /L*v %PatchTmp%\v844\setup.log 
del %PatchTmp%\setup.msi 

for %%a in (%1\*.msi) do copy %%a %PatchTmp%\setup.msi 
msiexec /qb /a %PatchTmp%\setup.msi TARGETDIR=%PatchTmp%\UpgradedImage /L*v %PatchTmp%\UpgradedImage\setup.log 
del %PatchTmp%\setup.msi 

copy patch.pcp %PatchTmp% 
set PatchDir=%CD% 
chdir %PatchTmp% 
msimsp -s patch.pcp -p Patch\patch.msp -l Patch\patch.log -f %PatchTmp%\Tmp -d 
 
rmdir /s /q %PatchTmp%\v840
rmdir /s /q %PatchTmp%\v841
rmdir /s /q %PatchTmp%\v844
rmdir /s /q %PatchTmp%\UpgradedImage 
rmdir /s /q %PatchTmp%\Tmp 
chdir %PatchDir% 

mkdir Patch 
mkdir Patch\%1 
copy %PatchTmp%\Patch\*.* Patch\%1\*.* 
rmdir /s /q %PatchTmp% 

:nopatch 
shift 
goto loop 

:end 
pause