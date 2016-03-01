# Microsoft Developer Studio Project File - Name="MfcExt" - Package Owner=<4>
# Microsoft Developer Studio Generated Build File, Format Version 6.00
# ** DO NOT EDIT **

# TARGTYPE "Win32 (x86) Static Library" 0x0104

CFG=MfcExt - Win32 Debug Static
!MESSAGE This is not a valid makefile. To build this project using NMAKE,
!MESSAGE use the Export Makefile command and run
!MESSAGE 
!MESSAGE NMAKE /f "MfcExt.mak".
!MESSAGE 
!MESSAGE You can specify a configuration when running NMAKE
!MESSAGE by defining the macro CFG on the command line. For example:
!MESSAGE 
!MESSAGE NMAKE /f "MfcExt.mak" CFG="MfcExt - Win32 Debug Static"
!MESSAGE 
!MESSAGE Possible choices for configuration are:
!MESSAGE 
!MESSAGE "MfcExt - Win32 Release" (based on "Win32 (x86) Static Library")
!MESSAGE "MfcExt - Win32 Debug" (based on "Win32 (x86) Static Library")
!MESSAGE "MfcExt - Win32 Debug Static" (based on "Win32 (x86) Static Library")
!MESSAGE "MfcExt - Win32 Release Static" (based on "Win32 (x86) Static Library")
!MESSAGE 

# Begin Project
# PROP AllowPerConfigDependencies 0
# PROP Scc_ProjName ""
# PROP Scc_LocalPath ""
CPP=cl.exe
RSC=rc.exe

!IF  "$(CFG)" == "MfcExt - Win32 Release"

# PROP BASE Use_MFC 2
# PROP BASE Use_Debug_Libraries 0
# PROP BASE Output_Dir "Release"
# PROP BASE Intermediate_Dir "Release"
# PROP BASE Target_Dir ""
# PROP Use_MFC 2
# PROP Use_Debug_Libraries 0
# PROP Output_Dir "lib"
# PROP Intermediate_Dir "Temp\Release"
# PROP Target_Dir ""
# ADD BASE CPP /nologo /MD /W3 /GX /O2 /D "WIN32" /D "NDEBUG" /D "_WINDOWS" /D "_AFXDLL" /Yu"stdafx.h" /FD /c
# ADD CPP /nologo /MD /W3 /GX /O2 /D "WIN32" /D "NDEBUG" /D "_WINDOWS" /D "_AFXDLL" /D "_MBCS" /Yu"stdafx.h" /FD /c
# ADD BASE RSC /l 0x809 /d "NDEBUG" /d "_AFXDLL"
# ADD RSC /l 0x809 /d "NDEBUG" /d "_AFXDLL"
BSC32=bscmake.exe
# ADD BASE BSC32 /nologo
# ADD BSC32 /nologo
LIB32=link.exe -lib
# ADD BASE LIB32 /nologo
# ADD LIB32 /nologo

!ELSEIF  "$(CFG)" == "MfcExt - Win32 Debug"

# PROP BASE Use_MFC 2
# PROP BASE Use_Debug_Libraries 1
# PROP BASE Output_Dir "Debug"
# PROP BASE Intermediate_Dir "Debug"
# PROP BASE Target_Dir ""
# PROP Use_MFC 2
# PROP Use_Debug_Libraries 1
# PROP Output_Dir "Debug"
# PROP Intermediate_Dir "Debug"
# PROP Target_Dir ""
# ADD BASE CPP /nologo /MDd /W3 /Gm /GX /ZI /Od /D "WIN32" /D "_DEBUG" /D "_WINDOWS" /D "_AFXDLL" /Yu"stdafx.h" /FD /GZ /c
# ADD CPP /nologo /MDd /W3 /Gm /GX /ZI /Od /D "WIN32" /D "_DEBUG" /D "_WINDOWS" /D "_MBCS" /D "_AFXDLL" /FR /Yu"stdafx.h" /FD /GZ /c
# ADD BASE RSC /l 0x809 /d "_DEBUG" /d "_AFXDLL"
# ADD RSC /l 0x809 /d "_DEBUG" /d "_AFXDLL"
BSC32=bscmake.exe
# ADD BASE BSC32 /nologo
# ADD BSC32 /nologo
LIB32=link.exe -lib
# ADD BASE LIB32 /nologo
# ADD LIB32 /nologo /out:"Lib\MfcExtD.lib"

