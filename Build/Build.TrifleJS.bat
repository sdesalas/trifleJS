@ECHO OFF

:: Define variables

SET CompilePath=..\bin\Release
SET MergePath=.\Merge
SET BinaryPath=.\Binary
SET ILMerge=Tools\ILMerge.exe
SET Zip=Tools\7za.exe
SET MsBuild40=C:\Windows\Microsoft.NET\Framework\v4.0.30319\MsBuild.exe
SET WinSDK70A=C:\Program Files\Microsoft SDKs\Windows\v7.0A\bin\.
SET WinSDK71=C:\Program Files\Microsoft SDKs\Windows\v7.1\bin\.

:: Check Windows SDK

IF EXIST "%WinSDK70A%" (
	SET "WinSDK=%WinSDK70A%"
) ELSE IF EXIST "%WinSDK71%" (
	SET "WinSDK=%WinSDK71%"
) ELSE (
	ECHO Please install Windows SDK 2010.
	ECHO http://www.microsoft.com/en-us/download/details.aspx?id=8279
	EXIT /b 1
)

ECHO Using Windows SDK at %WinSDK%

:: Check for MsBuild

IF EXIST "%MsBuild40%" (
	SET "MsBuild=%MsBuild40%"
) ELSE (
	ECHO Cannot Find MsBuild!
	EXIT /b 1
)

ECHO Using MsBuild at %MsBuild%

:: Clean up last release

RMDIR /S /Q %CompilePath%
RMDIR /S /Q %MergePath%
DEL %BinaryPath%\*.* /Q /F
MKDIR %MergePath%
MKDIR %BinaryPath%

:: Run MSBuild

%MsBuild% ..\TrifleJS.csproj /t:Rebuild /p:Configuration=Release

IF EXIST "%CompilePath%\TrifleJS.exe" (
	ECHO Compilation Ok!
) ELSE (
	ECHO Compilation Fail.
	EXIT /b
)

:: Prepare & Zip Release
XCOPY %CompilePath%\*.* %MergePath% /Y /E
DEL %MergePath%\TrifleJS.exe /Q
DEL %MergePath%\Interop.IWshRuntimeLibrary.dll /Q
DEL %MergePath%\Interop.CERTENROLLLib.dll /Q
DEL %MergePath%\Interop.SHDocVw.dll /Q
DEL %MergePath%\Newtonsoft.Json.dll /Q

ECHO 
ECHO Merging and zipping to \Build\Binary directory. 
ECHO NOTE: This will take about 2 minutes..

%ILMerge% /out:%MergePath%\TrifleJS.exe %CompilePath%\TrifleJS.exe %CompilePath%\Interop.IWshRuntimeLibrary.dll %CompilePath%\Interop.CERTENROLLLib.dll %CompilePath%\Interop.SHDocVw.dll %CompilePath%\Newtonsoft.Json.dll
DEL %MergePath%\TrifleJS.pdb
CD %MergePath%
..\%Zip% a ..\Binary\TrifleJS.Latest.zip -r *.*
CD ..
RMDIR /S /Q %MergePath%

ECHO SUCCESS!!

