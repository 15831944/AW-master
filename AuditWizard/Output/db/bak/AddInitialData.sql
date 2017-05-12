--------------------------------------------------------------------------------------------------
--                                                                            
--  File Name:   AddInitialData.sql                                   
--                                                                            
--  Description: Adds initial data to the database                                        
--                                                                            
--  Comments:    
--                                                                                                               
---------------------------------------------------------------------------------------------------

--use AuditWizard

--
--  INITIAL DATA
--
--  ASSET TYPES
--
DECLARE @nInsertedID int
INSERT INTO ASSET_TYPES (_NAME,_PARENTID,_AUDITABLE,_ICON,_INTERNAL) VALUES ('Computers' ,NULL ,1 ,'computers.png' ,1)
SET @nInsertedID = @@IDENTITY
INSERT INTO ASSET_TYPES (_NAME,_PARENTID,_AUDITABLE,_ICON,_INTERNAL) VALUES ('PC' ,@nInsertedID ,1 ,'computer.png' ,1)
INSERT INTO ASSET_TYPES (_NAME,_PARENTID,_AUDITABLE,_ICON,_INTERNAL) VALUES ('Server' ,@nInsertedID ,1 ,'server.png' ,1)
INSERT INTO ASSET_TYPES (_NAME,_PARENTID,_AUDITABLE,_ICON,_INTERNAL) VALUES ('Domain Controller' ,@nInsertedID ,1 ,'domaincontroller.png' ,1)
INSERT INTO ASSET_TYPES (_NAME,_PARENTID,_AUDITABLE,_ICON,_INTERNAL) VALUES ('Laptop' ,@nInsertedID ,1 ,'laptop.png' ,1)
--
INSERT INTO ASSET_TYPES (_NAME,_PARENTID,_ICON ,_INTERNAL) VALUES ('Peripherals' ,NULL ,'peripherals.png' ,1)
SET @nInsertedID = @@IDENTITY
INSERT INTO ASSET_TYPES (_NAME,_PARENTID,_ICON ,_INTERNAL) VALUES ('Hub' ,@nInsertedID ,'hub.png' ,1)
INSERT INTO ASSET_TYPES (_NAME,_PARENTID,_ICON ,_INTERNAL) VALUES ('Switch' ,@nInsertedID ,'switch.png' ,1)
INSERT INTO ASSET_TYPES (_NAME,_PARENTID,_ICON ,_INTERNAL) VALUES ('Router' ,@nInsertedID ,'router.png' ,1)
INSERT INTO ASSET_TYPES (_NAME,_PARENTID,_ICON ,_INTERNAL) VALUES ('Network Printer' ,@nInsertedID ,'printer.png' ,1)
--
INSERT INTO ASSET_TYPES (_NAME,_PARENTID,_ICON ,_INTERNAL) VALUES ('USB Devices' ,NULL ,'usb.png' ,1)
SET @nInsertedID = @@IDENTITY
INSERT INTO ASSET_TYPES (_NAME,_PARENTID,_ICON ,_INTERNAL) VALUES ('USB' ,@nInsertedID ,'usb.png' ,1)
INSERT INTO ASSET_TYPES (_NAME,_PARENTID,_ICON ,_INTERNAL) VALUES ('HIDClass' ,@nInsertedID ,'usb.png' ,1)
INSERT INTO ASSET_TYPES (_NAME,_PARENTID,_ICON ,_INTERNAL) VALUES ('Disk Drive' ,@nInsertedID ,'usb.png' ,1)
--
INSERT INTO ASSET_TYPES (_NAME,_PARENTID,_ICON ,_INTERNAL) VALUES ('Mobile Devices' ,NULL ,'pda.png' ,1)
SET @nInsertedID = @@IDENTITY

-- LICENSE TYPES
INSERT INTO LICENSE_TYPES (_NAME,_COUNTED) VALUES ('OEM' ,1)
INSERT INTO LICENSE_TYPES (_NAME,_COUNTED) VALUES ('Retail' ,1)
INSERT INTO LICENSE_TYPES (_NAME,_COUNTED) VALUES ('Open' ,1)
INSERT INTO LICENSE_TYPES (_NAME,_COUNTED) VALUES ('Select' ,1)
INSERT INTO LICENSE_TYPES (_NAME,_COUNTED) VALUES ('Enterprise' ,0)
INSERT INTO LICENSE_TYPES (_NAME,_COUNTED) VALUES ('Freeware' ,0)
INSERT INTO LICENSE_TYPES (_NAME,_COUNTED) VALUES ('Shareware' ,1)
INSERT INTO LICENSE_TYPES (_NAME,_COUNTED) VALUES ('Bespoke' ,0)
INSERT INTO LICENSE_TYPES (_NAME,_COUNTED) VALUES ('GNU' ,0)
INSERT INTO LICENSE_TYPES (_NAME,_COUNTED) VALUES ('OpenSource' ,0)
INSERT INTO LICENSE_TYPES (_NAME,_COUNTED) VALUES ('Evaluation' ,0)

