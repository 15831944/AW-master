:: Clear out the old files from the ToSign folder
delete .\ToSign\*.*

:: Copy Files to be signed
copy ..\AuditWizard\Output\Layton.*.dll .\ToSign
copy ..\AuditWizard\Output\AuditWizardv8.XmlSerializer.dll .\ToSign
copy ..\AuditWizard\Output\AWSNMP.dll .\ToSign
copy ..\AuditWizard\Output\Audit*.exe .\ToSign
copy "..\Static Files\Audit*.exe" .\ToSign
copy "..\AuditScanner\Scanners\Output\*.exe" .\ToSign

:: Sign Everything
signtool sign /f layton.pfx /d "AuditWizard Overview" /p lotus207 /t http://timestamp.comodoca.com/authenticode .\ToSign\*.*

:: Copy them all back to the signed folder
copy .\ToSign\*.* ..\AuditWizard\Output\signed

signtool sign /f layton_comodo.pfx /d "AuditWizard Overview" /p LayTech123! /t http://timestamp.comodoca.com/authenticode .\ToSign\*.*