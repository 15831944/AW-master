copy ..\AuditWizard\AW_Setup\Release\AuditWizard_v8.msi .

signtool sign /f SecurityKey.pfx /d "AuditWizard Overview" /p lotus207 /t http://timestamp.comodoca.com/authenticode AuditWizard_v8.msi

copy AuditWizard_v8.msi ..\Distribution
