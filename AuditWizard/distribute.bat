
: This script is used by AuditWizard v8
: Its purpose is to automate the copying of built executables from the Development PC
: to the distribution sources folder on DEVSERV (mapped as Z$)
:
: NOTE: This script does not copy everythijg - its main task is to copy those files which are built
: as part ofthe project.  Files which do not change can remain in the distribution sources folder
:
SET COMMONSRC=D:\DEVELOPMENT SOURCES\
SET SRC=D:\DEVELOPMENT SOURCES\AUDITWIZARD V8
SET DST=d:\DISTRIBUTION SOURCES\AuDITWIZARD V8
SET INSTALLDST=D:\Installations Area\AuditWizard v8\AuditWizard
SET DATABRIDGEDST=D:\DISTRIBUTION SOURCES\DataBridge

: First copy the AuditAgent
COPY "%SRC%\Output\AuditAgent.exe" "%DST%\AuditAgent"

: Now copy the AuditWizardService
COPY "%SRC%\Output\AuditWizardService.exe" "%DST%\AuditWizardService"


: Copy the main program DLLs
COPY "%SRC%\Output\Layton.AuditWizard.Administration.dll" "%DST%\Program"
COPY "%SRC%\Output\Layton.AuditWizard.Administration.dll.config" "%DST%\Program"
:
COPY "%SRC%\Output\Layton.AuditWizard.Applications.dll" "%DST%\Program"
COPY "%SRC%\Output\Layton.AuditWizard.Applications.dll.config" "%DST%\Program"
:
COPY "%SRC%\Output\Layton.AuditWizard.AuditTrail.dll" "%DST%\Program"
COPY "%SRC%\Output\Layton.AuditWizard.AuditTrail.dll.config" "%DST%\Program"
:
COPY "%SRC%\Output\Layton.AuditWizard.Common.dll" "%DST%\Program"
COPY "%SRC%\Output\Layton.AuditWizard.DataAccess.dll" "%DST%\Program"
:
COPY "%SRC%\Output\Layton.AuditWizard.Network.dll" "%DST%\Program"
COPY "%SRC%\Output\Layton.AuditWizard.Network.dll.config" "%DST%\Program"
:
COPY "%SRC%\Output\Layton.AuditWizard.Overview.dll" "%DST%\Program"
COPY "%SRC%\Output\Layton.AuditWizard.Overview.dll.config" "%DST%\Program"
:
COPY "%SRC%\Output\Layton.AuditWizard.Reports.dll" "%DST%\Program"
COPY "%SRC%\Output\Layton.AuditWizard.Reports.dll.config" "%DST%\Program"
:
COPY "%SRC%\Output\Layton.AuditWizard.Reports.dll" "%DST%\Program"
COPY "%SRC%\Output\Layton.AuditWizard.Reports.dll.config" "%DST%\Program"


: Copy Common DLLs
COPY "%COMMONSRC%\Output\Layton.Common.Controls.dll" "%DST%\Program"


: Copy the Scanner
COPY "%SRC%\Output\AuditScanner.exe" "%DST%\Program"


: Copy the Layton Framework 
COPY "%SRC%\Layton Framework\Assemblies\Layton.Cab.Interface.dll" "%DST%\Framework"
COPY "%SRC%\Layton Framework\Assemblies\Layton.Cab.Shell.exe" "%DST%\Framework"
COPY "%SRC%\Layton Framework\Assemblies\Layton.Cab.Shell.exe.config" "%DST%\Framework"
COPY "%SRC%\Layton Framework\Assemblies\Layton.Cab.Shell.ico" "%DST%\Framework"
:
COPY "%SRC%\Layton Framework\Assemblies\Layton.HtmlReports.dll" "%DST%\Framework"
COPY "%SRC%\Layton Framework\Assemblies\Layton.NetworkDiscovery.dll" "%DST%\Framework"
COPY "%SRC%\Layton Framework\Assemblies\Layton.NetworkDiscovery.dll.config" "%DST%\Framework"
:
COPY "%SRC%\Layton Framework\Assemblies\ProfileCatalog.xml" "%DST%\Framework"

: Copy Framework Redistributables
COPY "%SRC%\Layton Framework\Assemblies\IPAddressControlLib.dll" "%DST%\Framework"
COPY "%SRC%\Layton Framework\Assemblies\Infragistics.Practices.CompositeUI.WinForms.dll" "%DST%\Framework"
COPY "%SRC%\Layton Framework\Assemblies\Microsoft.Practices.CompositeUI.dll" "%DST%\Framework"
COPY "%SRC%\Layton Framework\Assemblies\Microsoft.Practices.CompositeUI.WinForms.dll" "%DST%\Framework"
COPY "%SRC%\Layton Framework\Assemblies\Microsoft.Practices.ObjectBuilder.dll" "%DST%\Framework"
COPY "%SRC%\Layton Framework\Assemblies\Pahvant.MSI.Interop.dll" "%DST%\Framework"
COPY "%SRC%\Layton Framework\Assemblies\PickerSample.dll" "%DST%\Framework"
COPY "%SRC%\Layton Framework\Assemblies\PickerSample.xml" "%DST%\Framework"