-- Default audited items to set the default icon settings
INSERT INTO AUDITEDITEMS (_CATEGORY ,_ASSETID, _ICON) VALUES ('Hardware' ,0 ,'hardware.png');
INSERT INTO AUDITEDITEMS (_CATEGORY ,_ASSETID, _ICON) VALUES ('Hardware|Display',0,'displayadapter.png');
INSERT INTO AUDITEDITEMS (_CATEGORY ,_ASSETID, _ICON) VALUES ('Hardware|Adapters|Display',0,'displayadapter.png');
INSERT INTO AUDITEDITEMS (_CATEGORY ,_ASSETID, _ICON) VALUES ('Hardware|Adapters|Network',0,'network.png');
INSERT INTO AUDITEDITEMS (_CATEGORY ,_ASSETID, _ICON) VALUES ('Hardware|Network',0,'network.png');
INSERT INTO AUDITEDITEMS (_CATEGORY ,_ASSETID, _ICON) VALUES ('Hardware|Multimedia',0,'multimedia.png');
INSERT INTO AUDITEDITEMS (_CATEGORY ,_ASSETID, _ICON) VALUES ('Hardware|Disk Drives',0,'diskdrives.png');
INSERT INTO AUDITEDITEMS (_CATEGORY ,_ASSETID, _ICON) VALUES ('Hardware|BIOS',0,'bios.png');
INSERT INTO AUDITEDITEMS (_CATEGORY ,_ASSETID, _ICON) VALUES ('Hardware|CPU',0,'cpu.png');
INSERT INTO AUDITEDITEMS (_CATEGORY ,_ASSETID, _ICON) VALUES ('Hardware|Memory',0,'memory.png');
INSERT INTO AUDITEDITEMS (_CATEGORY ,_ASSETID, _ICON) VALUES ('Hardware|Peripherals',0,'peripherals.png');
INSERT INTO AUDITEDITEMS (_CATEGORY ,_ASSETID, _ICON) VALUES ('Hardware|Peripherals|Modem',0,'modem.png');
INSERT INTO AUDITEDITEMS (_CATEGORY ,_ASSETID, _ICON) VALUES ('Hardware|Peripherals|Printer',0,'printer.png');
INSERT INTO AUDITEDITEMS (_CATEGORY ,_ASSETID, _ICON) VALUES ('Hardware|Peripherals|Keyboard',0,'keyboard.png');
INSERT INTO AUDITEDITEMS (_CATEGORY ,_ASSETID, _ICON) VALUES ('Hardware|Peripherals|Mouse',0,'mouse.png');
INSERT INTO AUDITEDITEMS (_CATEGORY ,_ASSETID, _ICON) VALUES ('Hardware|Ports',0,'ports.png');
INSERT INTO AUDITEDITEMS (_CATEGORY ,_ASSETID, _ICON) VALUES ('Hardware|Printers',0,'printer.png');
INSERT INTO AUDITEDITEMS (_CATEGORY ,_ASSETID, _ICON) VALUES ('Hardware|Physical Disk',0,'harddrive.png');
INSERT INTO AUDITEDITEMS (_CATEGORY ,_ASSETID, _ICON) VALUES ('System',0,'system.png');
INSERT INTO AUDITEDITEMS (_CATEGORY ,_ASSETID, _ICON) VALUES ('System|Environment',0,'environment.png');
INSERT INTO AUDITEDITEMS (_CATEGORY ,_ASSETID, _ICON) VALUES ('System|Locale',0,'locale.png');
INSERT INTO AUDITEDITEMS (_CATEGORY ,_ASSETID, _ICON) VALUES ('System|Network',0,'network.png');
INSERT INTO AUDITEDITEMS (_CATEGORY ,_ASSETID, _ICON) VALUES ('System|Registry',0,'registry.png');
INSERT INTO AUDITEDITEMS (_CATEGORY ,_ASSETID, _ICON) VALUES ('System|Windows Security',0,'windowssecurity.png');
INSERT INTO AUDITEDITEMS (_CATEGORY ,_ASSETID, _ICON) VALUES ('Internet|History',0,'ie_history.png');
INSERT INTO AUDITEDITEMS (_CATEGORY ,_ASSETID, _ICON) VALUES ('Internet|Cookie',0,'cookies.png');
INSERT INTO AUDITEDITEMS (_CATEGORY ,_ASSETID, _ICON) VALUES ('System|Windows Security|Windows Update',0,'windowsupdate.png');
INSERT INTO AUDITEDITEMS (_CATEGORY ,_ASSETID, _ICON) VALUES ('System|Windows Security|Windows Firewall',0,'firewall.png');
INSERT INTO AUDITEDITEMS (_CATEGORY ,_ASSETID, _ICON) VALUES ('System|Patches',0,'patch.png');