!ELSEIF  "$(CFG)" == "MfcExt - Win32 Debug Static"

# PROP BASE Use_MFC 2
# PROP BASE Use_Debug_Libraries 1
# PROP BASE Output_Dir "MfcExt___Win32_Debug_Static"
# PROP BASE Intermediate_Dir "MfcExt___Win32_Debug_Static"
# PROP BASE Target_Dir ""
# PROP Use_MFC 1
# PROP Use_Debug_Libraries 1
# PROP Output_Dir "Lib"
# PROP Intermediate_Dir "Temp\DebugStatic"
# PROP Target_Dir ""
# ADD BASE CPP /nologo /MDd /W3 /Gm /GX /ZI /Od /D "WIN32" /D "_DEBUG" /D "_WINDOWS" /D "_AFXDLL" /D "_MBCS" /Yu"stdafx.h" /FD /GZ /c
# ADD CPP /nologo /MTd /W3 /Gm /GX /ZI /Od /D "WIN32" /D "_DEBUG" /D "_WINDOWS" /D "_MBCS" /Yu"stdafx.h" /FD /GZ /c
# ADD BASE RSC /l 0x809 /d "_DEBUG" /d "_AFXDLL"
# ADD RSC /l 0x809 /d "_DEBUG"
BSC32=bscmake.exe
# ADD BASE BSC32 /nologo
# ADD BSC32 /nologo
LIB32=link.exe -lib
# ADD BASE LIB32 /nologo /out:"Lib\MfcExtD.lib"
# ADD LIB32 /nologo /out:"Lib\MfcExtSD.lib"

!ELSEIF  "$(CFG)" == "MfcExt - Win32 Release Static"

# PROP BASE Use_MFC 2
# PROP BASE Use_Debug_Libraries 0
# PROP BASE Output_Dir "MfcExt___Win32_Release_Static"
# PROP BASE Intermediate_Dir "MfcExt___Win32_Release_Static"
# PROP BASE Target_Dir ""
# PROP Use_MFC 1
# PROP Use_Debug_Libraries 0
# PROP Output_Dir "Lib"
# PROP Intermediate_Dir "Temp\ReleaseStatic"
# PROP Target_Dir ""
# ADD BASE CPP /nologo /MD /W3 /GX /O2 /D "WIN32" /D "NDEBUG" /D "_WINDOWS" /D "_AFXDLL" /D "_MBCS" /Yu"stdafx.h" /FD /c
# ADD CPP /nologo /MT /W3 /GX /O2 /D "WIN32" /D "NDEBUG" /D "_WINDOWS" /D "_MBCS" /Yu"stdafx.h" /FD /c
# ADD BASE RSC /l 0x809 /d "NDEBUG" /d "_AFXDLL"
# ADD RSC /l 0x809 /d "NDEBUG"
BSC32=bscmake.exe
# ADD BASE BSC32 /nologo
# ADD BSC32 /nologo
LIB32=link.exe -lib
# ADD BASE LIB32 /nologo /out:"Lib\MfcExt.lib"
# ADD LIB32 /nologo /out:".\Lib\MfcExtS.lib"

!ENDIF 

# Begin Target

# Name "MfcExt - Win32 Release"
# Name "MfcExt - Win32 Debug"
# Name "MfcExt - Win32 Debug Static"
# Name "MfcExt - Win32 Release Static"
# Begin Group "Source Files"

# PROP Default_Filter "cpp;c;cxx;rc;def;r;odl;idl;hpj;bat"
# Begin Source File

SOURCE=.\src\ColoredDialog.cpp
# End Source File
# Begin Source File

SOURCE=.\src\ColorStaticST.cpp
# End Source File
# Begin Source File

SOURCE=.\src\CpuSpeed.cpp
# End Source File
# Begin Source File

SOURCE=.\src\CpuType.cpp
# End Source File
# Begin Source File