: Redistributable files
COPY "%SRC%\Output\ChilkatDotNet2.dll" "%DST%\Redistributables"
COPY "%SRC%\Output\WizardBase.dll" "%DST%\Redistributables"

: Copy Installation Scripts
COPY "%SRC%\Installation Scripts\CreateDatabase.sql" "%INSTALLDST%\Script Files"
COPY "%SRC%\Installation Scripts\CreateStoredProcedures.sql" "%INSTALLDST%\Script Files"
COPY "%SRC%\Installation Scripts\CreateForeignKeys.sql" "%INSTALLDST%\Script Files"
COPY "%SRC%\Installation Scripts\AddInitialData.sql" "%INSTALLDST%\Script Files"
COPY "%SRC%\Installation Scripts\CheckSQL.vbs" "%INSTALLDST%\Script Files"

: Copy Documentation Files
COPY "%SRC%\Documentation\AuditWizard.chm" "%DST%\Documentation"
COPY "%SRC%\Documentation\ReleaseNotes.pdf" "%DST%\Documentation"
COPY "%SRC%\Documentation\UserGuide.pdf" "%DST%\Documentation"
COPY "%SRC%\Documentation\DataBridgeInstallation.pdf" "%DST%\Documentation"

: Copy Runtime icons
COPY "%SRC%\Runtime Icons\Large\*" "%DST%\Icons\Large"
COPY "%SRC%\Runtime Icons\Medium\*" "%DST%\Icons\Medium"
COPY "%SRC%\Runtime Icons\Small\*" "%DST%\Icons\Small"

: Copy USB and PDA Scanner
COPY "%SRC%\USBScan\LyncUSB.exe" "%DST%\Program\USBScan"
COPY "%SRC%\USBScan\uninstall LyncUSB.bat" "%DST%\Program\USBScan"
:
COPY "%SRC%\PDAScan\Condinst.dll" "%DST%\Program\PDAScan"
COPY "%SRC%\PDAScan\CondMgr.dll" "%DST%\Program\PDAScan"
COPY "%SRC%\PDAScan\HSAPI.dll" "%DST%\Program\PDAScan"
COPY "%SRC%\PDAScan\LyncAL.exe" "%DST%\Program\PDAScan"
COPY "%SRC%\PDAScan\LyncAS.exe" "%DST%\Program\PDAScan"
COPY "%SRC%\PDAScan\LyncBB.exe" "%DST%\Program\PDAScan"
COPY "%SRC%\PDAScan\LyncBBDLL.dll" "%DST%\Program\PDAScan"
COPY "%SRC%\PDAScan\LyncHS.dll" "%DST%\Program\PDAScan"
COPY "%SRC%\PDAScan\LyncInst.dll" "%DST%\Program\PDAScan"
COPY "%SRC%\PDAScan\LyncPalmApp.prc" "%DST%\Program\PDAScan"
COPY "%SRC%\PDAScan\LyncPN.dll" "%DST%\Program\PDAScan"
COPY "%SRC%\PDAScan\LyncPPC.dll" "%DST%\Program\PDAScan"
COPY "%SRC%\PDAScan\LyncReg.exe" "%DST%\Program\PDAScan"
COPY "%SRC%\PDAScan\LyncSP.exe" "%DST%\Program\PDAScan"
COPY "%SRC%\PDAScan\LyncSym.exe" "%DST%\Program\PDAScan"
COPY "%SRC%\PDAScan\SCRuntimeSetup22.exe" "%DST%\Program\PDAScan"
COPY "%SRC%\PDAScan\unregister.bat" "%DST%\Program\PDAScan"

: Canned Reports
COPY "%SRC%\CannedReports\*" "%DST%\Reports"

: Scanner Definitions
COPY "%SRC%\Scanners\*" "%DST%\Scanners"

: Databridge
COPY "%SRC%\Output\AuditWizardDataBridge.dll" "%DATABRIDGEDST%"
COPY "%SRC%\Output\AuditWizardDataBridge.tlb" "%DATABRIDGEDST%"
COPY "%SRC%\Output\Layton.AuditWizard.DataAccess.dll" "%DATABRIDGEDST%"
COPY "%SRC%\Documentation\DataBridgeInstallation.pdf" "%DATABRIDGEDST%"

: HelpBox Updates
COPY "%SRC%\HelpBox v5 scripts\*" "%DATABRIDGEDST%\HelpBox v5 Scripts"