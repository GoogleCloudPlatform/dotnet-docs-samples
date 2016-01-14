SETLOCAL
SET MSBUILD="%ProgramFiles(x86)%\MSBuild\14.0\Bin\MSBuild.exe"
SET NUGET="%ProgramFiles(x86)%\MSBuild\14.0\Bin\Nuget.exe"
SET MSTEST="%ProgramFiles(x86)%\Microsoft Visual Studio 14.0\Common7\IDE\MSTest.exe"
SET FAILED=0

CD bigquery\api\BigQueryUtil
%NUGET% restore
%MSBUILD% && %MSTEST% /testcontainer:test\bin\debug\test.dll || SET FAILED=1

CD ..\GettingStarted
%NUGET% restore
%MSBUILD% && %MSTEST% /testcontainer:test\bin\debug\test.dll || SET FAILED=1

CD ..\..\..\storage\api
%NUGET% restore
%MSBUILD% && %MSTEST% /testcontainer:test\bin\debug\test.dll || SET FAILED=1

CD ..\..\auth
%NUGET% restore
%MSBUILD% && %MSTEST% /testcontainer:test\bin\debug\test.dll || SET FAILED=1

@ECHO OFF
IF %FAILED% NEQ 0 GOTO failed_case
ENDLOCAL
ECHO SUCCEEDED
EXIT /b

:failed_case
ENDLOCAL
ECHO FAILED
EXIT /b 1
