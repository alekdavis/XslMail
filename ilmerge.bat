@echo off
cd %~dp0
set InputDir=bin\Release
set OutputDir=Tools\Merged
if not exist %OutputDir% mkdir %OutputDir%
echo Merging assemblies...
"packages\ILMerge.3.0.41\tools\net452\ilmerge.exe" "%InputDir%\XslMail.exe" /out:"%OutputDir%\XslMail.exe" "%InputDir%\AngleSharp.dll" "%InputDir%\C5.dll" "%InputDir%\CsQuery.dll" "%InputDir%\Newtonsoft.Json.dll" "%InputDir%\Plossum CommandLine.dll" "%InputDir%\PreMailer.Net.dll" "%InputDir%\System.Buffers.dll" "%InputDir%\System.Memory.dll" "%InputDir%\System.Numerics.Vectors.dll" "%InputDir%\System.Runtime.CompilerServices.Unsafe.dll" "%InputDir%\System.Text.Encoding.CodePages.dll" "%InputDir%\TidyHTML5Managed.dll"
echo Copying files...
copy "%InputDir%\tidy.x*.dll" "%OutputDir%" /Y
copy "%InputDir%\*.exe.config" "%OutputDir%" /Y
echo Done.