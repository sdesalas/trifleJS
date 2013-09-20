
@ECHO OFF

:: Define variables

SET OutPath=.\Output
SET CompilePath=..\bin\Release
SET ILMerge=Tools\ILMerge.exe
SET 7za=Tools\7za.exe
SET MsBuild35=C:\Windows\Microsoft.NET\Framework\v3.5\MsBuild.exe

:: Clean up last release

RMDIR /S /Y %CompilePath%
RMDIR /S /Y %OutPath%
MKDIR %OutPath%

:: Run MSBuild

IF EXIST "%MsBuild35%" (
	SET "MsBuild=%MsBuild35%"
) ELSE (
	ECHO Cannot Find MsBuild!
	EXIT /b 1
)

ECHO Using MsBuild at %MsBuild%

%MsBuild% ..\TrifleJS.csproj /t:Rebuild /p:Configuration=Release


IF EXIST "%CompilePath%\TrifleJS.exe" (
	ECHO Compilation Ok!
	ECHO Merging to \Build\Output directory. This will take about 2 minutes..
) ELSE (
	ECHO Compilation Fail.
	EXIT /b 1	
)

XCOPY %CompilePath%\*.* %OutPath%
DEL %OutPath%\TrifleJS.exe
DEL %OutPath%\Microsoft.mshtml.dll
%ILMerge% /out:%OutPath%\TrifleJS.exe %CompilePath%\TrifleJS.exe %CompilePath%\Microsoft.mshtml.dll
DEL %OutPath%\TrifleJS.pdb


ECHO SUCCESS!!