SOURCE=.\src\CsvSetup.cpp
# End Source File
# Begin Source File

SOURCE=.\src\DialogMenu.cpp
# End Source File
# Begin Source File

SOURCE=.\src\DialogScroll.cpp
# End Source File
# Begin Source File

SOURCE=.\src\DialogSK.cpp
# End Source File
# Begin Source File

SOURCE=.\src\DlgUnits.cpp
# End Source File
# Begin Source File

SOURCE=.\src\DosDate.cpp
# End Source File
# Begin Source File

SOURCE=.\src\filesys.cpp
# End Source File
# Begin Source File

SOURCE=.\src\ImageListEx.cpp
# End Source File
# Begin Source File

SOURCE=.\src\IniFile.cpp
# End Source File
# Begin Source File

SOURCE=.\src\InPlaceCtrls.cpp
# End Source File
# Begin Source File

SOURCE=.\src\IPAddress.cpp
# End Source File
# Begin Source File

SOURCE=.\src\listboxex.cpp
# End Source File
# Begin Source File

SOURCE=.\src\ListCtrlEx.cpp
# End Source File
# Begin Source File

SOURCE=.\src\MapEx.cpp
# End Source File
# Begin Source File

SOURCE=.\src\MapiMail.cpp
# End Source File
# Begin Source File

SOURCE=.\src\OsInfo.cpp
# End Source File
# Begin Source File

SOURCE=.\src\Registry.cpp
# End Source File
# Begin Source File

SOURCE=.\src\SECREG.CPP
# End Source File
# Begin Source File

SOURCE=.\StdAfx.cpp
# ADD CPP /Yc"stdafx.h"
# End Source File
# Begin Source File

SOURCE=.\src\Table.cpp
# End Source File
# Begin Source File

SOURCE=.\src\Task.cpp
# End Source File
# Begin Source File

SOURCE=.\src\treectrl.cpp
# End Source File
# Begin Source File

SOURCE=.\src\Utils.cpp
# End Source File
# Begin Source File

SOURCE=.\src\XMessageBox.cpp
# End Source File
# End Group
# Begin Group "Header Files"

# PROP Default_Filter "h;hpp;hxx;hm;inl"
# Begin Source File

SOURCE=.\Include\ARRAYS.H
# End Source File
# Begin Source File

SOURCE=.\Include\ColoredDialog.h
# End Source File
# Begin Source File

SOURCE=.\Include\ColorStaticST.h
# End Source File
# Begin Source File

SOURCE=.\Include\CpuSpeed.h
# End Source File
# Begin Source File

SOURCE=.\Include\CpuType.h
# End Source File
# Begin Source File

SOURCE=.\Include\CsvSetup.h
# End Source File
# Begin Source File

SOURCE=.\Include\DialogMenu.h
# End Source File
# Begin Source File

SOURCE=.\Include\DialogScroll.h
# End Source File
# Begin Source File

SOURCE=.\Include\DialogSK.h
# End Source File
# Begin Source File

SOURCE=.\Include\DlgUnits.h
# End Source File
# Begin Source File

SOURCE=.\Include\filesys.h
# End Source File
# Begin Source File

SOURCE=.\Include\IniFile.h
# End Source File
# Begin Source File

SOURCE=.\Include\ListCtrlEx.h
# End Source File
# Begin Source File

SOURCE=.\Include\MapEx.h
# End Source File
# Begin Source File

SOURCE=.\Include\MfcExt.h
# End Source File
# Begin Source File

SOURCE=.\Include\Registry.h
# End Source File
# Begin Source File

SOURCE=.\Include\SECREG.H
# End Source File
# Begin Source File

SOURCE=.\StdAfx.h
# End Source File
# Begin Source File

SOURCE=.\Include\table.h
# End Source File
# Begin Source File

SOURCE=.\Include\task.h
# End Source File
# Begin Source File

SOURCE=.\Include\utils.h
# End Source File
# Begin Source File

SOURCE=.\Include\XMessageBox.h
# End Source File
# End Group
# Begin Source File

SOURCE=.\Readme.txt
# End Source File
# End Target
# End Project
