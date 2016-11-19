@echo on
MSBuild\nugetv3 restore
call "C:\Program Files (x86)\Microsoft Visual Studio\2017\Enterprise\Common7\Tools\VsMSBuildCmd.bat"
call "C:\Program Files (x86)\Microsoft Visual Studio\2017\Enterprise\Common7\Tools\VsDevCmd.bat"
msbuild BuildPlugins.msbuild /p:VisualStudioVersion=15.0 /p:VsVersion=15.0 /p:VsFolder=vs17 /p:AssemblyPatcherTaskOn=true /p:EndVSQFile=AnalysisPlugin2017.VSQ /p:SkipCopy=No  > buildlog2017.txt
call "C:\Program Files (x86)\Microsoft Visual Studio 12.0\Common7\Tools\vsvars32.bat"
call "C:\Program Files (x86)\Microsoft Visual Studio 12.0\Common7\Tools\VsDevCmd.bat"
msbuild BuildPlugins.msbuild /p:VisualStudioVersion=12.0 /p:AssemblyPatcherTaskOn=true /p:EndVSQFile=AnalysisPlugin.VSQ /p:SkipCopy=No  > buildlog2013.txt
call "C:\Program Files (x86)\Microsoft Visual Studio 14.0\Common7\Tools\vsvars32.bat"
call "C:\Program Files (x86)\Microsoft Visual Studio 14.0\Common7\Tools\VsDevCmd.bat"
msbuild BuildPlugins.msbuild /p:VisualStudioVersion=14.0 /p:AssemblyPatcherTaskOn=true /p:EndVSQFile=AnalysisPlugin2015.VSQ /p:SkipCopy=No  > buildlog2015.txt
