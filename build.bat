@echo on
MSBuild\nugetv3 restore
call "C:\Program Files (x86)\Microsoft Visual Studio 12.0\Common7\Tools\vsvars32.bat"
call "C:\Program Files (x86)\Microsoft Visual Studio 12.0\Common7\Tools\VsDevCmd.bat"
msbuild BuildPlugins.msbuild /p:VisualStudioVersion=12.0 /p:AssemblyPatcherTaskOn=true /p:EndVSQFile=AnalysisPlugin.VSQ /p:SkipCopy=No  > buildlog2013lite.txt
call "C:\Program Files (x86)\Microsoft Visual Studio 14.0\Common7\Tools\vsvars32.bat"
call "C:\Program Files (x86)\Microsoft Visual Studio 14.0\Common7\Tools\VsDevCmd.bat"
msbuild BuildPlugins.msbuild /p:VisualStudioVersion=14.0 /p:AssemblyPatcherTaskOn=true /p:EndVSQFile=AnalysisPlugin2015.VSQ /p:SkipCopy=No  > buildlog2015lite.txt
