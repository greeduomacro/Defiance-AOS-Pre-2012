del Server.exe
del Server.pdb
C:\WINDOWS\Microsoft.NET\Framework64\v3.5\csc.exe /debug+ /unsafe /out:Server.exe /recurse:Server\*.cs /win32icon:Server\runuo.ico /optimize /warnaserror+ /d:Framework_3_5;ZLIB_WINAPI > Server.txt