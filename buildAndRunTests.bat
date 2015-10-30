SETLOCAL
SET MSBUILD="%ProgramFiles(x86)%\MSBuild\14.0\Bin\MSBuild.exe"
SET NUGET="%ProgramFiles(x86)%\MSBuild\14.0\Bin\Nuget.exe"
SET MSTEST="%ProgramFiles(x86)%\Microsoft Visual Studio 14.0\Common7\IDE\MSTest.exe"
REM Dump the environment variables.
SET

CD bigquery\api\BigQueryUtil
%NUGET% restore
%MSBUILD% 
%MSTEST% /testcontainer:test\bin\debug\test.dll

CD ..\GettingStarted
%NUGET% restore
%MSBUILD% 
%MSTEST% /testcontainer:test\bin\debug\test.dll

CD ..\..\..\storage\api
%NUGET% restore
%MSBUILD% 
%MSTEST% /testcontainer:test\bin\debug\test.dll
ENDLOCAL