-- Add default settings for upload etc 
INSERT INTO SETTINGS (_KEY, _VALUE) VALUES ('Publisher Filter' ,'')
INSERT INTO SETTINGS (_KEY, _VALUE) VALUES ('AutoUpload Enabled' ,'True')
INSERT INTO SETTINGS (_KEY, _VALUE) VALUES ('DeleteAfterUpload' ,'True')
INSERT INTO SETTINGS (_KEY, _VALUE) VALUES ('BackupAfterUpload' ,'False')
--
INSERT INTO SETTINGS (_KEY, _VALUE) VALUES ('Database AutoPurge Enabled' ,'True')
INSERT INTO SETTINGS (_KEY, _VALUE) VALUES ('Database History Purge' ,'3')
INSERT INTO SETTINGS (_KEY, _VALUE) VALUES ('Database History Purge Units' ,'1')
INSERT INTO SETTINGS (_KEY, _VALUE) VALUES ('Database Internet Purge' ,'28')
INSERT INTO SETTINGS (_KEY, _VALUE) VALUES ('Database Internet Purge Units' ,'0')
INSERT INTO SETTINGS (_KEY, _VALUE) VALUES ('Database Asset Purge' ,'6')
INSERT INTO SETTINGS (_KEY, _VALUE) VALUES ('Database Asset Purge Units' ,'1')
--
INSERT INTO SETTINGS (_KEY, _VALUE) VALUES ('AlertMonitorEnable' ,'False')
INSERT INTO SETTINGS (_KEY, _VALUE) VALUES ('AlertMonitorSettingsInterval' ,'30')
INSERT INTO SETTINGS (_KEY, _VALUE) VALUES ('AlertMonitorRecheckInterval' ,'60')
INSERT INTO SETTINGS (_KEY, _VALUE) VALUES ('AlertMonitorSystemTray' ,'False')
--
INSERT INTO SETTINGS (_KEY, _VALUE) VALUES ('RemoteDesktopCommand' ,'C:\Windows\system32\mstsc.exe /v:%A')

-- Add a default location and domain to the database
INSERT INTO LOCATIONS (_FULLNAME ,_NAME ,_PARENTID) VALUES ('Home' ,'Home' ,NULL)
INSERT INTO DOMAINS   (_NAME ,_PARENTID) VALUES ('Home' ,NULL)
    
-- Add a default Supplier to the database
INSERT INTO SUPPLIERS (_NAME) VALUES ('N/A')
    
-- Add a default administrator to the database noting that it's root location is the location added above
INSERT INTO USERS (_LOGIN, _FIRSTNAME ,_LASTNAME ,_PASSWORD ,_ACCESSLEVEL ,_ROOTLOCATION) VALUES ('Admin' ,'Administrator' ,'' ,'' ,0 ,1)

-- Add a LicenseWizardUser1 to the database
if not exists (select * from master.dbo.syslogins where loginname = 'AuditWizardUser1') 
	BEGIN
		declare @logindb nvarchar(132)
		select @logindb = 'AuditWizard' 
		if @logindb is null or not exists 
			(select * from master.dbo.sysdatabases where name = @logindb) 
		select @logindb = 'master' 
		exec sp_addlogin 'AuditWizardUser1', 'AuditWizard1', @logindb 
	END 
GO
if not exists (select * from dbo.sysusers where name = 'AuditWizardUser1' and uid < 16382)
	EXEC sp_grantdbaccess 'AuditWizardUser1', 'AuditWizardUser1' 
GO
exec sp_addrolemember 'db_owner', 'AuditWizardUser1' 
GO

-- And set the LicenseWizard Database Version
exec usp_SetVersion 8, 1, 'AuditWizard Database'    
GO