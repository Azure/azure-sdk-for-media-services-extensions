@echo off
echo Copying Media SDK signed managed DLLs and the pdbs to the final drop location...
echo Creating \drop\WAMSSDKEXT\lib\net45
md .\drop\WAMSSDKEXT\lib\net45
echo Copy MediaServices.Client.Extensions.dll
copy /y "\\mediadist\release\SDK\Client SDK Extensions\dlls\latest signed\Microsoft.WindowsAzure.MediaServices.Client.Extensions.dll" .\drop\WAMSSDKEXT\lib\net45\
echo Copy MediaServices.Client.pdb
copy /y .\Publish\Build\Release\Microsoft.WindowsAzure.MediaServices.Client.Extensions.pdb .\drop\WAMSSDKEXT\lib\net45\
echo Copy Nuget spec
copy /y .\.nuget\windowsazure.mediaservices.extensions.nuspec .\drop\WAMSSDKEXT\
if %ERRORLEVEL% neq 0 goto copyfailed
echo OK

echo Creating Media SDK NuGet Packages....
nuget.exe pack .\drop\WAMSSDKEXT\windowsazure.mediaservices.extensions.nuspec -o .\drop -Symbols
if %ERRORLEVEL% neq 0 goto packagingfailed
echo OK

echo Copying files to the packages share...
copy .\drop\*.* c:\packages
if %ERRORLEVEL% neq 0 goto copyfailed
echo OK

echo Removing all signed files from \\adksdksign\unsigned...
del /q c:\signing\signed\*.*
echo OK

echo Removing all unsigned files from \\adksdksign\unsigned...
del /q c:\signing\tosign\*.*
echo OK

echo SUCCESS. Signed files are available at \\adxsdksign\packages

exit /b 0

:packagingfailed

echo FAILED. Unable to create NuGet packages
exit /b -1

:copyfailed

echo FAILED. Unable to copy native DLLs
exit /b -1

:publishfailed

echo FAILED. Unable to publish to myget
exit /b -1

:signfailed
del /q c:\signing\tosign\*.*
echo FAILED. Signing tool failed.
exit /b -1