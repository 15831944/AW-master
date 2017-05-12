--------------------------------------------------------------------------------------------------
--                                                                            
--  File Name:   CreateStoredProcedures.sql                                   
--                                                                            
--  Description: SQL Script containing stored procedures for AuditWizard                                    
--                                                                                                               
---------------------------------------------------------------------------------------------------

--use AuditWizard    


SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--------------------------------------------------------------------------
--																		--
-- usp_users_enumerate													--
-- ===================													--
--																		--
--------------------------------------------------------------------------
--																		--
-- Description															--
-- ===========															--
--																		--
-- This stored procedure is part of the AuditWizard Product			--
-- It is used to enumerate the list of USERS declared					--
--																		--
--------------------------------------------------------------------------
--																		--
-- History																--
-- =======																--
--																		--
--	22-Jul-2008	Chris Drew		Initial Version							--
--																		--
--------------------------------------------------------------------------
CREATE PROCEDURE [dbo].[usp_users_enumerate]
AS   

	DECLARE @cSQL varchar(512) 
	
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT USERS._USERID ,USERS._LOGIN ,USERS._FIRSTNAME ,USERS._LASTNAME ,USERS._ACCESSLEVEL ,USERS._ROOTLOCATION
			,LOCATIONS._FULLNAME ,LOCATIONS._NAME
		 FROM USERS
		LEFT JOIN LOCATIONS ON (USERS._ROOTLOCATION = LOCATIONS._LOCATIONID)
END
GO 
 



SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--------------------------------------------------------------------------
--																		--
-- usp_user_update														--
-- ===============														--
--																		--
--------------------------------------------------------------------------
--																		--
-- Description															--
-- ===========															--
--																		--
-- This stored procedure is part of the AuditWizard Product				--
-- It is used to add a update a user definititon in the database		--
--																		--
--	All character strings passed to this function must be SQL formatted --
--																		--
--------------------------------------------------------------------------
--																		--
-- History																--
-- =======																--
--																		--
--	22-Jul-2006	Chris Drew		Initial Version							--
--																		--
--------------------------------------------------------------------------

CREATE PROCEDURE [dbo].[usp_user_update]
	@nUserID int,
	@cLogin varchar(255),
	@cFirstName varchar(255),
	@cLastName	varchar(255),
	@nAccessLevel int,
	@nRootLocation int
	
AS     
	SET nocount ON

	-- Update the user
	UPDATE USERS SET _LOGIN=@cLogin 
					,_FIRSTNAME=@cFirstName
					,_LASTNAME=@cLastName 
					,_ACCESSLEVEL=@nAccessLevel
					,_ROOTLOCATION=@nRootLocation
					WHERE _USERID=@nUserID
GO




SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--------------------------------------------------------------------------
--																		--
-- usp_user_getdetails													--
-- ===================													--
--																		--
--------------------------------------------------------------------------
--																		--
-- Description															--
-- ===========															--
--																		--
-- This stored procedure is part of the AuditWizard Product			--
-- It is used to recover full details for the specified user			--
--																		--
--------------------------------------------------------------------------
--																		--
-- History																--
-- =======																--
--																		--
--	22-Jul-2008	Chris Drew		Initial Version							--
--																		--
--------------------------------------------------------------------------
CREATE PROCEDURE [dbo].[usp_user_getdetails]
	@nUserId int
AS   
BEGIN
	SET NOCOUNT ON;

	SELECT USERS._USERID ,USERS._LOGIN ,USERS._FIRSTNAME ,USERS._LASTNAME ,USERS._ACCESSLEVEL ,USERS._ROOTLOCATION
			  ,LOCATIONS._FULLNAME ,LOCATIONS._NAME
			FROM USERS
			  LEFT JOIN LOCATIONS ON (USERS._ROOTLOCATION = LOCATIONS._LOCATIONID)
			WHERE _USERID=@nUserId
END
GO


SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--=========================================================
--
-- Name:		usp_preparestring
--
-- Author:		Chris Drew
-- Create date: 11/Feb/08
-- Description:	Ensure that any character data passed to 
--				the database is corectly formatted and does 
--				not contain any illegal characters
--
-- Parameters:
--
--=========================================================

CREATE PROCEDURE [dbo].[usp_preparestring]	@cInString varchar(5000),
									@cOutString varchar(5000) output
AS

	DECLARE @NERROR INT	-- ERROR VALUE

BEGIN	
	-- EXIT IF ZERO LENGTH STRING PASSED
	IF LEN(@CINSTRING) = 0
	BEGIN
		SET @COUTSTRING = ''''''
		SET @NERROR = @@ERROR
		RETURN @NERROR
	END

	-- REPLACE ANY SINGLE QUOTE WITH TWO SINGLES
	-- TRUNCATION OF STRINGS IN EXCESS OF 5000 WILL BE AUTOMATIC
	SET @COUTSTRING = '''' + REPLACE(@CINSTRING,'''','''''') + ''''

	-- RETURN

	SET @NERROR = @@ERROR	
	RETURN @NERROR    
END
GO




SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =================================================================
-- Author:		Chris Drew
-- Create date: 11/Feb/08
-- Description:	Insert a record into the database table with an Identity field
--
-- This procedure makes the assumption that the table name is 
-- NOT fully qualified when it is passed calling usp_SetTableName 
-- would slow up the procedure and degrade  performance
--    
-- Please ensure that ALL text data is SQL complient by calling 
-- usp_preparestring before passing to this function
--
-- Parameters:
--
-- ====================================================================
 
CREATE PROCEDURE [dbo].[usp_IdentityInsert]	
				@cTableName varchar(128),
				@cColumns varchar(5000),
				@cData varchar(5000),
				@nReturnID int output
AS

DECLARE
	@nReturn int,				-- return value from calling a stored procedure
	@cSQL varchar(8000)			-- sql statements

BEGIN
    SET NOCOUNT ON				-- stop procedure echoing counts etc to application

    -- insert data                                               
    SET @cSQL = 'INSERT INTO ' +  @cTableName  + '(' + @cColumns + ') values (' + @cData + ')'
    EXEC(@cSQL)    

	-- Now recover the last identity field
	set @nReturnID = @@IDENTITY
END
GO




SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--------------------------------------------------------------------------
--																		--
-- usp_applications_enumerate											--
-- ==========================											--
--																		--
--------------------------------------------------------------------------
--																		--
-- Description															--
-- ===========															--
--																		--
-- This stored procedure is part of the AuditWizard Product				--
-- It is used to return a list of applications audited for the 			--
-- specified Publisher along with their license counts					--													--
--																		--
--	NOTE: THIS STORED PROCEDURE ONLY DEALS WITH APPLICATIONS			--
--		  OPERATING SYSTEMS ARE DEALT WITH SEPARATELY					--
--																		--
--------------------------------------------------------------------------
--																		--
-- History																--
-- =======																--
--																		--
--	21-Feb-2008	Chris Drew		Initial Version							--
--																		--
--------------------------------------------------------------------------

CREATE PROCEDURE [dbo].[usp_applications_enumerate]
	@forPublisher varchar(5000),
	@showAllPublishers bit,
	@showIncluded bit,
	@showIgnored bit
AS  
	DECLARE @whereClause varchar(5050)
    DECLARE @cSQL varchar(5100)

	SET nocount ON  
	
	-- Do we need to add a where clause to filter for a specific publisher?
	IF (@showAllPublishers = 1)
		SET @whereClause = ' WHERE _ISOS=0'
	ELSE
		SET @whereClause = ' WHERE _PUBLISHER in (' + @forPublisher + ') AND _ISOS=0'

	-- Exclude aliased applications at this point
	SET @whereClause = @whereClause + ' AND _ALIASED_TOID=0'

	-- Are we showing licensable, non-licensable or both?
	IF (@showIncluded = 1 AND @showIgnored = 0) 
		SET @whereClause = @whereClause + ' AND _IGNORED=0'
	ELSE IF (@showIncluded = 0 AND @showIgnored = 1) 
		SET @whereClause = @whereClause + ' AND _IGNORED=1'

	-- Construct the whole query
	SET @cSQL = 'SELECT APPLICATIONS.* ,INSTALLCOUNTS.INSTALLCOUNT'
			+  ' FROM APPLICATIONS'
			+  ' LEFT JOIN'
			+  '  (SELECT APPLICATIONS._APPLICATIONID ,COUNT(APPLICATIONS._APPLICATIONID) INSTALLCOUNT '
			+  '  FROM APPLICATIONS '
			+  ' LEFT JOIN APPLICATION_INSTANCES S ON (APPLICATIONS._APPLICATIONID = S._APPLICATIONID) '
			+  ' LEFT JOIN ASSETS A ON (S._ASSETID = A._ASSETID) '
			+   @whereClause
			+  ' AND A._STOCK_STATUS<>3'
			+  ' GROUP BY APPLICATIONS._APPLICATIONID) INSTALLCOUNTS ON APPLICATIONS._APPLICATIONID = INSTALLCOUNTS._APPLICATIONID '
			+   @whereClause
			+  ' ORDER BY APPLICATIONS._PUBLISHER, APPLICATIONS._NAME'
	EXEC (@cSQL)
GO





SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--------------------------------------------------------------------------
--																		--
-- usp_applications_enumerate_aliases									--
-- ==================================									--
--																		--
--------------------------------------------------------------------------
--																		--
-- Description															--
-- ===========															--
--																		--
-- This stored procedure is part of the AuditWizard Product				--
-- It is used to return a list of applications which have been aliased	--
-- to other applications												--
--																		--
--------------------------------------------------------------------------
--																		--
-- History																--
-- =======																--
--																		--
--	13-Apr-2009	Chris Drew		Initial Version							--
--																		--
--------------------------------------------------------------------------

CREATE PROCEDURE [dbo].[usp_applications_enumerate_aliases]
AS  
	SET nocount ON  

SELECT A.*, B._PUBLISHER AS ASSIGNEDTO_PUBLISHER, B._NAME AS ASSIGNEDTO_NAME 
	FROM APPLICATIONS A
	LEFT JOIN APPLICATIONS B ON (A._ALIASED_TOID = B._APPLICATIONID) 
	WHERE A._ALIASED_TOID<>0
		ORDER BY A._PUBLISHER, A._NAME
GO






SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--------------------------------------------------------------------------
--																		--
-- usp_application_alias_count											--
-- ============================											--
--																		--
--------------------------------------------------------------------------
--																		--
-- Description															--
-- ===========															--
--																		--
-- This stored procedure is part of the AuditWizard Product				--
-- It is used to return a count of the applications which have been		--
-- aliased to this application											--
--																		--
--------------------------------------------------------------------------
--																		--
-- History																--
-- =======																--
--																		--
--	13-Apr-2009	Chris Drew		Initial Version							--
--																		--
--------------------------------------------------------------------------

CREATE PROCEDURE [dbo].[usp_application_alias_count]
	@applicationID int
AS  
	DECLARE @nReturnID int

	SET nocount ON  

	SET @nReturnID = (SELECT COUNT(*) FROM APPLICATIONS WHERE _ALIASED_TOID=@applicationID)
	return @nReturnID
GO


SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--------------------------------------------------------------------------
--																		--
-- usp_get_application													--
-- ===================													--
--																		--
--------------------------------------------------------------------------
--																		--
-- Description															--
-- ===========															--
--																		--
-- This stored procedure is part of the AuditWizard Product				--
-- It is used to return details for teh specific application			--
--																		--
--------------------------------------------------------------------------
--																		--
-- History																--
-- =======																--
--																		--
--	21-Feb-2008	Chris Drew		Initial Version							--
--																		--
--------------------------------------------------------------------------

CREATE PROCEDURE [dbo].[usp_get_application]
	@nApplicationID int
AS  
	SELECT APPLICATIONS.*
		FROM APPLICATIONS
		WHERE _APPLICATIONID = @nApplicationID
GO




SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--------------------------------------------------------------------------
--																		--
-- usp_asset_applications_enumerate									--
-- ===================================									--
--																		--
--------------------------------------------------------------------------
--																		--
-- Description															--
-- ===========															--
--																		--
-- This stored procedure is part of the AuditWizard Product				--
-- It is used to return a list of installed applications for the		--
-- specified asset													--
--																		--
--	NOTE: THIS STORED PROCEDURE ONLY DEALS WITH APPLICATIONS			--
--		  OPERATING SYSTEMS ARE DEALT WITH SEPARATELY					--
--																		--
--------------------------------------------------------------------------
--																		--
-- History																--
-- =======																--
--																		--
--	21-Feb-2008	Chris Drew		Initial Version							--
--																		--
--------------------------------------------------------------------------

CREATE PROCEDURE [dbo].[usp_asset_applications_enumerate]
	@assetID int,
	@forPublisher varchar(5000),
	@showIncluded bit,
	@showIgnored bit
AS     
	SET nocount ON

	DECLARE @cSQL varchar(5100)

	SET @cSQL = 'SELECT APPLICATION_INSTANCES._INSTANCEID ,APPLICATION_INSTANCES._APPLICATIONID ,APPLICATION_INSTANCES._ASSETID ,APPLICATION_INSTANCES._PRODUCTID ,APPLICATION_INSTANCES._CDKEY'
			+  ',APPLICATION_INSTANCES._SUPPORT_ALERTEMAIL'
			+  ',APPLICATIONS._NAME ,APPLICATIONS._VERSION ,APPLICATIONS._PUBLISHER ,APPLICATIONS._GUID ,APPLICATIONS._ISOS ,APPLICATIONS._IGNORED ,APPLICATIONS._ALIASED_TOID ,APPLICATIONS._USER_DEFINED'
		    +  ',ASSETS._NAME AS ASSETNAME'
			+  ',ASSET_TYPES._ICON AS ASSETICON'
			+  ',LOCATIONS._FULLNAME AS FULLLOCATIONNAME'
			+  ' FROM APPLICATION_INSTANCES'
			+  ' LEFT JOIN APPLICATIONS ON (APPLICATION_INSTANCES._APPLICATIONID = APPLICATIONS._APPLICATIONID)'
			+  ' LEFT JOIN ASSETS ON (APPLICATION_INSTANCES._ASSETID = ASSETS._ASSETID)'
			+  ' LEFT JOIN ASSET_TYPES ON (ASSETS._ASSETTYPEID=ASSET_TYPES._ASSETTYPEID)'
			+  ' LEFT JOIN LOCATIONS ON (ASSETS._LOCATIONID = LOCATIONS._LOCATIONID)'
			+  ' WHERE APPLICATION_INSTANCES._ASSETID=' + cast(@assetID as varchar(32))
			+  ' AND APPLICATIONS._ISOS=0'

	-- Handle whether or not to include 'ignored' applications
	IF (@showIncluded = 0 AND @showIgnored = 1)
		SET @cSQL = @cSQL +  ' AND APPLICATIONS._IGNORED=1'    
		
	ELSE IF (@showIncluded = 1 AND @showIgnored = 0)
		SET @cSQL = @cSQL +  ' AND APPLICATIONS._IGNORED=0'

	-- ...and optionally filter for a specific publisher also
	IF (@forPublisher <> '')
		SET @cSQL = @cSQL + ' AND APPLICATIONS._PUBLISHER IN (' + @forPublisher + ')'

	-- Order by application publisher and name
	SET @cSQL = @cSQL + ' ORDER BY APPLICATIONS._PUBLISHER, APPLICATIONS._NAME'

	-- Execute the query
	EXEC (@cSQL)
GO




SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--------------------------------------------------------------------------
--																		--
-- usp_os_enumerate														--
-- ================														--
--																		--
--------------------------------------------------------------------------
--																		--
-- Description															--
-- ===========															--
--																		--
-- This stored procedure is part of the AuditWizard Product				--
-- It is used to return a list of Operating Systems audited  			--
--																		--
--	NOTE: THIS STORED PROCEDURE ONLY DEALS WITH OPERATING SYSTEMS 		--
--																		--
--------------------------------------------------------------------------
--																		--
-- History																--
-- =======																--
--																		--
--	21-Feb-2008	Chris Drew		Initial Version							--
--																		--
--------------------------------------------------------------------------

CREATE PROCEDURE [dbo].[usp_os_enumerate]
AS  
	SET nocount ON  

	SELECT APPLICATIONS._APPLICATIONID ,APPLICATIONS._NAME ,APPLICATIONS._VERSION, INSTALLCOUNTS.installCount
			FROM APPLICATIONS
			LEFT OUTER JOIN
				(SELECT APPLICATIONS._APPLICATIONID ,COUNT(APPLICATIONS._APPLICATIONID) INSTALLCOUNT
					FROM APPLICATIONS LEFT OUTER JOIN APPLICATION_INSTANCES S ON (APPLICATIONS._APPLICATIONID = S._APPLICATIONID)
					WHERE _ISOS=1
					GROUP BY APPLICATIONS._APPLICATIONID) INSTALLCOUNTS 
			ON APPLICATIONS._APPLICATIONID = INSTALLCOUNTS._APPLICATIONID
			WHERE _ISOS=1
			ORDER BY APPLICATIONS._APPLICATIONID
GO




SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--------------------------------------------------------------------------
--																		--
-- usp_asset_getos													--
-- ==================													--
--																		--
--------------------------------------------------------------------------
--																		--
-- Description															--
-- ===========															--
--																		--
-- This stored procedure is part of the AuditWizard Product				--
-- It is used to return the OS entry if any for the	specified asset	--
--																		--
--	NOTE: THIS STORED PROCEDURE ONLY DEALS WITH OPERATING SYSTEMS		--
--																		--
--------------------------------------------------------------------------
--																		--
-- History																--
-- =======																--
--																		--
--	21-Feb-2008	Chris Drew		Initial Version							--
--																		--
--------------------------------------------------------------------------

CREATE PROCEDURE [dbo].[usp_asset_getos]
	@assetID int
AS     
	SET nocount ON

	SELECT APPLICATION_INSTANCES._INSTANCEID ,APPLICATION_INSTANCES._APPLICATIONID ,APPLICATION_INSTANCES._PRODUCTID ,APPLICATION_INSTANCES._CDKEY
			,APPLICATIONS._NAME ,APPLICATIONS._VERSION
		    ,ASSETS._ASSETID ,ASSETS._NAME AS ASSETNAME 
			,ASSET_TYPES._ICON AS ASSETICON
		FROM APPLICATION_INSTANCES
		LEFT JOIN APPLICATIONS ON (APPLICATION_INSTANCES._APPLICATIONID = APPLICATIONS._APPLICATIONID)
		LEFT JOIN ASSETS ON (APPLICATION_INSTANCES._ASSETID = ASSETS._ASSETID)
		LEFT JOIN ASSET_TYPES ON (ASSETS._ASSETTYPEID=ASSET_TYPES._ASSETTYPEID)
		WHERE APPLICATION_INSTANCES._ASSETID=@assetID AND APPLICATIONS._ISOS=1
GO




SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--------------------------------------------------------------------------
--																		--
-- usp_asset_update														--
-- ===================													--
--																		--
--------------------------------------------------------------------------
--																		--
-- Description															--
-- ===========															--
--																		--
-- This stored procedure is part of the AuditWizard Product			--
-- It is used to add a update a asset definititon in the database		--
--																		--
--------------------------------------------------------------------------
--																		--
-- History																--
-- =======																--
--																		--
--	18-Feb-2006	Chris Drew		Initial Version							--
--																		--
--------------------------------------------------------------------------

CREATE PROCEDURE [dbo].[usp_asset_update]
	@nAssetID int,
	@cName varchar(255),
	@nLocationID int,
	@nDomainID int,
	@cIPAddress varchar(255),
	@cMACAddress varchar(255),
	@nAssetTypeID int,
	@cMake varchar(255),
	@cModel varchar(255),
	@cSerial varchar(255),	
	@nParentAssetID int,
	@nSupplierID int,
	@nStockStatus int,
	@bAlertsEnabled bit
	
AS     
	DECLARE @cSQL as varchar(255)
	SET nocount ON   
	
	-- Format the first part of the command
	UPDATE ASSETS SET _NAME=@cName 
					, _LOCATIONID=@nLocationID
					,_DOMAINID=@nDomainID 
					,_IPADDRESS=@cIPaddress 
					,_MACADDRESS=@cMACAddress 
					,_ASSETTYPEID=@nAssetTypeID
					,_MAKE=@cMake
					,_MODEL=@cModel
					,_SERIAL_NUMBER=@cSerial
					,_PARENT_ASSETID=@nParentAssetID
					,_SUPPLIERID=@nSupplierID
					,_STOCK_STATUS=@nStockStatus
					,_ALERTS_ENABLED=@bAlertsEnabled
	WHERE _ASSETID=@nAssetID
GO



set ANSI_NULLS ON
set QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Chris Drew
-- Create date: 6/11/07
-- Description:	Returns the assets for the specified group.
-- =============================================
CREATE PROCEDURE [dbo].[usp_asset_getdetails]
	@nAssetID int
AS
    DECLARE @cSQL varchar(510)

BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
 
	SELECT ASSETS.*
		 , ASSET_TYPES._NAME AS ASSETTYPENAME 
		 , ASSET_TYPES._ICON AS ICON
		 , ASSET_TYPES._AUDITABLE AS AUDITABLE
		 , LOCATIONS._NAME AS LOCATIONNAME
		 , LOCATIONS._FULLNAME AS FULLLOCATIONNAME
		 , DOMAINS._NAME AS DOMAINNAME				
		 , ISNULL(SUPPLIERS._NAME, '') SUPPLIER_NAME

		FROM ASSETS 
			LEFT JOIN ASSET_TYPES ON (ASSETS._ASSETTYPEID=ASSET_TYPES._ASSETTYPEID)
			LEFT JOIN LOCATIONS   ON (ASSETS._LOCATIONID=LOCATIONS._LOCATIONID)
			LEFT JOIN DOMAINS	  ON (ASSETS._DOMAINID=DOMAINS._DOMAINID)
			LEFT JOIN SUPPLIERS   ON (ASSETS._SUPPLIERID = SUPPLIERS._SUPPLIERID)

		WHERE _ASSETID = @nAssetID
END
GO



SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--------------------------------------------------------------------------
--																		--
-- usp_publishers_enumerate												--
-- ========================												--
--																		--
--------------------------------------------------------------------------
--																		--
-- Description															--
-- ===========															--
--																		--
-- This stored procedure is part of the AuditWizard Product				--
-- It is used to return a list of publishers 				 			--
--																		--
--------------------------------------------------------------------------
--																		--
-- History																--
-- =======																--
--																		--
--	21-Feb-2008	Chris Drew		Initial Version							--
--																		--
--------------------------------------------------------------------------

CREATE PROCEDURE [dbo].[usp_publishers_enumerate]
						@publisherFilter varchar(5000)
AS  
	DECLARE @cSQL varchar(5100)

	SET nocount ON  
	SET @cSQL = 'SELECT DISTINCT(_PUBLISHER) FROM APPLICATIONS'
	IF (@publisherFilter <> '')
		SET @cSQL = @cSQL + ' WHERE _PUBLISHER in (' + @publisherFilter + ')'
	EXEC (@cSQL)
GO




SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--------------------------------------------------------------------------
--																		--
-- usp_statistics_topapplications										--
-- ==============================										--
--																		--
--------------------------------------------------------------------------
--																		--
-- Description															--
-- ===========															--
--																		--
-- This stored procedure is part of the AuditWizard Product				--
-- It is used to return statistics about the most commonly used apps	--
--																		--
--------------------------------------------------------------------------
--																		--
-- History																--
-- =======																--
--																		--
--	29-Feb-2008	Chris Drew		Initial Version							--
--																		--
--------------------------------------------------------------------------

CREATE PROCEDURE [dbo].[usp_statistics_topapplications]
						@resultCount int,
						@publisherFilter varchar(1024)
AS	
BEGIN
	DECLARE @SQL varchar(5000)
    SET ROWCOUNT @RESULTCOUNT 

	-- Construct the SQL statement itself
	SET @SQL = 'SELECT APPLICATIONS._NAME, INSTANCECOUNT=
				(
					SELECT COUNT(*) AS INSTANCECOUNT
					FROM APPLICATION_INSTANCES 
					WHERE APPLICATION_INSTANCES._APPLICATIONID = APPLICATIONS._APPLICATIONID 
						AND APPLICATIONS._ISOS=0 
						AND APPLICATIONS._IGNORED=0'


	-- Add on the Publisher Filter if one was specified
	IF (@publisherFilter <> '')
		SET @SQL = @SQL + ' AND APPLICATIONS._PUBLISHER IN (' + @publisherFilter + ')'
 	
	-- Close the nested select and add the second where 
	SET @SQL = @SQL + ') FROM APPLICATIONS 					
							WHERE APPLICATIONS._ISOS=0 
								AND APPLICATIONS._IGNORED=0'

	-- Add on the Publisher Filter if one was specified
	IF (@publisherFilter <> '')
		SET @SQL = @SQL + ' AND APPLICATIONS._PUBLISHER IN (' + @publisherFilter + ')'
	
	-- Finish with the ordering clause
	SET @SQL = @SQL + ' ORDER BY INSTANCECOUNT DESC'

	-- Execute the SQL
	EXEC (@SQL)
    SET ROWCOUNT 0 
END
GO




SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--------------------------------------------------------------------------
--																		--
-- usp_statistics_toppublishers											--
-- ============================											--
--																		--
--------------------------------------------------------------------------
--																		--
-- Description															--
-- ===========															--
--																		--
-- This stored procedure is part of the AuditWizard Product				--
-- It is used to return statistics about the most common publishers		--
--																		--
--------------------------------------------------------------------------
--																		--
-- History																--
-- =======																--
--																		--
--	29-Feb-2008	Chris Drew		Initial Version							--
--																		--
--------------------------------------------------------------------------

CREATE PROCEDURE [dbo].[usp_statistics_toppublishers]
						@resultCount int,
						@publisherFilter varchar(1024)
AS	
BEGIN
	DECLARE @cSQL varchar(1020)
    SET ROWCOUNT @RESULTCOUNT                       

	SET @cSQL = 'SELECT APPLICATIONS._PUBLISHER, COUNT(*) AS MYCOUNT'
			  + ' FROM APPLICATIONS WHERE APPLICATIONS._IGNORED=0'

	-- Add on publisher filter if specified
	IF (@publisherFilter <> '')
		SET @cSQL = @cSQL + ' AND _PUBLISHER in (' + @publisherFilter + ')'
 
    -- the grouping
    SET @cSQL = @cSQL + ' GROUP BY _PUBLISHER ORDER BY MYCOUNT DESC'
	EXEC (@cSQL)
    SET ROWCOUNT 0 
END
GO


set ANSI_NULLS ON
set QUOTED_IDENTIFIER ON
GO
--------------------------------------------------------------------------
--																		--
-- usp_statistics_msoffice												--
-- =======================												--
--																		--
--------------------------------------------------------------------------
--																		--
-- Description															--
-- ===========															--
--																		--
-- This stored procedure is part of the AuditWizard Product				--
-- It is used to return statistics about the most commonly used apps	--
--																		--
--------------------------------------------------------------------------
--																		--
-- History																--
-- =======																--
--																		--
--	29-Feb-2008	Chris Drew		Initial Version							--
--																		--
--------------------------------------------------------------------------

CREATE PROCEDURE [dbo].[usp_statistics_msoffice]
						@resultCount int
AS	
BEGIN
    SET ROWCOUNT @RESULTCOUNT 

	SELECT APPLICATIONS._NAME, INSTANCECOUNT=
	(
		SELECT COUNT(*) AS INSTANCECOUNT
					FROM APPLICATION_INSTANCES 
					WHERE APPLICATION_INSTANCES._APPLICATIONID = APPLICATIONS._APPLICATIONID 
						AND APPLICATIONS._ISOS=0 
						AND APPLICATIONS._IGNORED=0
						AND APPLICATIONS._NAME LIKE 'Microsoft Office %'
	) FROM APPLICATIONS 					

	WHERE APPLICATIONS._ISOS=0 
		AND APPLICATIONS._IGNORED=0
		AND APPLICATIONS._NAME LIKE 'Microsoft Office %'
	ORDER BY INSTANCECOUNT DESC

    SET ROWCOUNT 0 
END
GO






set ANSI_NULLS ON
set QUOTED_IDENTIFIER ON
GO
--------------------------------------------------------------------------
--																		--
-- usp_statistics_auditeditems											--
-- ===========================											--
--																		--
--------------------------------------------------------------------------
--																		--
-- Description															--
-- ===========															--
--																		--
-- This stored procedure is part of the AuditWizard Product				--
-- It is used to return statistics about audited items					--
--																		--
--------------------------------------------------------------------------
--																		--
-- History																--
-- =======																--
--																		--
--	21-Jan-2009	Chris Drew		Initial Version							--
--																		--
--------------------------------------------------------------------------

CREATE PROCEDURE [dbo].[usp_statistics_auditeditems]
						@resultCount int,
						@cCategory varchar(255),
						@cName varchar(255)
AS	
BEGIN
    SET ROWCOUNT @RESULTCOUNT 

	SELECT AUDITEDITEMS._VALUE, COUNT(_VALUE) AS INSTANCECOUNT
			FROM AUDITEDITEMS 
			WHERE _CATEGORY=@cCategory AND _NAME=@cName
	GROUP BY _VALUE
	ORDER BY INSTANCECOUNT DESC

    SET ROWCOUNT 0 
END
GO




set ANSI_NULLS ON
set QUOTED_IDENTIFIER ON
GO
--------------------------------------------------------------------------
--																		--
-- usp_statistics_assetttpes											--
-- =========================											--
--																		--
--------------------------------------------------------------------------
--																		--
-- Description															--
-- ===========															--
--																		--
-- This stored procedure is part of the AuditWizard Product				--
-- It is used to return statistics about the types of assets defined	--
--																		--
--------------------------------------------------------------------------
--																		--
-- History																--
-- =======																--
--																		--
--	08-Aug-2008	Chris Drew		Initial Version							--
--																		--
--------------------------------------------------------------------------

CREATE PROCEDURE [dbo].[usp_statistics_assettypes]
AS
BEGIN
  
	SELECT COUNT(*), ASSET_TYPES._NAME 
	FROM ASSETS 
	LEFT JOIN ASSET_TYPES ON ASSETS._ASSETTYPEID=ASSET_TYPES._ASSETTYPEID
	GROUP BY ASSET_TYPES._NAME
END
GO


SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--------------------------------------------------------------------------
--																		--
-- usp_licensetype_delete												--
-- ======================												--
--																		--
--------------------------------------------------------------------------
--																		--
-- Description															--
-- ===========															--
--																		--
-- This stored procedure is part of the AuditWizard Product				--
-- It is used to add a DELETE a License Type definititon 				--
--																		--
-- Note that it is the responsibility of the caller to handle any		--																		--
-- tables which may depend on this record								--
--																		--
--------------------------------------------------------------------------
--																		--
-- History																--
-- =======																--
--																		--
--	18-Feb-2006	Chris Drew		Initial Version							--
--																		--
--------------------------------------------------------------------------

CREATE PROCEDURE [dbo].[usp_licensetype_delete]
	@nLicenseTypeID int
AS     
	SET nocount ON
    
	-- Referential Integrity Checks
	-- ============================
	-- There are foreign keys to the LICENSETYPE table from LICENSES table
	-- so we must handle these first
	--
	-- Delete related entries in the LICENSES table
	DELETE FROM LICENSES WHERE _LICENSETYPEID=@nLicenseTypeID

	-- Now delete the licensetype itself
	DELETE FROM LICENSE_TYPES WHERE _LICENSETYPEID=@nLicenseTypeID
GO



SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--------------------------------------------------------------------------
--																		--
-- usp_license_delete													--
-- ==================													--
--																		--
--------------------------------------------------------------------------
--																		--
-- Description															--
-- ===========															--
--																		--
-- This stored procedure is part of the AuditWizard Product				--
-- It is used to add a DELETE a License  definititon 					--
--																		--
--------------------------------------------------------------------------
--																		--
-- History																--
-- =======																--
--																		--
--	18-Feb-2006	Chris Drew		Initial Version							--
--																		--
--------------------------------------------------------------------------

CREATE PROCEDURE [dbo].[usp_license_delete]
	@nLicenseID int
AS     
	SET nocount ON
	DELETE FROM LICENSES WHERE _LICENSEID=@nLicenseID
GO




SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--------------------------------------------------------------------------
--																		--
-- usp_licenses_enumerate												--
-- ======================												--
--																		--
--------------------------------------------------------------------------
--																		--
-- Description															--
-- ===========															--
--																		--
-- This stored procedure is part of the AuditWizard Product				--
-- It is used to enumerate the list of licenses declared				--
--																		--
--------------------------------------------------------------------------
--																		--
-- History																--
-- =======																--
--																		--
--	04-Mar-2008	Chris Drew		Initial Version							--
--																		--
--------------------------------------------------------------------------
CREATE PROCEDURE [dbo].[usp_licenses_enumerate]
	@nLicenseTypeID int
AS   

	DECLARE @cSQL varchar(512) 
	
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SET @cSQL = 'SELECT _LICENSEID ,_LICENSETYPEID ,_APPLICATIONID ,_COUNT ,_DOCUMENT'
			+  ' FROM LICENSES'   
			
	-- If we have specified a specific license type then add this as a WHERE clause
	IF (@nLicenseTypeID <> 0)
		SET @cSQL = @cSQL + ' WHERE _LICENSETYPEID=' + cast(@nLicenseTypeID as varchar(16))
	EXEC (@cSQL)
END
GO



set ANSI_NULLS ON
set QUOTED_IDENTIFIER ON
GO
--------------------------------------------------------------------------
--																		--
-- usp_audittrail_enumerate												--
-- ========================												--
--																		--
--------------------------------------------------------------------------
--																		--
-- Description															--
-- ===========															--
--																		--
-- This stored procedure is part of the AuditWizard Product				--
-- It is used to return a list of audit trail entries - note that we 	--
-- do not use this procedure for audit history records					--
--																		--
--																		--
--------------------------------------------------------------------------
--																		--
-- History																--
-- =======																--
--																		--
--	21-Feb-2008	Chris Drew		Initial Version							--
--																		--
--------------------------------------------------------------------------

CREATE PROCEDURE [dbo].[usp_audittrail_enumerate]
	@nClass int
AS 
	DECLARE @cSQL varchar(1024) 
	SET nocount ON  
	
BEGIN
	SET @cSQL = 'SELECT AUDITTRAIL.*'
		+		' ,ASSETS._NAME AS ASSETNAME'
		+		' ,LOCATIONS._FULLNAME AS FULLLOCATIONNAME'

	-- from the AUDITTRAIL table
		+		' FROM AUDITTRAIL'

	-- Join ASSETS to get the Asset Name
		+		' LEFT JOIN ASSETS ON (AUDITTRAIL._ASSETID = ASSETS._ASSETID)'

	--	Join Locations to get the full location name
		+		' LEFT JOIN LOCATIONS ON (ASSETS._LOCATIONID = LOCATIONS._LOCATIONID)'

	-- Add the start of the WHERE clause as we will always be filtering the ATE categories
		+		' WHERE AUDITTRAIL._CLASS'

	-- If we have specified a specific class then add this as a WHERE clause
	IF (@nClass <> -1)
		SET @cSQL = @cSQL + ' = ' + cast(@nClass as varchar(4))
	ELSE
		SET @cSQL = @cSQL + ' >= 100'

	-- Add on the ORDER clause
	SET @cSQL = @cSQL + ' ORDER BY _AUDITTRAILID'

	-- Execute this statement
	EXEC (@cSQL)
END
GO




SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--------------------------------------------------------------------------
--																		--
-- usp_audittrail_getassethistory										--
-- ========================												--
--																		--
--------------------------------------------------------------------------
--																		--
-- Description															--
-- ===========															--
--																		--
-- This stored procedure is part of the AuditWizard Product				--
-- It is used to return a list of audit trail entries which are history	--
-- records for the specified asset										--
--																		--
--------------------------------------------------------------------------
--																		--
-- History																--
-- =======																--
--																		--
--	16-Jan-2009	Chris Drew		Initial Version							--
--																		--
--------------------------------------------------------------------------

CREATE PROCEDURE [dbo].usp_audittrail_getassethistory
	@nAssetID int,
	@dtStartDate DateTime,
	@dtEndDate DateTime

AS 
	SET nocount ON  
	
BEGIN

	DECLARE @cStartDate varchar(20)
	DECLARE @cEndDate varchar(20)
	DECLARE @cSQL varchar(1024)

	-- Convert any start and/or end date specified
	IF (@dtStartDate is not null) and (@dtEndDate is not null)
	BEGIN
		SET @cStartDate = CONVERT(varchar(20), @dtStartDate ,120)
		SET @cEndDate = CONVERT(varchar(20), @dtEndDate ,120)
	END

	-- Build the SQL Query - basic select part first
	SET @cSQL = 'SELECT AUDITTRAIL.*'
		+		' ,ASSETS._NAME AS ASSETNAME'
		+		' ,LOCATIONS._FULLNAME AS FULLLOCATIONNAME'

	-- from the AUDITTRAIL table
		+		' FROM AUDITTRAIL'

	-- Join ASSETS to get the Asset Name
		+		' LEFT JOIN ASSETS ON (AUDITTRAIL._ASSETID = ASSETS._ASSETID)'

	--	Join Locations to get the full location name
		+		' LEFT JOIN LOCATIONS ON (ASSETS._LOCATIONID = LOCATIONS._LOCATIONID)'

	-- Initial WHERE to only include 
		+		' WHERE _CLASS <= 3'

	-- We are only interested in Asset History so can filter out any records which are
	-- not associated with an asset - or if we have specified an asset use that instead
	IF (@nAssetID = 0)
		SET @cSQL = @cSQL + ' AND AUDITTRAIL._ASSETID <> 0'
	ELSE
		SET @cSQL = @cSQL + ' AND AUDITTRAIL._ASSETID = ' + cast(@nAssetID as varchar(16))

	-- Are we filtering by date?
	IF (@dtStartDate is not null) and (@dtEndDate is not null)
	BEGIN
		SET @cSQL = @cSQL + N' AND CONVERT(datetime ,AUDITTRAIL._DATE ,120)'
			+	   ' BETWEEN ''' + @cStartDate + '''  AND ''' + @cEndDate + '''' 
	END

	-- Ordering
	SET @cSQL = @cSQL + ' ORDER BY _AUDITTRAILID'

	-- Execute

	print @cSQL
	EXEC (@cSQL)
END
GO




set ANSI_NULLS ON
set QUOTED_IDENTIFIER ON
GO
--------------------------------------------------------------------------
--																		--
-- [usp_get_lastauditdate]												--
-- ========================												--
--																		--
--------------------------------------------------------------------------
--																		--
-- Description															--
-- ===========															--
--																		--
-- This stored procedure is part of the AuditWizard Product				--
-- It is used to return a list of last audit dates for the specified	--
-- (or all) assets optionally which have (or have not) been audited		--
--	within the last 'n' days											--
--																		--
--------------------------------------------------------------------------
--																		--
-- History																--
-- =======																--
--																		--
--	16-Jan-2009	Chris Drew		Initial Version							--
--																		--
--------------------------------------------------------------------------

CREATE PROCEDURE [dbo].[usp_audittrail_lastauditdate]
	@nAssetID int,
	@nDays	int,
	@bAudited bit

AS 
	SET nocount ON  
	
BEGIN

	DECLARE @cSQL varchar(1024)
	DECLARE @cWHERE varchar(500)
	SET @cWHERE = ''

	-- Build the SQL Query - basic select part first
	SET @cSQL = 'SELECT DISTINCT(d._ASSETID), d._DATE'
		+		' ,ASSETS._NAME AS ASSETNAME' 
		+		' ,LOCATIONS._FULLNAME AS FULLLOCATIONNAME'

	-- from the AUDITTRAIL table
		+		' FROM AUDITTRAIL d'

	-- Now we do the inner join to get the last audit date of each asset
		+		' INNER JOIN'
		+		' (SELECT _ASSETID, MAX([_DATE]) AS Max_Date FROM AUDITTRAIL d GROUP BY _ASSETID) x '
		+		' ON d._ASSETID = x._ASSETID AND d.[_DATE] = x.Max_Date'

	-- Join ASSETS to get the Asset Name
		+		' LEFT JOIN ASSETS ON (d._ASSETID = ASSETS._ASSETID)'

	--	Join Locations to get the full location name
		+		' LEFT JOIN LOCATIONS ON (ASSETS._LOCATIONID = LOCATIONS._LOCATIONID)'


	-- Do we want to filter this for a specific asset?
	IF (@nAssetID <> 0)
		SET @cWHERE = ' WHERE d._ASSETID = ' + cast(@nAssetID as varchar(8))
	ELSE
		SET @cWHERE = ' WHERE d._ASSETID <> 0'

	-- Have we specified a number of days?
	IF (@nDays <> 0)
	BEGIN
		IF (@bAudited = 1)
			SET @cWHERE = @cWHERE + ' AND (datediff(day, d._DATE ,GETDATE()) <= ' + cast(@nDays as varchar(8)) + ')'
		ELSE		
			SET @cWHERE = @cWHERE + ' AND (datediff(day, d._DATE ,GETDATE()) > ' + cast(@nDays as varchar(8)) + ')'
	END

	-- Combine the SQL and WHERE clauses
	SET @cSQL = @cSQL + @cWHERE

	-- Ordering Clause
	SET @cSQL = @cSQL + ' ORDER BY ASSETNAME'

	-- Execute
	EXEC (@cSQL)
END
GO




SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--------------------------------------------------------------------------
--																		--
-- usp_applicationinstance_enumerate									--
-- =================================									--
--																		--
--------------------------------------------------------------------------
--																		--
-- Description															--
-- ===========															--
--																		--
-- This stored procedure is part of the AuditWizard Product				--
-- It is used to return a list of assets on which the specified		--
-- applications has been installed along with details of the install	--
--																		--
--	NOTE: THIS STORED PROCEDURE ONLY DEALS WITH APPLICATIONS			--
--		  OPERATING SYSTEMS ARE DEALT WITH SEPARATELY					--
--																		--
--------------------------------------------------------------------------
--																		--
-- History																--
-- =======																--
--																		--
--	25-Feb-2008	Chris Drew		Initial Version							--
--																		--
--------------------------------------------------------------------------

CREATE PROCEDURE [dbo].[usp_applicationinstance_enumerate]
	@applicationID int
AS     
	SET nocount ON
    
	SELECT APPLICATION_INSTANCES._INSTANCEID ,APPLICATION_INSTANCES._APPLICATIONID ,APPLICATION_INSTANCES._ASSETID ,APPLICATION_INSTANCES._PRODUCTID ,APPLICATION_INSTANCES._CDKEY
		  ,APPLICATIONS._NAME ,APPLICATIONS._VERSION ,APPLICATIONS._PUBLISHER ,APPLICATIONS._GUID ,APPLICATIONS._IGNORED ,APPLICATIONS._ALIASED_TOID ,APPLICATIONS._USER_DEFINED
		  ,ASSETS._NAME AS ASSETNAME
		  ,ASSET_TYPES._ICON AS ASSETICON
		  ,LOCATIONS._FULLNAME AS FULLLOCATIONNAME
		  ,LOCATIONS._NAME AS LOCATIONNAME
		  ,DOMAINS._NAME AS DOMAINNAME

		FROM APPLICATION_INSTANCES 
		LEFT JOIN APPLICATIONS ON (APPLICATION_INSTANCES._APPLICATIONID = APPLICATIONS._APPLICATIONID)
		LEFT JOIN ASSETS ON (APPLICATION_INSTANCES._ASSETID = ASSETS._ASSETID)
		LEFT JOIN ASSET_TYPES ON (ASSETS._ASSETTYPEID=ASSET_TYPES._ASSETTYPEID)
		LEFT JOIN LOCATIONS ON (ASSETS._LOCATIONID = LOCATIONS._LOCATIONID)
		LEFT JOIN DOMAINS ON (ASSETS._DOMAINID = DOMAINS._DOMAINID)
		WHERE APPLICATION_INSTANCES._APPLICATIONID=@applicationID

		-- This second WHERE clause removes any entries where the asset has been flagged as 'Disposed'
		-- eventually this will need to be configurable for the different states
		AND ASSETS._STOCK_STATUS<>3
GO




SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- ================================================================
--
-- Author:		<Chris Drew>
-- Create date: <July 17th 2008>
-- Description:	<Updates the publisher for a specified application>
--
-- =================================================================

CREATE PROCEDURE [dbo].[usp_application_changepublisher]
	@applicationID int,
	@publisher varchar(255)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	UPDATE APPLICATIONS SET _PUBLISHER = @publisher WHERE _APPLICATIONID = @applicationID
END
GO



SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--------------------------------------------------------------------------
--																		--
-- usp_application_statistics											--
-- ==========================											--
--																		--
--------------------------------------------------------------------------
--																		--
-- Description															--
-- ===========															--
--																		--
-- This stored procedure is part of the AuditWizard Product				--
-- It is used to return statistics about application use	 			--
--																		--
--	NOTE: THIS SP ONLY HANDLES APPLICATIONS								--
--																		--
--------------------------------------------------------------------------
--																		--
-- History																--
-- =======																--
--																		--
--	21-Feb-2008	Chris Drew		Initial Version							--
--																		--
--------------------------------------------------------------------------

CREATE PROCEDURE [dbo].[usp_audit_statistics]
AS  
	SET nocount ON  

	SELECT 'row'
		,(SELECT COUNT(_NAME) FROM APPLICATIONS WHERE _IGNORED=0 AND _ISOS=0) uniqueapplications
		,(SELECT COUNT(_NAME) FROM ASSETS WHERE _AGENT_STATUS<>0) deployedagents
		,(SELECT COUNT(_APPLICATIONID) FROM APPLICATION_INSTANCES WHERE _APPLICATIONID IN (SELECT _APPLICATIONID FROM APPLICATIONS WHERE _ISOS=0)) totalapplications
		,(SELECT COUNT(DISTINCT(_PUBLISHER)) FROM APPLICATIONS WHERE _IGNORED=0) publishers
GO



set ANSI_NULLS ON
set QUOTED_IDENTIFIER ON
GO
--------------------------------------------------------------------------
--																		--
-- [usp_statistics_auditdates]											--
-- ===========================											--
--																		--
--------------------------------------------------------------------------
--																		--
-- Description															--
-- ===========															--
--																		--
-- This stored procedure is part of the AuditWizard Product				--
-- It is used to return statistics about when assets were audited		--
--																		--
--------------------------------------------------------------------------
--																		--
-- History																--
-- =======																--
--																		--
--	29-Feb-2008	Chris Drew		Initial Version							--
--																		--
--------------------------------------------------------------------------

CREATE PROCEDURE [dbo].[usp_statistics_auditdates]
AS	
BEGIN

	SELECT 'row'
		,(SELECT COUNT(_ASSETID) FROM ASSETS WHERE (_LASTAUDIT is NULL)) notaudited
		,(SELECT COUNT(_ASSETID) FROM ASSETS WHERE (datediff(day, _LASTAUDIT ,GETDATE()) = 0)) today
		,(SELECT COUNT(_ASSETID) FROM ASSETS WHERE (datediff(day, _LASTAUDIT ,GETDATE()) > 7) AND (datediff(day, _LASTAUDIT ,GETDATE()) <= 14)) notinlast7
		,(SELECT COUNT(_ASSETID) FROM ASSETS WHERE (datediff(day, _LASTAUDIT ,GETDATE()) > 14) AND (datediff(day, _LASTAUDIT ,GETDATE()) <= 30)) notinlast14
		,(SELECT COUNT(_ASSETID) FROM ASSETS WHERE (datediff(day, _LASTAUDIT ,GETDATE()) > 30) AND (datediff(day, _LASTAUDIT ,GETDATE()) <= 30)) notinlast30
		,(SELECT COUNT(_ASSETID) FROM ASSETS WHERE (datediff(day, _LASTAUDIT ,GETDATE()) > 90)) over90days
END





SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--------------------------------------------------------------------------
--																		--
-- usp_application_set_ignored											--
-- ==========================											--
--																		--
--------------------------------------------------------------------------
--																		--
-- Description															--
-- ===========															--
--																		--
-- This stored procedure is part of the AuditWizard Product			--
-- It is used to ignore or not the specified application				--
--																		--
--	NOTE: THIS PROCEDURE CAN HANDLE APPLICATIONS AND OPERATING SYSTEM	--
--	      ENTRIES AS THERE ARE NO TYPE SPECIFIC OPERATIONS				--
--																		--
--------------------------------------------------------------------------
--																		--
-- History																--
-- =======																--
--																		--
--	18-Feb-2006	Chris Drew		Initial Version							--
--																		--
--------------------------------------------------------------------------

CREATE PROCEDURE [dbo].[usp_application_set_ignored]
	@nApplicationID int,
	@bIgnore bit
AS     
	SET nocount ON

BEGIN
	UPDATE APPLICATIONS SET _IGNORED=@bIgnore WHERE _APPLICATIONID = @nApplicationID	
END
GO




SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--------------------------------------------------------------------------
--																		--
-- usp_action_enumerate													--
-- ===================													--
--																		--
--------------------------------------------------------------------------
--																		--
-- Description															--
-- ===========															--
--																		--
-- This stored procedure is part of the AuditWizard Product				--
-- It is used to return a list of action entries						--
--																		--
--------------------------------------------------------------------------
--																		--
-- History																--
-- =======																--
--																		--
--	12-Aug-2008	Chris Drew		Initial Version							--
--																		--
--------------------------------------------------------------------------

CREATE PROCEDURE [dbo].[usp_action_enumerate]
AS 
	DECLARE @cSQL varchar(1024) 
	SET nocount ON  
	
BEGIN
	SELECT ACTIONS.[_ACTIONID]
		 , ACTIONS.[_TYPE] 
		 , ACTIONS.[_APPLICATIONID] 
		 , ACTIONS.[_ASSETS] 
		 , ACTIONS.[_STATUS] 
		 , ACTIONS.[_NOTES]
		 , APPLICATIONS.[_NAME]
	FROM dbo.ACTIONS
	LEFT JOIN dbo.APPLICATIONS ON (ACTIONS._APPLICATIONID = dbo.APPLICATIONS.[_APPLICATIONID])
	ORDER BY _ACTIONID

END
GO




SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--------------------------------------------------------------------------
--																		--
-- usp_action_delete													--
-- ==================													--
--																		--
--------------------------------------------------------------------------
--																		--
-- Description															--
-- ===========															--
--																		--
-- This stored procedure is part of the AuditWizard Product			--
-- It is used to add a DELETE an ACTION  definititon 					--
--																		--
--------------------------------------------------------------------------
--																		--
-- History																--
-- =======																--
--																		--
--	18-Feb-2006	Chris Drew		Initial Version							--
--																		--
--------------------------------------------------------------------------

CREATE PROCEDURE [dbo].[usp_action_delete]
	@nActionID int
AS     
	SET nocount ON
	DELETE FROM ACTIONS WHERE _ACTIONID=@nActionID
GO



SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--------------------------------------------------------------------------
--																		--
-- usp_applicationinstance_getcount										--
-- ================================										--
--																		--
--------------------------------------------------------------------------
--																		--
-- Description															--
-- ===========															--
--																		--
-- This stored procedure is part of the AuditWizard Product				--
-- It's used to return a count of application instances in the database	--
--																		--
--																		--
--	NOTE: THIS STORED PROCEDURE ONLY DEALS WITH APPLICATIONS			--
--		  OPERATING SYSTEMS ARE DEALT WITH SEPARATELY					--
--																		--
--																		--
--------------------------------------------------------------------------
--																		--
-- History																--
-- =======																--
--																		--
--	18-Feb-2006	Chris Drew		Initial Version							--
--																		--
--------------------------------------------------------------------------

CREATE PROCEDURE [dbo].[usp_applicationinstance_getcount]
AS     
	DECLARE @nReturnCount int
	SET nocount ON
	SET @nReturnCount = (SELECT count(*) FROM APPLICATION_INSTANCES 
						 LEFT JOIN APPLICATIONS ON (APPLICATIONS._APPLICATIONID = APPLICATION_INSTANCES._APPLICATIONID) 
						 WHERE APPLICATIONS._IGNORED=1 
							AND APPLICATIONS._ISOS=0)
	RETURN @nReturnCount
GO




SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--------------------------------------------------------------------------
--																		--
-- usp_add_application													--
-- ===================													--
--																		--
--------------------------------------------------------------------------
--																		--
-- Description															--
-- ===========															--
--																		--
-- This stored procedure is part of the AuditWizard Product				--
-- It is used to add a new entry to the APPLICATIONS table within the   --
-- database. 															--
--																		--
-- If an existing entry is found then its ID is returned otherwise we   --
-- return the ID of the newly created record. 							--
--																		--
--																		--
--	NOTE: THIS STORED PROCEDURE ONLY DEALS WITH APPLICATIONS			--
--		  OPERATING SYSTEMS ARE DEALT WITH SEPARATELY					--
--																		--
--------------------------------------------------------------------------
--																		--
-- History																--
-- =======																--
--																		--
--	18-Feb-2006	Chris Drew		Initial Version							--
--																		--
--------------------------------------------------------------------------

CREATE PROCEDURE [dbo].[usp_add_application]
	@cPublisher varchar(255),
	@cApplication varchar(255),
	@cVersion varchar(255),
	@cGuid varchar(255),
	@nAliasedToID int,
	@bUserDefined bit,
	@nReturnId int output
AS

DECLARE @nRetCode int

SET nocount ON
SET @nRetCode = 0

--
-- first check the database for an entry that matches the supplied application name
--
SET @nReturnId = isnull((SELECT min(_APPLICATIONID) FROM applications WHERE _name = @cApplication) ,0)

-- did we find a match ?
IF (@nReturnId = 0)
BEGIN

	INSERT INTO APPLICATIONS
		(_NAME ,_VERSION ,_PUBLISHER ,_GUID ,_ISOS ,_ALIASED_TOID ,_USER_DEFINED)
	VALUES
		(@cApplication, @cVersion, @cPublisher ,@cGuid ,0 ,@nAliasedToID ,@bUserDefined)

	SET @nReturnID = @@IDENTITY
END

RETURN @nRetCode
GO



SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- ================================================================
--
-- Author:		<Chris Drew>
-- Create date: <April 13th 2009>
-- Description:	<Updates the alias for a specified application>
--
-- =================================================================

CREATE PROCEDURE [dbo].[usp_application_alias]
	@applicationID int,			-- This is the application which has been aliased
	@aliasID int				-- ID Of the application to alias the above TO 
								-- o indicates that aliasing of the above application should be removed
AS
BEGIN
	DECLARE @nAliasedApplicationID int
	DECLARE @nAliasCount int

	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	-- First of all are we setting or clearing an alias?
	IF (@aliasID = 0)
	BEGIN

		-- Clearing an alias - there are x stages in this process
		-- 1> Find the ID of the application to which this is currently aliased
		-- 2> Update any APPLICATION_INSTANCES which point to the aliased application and which 
		--    have a saved _BASE_APLICATIONID to point to the _BASE_APPLICATIONID rather than the alias
		-- 3> Clear the alias flag for the application
		SET @nAliasedApplicationID = isnull((SELECT TOP 1 _ALIASED_TOID FROM APPLICATIONS 
											  WHERE _APPLICATIONID=@applicationID) ,0)

		-- If we found the aliased application we can continue
		IF (@nAliasedApplicationID <> 0)
		BEGIN
			-- Update any application instances which were aliased to the above rcovered application
			-- so that they point back to the original application
			UPDATE APPLICATION_INSTANCES SET _APPLICATIONID=_BASE_APPLICATIONID 
				WHERE _APPLICATIONID=@nAliasedApplicationID AND _BASE_APPLICATIONID<>0

			-- Clear the ALIASED_TOID field for the application which we have just un-aliased
			UPDATE APPLICATIONS SET _ALIASED_TOID=0 WHERE _APPLICATIONID=@applicationID
		END	
	END
	
	ELSE

	BEGIN
			-- Setting an alias = there are n stages in this process
			-- 1> Ensure that the application being aliased is not itself a target of an alias
			-- 2> Update the applications record to show that this application has been aliased
			-- 3> Update any application_instance records which currently point to this record 
			--    to point to the aliased application, saving the old application id so that we can
			--    unalias later
			EXEC @nAliasCount = usp_application_alias_count @applicationID
			IF (@nAliasCount = 0)
			BEGIN
				UPDATE APPLICATIONS SET _ALIASED_TOID=@aliasID WHERE _APPLICATIONID=@applicationID
		
				-- Update existing references
				UPDATE APPLICATION_INSTANCES SET _BASE_APPLICATIONID = _APPLICATIONID, _APPLICATIONID=@aliasID
					WHERE _APPLICATIONID=@applicationID
			END
	END

END
GO




SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--------------------------------------------------------------------------
--																		--
-- usp_check_support_alerts												--
-- ========================												--
--																		--
--------------------------------------------------------------------------
--																		--
-- Description															--
-- ===========															--
--																		--
-- This stored procedure is part of the AuditWizard Product				--
-- It is used to check for any support alerts having been triggered		--
--																		--
--------------------------------------------------------------------------
--																		--
-- History																--
-- =======																--
--																		--
--	28-Jul-2008	Chris Drew		Initial Version							--
--																		--
--------------------------------------------------------------------------

CREATE PROCEDURE [dbo].[usp_check_support_alerts]
AS

DECLARE @dtToday DateTime

SET nocount ON

BEGIN

	SET @dtToday = GETDATE();
	
	SELECT LICENSES._SUPPORT_EXPIRES, LICENSES._SUPPORT_ALERTDAYS, LICENSES._SUPPORT_ALERTBYEMAIL, LICENSES._SUPPORT_ALERTRECIPIENTS
		  ,APPLICATIONS._NAME 
	FROM LICENSES 
	LEFT JOIN APPLICATIONS ON (LICENSES._APPLICATIONID = APPLICATIONS._APPLICATIONID)
		WHERE _SUPPORTED=1 
		AND _SUPPORT_ALERTDAYS <> -1 
		AND ((_SUPPORT_EXPIRES - _SUPPORT_ALERTDAYS) <= @dtToday)

END
GO




SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--------------------------------------------------------------------------
--																		--
-- usp_statistics_dl													--
-- =================													--
--																		--
--------------------------------------------------------------------------
--																		--
-- Description															--
-- ===========															--
--																		--
-- This stored procedure is part of the AuditWizard Product				--
-- It is used to return statistics about the Declare License Phase		--
--																		--
--------------------------------------------------------------------------
--																		--
-- History																--
-- =======																--
--																		--
--	08-Aug-2008	Chris Drew		Initial Version							--
--																		--
--------------------------------------------------------------------------

CREATE PROCEDURE [dbo].[usp_statistics_dl]
AS  
	SET nocount ON  

	SELECT 'row'
		,(SELECT COUNT(_NAME) FROM APPLICATIONS WHERE _ISOS=0 )	uniqueapplications
	    ,(SELECT COUNT(_APPLICATIONID) FROM APPLICATION_INSTANCES WHERE _APPLICATIONID IN (SELECT _APPLICATIONID FROM APPLICATIONS WHERE _IGNORED=0 AND _ISOS=0)) includedapplicationinstances
		,(SELECT COUNT(_LICENSEID) FROM LICENSES) licensesdeclared
		,(SELECT SUM(_COUNT) FROM LICENSES WHERE _LICENSETYPEID in (SELECT _LICENSETYPEID FROM LICENSE_TYPES WHERE _COUNTED=1)) licenseinstancecount
GO




SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--------------------------------------------------------------------------
--																		--
-- usp_statistics_topos													--
-- ====================													--
--																		--
--------------------------------------------------------------------------
--																		--
-- Description															--
-- ===========															--
--																		--
-- This stored procedure is part of the AuditWizard Product				--
-- It is used to return statistics about the most commonly used OS's	--
--																		--
--------------------------------------------------------------------------
--																		--
-- History																--
-- =======																--
--																		--
--	29-Feb-2008	Chris Drew		Initial Version							--
--																		--
--------------------------------------------------------------------------

CREATE PROCEDURE [dbo].[usp_statistics_topos]
						@resultCount int
AS	
BEGIN
    SET ROWCOUNT @RESULTCOUNT 
	SELECT APPLICATIONS._NAME, 
		(SELECT COUNT(*) 
			FROM APPLICATION_INSTANCES 
			WHERE APPLICATION_INSTANCES._APPLICATIONID = APPLICATIONS._APPLICATIONID AND APPLICATIONS._ISOS=1)  
	FROM APPLICATIONS 
	ORDER BY 
		(SELECT COUNT(*) 
			FROM APPLICATION_INSTANCES 
			WHERE APPLICATION_INSTANCES._APPLICATIONID = APPLICATIONS._APPLICATIONID AND APPLICATIONS._ISOS=1) DESC
    SET ROWCOUNT 0 
END
GO




SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--------------------------------------------------------------------------
--																		--
-- usp_applicationlicense_enumerate										--
-- =================================									--
--																		--
--------------------------------------------------------------------------
--																		--
-- Description															--
-- ===========															--
--																		--
-- This stored procedure is part of the AuditWizard Product				--
-- It is used to return a list of licenses declared for the specified	--
-- applications 														--
--																		--
--	NOTE: THIS PROCEDURE CAN HANDLE APPLICATIONS AND OPERATING SYSTEM	--
--	      ENTRIES AS THERE ARE NO TYPE SPECIFIC OPERATIONS				--
--																		--
--------------------------------------------------------------------------
--																		--
-- History																--
-- =======																--
--																		--
--	25-Feb-2008	Chris Drew		Initial Version							--
--																		--
--------------------------------------------------------------------------

CREATE PROCEDURE [dbo].[usp_applicationlicense_enumerate]
	@applicationID int
AS     
	SET nocount ON
	SELECT LICENSES._LICENSEID, LICENSES._LICENSETYPEID ,LICENSES._COUNT
			,LICENSES._APPLICATIONID ,LICENSES._SUPPORTED, LICENSES._SUPPORT_EXPIRES ,LICENSES._SUPPORT_ALERTDAYS
			,LICENSES._SUPPORT_ALERTBYEMAIL ,LICENSES._SUPPORT_ALERTRECIPIENTS
			,LICENSES._SUPPLIERID
			,LICENSE_TYPES._NAME AS LICENSE_TYPES_NAME ,LICENSE_TYPES._COUNTED
			,APPLICATIONS._NAME AS APPLICATION_NAME
			,ISNULL(SUPPLIERS._NAME, '') SUPPLIER_NAME
	FROM LICENSES
		LEFT JOIN LICENSE_TYPES ON (LICENSES._LICENSETYPEID = LICENSE_TYPES._LICENSETYPEID)
		LEFT JOIN APPLICATIONS ON (LICENSES._APPLICATIONID = APPLICATIONS._APPLICATIONID)
		LEFT JOIN SUPPLIERS ON (LICENSES._SUPPLIERID = SUPPLIERS._SUPPLIERID)
	WHERE LICENSES._APPLICATIONID=@applicationID
GO
 
 
 
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--------------------------------------------------------------------------
--																		--
-- usp_supportcontract_enumerate										--
-- =============================										--
--																		--
--------------------------------------------------------------------------
--																		--
-- Description															--
-- ===========															--
--																		--
-- This stored procedure is part of the AuditWizard Product			--
-- It is used to return a list of all support contracts defined			--
--																		--
--------------------------------------------------------------------------
--																		--
-- History																--
-- =======																--
--																		--
--	11-Sep-2008	Chris Drew		Initial Version							--
--																		--
--------------------------------------------------------------------------

CREATE PROCEDURE [dbo].usp_supportcontract_enumerate
AS

SET nocount ON
BEGIN
	SELECT LICENSES._SUPPORT_EXPIRES,LICENSES._SUPPORT_ALERTDAYS ,LICENSES._SUPPORT_ALERTBYEMAIL
		,APPLICATIONS._NAME
	FROM LICENSES
	LEFT JOIN APPLICATIONS ON (LICENSES._APPLICATIONID = APPLICATIONS._APPLICATIONID)
	WHERE LICENSES._SUPPORTED = 1
END
GO




SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--------------------------------------------------------------------------
--																		--
-- usp_application_statistics											--
-- ==========================											--
--																		--
--------------------------------------------------------------------------
--																		--
-- Description															--
-- ===========															--
--																		--
-- This stored procedure is part of the AuditWizard Product				--
-- It is used to return statistics about OS use	 						--
--																		--
--	NOTE: THIS SP ONLY HANDLES OPERATING SYSTEMS						--
--																		--
--------------------------------------------------------------------------
--																		--
-- History																--
-- =======																--
--																		--
--	21-Feb-2008	Chris Drew		Initial Version							--
--																		--
--------------------------------------------------------------------------

CREATE PROCEDURE [dbo].[usp_os_statistics]
AS  
	SET nocount ON  

	SELECT 'row'
		,(SELECT COUNT(_NAME) FROM APPLICATIONS WHERE _ISOS=1) uniqueos
		,(SELECT COUNT(_APPLICATIONID) FROM APPLICATION_INSTANCES WHERE _APPLICATIONID NOT IN (SELECT _APPLICATIONID FROM APPLICATIONS WHERE _ISOS=1)) totalos
GO




SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--------------------------------------------------------------------------
--																		--
-- usp_add_os															--
-- ==========															--
--																		--
--------------------------------------------------------------------------
--																		--
-- Description															--
-- ===========															--
--																		--
-- This stored procedure is part of the AuditWizard Product				--
-- It is used to add a new entry to the APPLICATIONS table within the   --
-- database for an Operating System. 									--
--																		--
-- If an existing entry is found then its ID is returned otherwise we   --
-- return the ID of the newly created record.							--
--																		--
--	NOTE: THIS STORED PROCEDURE ONLY DEALS WITH OPERATING SYSTEMS,		--
--        APPLICATIONS ARE DEALT WITH SEPARATELY						--
--																		--
--------------------------------------------------------------------------
--																		--
-- History																--
-- =======																--
--																		--
--	18-Feb-2006	Chris Drew		Initial Version							--
--																		--
--------------------------------------------------------------------------

CREATE PROCEDURE [dbo].[usp_add_os]
	@cName	varchar(255),
	@cVersion varchar(255),
	@nReturnId int output
AS

DECLARE @nRetCode int,
	@cMessage varchar(255),
	@cQuery varchar(5000),
	@cColumns varchar(1000),
	@cValues varchar(1000),
	@cSqlName varchar(255),
	@cSqlVersion varchar(255)

SET nocount ON
SET @nRetCode = 0

--
-- first check the database for an entry that matches the supplied application name
--
SET @nReturnId = isnull((SELECT min(_APPLICATIONID) FROM APPLICATIONS WHERE _NAME = @cName AND _VERSION=@cVersion AND _ISOS=1) ,0)

-- did we find a match ?
IF (@nReturnId = 0)
BEGIN
	-- Convert input strings for the insert
	EXEC usp_preparestring @cName, @cSqlName output
	EXEC usp_preparestring @cVersion, @cSqlVersion output

	-- OK The Operating System does not exist at this time so we need to add it
	-- Construct the Column List String
	SET @cColumns = '_NAME ,_VERSION ,_PUBLISHER ,_GUID ,_ISOS'

	-- Construct the Values List String
	SET @cValues = @cSqlName + ',' + @cSqlVersion + ',''Microsoft Corporation, Inc.'','''',1'
	EXEC @nRetCode = usp_IdentityInsert 'APPLICATIONS' ,@cColumns ,@cValues ,@nReturnId output 
END

RETURN @nRetCode
GO




SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--------------------------------------------------------------------------
--																		--
-- usp_application_deleteorphans										--
-- =============================										--
--																		--
--------------------------------------------------------------------------
--																		--
-- Description															--
-- ===========															--
--																		--
-- This stored procedure is part of the AuditWizard Product				--
-- It is used to delete all orphaned application records - records are  -- 
-- orphaned if no references are found in either APPLICATION_INSTANCES 	--
-- or LICENSES tables													--
--																		--
--------------------------------------------------------------------------
--																		--
-- History																--
-- =======																--
--																		--
--	21-Feb-2008	Chris Drew		Initial Version							--
--																		--
--------------------------------------------------------------------------

CREATE PROCEDURE [dbo].[usp_application_deleteorphans]
AS  
	SET nocount ON
	
	-- Referential Integrity checks - APPLICATIONS is linked to the 
	-- ACTIONS, APPLICATION_INSTANCES and LICENSES table.  We need to
	-- check ALL of these tables to ensure that no refrences exist and that the
	-- application is not aliased as we do not want to lose the definition 	
	DELETE FROM APPLICATIONS 
		WHERE _APPLICATIONID NOT IN (SELECT _APPLICATIONID FROM APPLICATION_INSTANCES) 
		  AND _APPLICATIONID NOT IN (SELECT _APPLICATIONID FROM LICENSES)
		  AND _APPLICATIONID NOT IN (SELECT _APPLICATIONID FROM ACTIONS)
		  AND _ALIASED_TOID = 0
GO




SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--------------------------------------------------------------------------
--																		--
-- usp_osinstance_getcount												--
-- =======================												--
--																		--
--------------------------------------------------------------------------
--																		--
-- Description															--
-- ===========															--
--																		--
-- This stored procedure is part of the AuditWizard Product				--
-- It's used to return a count of OS instances in the database			--
--																		--
-- NOTE: THIS SP HANDLES OS ENTRIES ONLY								--
--																		--
--------------------------------------------------------------------------
--																		--
-- History																--
-- =======																--
--																		--
--	18-Feb-2006	Chris Drew		Initial Version							--
--																		--
--------------------------------------------------------------------------

CREATE PROCEDURE [dbo].[usp_osinstance_getcount]
AS     
	DECLARE @nReturnCount int
	SET nocount ON
	SET @nReturnCount = (SELECT count(*) FROM APPLICATION_INSTANCES 
						 LEFT JOIN APPLICATIONS 
							ON (APPLICATIONS._APPLICATIONID = APPLICATION_INSTANCES._APPLICATIONID) 
						 WHERE APPLICATIONS._ISOS=1 AND APPLICATIONS._IGNORED=0)
	RETURN @nReturnCount
GO




SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--------------------------------------------------------------------------
--																		--
-- usp_supplier_update													--
-- ==================													--
--																		--
--------------------------------------------------------------------------
--																		--
-- Description															--
-- ===========															--
--																		--
-- This stored procedure is part of the AuditWizard Product				--
-- It is used to add a update a Supplier definititon 					--
--																		--
--------------------------------------------------------------------------
--																		--
-- History																--
-- =======																--
--																		--
--	31-Jul-2008	Chris Drew		Initial Version							--
--																		--
--------------------------------------------------------------------------

CREATE PROCEDURE [dbo].[usp_supplier_update]
	@nSupplierID int,
	@cName		varchar(255),
	@cAddress1	varchar(255),
	@cAddress2	varchar(255),
	@cCity		varchar(255),
	@cState		varchar(255),
	@cZip		varchar(255),
	@cTelephone	varchar(255),
	@cContactName	varchar(255),
	@cContactEmail	varchar(255),
	@cWWW		varchar(255),
	@cFax		varchar(255),
	@cNotes		varchar(1020)
	
AS     
	DECLARE @cSQL as varchar(255)

	SET nocount ON 
BEGIN
	UPDATE SUPPLIERS SET
		_NAME=@cName 
		,_ADDRESS1=@cAddress1
		,_ADDRESS2=@cAddress2
		,_CITY=@cCity
		,_STATE=@cState
		,_ZIP=@cZip
		,_TELEPHONE=@cTelephone
		,_CONTACT_NAME=@cContactName
		,_CONTACT_EMAIL=@cContactEmail
		,_WWW=@cWWW
		,_FAX=@cFax
		,_NOTES=@cNotes
		WHERE _SUPPLIERID=@nSupplierID
END
GO




SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--------------------------------------------------------------------------
--																		--
-- usp_supplier_find													--
-- =================													--
--																		--
--------------------------------------------------------------------------
--																		--
-- Description															--
-- ===========															--
--																		--
-- This stored procedure is part of the AuditWizard Product				--
-- It is used to add a locate a Supplier within the database		   	--
--																		--
-- If an existing entry is found then its ID is returned otherwise we   --
-- return 0.															--
--																		--
--------------------------------------------------------------------------
--																		--
-- History																--
-- =======																--
--																		--
--	4-Aug-2008	Chris Drew		Initial Version							--
--																		--
--------------------------------------------------------------------------

CREATE PROCEDURE [dbo].[usp_supplier_find]
	@cName varchar(255)
AS
	DECLARE @nReturnID int

	SET nocount ON
	SET @nReturnID = isnull((SELECT _SUPPLIERID FROM SUPPLIERS WHERE _name=@cName),0)
	RETURN @nReturnID
GO




SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--------------------------------------------------------------------------
--																		--
-- usp_supplier_enumerate												--
-- ======================												--
--																		--
--------------------------------------------------------------------------
--																		--
-- Description															--
-- ===========															--
--																		--
-- This stored procedure is part of the AuditWizard Product				--
-- It is used to return a list of Supplier entries						--
--																		--
--------------------------------------------------------------------------
--																		--
-- History																--
-- =======																--
--																		--
--	31-Jul-2008	Chris Drew		Initial Version							--
--																		--
--------------------------------------------------------------------------

CREATE PROCEDURE [dbo].[usp_supplier_enumerate]
AS 
	DECLARE @cSQL varchar(1024) 
	SET nocount ON  
	
BEGIN
	SELECT _SUPPLIERID, _NAME ,_ADDRESS1 ,_ADDRESS2 ,_CITY, _STATE ,_ZIP ,_TELEPHONE 
			,_CONTACT_NAME ,_CONTACT_EMAIL, _WWW ,_FAX ,_NOTES
	FROM SUPPLIERS
	ORDER BY _SUPPLIERID

END
GO




SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--------------------------------------------------------------------------
--																		--
-- usp_supplier_delete													--
-- ===================													--
--																		--
--------------------------------------------------------------------------
--																		--
-- Description															--
-- ===========															--
--																		--
-- This stored procedure is part of the AuditWizard Product				--
-- It is used to add a DELETE a Supplier definititon 					--
--																		--
-- Note that it is the responsibility of the caller to handle any		--																		--
-- tables which may depend on this record								--
--																		--
--------------------------------------------------------------------------
--																		--
-- History																--
-- =======																--
--																		--
--	31-Jul-2008	Chris Drew		Initial Version							--
--																		--
--------------------------------------------------------------------------

CREATE PROCEDURE [dbo].[usp_supplier_delete]
	@nSupplierID int
AS     
	DECLARE @cSQL as varchar(255)
	SET nocount ON

	-- Referential Integrity Checks
	-- ============================
	--
	-- The SUPPLIERS table may be linked to from the LICENSES table - we set
	-- the supplier ID to 1 (No Supplier) if we find any which are currently linked
	-- to the supplier that we are deleting
	UPDATE LICENSES SET _SUPPLIERID=1 WHERE _SUPPLIERID=@nSupplierID

	-- Now delete the SUPPLIER
	DELETE FROM SUPPLIERS WHERE _SUPPLIERID=@nSupplierID

GO




SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--------------------------------------------------------------------------
--																		--
-- usp_statistics_da													--
-- =================													--
--																		--
--------------------------------------------------------------------------
--																		--
-- Description															--
-- ===========															--
--																		--
-- This stored procedure is part of the AuditWizard Product				--
-- It is used to return statistics about the 'Discover and Audit phase	--
--																		--
--------------------------------------------------------------------------
--																		--
-- History																--
-- =======																--
--																		--
--	08-Aug-2008	Chris Drew		Initial Version							--
--																		--
--------------------------------------------------------------------------

CREATE PROCEDURE [dbo].[usp_statistics_da]
AS  
	SET nocount ON  

SELECT 'row'
	,(SELECT COUNT(*) FROM ASSETS) discovered
	,(SELECT COUNT(*) FROM ASSETS WHERE _HIDDEN=1) hidden
	,(SELECT COUNT(*) FROM ASSETS where  _LASTAUDIT IS NOT NULL) audited 
	,(SELECT MAX(_LASTAUDIT) FROM ASSETS WHERE _HIDDEN=0) mostrecentaudit  
	,(SELECT COUNT(*) FROM ASSETS WHERE _LASTAUDIT IS NULL) notaudited
GO




set ANSI_NULLS ON
set QUOTED_IDENTIFIER ON
GO
--------------------------------------------------------------------------
--																		--
-- usp_statistics_assets												--
-- =====================												--
--																		--
--------------------------------------------------------------------------
--																		--
-- Description															--
-- ===========															--
--																		--
-- This stored procedure is part of the AuditWizard Product				--
-- It is used to return statistics about assets and their states		--
--																		--
--------------------------------------------------------------------------
--																		--
-- History																--
-- =======																--
--																		--
--	08-Aug-2008	Chris Drew		Initial Version							--
--																		--
--------------------------------------------------------------------------

CREATE PROCEDURE [dbo].[usp_statistics_assets]
AS  
	SET nocount ON  

SELECT 'row'
	,(SELECT COUNT(*) FROM ASSETS) discovered
	,(SELECT COUNT(*) FROM ASSETS where  _LASTAUDIT IS NOT NULL) audited 
	,(SELECT COUNT(*) FROM ASSETS WHERE _LASTAUDIT IS NULL) notaudited
	,(SELECT COUNT(*) FROM ASSETS WHERE _STOCK_STATUS = 0) stock
	,(SELECT COUNT(*) FROM ASSETS WHERE _STOCK_STATUS = 1) inuse
	,(SELECT COUNT(*) FROM ASSETS WHERE _STOCK_STATUS = 2) pending
	,(SELECT COUNT(*) FROM ASSETS WHERE _STOCK_STATUS = 3) disposed
	,(SELECT MAX(_LASTAUDIT) FROM ASSETS WHERE _HIDDEN=0) mostrecentaudit  
GO




set ANSI_NULLS ON
set QUOTED_IDENTIFIER ON
GO
--------------------------------------------------------------------------
--																		--
-- usp_statistics_asset_states											--
-- ===========================											--
--																		--
--------------------------------------------------------------------------
--																		--
-- Description															--
-- ===========															--
--																		--
-- This stored procedure is part of the AuditWizard Product				--
-- It is used to return statistics about asset states					--
--																		--
--------------------------------------------------------------------------
--																		--
-- History																--
-- =======																--
--																		--
--	08-Aug-2008	Chris Drew		Initial Version							--
--																		--
--------------------------------------------------------------------------

CREATE PROCEDURE [dbo].[usp_statistics_asset_states]
AS  
	SET nocount ON  

	SELECT COUNT(*) 
		,CASE _STOCK_STATUS
			WHEN 0 THEN 'In Stock'
			WHEN 1 THEN 'In Use'
			WHEN 2 THEN 'Pending Disposal'
			WHEN 3 THEN 'Disposed'
		END
	FROM ASSETS 
	GROUP BY ASSETS._STOCK_STATUS
GO


SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--------------------------------------------------------------------------
--																		--
-- usp_applicationinstance_update										--
-- ==============================										--
--																		--
--------------------------------------------------------------------------
--																		--
-- Description															--
-- ===========															--
--																		--
-- This stored procedure is part of the AuditWizard Product				--
-- It is used to add a update an application instance in the database	--
--																		--
--	All character strings passed to this function must be SQL formatted --
--																		--
--------------------------------------------------------------------------
--																		--
-- History																--
-- =======																--
--																		--
--	28-Jul-2008	Chris Drew		Initial Version							--
--																		--
--------------------------------------------------------------------------

CREATE PROCEDURE [dbo].[usp_applicationinstance_update]
	@nInstanceID		int,
	@cProductID			varchar(255),
	@cCDKey				varchar(255)
	
AS     
	DECLARE @cSQL as varchar(255)
	SET nocount ON

	-- Format the first part of the command
	SET @cSQL = 'UPDATE APPLICATION_INSTANCES SET '
				+ '_PRODUCTID = ' + @cProductID 
				+ ',_CDKEY = ' + @cCDKey 
			    + ' WHERE _INSTANCEID = ' + CAST(@nInstanceID AS VARCHAR(16))
	EXEC (@cSQL)
GO




SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--------------------------------------------------------------------------
--																		--          	
-- usp_add_application_instance											--
-- ============================											--
--																		--
--------------------------------------------------------------------------
--																		--
-- Description															--
-- ===========															--
--																		--
-- This stored procedure is part of the AuditWizard Product				--
-- It is used to add a new entry to the APPLICATION_INSTANCES table 	--
-- within the database.													--
--																		--
-- An Application Instance relates an entry in the APPLICATIONS table	--
-- to the ASSETS table and identifies an installed instance of the 	--
-- application on the asset											--
--																		--
--	NOTE: THIS PROCEDURE CAN HANDLE APPLICATIONS AND OPERATING SYSTEM	--
--	      ENTRIES AS THERE ARE NO TYPE SPECIFIC OPERATIONS				--
--																		--
--------------------------------------------------------------------------
--																		--
-- History																--
-- =======																--
--																		--
--	18-Feb-2008		Chris Drew		Initial Version						--
--																		--
--------------------------------------------------------------------------

CREATE PROCEDURE [dbo].[usp_add_application_instance]
	@nAssetID int,
	@nApplicationID int,
	@cProductID varchar(255),
	@cCDKey varchar(255),
	@nReturnID int output
AS

DECLARE @nRetCode int,
	@cMessage varchar(255),
	@cQuery varchar(5000),
	@cColumns varchar(1000),
	@cValues varchar(1000),
	@cSqlProductID varchar(255),
	@cSqlCDKey varchar(255),
	@cBuffer varchar(255)

	SET nocount ON
	set @nRetCode = 0

	-- Prepare the Serial Number and install date
	EXEC usp_preparestring @cProductID, @cSqlProductID output
	EXEC usp_preparestring @cCDKey, @cSqlCDKey output

	-- try and find an existing matching record
	SET @nReturnID = isnull((SELECT _INSTANCEID FROM APPLICATION_INSTANCES WHERE _ASSETID = @nAssetID AND _APPLICATIONID = @nApplicationID),0)

	-- did we find a match ?
	IF (@nReturnID = 0)
	BEGIN
		-- NO, insert a record, sort out column names and values
		SET @cColumns = '_APPLICATIONID ,_ASSETID ,_PRODUCTID ,_CDKEY'
		SET @cValues = cast(@nApplicationID as varchar(32)) + ',' + cast(@nAssetID as varchar(32)) + ',' + @cSqlProductID + ',' + @cSqlCDKey
		EXEC @nRetCode = usp_IdentityInsert 'APPLICATION_INSTANCES',@cColumns ,@cValues ,@nReturnID output 
		--
		RETURN @nRetCode
	END

	-- Match was found so we simply update the Product ID and CDKEY
	SET @cQuery = 'UPDATE application_instances SET '

	-- store Product ID (if we have one)
	IF (@cSqlProductID <> '')
	BEGIN
		SET @cQuery = @cQuery + '_PRODUCTID = ' + @cSqlProductID
	END

	-- store CD KEy (if we have one)
	IF (@cSqlCDKey <> '')
	BEGIN
		IF (@cSqlProductID <> '')
		BEGIN
			SET @cQuery = @cQuery + ','
		END
		SET @cQuery = @cQuery + '_cdkey = ' + @cSqlCDKey
	END

	-- and add the WHERE clause to limit to just this instance
	SET @cQuery = @cQuery + 'WHERE _INSTANCEID = ' + cast(@nReturnID as varchar(32))
	EXEC(@cQuery)

RETURN  @nRetCode
GO




SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--------------------------------------------------------------------------
--																		--
-- usp_applicationinstance_delete										--
-- ==============================										--
--																		--
--------------------------------------------------------------------------
--																		--
-- Description															--
-- ===========															--
--																		--
-- This stored procedure is part of the AuditWizard Product				--
-- It is used to return a delete all application instance records for   -- 
-- the specified asset - this may leave orphaned application and    	--
-- these will need to be handled separately								--
--																		--
--------------------------------------------------------------------------
--																		--
-- History																--
-- =======																--
--																		--
--	21-Feb-2008	Chris Drew		Initial Version							--
--																		--
--------------------------------------------------------------------------

CREATE PROCEDURE [dbo].[usp_applicationinstance_delete]
	@forAssetID int
AS  
	SET nocount ON  	
	DELETE FROM APPLICATION_INSTANCES WHERE _ASSETID=@forAssetID
GO




SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--------------------------------------------------------------------------
--																		--
-- usp_asset_statistics												--
-- ==========================											--
--																		--
--------------------------------------------------------------------------
--																		--
-- Description															--
-- ===========															--
--																		--
-- This stored procedure is part of the AuditWizard Product				--
-- It is used to return statistics about assets use	 				--
--																		--
--------------------------------------------------------------------------
--																		--
-- History																--
-- =======																--
--																		--
--	21-Feb-2008	Chris Drew		Initial Version							--
--																		--
--------------------------------------------------------------------------

CREATE PROCEDURE [dbo].[usp_asset_statistics]
	@oldAuditDays int
AS  
DECLARE @oldAuditDate datetime
	SET nocount ON  

	-- Ensure that we create a date in the past for the old audit marker
	IF (@oldAuditDays > 0)
		SET @oldAuditDays = -@oldAuditDays 
	SET @oldAuditDate = DATEADD(day, @oldAuditDays, GETDATE())

SELECT 'row'
	,(SELECT COUNT(*) FROM ASSETS) discovered
	,(SELECT COUNT(*) FROM ASSETS WHERE _HIDDEN=1) hidden
	,(SELECT count(distinct(_ASSETID)) FROM APPLICATION_INSTANCES) audited
	,(SELECT MAX(_LASTAUDIT) FROM ASSETS WHERE _HIDDEN=0) mostrecentaudit  
	,(SELECT COUNT(*) FROM ASSETS WHERE _ASSETID not in (SELECT distinct(_ASSETID) FROM APPLICATION_INSTANCES)) notaudited
	,(SELECT COUNT(*) FROM ASSETS WHERE (_LASTAUDIT <= @oldAuditDate)) outofdate
GO




SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--------------------------------------------------------------------------
--																		--
-- usp_statistics_topassets											--
-- ===========================											--
--																		--
--------------------------------------------------------------------------
--																		--
-- Description															--
-- ===========															--
--																		--
-- This stored procedure is part of the AuditWizard Product				--
-- It is used to return statistics about assets use	 				--
--																		--
--------------------------------------------------------------------------
--																		--
-- History																--
-- =======																--
--																		--
--	29-Feb-2008	Chris Drew		Initial Version							--
--																		--
--------------------------------------------------------------------------

CREATE PROCEDURE [dbo].[usp_statistics_topassets]
						@resultCount int,
						@publisherFilter varchar(1024)
AS	
BEGIN
    SET ROWCOUNT @RESULTCOUNT 
	SELECT ASSETS._NAME, 
		(SELECT COUNT(*) 
			FROM APPLICATION_INSTANCES 
			WHERE APPLICATION_INSTANCES._ASSETID = ASSETS._ASSETID AND _HIDDEN=0)  
	FROM ASSETS 
	ORDER BY 
		(SELECT COUNT(*) 
			FROM APPLICATION_INSTANCES 
			WHERE APPLICATION_INSTANCES._ASSETID = ASSETS._ASSETID AND _HIDDEN=0) DESC
    SET ROWCOUNT 0 
END
GO




SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--------------------------------------------------------------------------
--																		--
-- usp_audittrail_delete												--
-- =====================												--
--																		--
--------------------------------------------------------------------------
--																		--
-- Description															--
-- ===========															--
--																		--
-- This stored procedure is part of the AuditWizard Product			--
-- It is used to return a delete any Audit Trail Entry from the database  -- 
--																		--
--------------------------------------------------------------------------
--																		--
-- History																--
-- =======																--
--																		--
--	17-Apr-2008	Chris Drew		Initial Version							--
--																		--
--------------------------------------------------------------------------

CREATE PROCEDURE [dbo].[usp_audittrail_delete]
	@nAuditTrailID int
AS  
	SET nocount ON  	
	DELETE FROM AUDITTRAIL WHERE _AUDITTRAILID=@nAuditTrailID
GO




SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--------------------------------------------------------------------------
--																		--
-- usp_audittrail_purge													--
-- ====================													--
--																		--
--------------------------------------------------------------------------
--																		--
-- Description															--
-- ===========															--
--																		--
-- This stored procedure is part of the AuditWizard Product			--
-- It is used to purge the audit trail of all records prior to the 		-- 
-- date																	--
--																		--
--------------------------------------------------------------------------
--																		--
-- History																--
-- =======																--
--																		--
--	4-May-2008	Chris Drew		Initial Version							--
--																		--
--------------------------------------------------------------------------

CREATE PROCEDURE [dbo].[usp_audittrail_purge]
	@dtPurgeDate DateTime
AS  
	DECLARE @nRowCount int

	SET nocount ON  	
	DELETE FROM AUDITTRAIL where (datediff(day, _DATE, @dtPurgeDate) > 0)
	SET @nRowCount = @@ROWCOUNT 
	return @nRowCount;
GO




SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--------------------------------------------------------------------------
--																		--
-- usp_support_statistics												--
-- ======================												--
--																		--
--------------------------------------------------------------------------
--																		--
-- Description															--
-- ===========															--
--																		--
-- This stored procedure is part of the AuditWizard Product				--
-- It is used to return statistics about application use	 			--
--																		--
--	NOTE: THIS SP ONLY HANDLES APPLICATIONS								--
--																		--
--------------------------------------------------------------------------
--																		--
-- History																--
-- =======																--
--																		--
--	29-Jul-2008	Chris Drew		Initial Version							--
--																		--
--------------------------------------------------------------------------

CREATE PROCEDURE [dbo].[usp_support_statistics]
AS 
	DECLARE @dtToday DateTime
	DECLARE @dtThisWeek DateTime
	DECLARE @dtThisMonth DateTime
 
	SET nocount ON  
 	SET @dtToday = GETDATE()
 	SET @dtThisWeek = DATEADD(week ,1 ,@dtToday)
 	SET @dtThisMonth = DATEADD(month ,1 ,@dtToday)

	SELECT 'row'
	,(SELECT COUNT(_APPLICATIONID) FROM LICENSES WHERE _SUPPORTED=1 AND (DATEDIFF(day ,@dtToday ,_SUPPORT_EXPIRES) < 0)) expired
	,(SELECT COUNT(_APPLICATIONID) FROM LICENSES WHERE _SUPPORTED=1 AND (DATEDIFF(day ,@dtToday ,_SUPPORT_EXPIRES) = 0)) expiretoday
	,(SELECT COUNT(_APPLICATIONID) FROM LICENSES WHERE _SUPPORTED=1 AND (DATEDIFF(day ,@dtToday ,_SUPPORT_EXPIRES) > 0) AND (DATEDIFF(day ,@dtThisWeek ,_SUPPORT_EXPIRES) <= 0)) expirethisweek
	,(SELECT COUNT(_APPLICATIONID) FROM LICENSES WHERE _SUPPORTED=1 AND (DATEDIFF(day ,@dtToday ,_SUPPORT_EXPIRES) > 7) AND (DATEDIFF(day ,@dtThisMonth ,_SUPPORT_EXPIRES) <= 0)) expirethismonth
GO



set ANSI_NULLS ON
set QUOTED_IDENTIFIER ON
GO
--------------------------------------------------------------------------
--																		--
-- usp_statistics_alerts												--
-- =====================												--
--																		--
--------------------------------------------------------------------------
--																		--
-- Description															--
-- ===========															--
--																		--
-- This stored procedure is part of the AuditWizard Product				--
-- It is used to return statistics about alerts generated				--
--																		--
--------------------------------------------------------------------------
--																		--
-- History																--
-- =======																--
--																		--
--	08-Aug-2008	Chris Drew		Initial Version							--
--																		--
--------------------------------------------------------------------------

CREATE PROCEDURE [dbo].[usp_statistics_alerts]
AS  
	DECLARE @dtToday DateTime
	DECLARE @dtThisWeek DateTime
	DECLARE @dtThisMonth DateTime
 
	SET nocount ON  
 	SET @dtToday = GETDATE()
 	SET @dtThisWeek = DATEADD(week ,1 ,@dtToday)
 	SET @dtThisMonth = DATEADD(month ,1 ,@dtToday)

	SELECT 'row'
		,(SELECT MAX(_ALERTDATE) FROM ALERTS) lastalert
		,(SELECT COUNT(_ALERTID) FROM ALERTS WHERE (DATEDIFF(day ,@dtToday ,_ALERTDATE) = 0)) alertstoday
		,(SELECT COUNT(_ALERTID) FROM ALERTS WHERE (DATEDIFF(day ,@dtThisWeek ,_ALERTDATE) <= 0)) alertsthisweek
		,(SELECT COUNT(_ALERTID) FROM ALERTS WHERE (DATEDIFF(day ,@dtThisMonth ,_ALERTDATE) <= 0)) alertsthismonth
GO




SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--------------------------------------------------------------------------
--																		--
-- usp_license_update													--
-- ==================													--
--																		--
--------------------------------------------------------------------------
--																		--
-- Description															--
-- ===========															--
--																		--
-- This stored procedure is part of the AuditWizard Product				--
-- It is used to add a update a License definititon 					--
--																		--
--------------------------------------------------------------------------
--																		--
-- History																--
-- =======																--
--																		--
--	18-Feb-2006	Chris Drew		Initial Version							--
--																		--
--------------------------------------------------------------------------

CREATE PROCEDURE [dbo].[usp_license_update]
	@nLicenseID int,
	@nLicenseTypeID int,
	@nApplicationID int,
	@nCount int,
	@bSupported bit,
	@dtSupportExpiry DateTime,
	@nSupportAlertDays int,
	@bSupportAlertEmail bit,
	@cSupportAlertRecipients varchar(1020),
	@nSupplierID int
	
AS     
	DECLARE @cSQL as varchar(5000)

	SET nocount ON 
BEGIN    
	UPDATE LICENSES SET _LICENSETYPEID = @nLicenseTypeID 
						,_APPLICATIONID = @nApplicationID
						,_COUNT = @nCount
						,_SUPPORTED = @bSupported
						,_SUPPORT_EXPIRES = @dtSupportExpiry
						,_SUPPORT_ALERTDAYS = @nSupportAlertDays
						,_SUPPORT_ALERTBYEMAIL = @bSupportAlertEmail
						,_SUPPORT_ALERTRECIPIENTS = @cSupportAlertRecipients
						,_SUPPLIERID = @nSupplierID
					WHERE _LICENSEID = @nLicenseID
END
GO




SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--------------------------------------------------------------------------
--																		--
-- usp_licensetype_update												--
-- ======================												--
--																		--
--------------------------------------------------------------------------
--																		--
-- Description															--
-- ===========															--
--																		--
-- This stored procedure is part of the AuditWizard Product				--
-- It is used to add a update a License Type definititon 				--
--																		--
--------------------------------------------------------------------------
--																		--
-- History																--
-- =======																--
--																		--
--	18-Feb-2006	Chris Drew		Initial Version							--
--																		--
--------------------------------------------------------------------------

CREATE PROCEDURE [dbo].[usp_licensetype_update]
	@nLicenseTypeID int,
	@bPerPC bit
	
AS     
	DECLARE @cSQL as varchar(255)
	SET nocount ON 
BEGIN
	UPDATE LICENSE_TYPES SET _COUNTED=@bPerPC 
		WHERE _LICENSETYPEID = @nLicenseTypeID	
END
GO




SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--------------------------------------------------------------------------
--																		--
-- usp_licensetype_find													--
-- =================													--
--																		--
--------------------------------------------------------------------------
--																		--
-- Description															--
-- ===========															--
--																		--
-- This stored procedure is part of the AuditWizard Product				--
-- It is used to add a locate a licensetype within the database		   	--
--																		--
-- If an existing entry is found then its ID is returned otherwise we   --
-- return 0.															--
--																		--
--------------------------------------------------------------------------
--																		--
-- History																--
-- =======																--
--																		--
--	18-Feb-2006	Chris Drew		Initial Version							--
--																		--
--------------------------------------------------------------------------

CREATE PROCEDURE [dbo].[usp_licensetype_find]
	@cName varchar(255)
AS
	DECLARE @nReturnID int

	SET nocount ON
	SET @nReturnID = isnull((SELECT _LICENSETYPEID FROM LICENSE_TYPES WHERE _name=@cName),0)
	RETURN @nReturnID
GO




SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--------------------------------------------------------------------------
--																		--
-- usp_licensetype_enumerate											--
-- =========================											--
--																		--
--------------------------------------------------------------------------
--																		--
-- Description															--
-- ===========															--
--																		--
-- This stored procedure is part of the AuditWizard Product				--
-- It is used to enumerate the list of license types defined			--
--																		--
--------------------------------------------------------------------------
--																		--
-- History																--
-- =======																--
--																		--
--	04-Mar-2008	Chris Drew		Initial Version							--
--																		--
--------------------------------------------------------------------------
CREATE PROCEDURE [dbo].[usp_licensetype_enumerate]
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT * FROM LICENSE_TYPES
END
GO




SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--------------------------------------------------------------------------
--																		--
-- usp_action_update													--
-- =================													--
--																		--
--------------------------------------------------------------------------
--																		--
-- Description															--
-- ===========															--
--																		--
-- This stored procedure is part of the AuditWizard Product				--
-- It is used to add a update an action record							--
--																		--
--------------------------------------------------------------------------
--																		--
-- History																--
-- =======																--
--																		--
--	12-Aug-2008	Chris Drew		Initial Version							--
--																		--
--------------------------------------------------------------------------

CREATE PROCEDURE [dbo].[usp_action_update]
	@nActionID INT,
	@nStatus INT,
	@cAssets VARCHAR(5000),
	@cNotes VARCHAR(1020)
AS     
	SET nocount ON 
BEGIN
	DECLARE @SQL VARCHAR(5000)
	
	SET @SQL = 'UPDATE ACTIONS SET _STATUS = ' + CAST(@nStatus AS VARCHAR(4)) 
			+ ', _ASSETS=' + @cAssets 
			+ ', _NOTES=' + @cNotes 
			+ ' WHERE _ACTIONID = ' + CAST(@nActionID AS VARCHAR(16))
	EXEC (@SQL)
END
GO




SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--------------------------------------------------------------------------
--																		--
-- usp_statistics_ca													--
-- =================													--
--																		--
--------------------------------------------------------------------------
--																		--
-- Description															--
-- ===========															--
--																		--
-- This stored procedure is part of the AuditWizard Product				--
-- It is used to return statistics about the Create Actions Phase		--
--																		--
--------------------------------------------------------------------------
--																		--
-- History																--
-- =======																--
--																		--
--	08-Aug-2008	Chris Drew		Initial Version							--
--																		--
--------------------------------------------------------------------------

CREATE PROCEDURE [dbo].[usp_statistics_ca]
AS  
	SET nocount ON  

	SELECT 'row'
		,(SELECT COUNT(_ACTIONID) FROM ACTIONS) actioncount
GO




SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--------------------------------------------------------------------------
--																		--
-- usp_statistics_ra													--
-- =================													--
--																		--
--------------------------------------------------------------------------
--																		--
-- Description															--
-- ===========															--
--																		--
-- This stored procedure is part of the AuditWizard Product				--
-- It is used to return statistics about the Review Actions Phase		--
--																		--
--------------------------------------------------------------------------
--																		--
-- History																--
-- =======																--
--																		--
--	08-Aug-2008	Chris Drew		Initial Version							--
--																		--
--------------------------------------------------------------------------

CREATE PROCEDURE [dbo].[usp_statistics_ra]
AS  
	SET nocount ON  

	SELECT 'row'
		,(SELECT COUNT(_ACTIONID) FROM ACTIONS WHERE _STATUS<>0) reviewed
		,(SELECT COUNT(_ACTIONID) FROM ACTIONS WHERE _STATUS=0) notreviewed
GO




SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--------------------------------------------------------------------------
--																		--
-- usp_settings_setsetting												--
-- ======================												--
--																		--
--------------------------------------------------------------------------
--																		--
-- Description															--
-- ===========															--
--																		--
-- This stored procedure is part of the AuditWizard Product				--
-- It is used to set the value for the specified setting				--
--																		--
--------------------------------------------------------------------------
--																		--
-- History																--
-- =======																--
--																		--
--	28-Mar-2008	Chris Drew		Initial Version							--
--																		--
--------------------------------------------------------------------------

CREATE PROCEDURE [dbo].[usp_settings_setsetting]
	@key varchar(255),
	@value varchar(255)
	
AS     
	SET nocount ON 
BEGIN
	IF NOT EXISTS (SELECT * FROM SETTINGS WHERE _KEY=@key)
		BEGIN
			INSERT INTO SETTINGS (_KEY,_VALUE) VALUES (@key ,@value)
		END
	ELSE
		BEGIN
			UPDATE SETTINGS SET _VALUE=@value WHERE _KEY=@key
		END
END
GO




SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--------------------------------------------------------------------------
--																		--
-- usp_settings_getsetting												--
-- ======================												--
--																		--
--------------------------------------------------------------------------
--																		--
-- Description															--
-- ===========															--
--																		--
-- This stored procedure is part of the AuditWizard Product				--
-- It is used to recover the value for the specified setting			--
--																		--
--------------------------------------------------------------------------
--																		--
-- History																--
-- =======																--
--																		--
--	28-Mar-2008	Chris Drew		Initial Version							--
--																		--
--------------------------------------------------------------------------

CREATE PROCEDURE [dbo].[usp_settings_getsetting]
	@key varchar(255),
	@retValue varchar(255) output
	
AS     
	SET nocount ON 
BEGIN     
	SET @retValue = (SELECT _VALUE FROM SETTINGS WHERE _KEY=@key)
END
GO




SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--------------------------------------------------------------------------
--																		--
-- usp_add_user															--
-- ============															--
--																		--
--------------------------------------------------------------------------
--																		--
-- Description															--
-- ===========															--
--																		--
-- This stored procedure is part of the AuditWizard Product				--
-- It is used to add a new entry to the USERS table within the			--
-- database. 															--
--																		--
-- If an existing entry is found then its ID is returned otherwise we   --
-- return the ID of the newly created record. 							--
--																		--
--------------------------------------------------------------------------
--																		--
-- History																--
-- =======																--
--																		--
--	22-Jul-2006	Chris Drew		Initial Version							--
--																		--
--------------------------------------------------------------------------

CREATE PROCEDURE [dbo].[usp_add_user]
	@cLogin varchar(255),
	@cFirstName varchar(255),
	@cLastName	varchar(255),
	@nAccessLevel int,
	@nRootLocation int,
	@nReturnId int output
AS

DECLARE @nRetCode int,
	@cMessage varchar(255)

SET nocount ON
SET @nRetCode = 0

--
-- first check the database for an entry that matches the supplied logon name
--
SET @nReturnId = isnull((SELECT _USERID FROM USERS WHERE _LOGIN = @cLogin) ,0)

-- did we find a match ?
IF (@nReturnId = 0)
BEGIN

	INSERT INTO USERS
		(_LOGIN, _FIRSTNAME, _LASTNAME, _ACCESSLEVEL, _ROOTLOCATION)
	VALUES
		(@cLogin, @cFirstName, @cLastName, @nAccessLevel, @nRootLocation)

	SET @nReturnID = @@IDENTITY
END
RETURN @nRetCode
GO




SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--------------------------------------------------------------------------
--																		--
-- usp_user_setpassword													--
-- ====================													--
--																		--
--------------------------------------------------------------------------
--																		--
-- Description															--
-- ===========															--
--																		--
-- This stored procedure is part of the AuditWizard Product				--
-- It is used to set the password for a user in the database			--
--																		--
--	All character strings passed to this function must be SQL formatted --
--																		--
--------------------------------------------------------------------------
--																		--
-- History																--
-- =======																--
--																		--
--	22-Jul-2006	Chris Drew		Initial Version							--
--																		--
--------------------------------------------------------------------------

CREATE PROCEDURE [dbo].[usp_user_setpassword]
	@nUserID int,
	@cPassword varchar(255)
	
AS     
	DECLARE @cSQL as varchar(255)
	SET nocount ON
	UPDATE USERS SET _PASSWORD = @cPassword WHERE _USERID = @nUserID
GO




SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--------------------------------------------------------------------------
--																		--
-- usp_user_checkpassword												--
-- ======================												--
--																		--
--------------------------------------------------------------------------
--																		--
-- Description															--
-- ===========															--
--																		--
-- This stored procedure is part of the AuditWizard Product				--
-- It is used to check the specified user/password combination and 		--
-- return the user id													--
--																		--
--	All character strings passed to this function must be SQL formatted --
--																		--
--------------------------------------------------------------------------
--																		--
-- History																--
-- =======																--
--																		--
--	22-Jul-2006	Chris Drew		Initial Version							--
--																		--
--------------------------------------------------------------------------

CREATE PROCEDURE [dbo].[usp_user_checkpassword]
	@cLogin varchar(255),
	@cPassword varchar(255),
	@nReturnID int output
	
AS     
	SET nocount ON 
BEGIN  
	SET @nReturnID = isnull((SELECT _USERID FROM USERS WHERE _LOGIN=@cLogin AND _PASSWORD=@cPassword),0)
END
GO




SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--------------------------------------------------------------------------
--																		--
-- usp_user_delete														--
-- ===============														--
--																		--
--------------------------------------------------------------------------
--																		--
-- Description															--
-- ===========															--
--																		--
-- This stored procedure is part of the AuditWizard Product				--
-- It is used to add a DELETE a User definititon 						--
--																		--
-- Note that it is the responsibility of the caller to handle any		--																		--
-- tables which may depend on this record								--
--																		--
--------------------------------------------------------------------------
--																		--
-- History																--
-- =======																--
--																		--
--	23-Jul-2008	Chris Drew		Initial Version							--
--																		--
--------------------------------------------------------------------------

CREATE PROCEDURE [dbo].[usp_user_delete]
	@nUserID int
AS     
	SET nocount ON
	DELETE FROM USERS WHERE _USERID=@nUserID
GO




SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Chris Drew
-- Create date: 11/Feb/08
-- Description:	Sets the AuditWizard Database Version
--
-- Parameters:
--   Major Version Number (int)
--   Minor Version Number (int)
--   optional title parameter (varchar(255))
--
-- =============================================
 
CREATE PROCEDURE [dbo].[usp_SetVersion] 
					@intMajor as int,
					@intMinor as int,
					@vchTitle varchar(255) = ''
AS

-- Parameters:
	-- Major Version Number (int)
	-- Minor Version Number (int)
	-- optional title parameter (varchar(256))

declare	@intError as int,
	@dtmDate as datetime,
	@vchSqlVer as varchar(1024),
	@vchUser as varchar(64),
	@vchError as varchar(256)

BEGIN

	set nocount on

-- set Title if not supplied
	if len(@vchTitle) = 0
		set @vchTitle = 'AuditWizard Database'

-- get date
	set @dtmDate = getdate()

-- get SQL Server Version for comment field
	set @vchSqlVer = (select @@Version)
	
-- get system users name
	set @vchUser = suser_sname()

-- empty version table

	delete from VERSION

-- insert values into VERSION table

	insert into VERSION (_TITLE,_MAJOR,_MINOR,_DATE,_SQLVER,_USER)
	values (@vchTitle,@intMajor,@intMinor,@dtmDate,@vchSqlVer,@vchUser)

END
GO





SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Chris Drew
-- Create date: 11/Feb/08
-- Description:	Gets the AuditWizard Database Version
--
-- Parameters:
--   Major Version Number (int)
--   Minor Version Number (int)
--   optional title parameter (varchar(255))
--
-- =============================================
 
CREATE PROCEDURE [dbo].[usp_get_version] 
AS

BEGIN

	set nocount on
	
	SELECT * FROM VERSION
END
GO



SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--------------------------------------------------------------------------
--																		--
-- usp_alert_enumerate													--
-- ===================													--
--																		--
--------------------------------------------------------------------------
--																		--
-- Description															--
-- ===========															--
--																		--
-- This stored procedure is part of the AuditWizard Product				--
-- It is used to return a list of alert entries							--
--																		--
--------------------------------------------------------------------------
--																		--
-- History																--
-- =======																--
--																		--
--	30-Jul-2008	Chris Drew		Initial Version							--
--																		--
--------------------------------------------------------------------------

CREATE PROCEDURE [dbo].[usp_alert_enumerate]
	@dtSinceDate DateTime
AS 
	DECLARE @cSQL varchar(1024) 
	SET nocount ON  
	
BEGIN
	SELECT ALERTS.*
	FROM ALERTS
	WHERE _ALERTDATE >= @dtSinceDate
	ORDER BY _ALERTID

END
GO




SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--------------------------------------------------------------------------
--																		--
-- usp_alert_purge														--
-- ===============														--
--																		--
--------------------------------------------------------------------------
--																		--
-- Description															--
-- ===========															--
--																		--
-- This stored procedure is part of the AuditWizard Product				--
-- It is used to purge the alerts table of all records prior to the 	-- 
-- date																	--
--																		--
--------------------------------------------------------------------------
--																		--
-- History																--
-- =======																--
--																		--
--	30-Jul-2008	Chris Drew		Initial Version							--
--																		--
--------------------------------------------------------------------------

CREATE PROCEDURE [dbo].[usp_alert_purge]
	@dtPurgeDate DateTime
AS  
	DECLARE @nRowCount int

	SET nocount ON  	
	DELETE FROM ALERTS 
		WHERE (datediff(day, _ALERTDATE, @dtPurgeDate) > 0)
	SET @nRowCount = @@ROWCOUNT 
	return @nRowCount;
GO




SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--------------------------------------------------------------------------
--																		--
-- usp_alert_update														--
-- ================														--
--																		--
--------------------------------------------------------------------------
--																		--
-- Description															--
-- ===========															--
--																		--
-- This stored procedure is part of the AuditWizard Product				--
-- It is used to add a update the last alert date for an alert			--
--																		--
--------------------------------------------------------------------------
--																		--
-- History																--
-- =======																--
--																		--
--	30-Jul-2008	Chris Drew		Initial Version							--
--																		--
--------------------------------------------------------------------------

CREATE PROCEDURE [dbo].[usp_alert_update]
	@nAlertID int,
	@dtAlertDate DateTime
AS     
	SET nocount ON 
BEGIN
	DECLARE @SQL VARCHAR(5000)
	
	SET @SQL = 'UPDATE ALERTS SET _ALERTDATE = ' + @dtAlertDate 
				+ 'WHERE _ALERTID = ' + CAST(@nAlertID AS VARCHAR(16))
	EXEC (@SQL)
END
GO




SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--------------------------------------------------------------------------
--																		--
-- usp_alert_delete														--
-- ================														--
--																		--
--------------------------------------------------------------------------
--																		--
-- Description															--
-- ===========															--
--																		--
-- This stored procedure is part of the AuditWizard Product				--
-- It is used to DELETE an Alert										--
--																		--
--------------------------------------------------------------------------
--																		--
-- History																--
-- =======																--
--																		--
--	5-Mar-2009	Chris Drew		Initial Version							--
--																		--
--------------------------------------------------------------------------

CREATE PROCEDURE [dbo].usp_alert_delete
	@nAlertID int
	
AS    
	DELETE FROM ALERTS WHERE _ALERTID = @nAlertID
RETURN
GO




SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--------------------------------------------------------------------------
--																		--
-- usp_audittrail_add													--
-- ==================													--
--																		--
--------------------------------------------------------------------------
--																		--
-- Description															--
-- ===========															--
--																		--
-- This stored procedure is part of the AuditWizard Product				--
-- It is used to add an audit trail entry to the database				--
--																		--
--	Pass strings as SQL formatted										-- 
--																		--
--------------------------------------------------------------------------
--																		--
-- History																--
-- =======																--
--																		--
--	17-Apr-2008	Chris Drew		Initial Version							--
--																		--
--------------------------------------------------------------------------

CREATE PROCEDURE [dbo].[usp_audittrail_add]
	@nAssetID int,
	@cUsername varchar(255),
	@dtDate datetime,
	@nClass int,
	@nType int,
	@cKey varchar(255),
	@cOldValue varchar(255),
	@cNewValue varchar(255),
	@nReturnID int output
AS

DECLARE @nRetCode int,
	@cColumns varchar(1000),
	@cValues varchar(1200),
	@cDate varchar(255),
	@cSqlDate varchar(255)

SET nocount ON
SET @nRetCode = 0 

BEGIN
	SET @cDate = cast(@dtDate as varchar(32))
	EXEC usp_preparestring @cDate, @cSqlDate output

	SET @cColumns = '_ASSETID ,_USERNAME ,_DATE ,_CLASS ,_TYPE ,_KEY ,_VALUE1 ,_VALUE2'
	SET @cValues  =  cast(@nAssetID as varchar(32)) + ','
				  +  @cUsername  + ','
				  +  @cSqlDate + ','
				  +  cast(@nClass as varchar(4)) + ','
				  +  cast(@nType as varchar(4)) + ','
				  +  @cKey  + ','
				  +  @cOldValue  + ','
				  +  @cNewValue

	print 'Columns: ' + @cColumns
	print 'Values:  ' + @cValues

	-- Insert the record
	EXEC @nRetCode = usp_IdentityInsert 'audittrail' ,@cColumns ,@cValues ,@nReturnId output 

END
RETURN  @nRetCode
GO




SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--------------------------------------------------------------------------
--																		--
-- usp_licensetype_add													--
-- ===================													--
--																		--
--------------------------------------------------------------------------
--																		--
-- Description															--
-- ===========															--
--																		--
-- This stored procedure is part of the AuditWizard Product				--
-- It is used to add a new license type record							--
--																		--
--------------------------------------------------------------------------
--																		--
-- History																--
-- =======																--
--																		--
--	04-Mar-2008	Chris Drew		Initial Version							--
--																		--
--------------------------------------------------------------------------

CREATE PROCEDURE [dbo].[usp_licensetype_add]
	@cName varchar(255),
	@bPerPC bit,
	@nReturnID int output
AS

DECLARE @nRetCode int,
	@cColumns varchar(1000),
	@cValues varchar(1000),
	@cSqlName varchar(255)

SET nocount ON
SET @nRetCode = 0 

BEGIN
	EXEC usp_preparestring @cName, @cSqlName output

	SET @cColumns = '_name ,_counted'
	SET @cValues = @cSqlName + ',' + cast(@bPerPC as varchar(1))
	EXEC @nRetCode = usp_IdentityInsert 'license_types' ,@cColumns ,@cValues ,@nReturnId output 
END
RETURN  @nRetCode
GO




SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--------------------------------------------------------------------------
--																		--
-- usp_license_add														--
-- ===============														--
--																		--
--------------------------------------------------------------------------
--																		--
-- Description															--
-- ===========															--
--																		--
-- This stored procedure is part of the AuditWizard Product				--
-- It is used to add a new license record								--
--																		--
--------------------------------------------------------------------------
--																		--
-- History																--
-- =======																--
--																		--
--	04-Mar-2008	Chris Drew		Initial Version							--
--																		--
--------------------------------------------------------------------------

CREATE PROCEDURE [dbo].[usp_license_add]
	@nLicenseTypeID int,
	@nForApplicationID int,
	@nCount int,
	@bSupported bit,
	@dtSupportExpiry DateTime,
	@nSupportAlertDays int,
	@bSupportAlertEmail bit,
	@cSupportAlertRecipients varchar(1020),
	@nSupplierID int,
	@nReturnID int output
AS

DECLARE @nRetCode int
SET nocount ON
SET @nRetCode = 0 

BEGIN

	INSERT INTO LICENSES
		(_LICENSETYPEID ,_APPLICATIONID ,_COUNT ,_SUPPORTED ,_SUPPORT_EXPIRES 
		,_SUPPORT_ALERTDAYS ,_SUPPORT_ALERTBYEMAIL ,_SUPPORT_ALERTRECIPIENTS ,_SUPPLIERID)
	VALUES
		(@nLicenseTypeID, @nForApplicationID, @nCount, @bSupported 
		,@dtSupportExpiry, @nSupportAlertDays, @bSupportAlertEmail, @cSupportAlertRecipients
		,@nSupplierID)

	SET @nReturnID = @@IDENTITY

END
RETURN  @nRetCode
GO




SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--------------------------------------------------------------------------
--																		--
-- usp_action_add														--
-- ==============														--
--																		--
--------------------------------------------------------------------------
--																		--
-- Description															--
-- ===========															--
--																		--
-- This stored procedure is part of the AuditWizard Product				--
-- It is used to add an action to the database							--
--																		--
-- Character strings passed to this function must be raw and not SQL    --
--																		--
--------------------------------------------------------------------------
--																		--
-- History																--
-- =======																--
--																		--
--	12-Aug-2008	Chris Drew		Initial Version							--
--																		--
--------------------------------------------------------------------------

CREATE PROCEDURE [dbo].[usp_action_add]
	@nType int,
	@nApplicationID int,
	@nStatus int,
	@cAssets varchar(5000),
	@cNotes varchar(1020)
AS

DECLARE @nRetCode int,
	@cColumns varchar(1000),
	@cValues varchar(1000),
	@nReturnID int
 
SET nocount ON

BEGIN

	-- Format the columns and values used in the INSERT			                         
	SET @cColumns = '_APPLICATIONID ,_TYPE, _STATUS ,_ASSETS, _NOTES'
	SET @cValues = cast(@nApplicationID as varchar(8)) + ',' 
					+ cast(@nType as varchar(4)) + ',' 
					+ cast(@nStatus as varchar(4)) + ',' 
					+ @cAssets + ','
					+ @cNotes 
	EXEC usp_IdentityInsert 'ACTIONS' ,@cColumns ,@cValues ,@nReturnId output 
END
RETURN  @nReturnID
GO




SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--------------------------------------------------------------------------
--																		--
-- usp_alert_add														--
-- ==============														--
--																		--
--------------------------------------------------------------------------
--																		--
-- Description															--
-- ===========															--
--																		--
-- This stored procedure is part of the AuditWizard Product				--
-- It is used to add an alert to the database							--
--																		--
-- Character strings passed to this function must be raw and not SQL    --
--																		--
--------------------------------------------------------------------------
--																		--
-- History																--
-- =======																--
--																		--
--	30-Jul-2008	Chris Drew		Initial Version							--
--																		--
--------------------------------------------------------------------------

CREATE PROCEDURE [dbo].[usp_alert_add]
	@nType int,
	@nCategory int,
	@cMessage varchar(510),
	@cField1 varchar(1020),
	@cField2 varchar(1020),
	@cAssetName varchar(255),
	@cAlertName varchar(255)
AS

DECLARE @nRetCode int,
	@cColumns varchar(1000),
	@cValues varchar(1000),
	@nReturnID int,
	@dtmDate as datetime
 
SET nocount ON

BEGIN
	set @dtmDate = getdate()

	-- First make sure that we do not already have an alert with the same message
	--SET @nReturnID = isnull((SELECT _ALERTID from ALERTS WHERE _MESSAGE=@cMessage) ,0)
	SET @nReturnID = 0
	IF (@nReturnID = 0)
	BEGIN
	
		INSERT INTO ALERTS
			(_TYPE, _CATEGORY ,_MESSAGE ,_FIELD1 ,_FIELD2 ,_STATUS ,_ALERTDATE ,_ASSETNAME ,_ALERTNAME)
		VALUES
			(@nType, @nCategory, @cMessage, @cField1, @cField2 ,0 ,@dtmDate ,@cAssetName ,@cAlertName)

		SET @nReturnID = @@IDENTITY
	END
END
RETURN  @nReturnID
GO
      
      
      
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--------------------------------------------------------------------------
--																		--
-- usp_alert_set_status													--
-- ====================													--
--																		--
--------------------------------------------------------------------------
--																		--
-- Description															--
-- ===========															--
--																		--
-- This stored procedure is part of the AuditWizard Product			--
-- It is used to set the current status for an alert					--
--																		--
--------------------------------------------------------------------------
--																		--
-- History																--
-- =======																--
--																		--
--	30-Jul-2008	Chris Drew		Initial Version							--
--																		--
--------------------------------------------------------------------------

CREATE PROCEDURE [dbo].[usp_alert_set_status]
	@nAlertID int,
	@nStatus int
AS     
	SET nocount ON 
	UPDATE ALERTS SET _STATUS=@nStatus WHERE _ALERTID=@nAlertID	
GO



SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--------------------------------------------------------------------------
--																		--
-- usp_supplier_add														--
-- ==============														--
--																		--
--------------------------------------------------------------------------
--																		--
-- Description															--
-- ===========															--
--																		--
-- This stored procedure is part of the AuditWizard Product				--
-- It is used to add a supplier to the database							--
--																		--
-- Character strings passed to this function must be raw and not SQL    --
--																		--
--------------------------------------------------------------------------
--																		--
-- History																--
-- =======																--
--																		--
--	30-Jul-2008	Chris Drew		Initial Version							--
--																		--
--------------------------------------------------------------------------

CREATE PROCEDURE [dbo].[usp_supplier_add]
	@cName	varchar(255),
	@cAddress1	varchar(255),
	@cAddress2	varchar(255),
	@cCity		varchar(255),
	@cState		varchar(255),
	@cZip		varchar(255),
	@cTelephone	varchar(255),
	@cContactName	varchar(255),
	@cContactEmail	varchar(255),
	@cWWW		varchar(255),
	@cFax		varchar(255),
	@cNotes		varchar(1020)
AS

DECLARE @nRetCode int,
	@cColumns varchar(1000),
	@cValues varchar(1000),
	@nReturnID int
 
SET nocount ON

BEGIN

	-- Format the columns and values used in the INSERT			                         
	SET @cColumns = '_NAME, _ADDRESS1, _ADDRESS2, _CITY, _STATE, _ZIP, _TELEPHONE, _CONTACT_NAME, _CONTACT_EMAIL, _WWW, _FAX ,_NOTES'
	SET @cValues = @cName + ',' + @cAddress1 + ',' + @cAddress2 + ',' + @cCity 
				+ ',' + @cState + ',' + @cZip + ',' + @cTelephone + ',' + @cContactName + ',' + @cContactEmail 
				+ ',' + @cWWW + ',' + @cFax + ',' + @cNotes
	EXEC usp_IdentityInsert 'SUPPLIERS' ,@cColumns ,@cValues ,@nReturnId output 
END
RETURN  @nReturnID
GO




SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--------------------------------------------------------------------------
--																		--
-- usp_asset_find													--
-- =================													--
--																		--
--------------------------------------------------------------------------
--																		--
-- Description															--
-- ===========															--
--																		--
-- This stored procedure is part of the AuditWizard Product				--
-- It is used to add a locate a asset within the database		   	--
--																		--
-- If an existing entry is found then its ID is returned otherwise we   --
-- return 0.															--
--																		--
--------------------------------------------------------------------------
--																		--
-- History																--
-- =======																--
--																		--
--	18-Feb-2006	Chris Drew		Initial Version							--
--																		--
--------------------------------------------------------------------------

CREATE PROCEDURE [dbo].[usp_asset_find]
	@cName varchar(255),
	@cUniqueID varchar(255)
AS

declare @nAssetID int
declare @cMatchedValue varchar(255)
declare @nMatchID int

SET NOCOUNT ON
print '...executing usp_asset_find for asset with name [' + @cName + '] uniqueid [' + @cUniqueID + ']'

-- First try and find a match for this asset using the unique id passed in.  If this was blan
-- then skip this step
IF (@cUniqueID <> '')
	BEGIN
	SET @nMatchID = isnull((SELECT _ASSETID from ASSETS where _UNIQUEID=@cUniqueID) ,0)
	if (@nMatchID <> 0) 
		BEGIN
		-- We found a Asset with the same unique ID so recover it's name also
		print 'Matched on UniqueID'
		SET @cMatchedValue = (SELECT _NAME FROM ASSETS where _ASSETID=@nMatchID)

		-- Does this match the name of the asset passed in to us
		IF (@cMatchedValue <> @cName)
			UPDATE ASSETS SET _NAME=@cName WHERE _ASSETID=@nMatchID
		END

	ELSE

		-- OK we have not found a match for this asset when matching by the UNIQUEID field so we shall
		-- now try again using the asset name
		BEGIN
		print 'NO Match on UniqueID - check Asset Name'
		SET @nMatchID = isnull((SELECT _ASSETID from ASSETS where _NAME=@cName) ,0)
	
		-- Did we get a match - if yes then 
		IF (@nMatchID <> 0)
			BEGIN
			-- A match has been found when comparing using the asset name - we already know
			-- however that the unique ids differ so we have 2 possibilities here
			-- 1> The unique id found is null indicating that this asset has never been audited in
			--    which case we DO have a match
			-- 2> The unique id found is present in which case we have to assume that there are two
			--    instances of the same named PC that have been audited
			print 'Matched on Name'
			SET @cMatchedValue = (SELECT _UNIQUEID FROM ASSETS where _ASSETID=@nMatchID)

			-- Is the unique id recovered blank?  This indicates a PC that has not been audited
			IF (@cMatchedValue <> '')
				BEGIN
				print 'Matched asset DOES have a unique ID'
				-- Does this match the uniqueid of the asset passed in to us
				IF (@cMatchedValue <> @cUniqueID)
					BEGIN
					print 'Unique IDs do NOT match - assume different assets'
					SET @nMatchID = 0
					END
				END
			END
		END
	END

-- No UniqueID was supplied - we therefore assume that this asset is being added as part
-- of a network discovery process and we simply need to match on name
ELSE
	BEGIN
		print 'Unique ID was not specified, assuming a network discovery asset'
		SET @nMatchID = isnull((SELECT MIN(_ASSETID) from ASSETS where _NAME=@cName) ,0)
	END

-- Return the Match ID as the found asset if any
print '...exiting usp_asset_find with index ' + cast(@nMatchID as varchar(7))
RETURN @nMatchID
GO




SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--------------------------------------------------------------------------
--																		--
-- usp_asset_requestaudit											--
-- =========================											--
--																		--
--------------------------------------------------------------------------
--																		--
-- Description															--
-- ===========															--
--																		--
-- This stored procedure is part of the AuditWizard Product				--
-- It is used to set or clear the 'request audit' flag					--
--																		--
--------------------------------------------------------------------------
--																		--
-- History																--
-- =======																--
--																		--
--	26-Mar-2008	Chris Drew		Initial Version							--
--																		--
--------------------------------------------------------------------------

CREATE PROCEDURE [dbo].[usp_asset_requestaudit]
	@nAssetID int,
	@bSetOrClear bit
AS     
	SET nocount ON

BEGIN     
	UPDATE ASSETS 
		SET _REQUESTAUDIT=@bSetOrClear 
		WHERE _ASSETID = @nAssetID	
END
GO




SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--------------------------------------------------------------------------
--																		--
-- usp_asset_update_agent_status										--
-- =============================										--
--																		--
--------------------------------------------------------------------------
--																		--
-- Description															--
-- ===========															--
--																		--
-- This stored procedure is part of the AuditWizard Product				--
-- It is used to add a update a asset definititon in the database		--
-- with the current deployment state of the remote audit client for		--
-- this asset															--
--																		--
--------------------------------------------------------------------------
--																		--
-- History																--
-- =======																--
--																		--
--	12-May-2008	Chris Drew		Initial Version							--
--																		--
--------------------------------------------------------------------------

CREATE PROCEDURE [dbo].[usp_asset_update_agent_status]
	@nAssetID int,
	@nAgentStatus int	
AS     
	SET nocount ON
	UPDATE ASSETS SET _AGENT_STATUS = + cast(@nAgentStatus as varchar(4)) 
		WHERE _ASSETID = + cast(@nAssetID as varchar(16))
GO




set ANSI_NULLS ON
set QUOTED_IDENTIFIER ON
GO
--------------------------------------------------------------------------
--																		--
-- usp_asset_update_stock_status										--
-- =============================										--
--																		--
--------------------------------------------------------------------------
--																		--
-- Description															--
-- ===========															--
--																		--
-- This stored procedure is part of the AuditWizard Product				--
-- It is used to add a update a asset definititon in the database		--
-- with the current stock state											--
--																		--
--------------------------------------------------------------------------
--																		--
-- History																--
-- =======																--
--																		--
--	12-May-2008	Chris Drew		Initial Version							--
--																		--
--------------------------------------------------------------------------

CREATE PROCEDURE [dbo].[usp_asset_update_stock_status]
	@nAssetID int,
	@nStockStatus int	
AS     
	SET nocount ON
	UPDATE ASSETS SET _STOCK_STATUS = @nStockStatus
		WHERE _ASSETID = @nAssetID
GO




SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--------------------------------------------------------------------------
--																		--
-- usp_asset_set_lastauditdate											--
-- ===========================											--
--																		--
--------------------------------------------------------------------------
--																		--
-- Description															--
-- ===========															--
--																		--
-- This stored procedure is part of the AuditWizard Product			--
-- It is used to set the last audit date for the specified asset		--
--																		--
--------------------------------------------------------------------------
--																		--
-- History																--
-- =======																--
--																		--
--	18-Feb-2006	Chris Drew		Initial Version							--
--																		--
--------------------------------------------------------------------------

CREATE PROCEDURE [dbo].[usp_asset_set_lastauditdate]
	@nAssetID int,
	@dtDate	DateTime
AS     
	DECLARE @cSQL varchar(510)
	SET nocount ON

	UPDATE ASSETS SET _LASTAUDIT=@dtDate WHERE _ASSETID = @nAssetID

GO




SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--------------------------------------------------------------------------
--																		--
-- usp_asset_set_parent													--
-- ====================													--
--																		--
--------------------------------------------------------------------------
--																		--
-- Description															--
-- ===========															--
--																		--
-- This stored procedure is part of the AuditWizard Product				--
-- It is used to set the parent for the specified asset					--
--																		--
--------------------------------------------------------------------------
--																		--
-- History																--
-- =======																--
--																		--
--	18-Feb-2006	Chris Drew		Initial Version							--
--																		--
--------------------------------------------------------------------------

CREATE PROCEDURE [dbo].[usp_asset_set_parent]
	@nAssetID int,
	@nParentID int
AS     
	DECLARE @cSQL varchar(510)
	SET nocount ON

	UPDATE ASSETS SET _PARENT_ASSETID = @nParentID WHERE _ASSETID = @nAssetID
GO




SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--------------------------------------------------------------------------
--																		--
-- usp_asset_hide													--
-- =================													--
--																		--
--------------------------------------------------------------------------
--																		--
-- Description															--
-- ===========															--
--																		--
-- This stored procedure is part of the AuditWizard Product				--
-- It is used to hide or show the specified asset					--
--																		--
--------------------------------------------------------------------------
--																		--
-- History																--
-- =======																--
--																		--
--	18-Feb-2006	Chris Drew		Initial Version							--
--																		--
--------------------------------------------------------------------------

CREATE PROCEDURE [dbo].[usp_asset_hide]
	@nAssetID int,
	@bHideOrShow bit
AS     
	SET nocount ON

BEGIN
	UPDATE ASSETS SET _HIDDEN=@bHideOrShow WHERE _ASSETID = cast(@nAssetID as varchar(16))	
END
GO




SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--------------------------------------------------------------------------
--																		--
-- usp_asset_getcount												--
-- =====================												--
--																		--
--------------------------------------------------------------------------
--																		--
-- Description															--
-- ===========															--
--																		--
-- This stored procedure is part of the AuditWizard Product				--
-- It is used to return a count of assets in the database			--
--																		--
--------------------------------------------------------------------------
--																		--
-- History																--
-- =======																--
--																		--
--	18-Feb-2006	Chris Drew		Initial Version							--
--																		--
--------------------------------------------------------------------------

CREATE PROCEDURE [dbo].[usp_asset_getcount]
	@bAuditedOnly bit,
	@bVisibleOnly bit
AS     
	DECLARE @cSQL as varchar(255)
	DECLARE @cWHERE as varchar(255)
	DECLARE @nReturnCount int
	
	SET nocount ON

	-- Base command
	SET @cSQL = 'SELECT count(*) FROM ASSETS'
	SET @cWHERE = ''
	
	-- Add on where clauses as appropriate
	IF (@bVisibleOnly = 1) 
		BEGIN 
			IF (@bAuditedOnly = 1)
				-- Visible and audited only
				SET @nReturnCount = (SELECT count(*) FROM ASSETS WHERE _HIDDEN=0 AND _LASTAUDIT <> '')
			ELSE
				-- Visible but whether audited or not
				SET @nReturnCount = (SELECT count(*) FROM ASSETS WHERE _HIDDEN=0)
		END
	ELSE
		BEGIN	
			IF (@bAuditedOnly = 1)

				-- audited only but ignore hidden/visible
				SET @nReturnCount = (SELECT count(*) FROM assets WHERE _lastaudit <> '')
			ELSE
				-- All assets regardles of hidden or audited
				SET @nReturnCount = (SELECT count(*) FROM assets)
		END
		
	RETURN @nReturnCount
GO






SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Chris Drew
-- Create date: 6/11/07
-- Description:	Returns a table of assets
-- =============================================
CREATE PROCEDURE [dbo].[usp_asset_enumerate]
	@nLocationID	int,
	@nDomainID		int,
	@showStock		bit,
	@showinUse		bit,
	@showPending	bit,
	@showDisposed	bit
AS
BEGIN
	DECLARE @cSql varchar(1024)
	DECLARE @cWhere varchar(1024)

	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	-- Create the basic select statement
	SET @cSql = 'SELECT ASSETS.*
					 , ASSET_TYPES._NAME AS ASSETTYPENAME 
					 , ASSET_TYPES._ICON AS ICON
					 , ASSET_TYPES._AUDITABLE AS AUDITABLE
					 , LOCATIONS._NAME AS LOCATIONNAME
					 , LOCATIONS._FULLNAME AS FULLLOCATIONNAME
					 , DOMAINS._NAME AS DOMAINNAME				
					 , ISNULL(SUPPLIERS._NAME, '''') SUPPLIER_NAME
				FROM ASSETS 
					LEFT JOIN ASSET_TYPES ON (ASSETS._ASSETTYPEID=ASSET_TYPES._ASSETTYPEID)
					LEFT JOIN LOCATIONS   ON (ASSETS._LOCATIONID=LOCATIONS._LOCATIONID)
					LEFT JOIN DOMAINS	  ON (ASSETS._DOMAINID=DOMAINS._DOMAINID)
					LEFT JOIN SUPPLIERS   ON (ASSETS._SUPPLIERID = SUPPLIERS._SUPPLIERID)
				WHERE ASSETS._PARENT_ASSETID=0'
				
	-- Are we filtering on Location ID?
	IF (@nLocationID <> 0)
		SET @cSql = @cSql + ' AND ASSETS._LOCATIONID=' + cast(@nLocationID as varchar(16))	

	ELSE IF (@nDomainID <> 0)
		SET @cSql = @cSql + ' AND ASSETS._DOMAINID=' + cast(@nDomainID as varchar(16))	

	-- Add on the different type of asset state
	SET @cWhere = ''
	IF (@showStock = 1)
		SET @cWhere = ' AND (_STOCK_STATUS=0'

	-- add on 'Show Inuse'
	IF (@showInUse = 1)
	BEGIN
		IF @cWhere = ''
			SET @cWhere = ' AND (_STOCK_STATUS=1'
		ELSE 
			SET @cWhere = @cWhere + ' OR _STOCK_STATUS=1'
	END

	-- add on show 'pending disposal'
	IF (@showPending = 1)
	BEGIN
		IF @cWhere = ''
			SET @cWhere = ' AND (_STOCK_STATUS=2'
		ELSE 
			SET @cWhere = @cWhere + ' OR _STOCK_STATUS=2'
	END

	IF (@showDisposed = 1)
	BEGIN
		IF @cWhere = ''
			SET @cWhere = ' AND (_STOCK_STATUS=3'
		ELSE 
			SET @cWhere = @cWhere + ' OR _STOCK_STATUS=3'
	END

	SET @cSql = @cSql + @cWhere + ')'
	print @cSql

	-- Order by the asset name
	SET @cSql = @cSql + ' ORDER BY ASSETS._NAME'

	-- Now execute this statement
	EXEC (@cSql)
END
GO




SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Chris Drew
-- Create date: 6/11/07
-- Description:	Returns a table of assets which have the AuditAgent deployed on them
-- =============================================
CREATE PROCEDURE [dbo].[usp_asset_enumerate_deployed]
AS
BEGIN

	SELECT ASSETS.*
		 , ASSET_TYPES._NAME AS ASSETTYPENAME 
		 , ASSET_TYPES._ICON AS ICON
		 , ASSET_TYPES._AUDITABLE AS AUDITABLE
		 , LOCATIONS._NAME AS LOCATIONNAME
		 , LOCATIONS._FULLNAME AS FULLLOCATIONNAME
		 , DOMAINS._NAME AS DOMAINNAME				
		 , ISNULL(SUPPLIERS._NAME, '''') SUPPLIER_NAME
	FROM ASSETS 
		LEFT JOIN ASSET_TYPES ON (ASSETS._ASSETTYPEID=ASSET_TYPES._ASSETTYPEID)
		LEFT JOIN LOCATIONS   ON (ASSETS._LOCATIONID=LOCATIONS._LOCATIONID)
		LEFT JOIN DOMAINS	  ON (ASSETS._DOMAINID=DOMAINS._DOMAINID)
		LEFT JOIN SUPPLIERS   ON (ASSETS._SUPPLIERID = SUPPLIERS._SUPPLIERID)
	WHERE _AGENT_STATUS <> 0

END
GO



SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Chris Drew
-- Create date: 6/11/07
-- Description:	Returns a table of assets
-- =============================================
CREATE PROCEDURE [dbo].[usp_asset_enumerate_alerts_disabled]
AS
BEGIN

	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	-- Create the basic select statement
	SELECT _ASSETID ,_NAME
	FROM ASSETS 
	WHERE ASSETS._ALERTS_ENABLED = 0
END
GO


SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Chris Drew
-- Create date: 6/11/07
-- Description:	Returns a table of child assets
-- =============================================
CREATE PROCEDURE [dbo].[usp_childasset_enumerate]
	@nAssetID	int
AS
BEGIN

	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	-- Create the basic select statement
	SELECT ASSETS.*
		 , ASSET_TYPES._NAME AS ASSETTYPENAME 
		 , ASSET_TYPES._ICON AS ICON
		 , ASSET_TYPES._AUDITABLE AS AUDITABLE
		 , LOCATIONS._NAME AS LOCATIONNAME
		 , LOCATIONS._FULLNAME AS FULLLOCATIONNAME
		 , DOMAINS._NAME AS DOMAINNAME				
		 , ISNULL(SUPPLIERS._NAME, '''') SUPPLIER_NAME
	FROM ASSETS 
		LEFT JOIN ASSET_TYPES ON (ASSETS._ASSETTYPEID=ASSET_TYPES._ASSETTYPEID)
		LEFT JOIN LOCATIONS   ON (ASSETS._LOCATIONID=LOCATIONS._LOCATIONID)
		LEFT JOIN DOMAINS	  ON (ASSETS._DOMAINID=DOMAINS._DOMAINID)
		LEFT JOIN SUPPLIERS   ON (ASSETS._SUPPLIERID = SUPPLIERS._SUPPLIERID)
	WHERE ASSETS._PARENT_ASSETID=@nAssetID

END
GO


SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--------------------------------------------------------------------------
--																		--
-- usp_asset_add														--
-- ================														--
--																		--
--------------------------------------------------------------------------
--																		--
-- Description															--
-- ===========															--
--																		--
-- This stored procedure is part of the AuditWizard Product			--
-- It is used to add a asset definititon in the database				--
--																		--
-- Character strings passed to this function must be raw and not SQL    --
--																		--
--------------------------------------------------------------------------
--																		--
-- History																--
-- =======																--
--																		--
--	18-Feb-2006	Chris Drew		Initial Version							--
--																		--
--------------------------------------------------------------------------

CREATE PROCEDURE [dbo].[usp_asset_add]
	@cUniqueID varchar(255),
	@cName varchar(255),
	@nLocationID int,
	@nDomainID int,
	@cIPAddress varchar(255),
	@cMACAddress varchar(255),
	@nAssetTypeID int,
	@cMake varchar(255),
	@cModel varchar(255),
	@cSerial varchar(255),
	@nParentAssetID int,
	@nSupplierID int,
	@nStockStatus int,
	@bAlertsEnabled bit
AS

DECLARE @nRetCode int,
	@cColumns varchar(1000),
	@cValues varchar(1000),
	@nReturnID int
 
SET nocount ON

BEGIN

	-- First of all lets see if this asset already exists
	EXEC @nReturnID = usp_asset_find @cName, @cUniqueID

	-- If the asset does not already exist then add it
	IF (@nReturnID = 0)
	BEGIN 			                         

		INSERT INTO ASSETS
			(_UNIQUEID ,_NAME ,_LOCATIONID ,_DOMAINID ,_IPADDRESS ,_MACADDRESS ,_ASSETTYPEID ,_MAKE ,_MODEL ,_SERIAL_NUMBER ,_PARENT_ASSETID ,_SUPPLIERID ,_STOCK_STATUS ,_ALERTS_ENABLED)
		VALUES
			(@cUniqueID, @cName, @nLocationID ,@nDomainID ,@cIPAddress ,@cMACAddress ,@nAssetTypeID ,@cMake ,@cModel ,@cSerial ,@nParentAssetID ,@nSupplierID ,@nStockStatus ,@bAlertsEnabled)

		SET @nReturnID = @@IDENTITY
	END

	ELSE
		BEGIN
			EXEC usp_asset_update @nReturnID ,@cName ,@nLocationID ,@nDomainID ,@cIPAddress ,@cMACAddress ,@nAssetTypeID ,@cMake ,@cModel ,@cSerial, @nParentAssetID ,@nSupplierID ,@nStockStatus ,@bAlertsEnabled
		END
END
RETURN  @nReturnID
GO




SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--------------------------------------------------------------------------
--																		--          	
-- usp_add_installed_application										--
-- =============================										--
--																		--
--------------------------------------------------------------------------
--																		--
-- Description															--
-- ===========															--
--																		--
-- This stored procedure is part of the AuditWizard Product				--
-- It is used to add a new Installed Application to the database	 	--
--																		--
-- There are two parts to this operation								--
--	1> Create a new Application (or get an existing one)				--
--  2> Create a new application instance (or update an existing one		--
--																		--
--	NOTE: THIS STORED PROCEDURE ONLY DEALS WITH APPLICATIONS			--
--		  OPERATING SYSTEMS ARE DEALT WITH SEPARATELY					--
--																		--
--	PASS RAW STRINGS INTO THIS FUNCTION -DO NOT MAKE THEM SQL COMPLIENT --
--																		--
--------------------------------------------------------------------------
--																		--
-- History																--
-- =======																--
--																		--
--	18-Feb-2008		Chris Drew		Initial Version						--
--																		--
--------------------------------------------------------------------------

CREATE PROCEDURE [dbo].[usp_add_installed_application]
	@nAssetID int,
	@cPublisher varchar(255),
	@cApplication varchar(255),
	@cVersion varchar(255),
	@cGuid varchar(255),
	@cProductID varchar(255),
	@cCDKey varchar(255),
	@nReturnApplicationID int output,
	@nReturnInstanceID int output
AS

DECLARE @nRetCode int
SET nocount ON
set @nRetCode = 0 

BEGIN
	-- First of all add the application to the database or recover the id of an 
	-- existing entry
	EXEC usp_add_application @cPublisher, @cApplication ,@cVersion ,@cGuid ,0 ,0 ,@nReturnApplicationID output

	-- If we added (or found an existing application) add an instance for this application
	if (@nReturnApplicationID <> 0)
		BEGIN
			EXEC @nRetCode = usp_add_application_instance @nAssetID ,@nReturnApplicationID ,@cProductID ,@cCDKey ,@nReturnInstanceID output
		END
END

RETURN  @nRetCode
GO




SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--------------------------------------------------------------------------
--																		--          	
-- usp_add_installed_os													--
-- ====================													--
--																		--
--------------------------------------------------------------------------
--																		--
-- Description															--
-- ===========															--
--																		--
-- This stored procedure is part of the AuditWizard Product				--
-- It is used to add a new Installed OS to the database	 				--
--																		--
-- There are two parts to this operation								--
--	1> Create a new OS (or get an existing one)							--
--  2> Create a new OS instance (or update an existing one				--
--																		--
--	NOTE: THIS STORED PROCEDURE ONLY DEALS WITH OPERATING SYSTEMS,		--
--        APPLICATIONS ARE DEALT WITH SEPARATELY						--
--																		--
--------------------------------------------------------------------------
--																		--
-- History																--
-- =======																--
--																		--
--	18-Feb-2008		Chris Drew		Initial Version						--
--																		--
--------------------------------------------------------------------------

CREATE PROCEDURE [dbo].[usp_add_installed_os]
	@nAssetID int,
	@cName varchar(255),
	@cVersion varchar(255),
	@cProductID varchar(255),
	@cCDKey varchar(255),
	@nReturnOSID int output,
	@nReturnInstanceID int output
AS

DECLARE @nRetCode int

SET nocount ON
set @nRetCode = 0 

BEGIN
	-- First of all add the OS to the database or recover the id of an 
	-- existing entry
	EXEC usp_add_os @cName ,@cVersion ,@nReturnOSID output

	-- If we added (or found an existing OS add an instance for this OS
	-- Note that we call the generic add_application_instance SP for this
	if (@nReturnOSID <> 0)
		BEGIN
			EXEC @nRetCode = usp_add_application_instance @nAssetID ,@nReturnOSID ,@cProductID ,@cCDKey ,@nReturnInstanceID output
		END
END

RETURN  @nRetCode
GO


SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--------------------------------------------------------------------------
--																		--
-- usp_asset_delete_tables												--
-- =======================												--
--																		--
--------------------------------------------------------------------------
--																		--
-- Description															--
-- ===========															--
--																		--
-- This stored procedure is part of the AuditWizard Product				--
-- It is used to add a DELETE the related tables for a specific asset	--
--																		--
--------------------------------------------------------------------------
--																		--
-- History																--
-- =======																--
--																		--
--	29-Apr-2009	Chris Drew		Initial Version							--
--																		--
--------------------------------------------------------------------------

CREATE PROCEDURE [dbo].[usp_asset_delete_tables]
	@nAssetID int
AS    
BEGIN 
	SET nocount ON

	-- Delete related entries in the APPLICATION_INSTANCES table
	-- Note that this may leave unresolved references in the Applications table which we will have to handle also
	DELETE FROM APPLICATION_INSTANCES WHERE _ASSETID=@nAssetID

	-- Clean up any applications orphaned by the above
	EXEC usp_application_deleteorphans    
    
	-- Delete related entries in the AuditedItems table
	DELETE FROM AUDITEDITEMS WHERE _ASSETID=@nAssetID

	-- Delete related entries in the AUDITTRAIL table
	DELETE FROM AUDITTRAIL WHERE _ASSETID=@nAssetID

	-- Delete related entries in the DOCUMENTS table
	DELETE FROM DOCUMENTS WHERE _SCOPE=0 AND _PARENTID=@nAssetID

	-- Delete related entries in the NOTES table
	DELETE FROM NOTES WHERE _SCOPE=0 AND _PARENTID=@nAssetID

	-- Delete related entries in the OPERATIONS table
	DELETE FROM OPERATIONS WHERE _ASSETID=@nAssetID
  
	-- Delete related entries in the UserData_values table
	DELETE FROM USERDATA_VALUES WHERE _PARENTTYPE=0 AND _PARENTID=@nAssetID

	-- Delete related entries in the FS_FILES table
	DELETE FROM FS_FILES WHERE _ASSETID=@nAssetID

	-- Delete related entries in the FS_FOLDERS table
	DELETE FROM FS_FOLDERS WHERE _ASSETID=@nAssetID

	-- Delete related entries in the ALERTS table
	DELETE FROM ALERTS WHERE _ASSETNAME = (SELECT _NAME FROM ASSETS WHERE _ASSETID=@nAssetID)

	-- Now we can delete the asset itself
	DELETE FROM ASSETS WHERE _ASSETID=@nAssetID

END
GO



SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--------------------------------------------------------------------------
--																		--
-- usp_asset_delete													--
-- ===================													--
--																		--
--------------------------------------------------------------------------
--																		--
-- Description															--
-- ===========															--
--																		--
-- This stored procedure is part of the AuditWizard Product				--
-- It is used to add a DELETE a asset definititon in the database	--
--																		--
--------------------------------------------------------------------------
--																		--
-- History																--
-- =======																--
--																		--
--	18-Feb-2006	Chris Drew		Initial Version							--
--																		--
--------------------------------------------------------------------------

CREATE PROCEDURE [dbo].[usp_asset_delete]
	@nAssetID int
AS    
BEGIN 
	SET nocount ON
	DECLARE @nChildAssetID int

	-- Referential Integrity Checks
	-- ============================ 
	--
	-- ASSETS are linked to
	--  CHILD ASSETS !!!!
	--	APPLICATION_INSTANCES
	--	AUDITEDITEMS
	--	AUDITTRAIL
	--	DOCUMENTS
	--	NOTES
	--	OPERATIONS
	--	USERDATA_VALUES
	--	FS_FILES
	--  FS_FOLDER
	--	ALERTS
	--
	-- We need to delete any child assets first as we cannot leave these orphaned
	DECLARE cursorFiles CURSOR FAST_FORWARD FOR 
		SELECT _ASSETID FROM ASSETS WHERE _PARENT_ASSETID = @nAssetID

	OPEN cursorFiles 
	FETCH NEXT FROM cursorFiles INTO @nChildAssetID

		WHILE @@FETCH_STATUS = 0  
		BEGIN 
			-- OK - for this asset and application create an application instance
			IF (@nChildAssetID <> 0)
				EXEC usp_asset_delete_tables @nChildAssetID

			-- Fetch the next record
			FETCH NEXT FROM cursorFiles INTO @nChildAssetID
		END
	CLOSE cursorFiles 
	DEALLOCATE cursorFiles

	-- OK - now delete all of the tables for the parent asset
	EXEC usp_asset_delete_tables @nAssetID
END 
GO	





SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--------------------------------------------------------------------------
--																		--
-- usp_get_security														--
-- ================														--
--																		--
--------------------------------------------------------------------------
--																		--
-- Description															--
-- ===========															--
--																		--
-- This stored procedure is part of the AuditWizard Product			--
-- It is used to return whether or not security is currently enabled	--
--																		--
--------------------------------------------------------------------------
--																		--
-- History																--
-- =======																--
--																		--
--	26-Aug-2008	Chris Drew		Initial Version							--
--																		--
--------------------------------------------------------------------------

CREATE PROCEDURE [dbo].[usp_get_security]
	@bEnabled bit output
	
AS     
	SET nocount ON 
BEGIN
	DECLARE @cValue varchar(255)
	EXEC [dbo].[usp_settings_getsetting] 'SecurityEnabled' ,@cValue output
	if (@cValue = 'True')
		SET @bEnabled = 1
	else
		SET @bEnabled = 0
END
GO


SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--------------------------------------------------------------------------
--																		--
-- usp_set_security														--
-- ================														--
--																		--
--------------------------------------------------------------------------
--																		--
-- Description															--
-- ===========															--
--																		--
-- This stored procedure is part of the AuditWizard Product			--
-- It is used to recover the current security state						--
--																		--
--------------------------------------------------------------------------
--																		--
-- History																--
-- =======																--
--																		--
--	25-Aug-2008	Chris Drew		Initial Version							--
--																		--
--------------------------------------------------------------------------

CREATE PROCEDURE [dbo].[usp_set_security]
	@bEnable bit
	
AS     
	SET nocount ON 
BEGIN
	IF (@bEnable = 0)
		EXEC [dbo].[usp_settings_setsetting] 'SecurityEnabled' ,'False'
	ELSE
		EXEC [dbo].[usp_settings_setsetting] 'SecurityEnabled' ,'True'
END
GO





SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--------------------------------------------------------------------------
--																		--
-- usp_assettypes_enumerate												--
-- ========================												--
--																		--
--------------------------------------------------------------------------
--																		--
-- Description															--
-- ===========															--
--																		--
-- This stored procedure is part of the AuditWizard Product			--
-- It is used to return a list of asset type entries					--
--																		--
--------------------------------------------------------------------------
--																		--
-- History																--
-- =======																--
--																		--
--	16-Sep-2008	Chris Drew		Initial Version							--
--																		--
--------------------------------------------------------------------------

CREATE PROCEDURE [dbo].[usp_assettype_enumerate]
AS 
	SET nocount ON  
	
BEGIN
	SELECT ASSET_TYPES.*
	FROM dbo.ASSET_TYPES
	ORDER BY _ASSETTYPEID
END
GO




SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--------------------------------------------------------------------------
--																		--
-- usp_assettype_add													--
-- =================													--
--																		--
--------------------------------------------------------------------------
--																		--
-- Description															--
-- ===========															--
--																		--
-- This stored procedure is part of the AuditWizard Product			--
-- It is used to add an Asset Type to the database						--
--																		--
-- Character strings passed to this function must be SQL prepared	    --
--																		--
--------------------------------------------------------------------------
--																		--
-- History																--
-- =======																--
--																		--
--	15-Sep-2008	Chris Drew		Initial Version							--
--																		--
--------------------------------------------------------------------------

CREATE PROCEDURE [dbo].[usp_assettype_add]
	@cName varchar(255),
	@nParentID int,
	@bAuditable bit,
	@cIcon varchar(255),
	@nReturnID int output
AS

DECLARE @nRetCode int

SET nocount ON
SET @nRetCode = 0 

BEGIN

	INSERT INTO ASSET_TYPES
		(_NAME ,_PARENTID ,_AUDITABLE ,_ICON)
	VALUES
		(@cName, @nParentID, @bAuditable ,@cIcon)

	SET @nReturnID = @@IDENTITY

END
RETURN  @nRetCode
GO





SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--------------------------------------------------------------------------
--																		--
-- usp_assettype_delete													--
-- ==================													--
--																		--
--------------------------------------------------------------------------
--																		--
-- Description															--
-- ===========															--
--																		--
-- This stored procedure is part of the AuditWizard Product			--
-- It is used to add a DELETE an Asset Type definititon 				--
--																		--
--------------------------------------------------------------------------
--																		--
-- History																--
-- =======																--
--																		--
--	18-Feb-2006	Chris Drew		Initial Version							--
--																		--
--------------------------------------------------------------------------

CREATE PROCEDURE [dbo].[usp_assettype_delete]
	@nAssetTypeID int
AS    

	DECLARE @nParentID int,
			@nReferences int,
			@nRetCode int

	-- Assume success
	SET @nRetCode = 0

	-- Get the parent of this asset type if any
	SET @nParentID = (SELECT _PARENTID FROM ASSET_TYPES WHERE _ASSETTYPEID=@nAssetTypeID)

	-- If the parent is null then all we need to worry about is any references to this Category
	-- within the ASSET_TYPES table
	IF (@nParentID = null)
		BEGIN
			SET  @nReferences = isnull((SELECT min(_ASSETTYPEID) FROM ASSET_TYPES WHERE _PARENTID = @nAssetTypeID) ,0)
		END
	ELSE
		BEGIN
			SET  @nReferences = isnull((SELECT min(_ASSETID) FROM ASSETS WHERE _ASSETTYPEID=@nAssetTypeID) ,0)
		END

	-- If we have references then we return an error status otherwise we do the delete and return success	
	IF (@nReferences <> 0)
		BEGIN
			SET @nRetCode = -1
		END
	ELSE
		BEGIN
			DELETE FROM ASSET_TYPES WHERE _ASSETTYPEID = @nAssetTypeID
		END

	RETURN @nRetCode
GO
 
 
 
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--------------------------------------------------------------------------
--																		--
-- usp_assettype_update													--
-- ====================													--
--																		--
--------------------------------------------------------------------------
--																		--
-- Description															--
-- ===========															--
--																		--
-- This stored procedure is part of the AuditWizard Product			--
-- It is used to add a update an Asset Type definititon 				--
--																		--
--------------------------------------------------------------------------
--																		--
-- History																--
-- =======																--
--																		--
--	18-Sep-2008	Chris Drew		Initial Version							--
--																		--
--------------------------------------------------------------------------

CREATE PROCEDURE [dbo].[usp_assettype_update]
	@nAssetTypeID int,
	@cName varchar(255),
	@nParentID int,
	@cIcon varchar(255)
	
AS     
BEGIN
	UPDATE ASSET_TYPES 
		SET 
			_NAME=@cName,
			_PARENTID=@nParentID,
			_ICON=@cIcon
		WHERE
			_ASSETTYPEID = @nAssetTypeID
END
GO




SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--------------------------------------------------------------------------
--																		--
-- usp_udd_enumerate													--
-- =================													--
--																		--
--------------------------------------------------------------------------
--																		--
-- Description															--
-- ===========															--
--																		--
-- This stored procedure is part of the AuditWizard Product				--
-- It is used to return a list of User Defined Data Definitions			--
-- with the specified scope												--
--																		--
--------------------------------------------------------------------------
--																		--
-- History																--
-- =======																--
--																		--
--	10-Dec-2008	Chris Drew		Initial Version							--
--																		--
--------------------------------------------------------------------------

CREATE PROCEDURE [dbo].[usp_udd_enumerate]
	@nScope int
AS 
	SET nocount ON  
	
BEGIN
	IF (@nScope = -1)
		SELECT USERDATA_DEFINITIONS.*
				,ASSET_TYPES._NAME  AS ASSETTYPENAME
		FROM dbo.USERDATA_DEFINITIONS
		LEFT JOIN ASSET_TYPES on USERDATA_DEFINITIONS._APPLIESTO = ASSET_TYPES._ASSETTYPEID
		ORDER BY USERDATA_DEFINITIONS._PARENTID
	ELSE
		SELECT USERDATA_DEFINITIONS.*
				,ASSET_TYPES._NAME  AS ASSETTYPENAME
		FROM dbo.USERDATA_DEFINITIONS
		LEFT JOIN ASSET_TYPES on USERDATA_DEFINITIONS._APPLIESTO = ASSET_TYPES._ASSETTYPEID
		WHERE _SCOPE=@nScope
		ORDER BY USERDATA_DEFINITIONS._PARENTID
END
GO




SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--------------------------------------------------------------------------
--																		--
-- usp_udd_add															--
-- ===========															--
--																		--
--------------------------------------------------------------------------
--																		--
-- Description															--
-- ===========															--
--																		--
-- This stored procedure is part of the AuditWizard Product				--
-- It is used to add a User Data Definition to the database				--
--																		--
-- Character strings passed to this function must be SQL prepared	    --
--																		--
--------------------------------------------------------------------------
--																		--
-- History																--
-- =======																--
--																		--
--	15-Sep-2008	Chris Drew		Initial Version							--
--																		--
--------------------------------------------------------------------------

CREATE PROCEDURE [dbo].[usp_udd_add]
	@cName 		 varchar(255), 
	@nParentID	int,
	@bIsMandatory int,
	@nType		int,
	@nAppliesTo	int,
	@cValue1	varchar(255),
	@cValue2	varchar(255),
	@nTabOrder	int,
	@cIcon		varchar(255),
	@nScope		int,
	@nReturnID 	int output
AS

DECLARE @nRetCode int

SET nocount ON
SET @nRetCode = 0 

BEGIN

	INSERT INTO USERDATA_DEFINITIONS
		(_NAME, _PARENTID ,_ISMANDATORY ,_TYPE ,_APPLIESTO ,_VALUE1 ,_VALUE2 ,_TABORDER ,_ICON ,_SCOPE)
	VALUES
		(@cName, @nParentID ,@bIsMandatory ,@nType ,@nAppliesTo ,@cValue1 ,@cValue2 ,@nTabOrder ,@cIcon ,@nScope)

	SET @nReturnID = @@IDENTITY

END
RETURN  @nRetCode
GO
  
  
  

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--------------------------------------------------------------------------
--																		--
-- usp_udd_update														--
-- ==============														--
--																		--
--------------------------------------------------------------------------
--																		--
-- Description															--
-- ===========															--
--																		--
-- This stored procedure is part of the AuditWizard Product			--
-- It is used to update a User Data Definition to the database			--
--																		--
-- Character strings passed to this function must be SQL prepared	    --
--																		--
--------------------------------------------------------------------------
--																		--
-- History																--
-- =======																--
--																		--
--	17-Sep-2008	Chris Drew		Initial Version							--
--																		--
--------------------------------------------------------------------------

CREATE PROCEDURE [dbo].[usp_udd_update]
	@nUserDefID		int,
	@nCategoryID	int,
	@cName 			varchar(255), 
	@bIsMandatory	int,
	@nType			int,
	@nAppliesTo		int,
	@cValue1		varchar(255),
	@cValue2		varchar(255),
	@nTabOrder		int,
	@cIcon			varchar(255)

AS

DECLARE @nRetCode int

SET nocount ON
SET @nRetCode = 0 

BEGIN

	UPDATE USERDATA_DEFINITIONS SET
		_NAME = @cName,
		_PARENTID = @nCategoryID,
		_ISMANDATORY = @bIsMandatory,
		_TYPE = @nType,
		_APPLIESTO = @nAppliesTo,
		_VALUE1 = @cValue1,
		_VALUE2 = @cValue2,
		_TABORDER = @nTabOrder,
		_ICON = @cIcon
	WHERE
		_USERDEFID = @nUserDefID

END
RETURN  @nRetCode
GO



SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--------------------------------------------------------------------------
--																		--
-- usp_udd_delete														--
-- ==============														--
--																		--
--------------------------------------------------------------------------
--																		--
-- Description															--
-- ===========															--
--																		--
-- This stored procedure is part of the AuditWizard Product			--
-- It is used to add a DELETE a User Data definititon 					--
--																		--
--------------------------------------------------------------------------
--																		--
-- History																--
-- =======																--
--																		--
--	16-Sep-2008	Chris Drew		Initial Version							--
--																		--
--------------------------------------------------------------------------

CREATE PROCEDURE [dbo].[usp_udd_delete]
	@nUserDataID int
AS     
	DECLARE @nParentID int,
			@nReferences int,
			@nRetCode int

	-- Assume success
	SET @nRetCode = 0

	-- Get the parent of this user data category/field if none then this is a category
	-- and we only need to worry about if there are any child fields
	SET @nParentID = (SELECT _PARENTID FROM USERDATA_DEFINITIONS WHERE _USERDEFID=@nUserDataID)

	-- If the parent is null then all we need to worry about is any references to this Category
	-- within the ASSET_TYPES table
	IF (@nParentID = null)
		BEGIN
			SET  @nReferences = isnull((SELECT min(_USERDEFID) FROM USERDATA_DEFINITIONS WHERE _PARENTID = @nUserDataID) ,0)
		END
	ELSE
		-- Not a category so check for references to this field
		BEGIN
			SET  @nReferences = isnull((SELECT min(_USERVALUEID) FROM USERDATA_VALUES WHERE _PARENTID=@nUserDataID) ,0)
		END

	-- If we have references then we return an error status otherwise we do the delete and return success	
	IF (@nReferences <> 0)
		BEGIN
			SET @nRetCode = -1
		END
	ELSE
		BEGIN
			DELETE FROM USERDATA_DEFINITIONS WHERE _USERDEFID = @nUserDataID
		END

	RETURN @nRetCode
GO
   
   
   




SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--------------------------------------------------------------------------
--																		--
-- usp_picklist_enumerate												--
-- ======================												--
--																		--
--------------------------------------------------------------------------
--																		--
-- Description															--
-- ===========															--
--																		--
-- This stored procedure is part of the AuditWizard Product			--
-- It is used to return a list of PickList and PickItem entries			--
--																		--
--------------------------------------------------------------------------
--																		--
-- History																--
-- =======																--
--																		--
--	19-Sep-2008	Chris Drew		Initial Version							--
--																		--
--------------------------------------------------------------------------

CREATE PROCEDURE [dbo].[usp_picklist_enumerate]
AS 
	SET nocount ON  
	
BEGIN
	SELECT _PICKLISTID, _NAME ,_PARENTID 
	 FROM PICKLISTS 
	 ORDER BY _PICKLISTID
END
GO




SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--------------------------------------------------------------------------
--																		--
-- usp_pickitem_enumerate												--
-- ======================												--
--																		--
--------------------------------------------------------------------------
--																		--
-- Description															--
-- ===========															--
--																		--
-- This stored procedure is part of the AuditWizard Product				--
-- It will return a list of PickItem entries for the specified list		--
--																		--
--------------------------------------------------------------------------
--																		--
-- History																--
-- =======																--
--																		--
--	19-Sep-2008	Chris Drew		Initial Version							--
--																		--
--------------------------------------------------------------------------

CREATE PROCEDURE [dbo].[usp_pickitem_enumerate]
	@nPicklistID int
AS 
	SET nocount ON  
	
BEGIN
	SELECT _PICKLISTID, _NAME ,_PARENTID 
	 FROM PICKLISTS 
	WHERE _PARENTID = @nPicklistID
	 ORDER BY _PICKLISTID
END
GO




SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--------------------------------------------------------------------------
--																		--
-- usp_picklist_add														--
-- ================														--
--																		--
--------------------------------------------------------------------------
--																		--
-- Description															--
-- ===========															--
--																		--
-- This stored procedure is part of the AuditWizard Product			--
-- It is used to add a PickList / PickItem to the database				--
--																		--
-- Character strings passed to this function must be SQL prepared	    --
--																		--
--------------------------------------------------------------------------
--																		--
-- History																--
-- =======																--
--																		--
--	19-Sep-2008	Chris Drew		Initial Version							--
--																		--
--------------------------------------------------------------------------

CREATE PROCEDURE [dbo].[usp_picklist_add]
	@cName 		 varchar(255), 
	@nParentID	int,
	@nReturnID 	int output
AS

DECLARE @nRetCode int

SET nocount ON
SET @nRetCode = 0 

BEGIN

	INSERT INTO PICKLISTS
		(_NAME ,_PARENTID)
	VALUES
		(@cName, @nParentID)

	SET @nReturnID = @@IDENTITY

END
RETURN  @nRetCode
GO
  
  
  

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--------------------------------------------------------------------------
--																		--
-- usp_picklist_update													--
-- ===================													--
--																		--
--------------------------------------------------------------------------
--																		--
-- Description															--
-- ===========															--
--																		--
-- This stored procedure is part of the AuditWizard Product			--
-- It is used to update a Picklist Definition to the database			--
--																		--
-- Character strings passed to this function must be SQL prepared	    --
--																		--
--------------------------------------------------------------------------
--																		--
-- History																--
-- =======																--
--																		--
--	19-Sep-2008	Chris Drew		Initial Version							--
--																		--
--------------------------------------------------------------------------

CREATE PROCEDURE [dbo].[usp_picklist_update]
	@nPicklistID int,
	@cName 		 varchar(255) 
AS

DECLARE @nRetCode int

SET nocount ON
SET @nRetCode = 0 

BEGIN

	UPDATE PICKLISTS  SET
		_NAME = @cName
	WHERE
		_PICKLISTID = @nPicklistID

END
RETURN  @nRetCode




SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--------------------------------------------------------------------------
--																		--
-- usp_picklist_delete													--
-- ===================													--
--																		--
--------------------------------------------------------------------------
--																		--
-- Description															--
-- ===========															--
--																		--
-- This stored procedure is part of the AuditWizard Product			--
-- It is used to DELETE a Picklist definititon 							--
--																		--
--------------------------------------------------------------------------
--																		--
-- History																--
-- =======																--
--																		--
--	16-Sep-2008	Chris Drew		Initial Version							--
--																		--
--------------------------------------------------------------------------

CREATE PROCEDURE [dbo].[usp_picklist_delete]
	@nPicklistID int
AS  
DECLARE @nRetCode int

SET nocount ON
SET @nRetCode = 0 

	BEGIN   
	DELETE FROM PICKLISTS WHERE _PICKLISTID = @nPicklistID
	END

	RETURN @nRetCode
GO   







SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--------------------------------------------------------------------------
--																		--
-- usp_set_organization													--
-- ====================													--
--																		--
--------------------------------------------------------------------------
--																		--
-- Description															--
-- ===========															--
--																		--
-- This stored procedure is part of the AuditWizard Product				--
-- It is used to set the organization name as the root name in the      --
-- domains and locations tables.										--
--																		--
--------------------------------------------------------------------------
--																		--
-- History																--
-- =======																--
--																		--
--	06-Dec-2008	Chris Drew		Initial Version							--
--																		--
--------------------------------------------------------------------------

CREATE PROCEDURE [dbo].[usp_set_organization]
	@cOrganization varchar(255)
AS
	SET nocount ON  
	
BEGIN 
	UPDATE LOCATIONS SET _FULLNAME=@cOrganization ,_NAME=@cOrganization WHERE _PARENTID is NULL
	UPDATE DOMAINS SET _NAME=@cOrganization WHERE _PARENTID is NULL
END
GO







SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--------------------------------------------------------------------------
--																		--
-- usp_locations_enumerate												--
-- ========================												--
--																		--
--------------------------------------------------------------------------
--																		--
-- Description															--
-- ===========															--
--																		--
-- This stored procedure is part of the AuditWizard Product			--
-- It is used to return a list of locations (for the specified parent)  --
--																		--
-- ParentID - ID of any parebnt location to return children of.			--
--	>0 Return children o fthis parent locations							--																		--
--	0  return top level locations										--
--	-1 return ALL locations												--
--																		--
--------------------------------------------------------------------------
--																		--
-- History																--
-- =======																--
--																		--
--	29-Sep-2008	Chris Drew		Initial Version							--
--																		--
--------------------------------------------------------------------------

CREATE PROCEDURE [dbo].[usp_locations_enumerate]  
	@nParentID int,
	@bShowAll bit
AS
	DECLARE @cSql varchar(1024) 
	DECLARE @cWhere varchar(1024) 
	SET nocount ON  
	
BEGIN 
	SET @cSql = 'SELECT _LOCATIONID, _FULLNAME, _NAME ,_PARENTID ,_START_IPADDRESS ,_END_IPADDRESS ,_HIDDEN'
		+       ' FROM dbo.LOCATIONS'

	-- Format the WHERE clause if required
	SET @cWhere = ''

	-- First we need the WHERE for the parent if specified
	IF (@nParentID > 0)
		SET @cWhere = ' WHERE _PARENTID = ' + cast(@nParentID as varchar(16))	
	ELSE IF (@nParentID = 0)
		SET @cWhere = ' WHERE _PARENTID IS NULL'	

	-- Now add on whether to show hidden locations or not
	IF (@bShowAll = 0)
	BEGIN
		IF (@cWhere = '')
			SET @cWhere = ' WHERE _HIDDEN=0'
		ELSE 
			SET @cWhere = @cWhere + ' AND _HIDDEN=0'
	END		

	-- Add on an ordering clause
	SET @cSql = @cSql + @cWhere + ' ORDER BY _NAME'

	-- ...end execute the command
	EXEC (@cSQL)
END
GO



set ANSI_NULLS ON
set QUOTED_IDENTIFIER ON
GO
--------------------------------------------------------------------------
--																		--
-- usp_locations_enumerate_all											--
-- ===========================											--
--																		--
--------------------------------------------------------------------------
--																		--
-- Description															--
-- ===========															--
--																		--
-- This stored procedure is part of the AuditWizard Product				--
-- It is used to return a list of ALL locations defined in the database --
--																		--
--------------------------------------------------------------------------
--																		--
-- History																--
-- =======																--
--																		--
--	26-Jan-2009	Chris Drew		Initial Version							--
--																		--
--------------------------------------------------------------------------

CREATE PROCEDURE [dbo].[usp_locations_enumerate_all]  
AS
	SET nocount ON  
	
BEGIN 
	
	SELECT _LOCATIONID, _FULLNAME, _NAME ,_PARENTID ,_START_IPADDRESS ,_END_IPADDRESS ,_HIDDEN
		FROM dbo.LOCATIONS
END
GO





set ANSI_NULLS ON
set QUOTED_IDENTIFIER ON
GO
--------------------------------------------------------------------------
--																		--
-- usp_location_add														--
-- ================														--
--																		--
--------------------------------------------------------------------------
--																		--
-- Description															--
-- ===========															--
--																		--
-- This stored procedure is part of the AuditWizard Product			--
-- It is used to add a Location to the database							--
--																		--
-- Character strings passed to this function must be SQL prepared	    --
--																		--
--------------------------------------------------------------------------
--																		--
-- History																--
-- =======																--
--																		--
--	02-Oct-2008	Chris Drew		Initial Version							--
--																		--
--------------------------------------------------------------------------

CREATE PROCEDURE [dbo].[usp_location_add]
	@cFullName 	 varchar(255), 
	@cName 		 varchar(255), 
	@nParentID	int,
	@cStartIP	varchar(32),
	@cEndIP		varchar(32),
	@bHidden	bit,
	@nReturnID 	int output
AS

DECLARE @nRetCode int

SET nocount ON
SET @nRetCode = 0 

BEGIN

	-- If a location with this name already exists then return its database ID 
	-- rather than inserting a new group
	SET @nReturnID = isnull((SELECT _LOCATIONID FROM LOCATIONS WHERE _FULLNAME=@cFullName),0)

	IF (@nReturnID = 0)
	BEGIN
		INSERT INTO LOCATIONS
			(_FULLNAME, _NAME ,_PARENTID ,_START_IPADDRESS ,_END_IPADDRESS ,_HIDDEN)
		VALUES
			(@cFullName ,@cName, @nParentID ,@cStartIP ,@cEndIP ,@bHidden)

		SET @nReturnID = @@IDENTITY
	END

END
RETURN  @nRetCode


  
  
  

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--------------------------------------------------------------------------
--																		--
-- usp_location_update													--
-- ===================													--
--																		--
--------------------------------------------------------------------------
--																		--
-- Description															--
-- ===========															--
--																		--
-- This stored procedure is part of the AuditWizard Product			--
-- It is used to update a Location in the database			--
--																		--
-- Character strings passed to this function must be SQL prepared	    --
--																		--
--------------------------------------------------------------------------
--																		--
-- History																--
-- =======																--
--																		--
--	19-Sep-2008	Chris Drew		Initial Version							--
--																		--
--------------------------------------------------------------------------

CREATE PROCEDURE [dbo].[usp_location_update]
	@nLocationID int,
	@cFullName 	 varchar(255),
	@cName 		 varchar(255),
	@nParentID	int,
	@cStartIP	varchar(32),
	@cEndIP		varchar(32),
	@bHidden	bit
AS

DECLARE @nRetCode int

SET nocount ON
SET @nRetCode = 0 

BEGIN

	UPDATE LOCATIONS  SET 
		_FULLNAME	= @cFullName,
		_NAME		= @cName, 
		_PARENTID	= @nParentID,
		_START_IPADDRESS = @cStartIP,
		_END_IPADDRESS	= @cEndIP,
		_HIDDEN		= @bHidden
	WHERE
		_LOCATIONID = @nLocationID

END
RETURN  @nRetCode
GO



SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--------------------------------------------------------------------------
--																		--
-- usp_location_delete													--
-- ===================													--
--																		--
--------------------------------------------------------------------------
--																		--
-- Description															--
-- ===========															--
--																		--
-- This stored procedure is part of the AuditWizard Product			--
-- It is used to DELETE a location definititon 							--
--																		--
-- NOTE: It is the responsibility of the caller to ensure that referential --
-- integrity is retained.  This procedure will try to ensure that a     --
-- location is not deleted while referenced but will simply fail if     --
-- it detects any references											--
--																		--
--------------------------------------------------------------------------
--																		--
-- History																--
-- =======																--
--																		--
--	16-Sep-2008	Chris Drew		Initial Version							--
--																		--
--------------------------------------------------------------------------

CREATE PROCEDURE [dbo].[usp_location_delete]
	@nLocationID int
AS  
DECLARE @nRetCode int,
		@parentLocationID int,
		@nReferences int

SET nocount ON
SET @nRetCode = 0 

BEGIN   

	-- Get the database ID of the parent of this location
	SET @parentLocationID = (SELECT _PARENTID FROM LOCATIONS WHERE _LOCATIONID=@nLocationID)

	-- re-locate any assets parented to us to our parent
	UPDATE ASSETS SET _LOCATIONID=@parentLocationID WHERE _LOCATIONID=@nLocationID

	-- Check for child locations - we cannot continue of we have children
	SET  @nReferences = isnull((SELECT min(_LOCATIONID) FROM LOCATIONS WHERE _PARENTID = @nLocationID) ,0)

	-- Delete the location ONLY if there are no references to it
	IF (@nReferences <> 0)
		BEGIN
			SET @nRetCode = -1
		END
	ELSE
		BEGIN
			DELETE FROM LOCATIONS WHERE _LOCATIONID = @nLocationID
		END

END
RETURN @nRetCode
GO 




set ANSI_NULLS ON
set QUOTED_IDENTIFIER ON
GO
--------------------------------------------------------------------------
--																		--
-- usp_location_find													--
-- ==================													--
--																		--
--------------------------------------------------------------------------
--																		--
-- Description															--
-- ===========															--
--																		--
-- This stored procedure is part of the AuditWizard Product				--
-- It is used to FIND a location with the specified (full) name			--
--																		--
--------------------------------------------------------------------------
--																		--
-- History																--
-- =======																--
--																		--
--	9-Feb-2009	Chris Drew		Initial Version							--
--																		--
--------------------------------------------------------------------------

CREATE PROCEDURE [dbo].[usp_location_find]
	@cFullName varchar(510)
AS
	DECLARE @nReturnID int

	SET nocount ON
	SET @nReturnID = isnull((SELECT _LOCATIONID FROM LOCATIONS WHERE _FULLNAME=@cFullName),0)
	RETURN @nReturnID





SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--------------------------------------------------------------------------
--																		--
-- usp_domains_enumerate												--
-- =====================												--
--																		--
--------------------------------------------------------------------------
--																		--
-- Description															--
-- ===========															--
--																		--
-- This stored procedure is part of the AuditWizard Product			--
-- It is used to return a list of locations (for the specified parent)  --
--																		--
-- ParentID - ID of any parent domain to return children of.			--
--	>0 Return children of this parent domain							--																		--
--	0  return top level domain											--
--	-1 return ALL domains												--
--																		--
--------------------------------------------------------------------------
--																		--
-- History																--
-- =======																--
--																		--
--	29-Sep-2008	Chris Drew		Initial Version							--
--																		--
--------------------------------------------------------------------------

CREATE PROCEDURE [dbo].[usp_domains_enumerate]  
	@nParentID int,
	@bShowAll bit
AS
	DECLARE @cSql varchar(1024) 
	DECLARE @cWhere varchar(1024) 
	SET nocount ON  
	
BEGIN 
	SET @cSql = 'SELECT _DOMAINID, _NAME ,_PARENTID ,_HIDDEN'
		+       ' FROM dbo.DOMAINS'

	-- Format the WHERE clause if required
	SET @cWhere = ''

	-- First we need the WHERE for the parent if specified
	IF (@nParentID > 0)
		SET @cWhere = ' WHERE _PARENTID = ' + cast(@nParentID as varchar(16))	
	ELSE IF (@nParentID = 0)
		SET @cWhere = ' WHERE _PARENTID IS NULL'	

	-- Now add on whether to show hidden locations or not
	IF (@bShowAll = 0)
	BEGIN
		IF (@cWhere = '')
			SET @cWhere = ' WHERE _HIDDEN=0'
		ELSE 
			SET @cWhere = @cWhere + ' AND _HIDDEN=0'
	END		

	-- Add on an ordering clause
	SET @cSql = @cSql + @cWhere + ' ORDER BY _NAME'

	-- ...end execute the command
	EXEC (@cSQL)
END
GO




SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--------------------------------------------------------------------------
--																		--
-- usp_domain_add														--
-- ===============														--
--																		--
--------------------------------------------------------------------------
--																		--
-- Description															--
-- ===========															--
--																		--
-- This stored procedure is part of the AuditWizard Product			--
-- It is used to add a Domain to the database							--
--																		--
-- Character strings passed to this function must be SQL prepared	    --
--																		--
--------------------------------------------------------------------------
--																		--
-- History																--
-- =======																--
--																		--
--	02-Oct-2008	Chris Drew		Initial Version							--
--																		--
--------------------------------------------------------------------------

CREATE PROCEDURE [dbo].[usp_domain_add]
	@cName 		varchar(255), 
	@nParentID	int,
	@bHidden	bit,
	@nReturnID 	int output
AS

DECLARE @nRetCode int

SET nocount ON
SET @nRetCode = 0 

BEGIN

	INSERT INTO DOMAINS
		(_NAME ,_PARENTID ,_HIDDEN)
	VALUES
		(@cName ,@nParentID ,@bHidden)

	SET @nReturnID = @@IDENTITY

END
RETURN  @nRetCode
GO
  
  
  

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--------------------------------------------------------------------------
--																		--
-- usp_domain_update													--
-- ===================													--
--																		--
--------------------------------------------------------------------------
--																		--
-- Description															--
-- ===========															--
--																		--
-- This stored procedure is part of the AuditWizard Product			--
-- It is used to update a domain in the database			--
--																		--
-- Character strings passed to this function must be SQL prepared	    --
--																		--
--------------------------------------------------------------------------
--																		--
-- History																--
-- =======																--
--																		--
--	19-Sep-2008	Chris Drew		Initial Version							--
--																		--
--------------------------------------------------------------------------

CREATE PROCEDURE [dbo].[usp_domain_update]
	@nDomainID	int,
	@cName 		varchar(255),
	@nParentID	int,
	@bHidden	bit
AS

DECLARE @nRetCode int

SET nocount ON
SET @nRetCode = 0 

BEGIN

	UPDATE DOMAINS  SET 
		_NAME		= @cName, 
		_PARENTID	= @nParentID,
		_HIDDEN		= @bHidden
	WHERE
		_DOMAINID	= @nDomainID

END
RETURN  @nRetCode
GO



SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--------------------------------------------------------------------------
--																		--
-- usp_domain_delete													--
-- =================													--
--																		--
--------------------------------------------------------------------------
--																		--
-- Description															--
-- ===========															--
--																		--
-- This stored procedure is part of the AuditWizard Product			--
-- It is used to DELETE a domain definititon 							--
--																		--
-- NOTE: It is the responsibility of the caller to ensure that referential --
-- integrity is retained.  This procedure will try to ensure that a     --
-- domain is not deleted while referenced but will simply fail if     --
-- it detects any references											--
--																		--
--------------------------------------------------------------------------
--																		--
-- History																--
-- =======																--
--																		--
--	16-Sep-2008	Chris Drew		Initial Version							--
--																		--
--------------------------------------------------------------------------

CREATE PROCEDURE [dbo].[usp_domain_delete]
	@nDomainID int
AS  
DECLARE @nRetCode int

SET nocount ON
SET @nRetCode = 0 

BEGIN   

	-- re-locate any assets parented to us to our parent
	UPDATE ASSETS SET _DOMAINID=0 WHERE _DOMAINID=@nDomainID
	DELETE FROM DOMAINS WHERE _DOMAINID = @nDomainID

END
RETURN @nRetCode
GO 






set ANSI_NULLS ON
set QUOTED_IDENTIFIER ON
GO
--------------------------------------------------------------------------
--																		--
-- usp_auditeditems_enumerate											--
-- ==========================											--
--																		--
--------------------------------------------------------------------------
--																		--
-- Description															--
-- ===========															--
--																		--
-- This stored procedure is part of the AuditWizard Product				--
-- It is used to enumerate the list of licenses declared				--
--																		--
--------------------------------------------------------------------------
--																		--
-- History																--
-- =======																--
--																		--
--	04-Mar-2008	Chris Drew		Initial Version							--
--																		--
--------------------------------------------------------------------------
CREATE PROCEDURE [dbo].[usp_auditeditems_enumerate]
	@assetID int,
	@parentCategory varchar(1024),
	@bAllChildren bit
AS   
	
BEGIN

DECLARE @cSql varchar(1024)
DECLARE @cWhere varchar(1024)

	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	-- Base statement
	SET @cWhere = ''
	SET @cSql = 'SELECT AUDITEDITEMS.* FROM AUDITEDITEMS'

	-- Add on WHERE clause for Asset if required
	IF (@assetID <> 0)
	BEGIN
		IF (@cWhere = '')
			SET @cWhere = ' WHERE _ASSETID=' + cast(@assetID as varchar(16))
		ELSE
			SET @cWhere = @cWhere + ' AND _ASSETID=' + cast(@assetID as varchar(16))
	END

	-- Add on WHERE clause if we want have specified the parent category
	IF (@parentCategory <> '')
	BEGIN
			IF (@cWhere = '')
				SET @cWhere = ' WHERE _CATEGORY LIKE ''' + @parentCategory + ''' + ''|%'''
			ELSE
				SET @cWhere = @cWhere + ' AND _CATEGORY LIKE ''' + @parentCategory + ''' + ''|%'''

			-- If we have requested to limit to immediate children only
			IF (@bAllChildren = 0)
			BEGIN
				SET @cWhere = @cWhere + ' AND CHARINDEX(''|'', _CATEGORY, len(''' + @parentCategory + ''' + ''|'') + 1) = 0'
			END
	END

	-- Construct the full command
	SET @cSQL = @cSQL + @cWhere

	-- AND EXECUTE IT
	EXEC (@cSQL)
END
GO




set ANSI_NULLS ON
set QUOTED_IDENTIFIER ON
GO
--------------------------------------------------------------------------
--																		--
-- usp_auditeditems_enumerateicons										--
-- ==========================											--
--																		--
--------------------------------------------------------------------------
--																		--
-- Description															--
-- ===========															--
--																		--
-- This stored procedure is part of the AuditWizard Product			--
-- It is used to enumerate the list of icons used for audited items		--
--																		--
--------------------------------------------------------------------------
--																		--
-- History																--
-- =======																--
--																		--
--	24-Nov-2008	Chris Drew		Initial Version							--
--																		--
--------------------------------------------------------------------------
CREATE PROCEDURE [dbo].usp_auditeditems_enumerateicons
AS   

	
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
    -- Insert statements for procedure here
	SELECT _CATEGORY ,_ICON FROM AUDITEDITEMS WHERE _ASSETID=0
END
GO




set ANSI_NULLS ON
set QUOTED_IDENTIFIER ON
GO
--------------------------------------------------------------------------
--																		--
-- usp_auditeditems_enumerate_categories								--
-- ==========================											--
--																		--
--------------------------------------------------------------------------
--																		--
-- Description															--
-- ===========															--
--																		--
-- This stored procedure is part of the AuditWizard Product			--
-- It is used to enumerate the list of licenses declared				--
--																		--
--------------------------------------------------------------------------
--																		--
-- History																--
-- =======																--
--																		--
--	04-Mar-2008	Chris Drew		Initial Version							--
--																		--
--------------------------------------------------------------------------
CREATE PROCEDURE [dbo].[usp_auditeditems_enumerate_categories]
	@parentCategory varchar(1024)
AS   
	
BEGIN
	SELECT DISTINCT(_CATEGORY), _ICON
	FROM AUDITEDITEMS
	WHERE (_CATEGORY LIKE @parentCategory + '|%' 
		AND CHARINDEX('|', _CATEGORY, len(@parentCategory + '|') + 1) = 0)	
END
GO




set ANSI_NULLS ON
set QUOTED_IDENTIFIER ON
GO
--------------------------------------------------------------------------
--																		--
-- usp_auditeditems_enumerate_category_names							--
-- =========================================							--
--																		--
--------------------------------------------------------------------------
--																		--
-- Description															--
-- ===========															--
--																		--
-- This stored procedure is part of the AuditWizard Product				--
-- It is used to return a list of categories with values (names)		--
--																		--
--------------------------------------------------------------------------
--																		--
-- History																--
-- =======																--
--																		--
--	04-Mar-2008	Chris Drew		Initial Version							--
--																		--
--------------------------------------------------------------------------
CREATE PROCEDURE [dbo].[usp_auditeditems_enumerate_category_names]
	@parentCategory varchar(1024)
AS   
	
BEGIN
	SELECT DISTINCT(_CATEGORY) ,_NAME ,_ICON FROM AUDITEDITEMS  
		WHERE _CATEGORY =  @parentCategory AND _NAME<>''

END
GO



set ANSI_NULLS ON
set QUOTED_IDENTIFIER ON
GO
--------------------------------------------------------------------------
--																		--
-- usp_auditeditems_enumerate_values									--
-- ==================================									--
--																		--
--------------------------------------------------------------------------
--																		--
-- Description															--
-- ===========															--
--																		--
-- This stored procedure is part of the AuditWizard Product				--
-- It is used to return a list of values for the specified AuditItem	--
-- field when passed the CAtEGORY and NAME.  If an ASSETID is also		--
-- passed we limit the data returned to the specified asset				--
--																		--
--------------------------------------------------------------------------
--																		--
-- History																--
-- =======																--
--																		--
--	04-Mar-2008	Chris Drew		Initial Version							--
--																		--
--------------------------------------------------------------------------
CREATE PROCEDURE [dbo].[usp_auditeditems_enumerate_values]
	@assetID int,
	@cCategory varchar(1024),
	@cName	varchar(1024)
AS   
	
BEGIN

DECLARE @cSql varchar(1024)

	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	-- Base statement
	SET @cSql = 'SELECT _ASSETID ,_VALUE ,_DISPLAY_UNITS ,_DATATYPE FROM AUDITEDITEMS'
			+   ' WHERE _category = ''' + @cCategory + ''' AND _NAME = ''' + @cName + ''''

	-- Add on WHERE clause for Asset if required
	IF (@assetID <> 0)
		SET @cSql = @cSql + ' AND _ASSETID=' + cast(@assetID as varchar(16))

	-- AND EXECUTE IT
	EXEC (@cSQL)
END
GO



set ANSI_NULLS ON
set QUOTED_IDENTIFIER ON
GO
--------------------------------------------------------------------------
--																		--
-- usp_auditeditems_delete												--
-- =======================												--
--																		--
--------------------------------------------------------------------------
--																		--
-- Description															--
-- ===========															--
--																		--
-- This stored procedure is part of the AuditWizard Product			--
-- It is used to return a delete all Audited Item records for			-- 
-- the specified asset.  This is typically done prior to an upload of   --								
-- a new audit for this asset to clear out old data						--
--																		--
--------------------------------------------------------------------------
--																		--
-- History																--
-- =======																--
--																		--
--	24-Nov-2008	Chris Drew		Initial Version							--
--																		--
--------------------------------------------------------------------------

CREATE PROCEDURE [dbo].[usp_auditeditems_delete]
	@forAssetID int
AS  
	SET nocount ON 
	IF @forAssetID <> 0		
		DELETE FROM AUDITEDITEMS WHERE _ASSETID=@forAssetID
GO




set ANSI_NULLS ON
set QUOTED_IDENTIFIER ON
GO
--------------------------------------------------------------------------
--																		--
-- usp_auditeditem_add													--
-- ===================													--
--																		--
--------------------------------------------------------------------------
--																		--
-- Description															--
-- ===========															--
--																		--
-- This stored procedure is part of the AuditWizard Product			--
-- It is used to add an audited item to the database					--
--																		--
-- Character strings passed to this function must be raw and not SQL    --
--																		--
--------------------------------------------------------------------------
--																		--
-- History																--
-- =======																--
--																		--
--	23-Nov-2008	Chris Drew		Initial Version							--
--																		--
--------------------------------------------------------------------------

CREATE PROCEDURE [dbo].[usp_auditeditem_add]
	@cCategory varchar(1024),
	@cName varchar(1024),
	@cValue varchar(1024),
	@cIcon varchar(255),
	@nAssetID int,
	@cDisplayUnits varchar(32),
	@nDataType int,
	@bHistoried bit,
	@bGrouped bit
AS

DECLARE @nRetCode int,
	@cColumns varchar(1000),
	@cValues varchar(1000),
	@nReturnID int
 
SET nocount ON

BEGIN

	INSERT INTO AUDITEDITEMS
		(_CATEGORY ,_NAME, _VALUE, _ICON, _ASSETID, _DISPLAY_UNITS, _DATATYPE ,_HISTORIED ,_GROUPED)
	VALUES
		(@cCategory, @cName, @cValue, @cIcon, @nAssetID, @cDisplayUnits, @nDataType, @bHistoried ,@bGrouped)

	SET @nReturnID = @@IDENTITY
END
RETURN  @nReturnID
GO




SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--------------------------------------------------------------------------
--																		--
-- usp_get_auditeditem													--
-- ===================													--
--																		--
--------------------------------------------------------------------------
--																		--
-- Description															--
-- ===========															--
--																		--
-- This stored procedure is part of the AuditWizard Product				--
-- It is used to return details for the specific audited item			--
--																		--
--------------------------------------------------------------------------
--																		--
-- History																--
-- =======																--
--																		--
--	21-Feb-2008	Chris Drew		Initial Version							--
--																		--
--------------------------------------------------------------------------

CREATE PROCEDURE [dbo].[usp_get_auditeditem]
	@itemID int
AS  
	SELECT *
		FROM AUDITEDITEMS
		WHERE _AUDITEDITEMID = @itemID
GO





set ANSI_NULLS ON
set QUOTED_IDENTIFIER ON
GO
--------------------------------------------------------------------------
--																		--
-- usp_statistics_processorspeeds										--
-- ==============================										--
--																		--
--------------------------------------------------------------------------
--																		--
-- Description															--
-- ===========															--
--																		--
-- This stored procedure is part of the AuditWizard Product			--
-- It is used to return statistics about processor speeds				--
--																		--
--------------------------------------------------------------------------
--																		--
-- History																--
-- =======																--
--																		--
--	29-Feb-2008	Chris Drew		Initial Version							--
--																		--
--------------------------------------------------------------------------

CREATE PROCEDURE [dbo].[usp_statistics_topprocessorspeeds]
AS	
BEGIN

SELECT t.range as speedrange, count(*) as number
FROM
(	select case 
			when cast(_VALUE as int) >=0 and cast(_VALUE as int) <=1000 then '0 - 1Ghz'
			when cast(_VALUE as int) >1000 and cast(_VALUE as int) <=1500 then '1.0Ghz - 1.5GHz'
			when cast(_VALUE as int) >1500 and cast(_VALUE as int) <=2000 then '1.5Ghz - 2.0Ghz'
			when cast(_VALUE as int) >2000 and cast(_VALUE as int) <=2500 then '2Ghz - 2.5Ghz'
			when cast(_VALUE as int) >2500 and cast(_VALUE as int) <=3000 then '2.5Ghz - 3.0Ghz'
			when cast(_VALUE as int) >3000 and cast(_VALUE as int) <=3500 then '3Ghz - 3.5Ghz'
			when cast(_VALUE as int) >3500 and cast(_VALUE as int) <=4000 then '3.5Ghz - 4.0Ghz'
			when cast(_VALUE as int) >4000 and cast(_VALUE as int) <=4500 then '4Ghz - 4.5Ghz'
			when cast(_VALUE as int) >4500 and cast(_VALUE as int) <=5000 then '4.5Ghz - 5.0Ghz'
			else '>5Ghz' end as range
	FROM AUDITEDITEMS WHERE _CATEGORY='Hardware|CPU' AND _NAME='Speed'
) t
GROUP BY t.range

END
GO



set ANSI_NULLS ON
set QUOTED_IDENTIFIER ON
GO
--------------------------------------------------------------------------
--																		--
-- [usp_statistics_topmemorycapacity]									--
-- ==============================										--
--																		--
--------------------------------------------------------------------------
--																		--
-- Description															--
-- ===========															--
--																		--
-- This stored procedure is part of the AuditWizard Product				--
-- It is used to return statistics about processor speeds				--
--																		--
--------------------------------------------------------------------------
--																		--
-- History																--
-- =======																--
--																		--
--	29-Feb-2008	Chris Drew		Initial Version							--
--																		--
--------------------------------------------------------------------------

CREATE PROCEDURE [dbo].[usp_statistics_topmemorycapacity]
AS	
BEGIN

SELECT t.range as speedrange, count(*) as number
FROM
(	select case 
			when cast(_VALUE as int) >=0 and cast(_VALUE as int) <=500 then '<=500MB'
			when cast(_VALUE as int) >500 and cast(_VALUE as int) <=1000 then '501MB - 1GB'
			when cast(_VALUE as int) >1000 and cast(_VALUE as int) <=1500 then '1GB - 1.5GB'
			when cast(_VALUE as int) >1500 and cast(_VALUE as int) <=2000 then '1.5GB - 2GB'
			when cast(_VALUE as int) >2000 and cast(_VALUE as int) <=3000 then '2GB - 3GB'
			when cast(_VALUE as int) >3000 and cast(_VALUE as int) <=3500 then '3GB - 4GB'
			when cast(_VALUE as int) >3500 and cast(_VALUE as int) <=4000 then '4GB - 5GB'
			when cast(_VALUE as int) >4000 and cast(_VALUE as int) <=4500 then '5GB - 6GB'
			when cast(_VALUE as int) >4500 and cast(_VALUE as int) <=5000 then '6GB - 7GB'
			else '>7GB' end as range
	FROM AUDITEDITEMS WHERE _CATEGORY='Hardware|Memory' AND _NAME='Total RAM'
) t
GROUP BY t.range

END
GO






SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--------------------------------------------------------------------------
--																		--
-- usp_notes_enumerate													--
-- ===================													--
--																		--
--------------------------------------------------------------------------
--																		--
-- Description															--
-- ===========															--
--																		--
-- This stored procedure is part of the AuditWizard Product				--
-- It is used to return a list of notes for the specified item			--
--																		--
--------------------------------------------------------------------------
--																		--
-- History																--
-- =======																--
--																		--
--	07-Jan-2009	Chris Drew		Initial Version							--
--																		--
--------------------------------------------------------------------------

CREATE PROCEDURE [dbo].[usp_notes_enumerate]
	@nScope int,
	@nParentID int
AS 
	SET nocount ON  
	
BEGIN
	SELECT NOTES.*
		FROM dbo.NOTES
		WHERE _SCOPE = @nScope AND _PARENTID = @nParentID
	ORDER BY _DATE
END
GO




SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--------------------------------------------------------------------------
--																		--
-- usp_note_add															--
-- ============															--
--																		--
--------------------------------------------------------------------------
--																		--
-- Description															--
-- ===========															--
--																		--
-- This stored procedure is part of the AuditWizard Product				--
-- It is used to add a Note to the database								--
--																		--
-- Character strings passed to this function must be SQL prepared	    --
--																		--
--------------------------------------------------------------------------
--																		--
-- History																--
-- =======																--
--																		--
--	07-Jan-2009	Chris Drew		Initial Version							--
--																		--
--------------------------------------------------------------------------

CREATE PROCEDURE [dbo].[usp_note_add]
	@dtDate DateTime,
	@nScope	int,
	@nParentID int,
	@cText varchar(4000),
	@cUser varchar(255),
	@nReturnID int output
AS

SET nocount ON

BEGIN

	INSERT INTO NOTES
		(_DATE ,_SCOPE ,_PARENTID ,_TEXT ,_USER)
	VALUES
		(@dtDate ,@nScope ,@nParentID ,@cText ,@cUser)

	SET @nReturnID = @@IDENTITY

END
RETURN
GO





SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--------------------------------------------------------------------------
--																		--
-- usp_note_delete														--
-- ===============														--
--																		--
--------------------------------------------------------------------------
--																		--
-- Description															--
-- ===========															--
--																		--
-- This stored procedure is part of the AuditWizard Product				--
-- It is used to add a DELETE a Note 									--
--																		--
--------------------------------------------------------------------------
--																		--
-- History																--
-- =======																--
--																		--
--	07-Jan-2009	Chris Drew		Initial Version							--
--																		--
--------------------------------------------------------------------------

CREATE PROCEDURE [dbo].[usp_note_delete]
	@nNoteID int
AS    
	DELETE FROM NOTES WHERE _NOTEID = @nNoteID
RETURN
GO
 
 
 
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--------------------------------------------------------------------------
--																		--
-- usp_note_update														--
-- ===============														--
--																		--
--------------------------------------------------------------------------
--																		--
-- Description															--
-- ===========															--
--																		--
-- This stored procedure is part of the AuditWizard Product				--
-- It is used to add a update NOTE definititon 							--
--																		--
--------------------------------------------------------------------------
--																		--
-- History																--
-- =======																--
--																		--
--	18-Sep-2008	Chris Drew		Initial Version							--
--																		--
--------------------------------------------------------------------------

CREATE PROCEDURE [dbo].[usp_note_update]
	@nNoteID int,
	@cText varchar(4000)
	
AS     
BEGIN
	UPDATE NOTES
		SET 
			_TEXT=@cText
		WHERE
			_NOTEID = @nNoteID
END
GO





SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--------------------------------------------------------------------------
--																		--
-- usp_documents_enumerate												--
-- ===================													--
--																		--
--------------------------------------------------------------------------
--																		--
-- Description															--
-- ===========															--
--																		--
-- This stored procedure is part of the AuditWizard Product				--
-- It is used to return a list of documents for the specified item			--
--																		--
--------------------------------------------------------------------------
--																		--
-- History																--
-- =======																--
--																		--
--	07-Jan-2009	Chris Drew		Initial Version							--
--																		--
--------------------------------------------------------------------------

CREATE PROCEDURE [dbo].[usp_documents_enumerate]
	@nScope int,
	@nParentID int
AS 
	SET nocount ON  
	
BEGIN
	SELECT DOCUMENTS.*
		FROM dbo.DOCUMENTS
		WHERE _SCOPE = @nScope AND _PARENTID = @nParentID
END
GO




SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--------------------------------------------------------------------------
--																		--
-- usp_document_add														--
-- ============															--
--																		--
--------------------------------------------------------------------------
--																		--
-- Description															--
-- ===========															--
--																		--
-- This stored procedure is part of the AuditWizard Product				--
-- It is used to add a Documemnt to the database						--
--																		--
--------------------------------------------------------------------------
--																		--
-- History																--
-- =======																--
--																		--
--	07-Jan-2009	Chris Drew		Initial Version							--
--																		--
--------------------------------------------------------------------------

CREATE PROCEDURE [dbo].usp_document_add
	@nScope	int,
	@nParentID int,
	@cName varchar(255),
	@cPath varchar(255),
	@nReturnID int output
AS

SET nocount ON

BEGIN

	INSERT INTO DOCUMENTS
		(_SCOPE ,_NAME ,_PATH ,_PARENTID)
	VALUES
		(@nScope ,@cName ,@cPath ,@nParentID)

	SET @nReturnID = @@IDENTITY

END
RETURN
GO





SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--------------------------------------------------------------------------
--																		--
-- usp_document_delete													--
-- ===============														--
--																		--
--------------------------------------------------------------------------
--																		--
-- Description															--
-- ===========															--
--																		--
-- This stored procedure is part of the AuditWizard Product				--
-- It is used to add a DELETE a Document								--
--																		--
--------------------------------------------------------------------------
--																		--
-- History																--
-- =======																--
--																		--
--	07-Jan-2009	Chris Drew		Initial Version							--
--																		--
--------------------------------------------------------------------------

CREATE PROCEDURE [dbo].usp_document_delete
	@nDocumentID int
AS    
	DELETE FROM DOCUMENTS WHERE _DOCUMENTID = @nDocumentID
RETURN
GO
 
 
 
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--------------------------------------------------------------------------
--																		--
-- usp_document_update													--
-- ===============														--
--																		--
--------------------------------------------------------------------------
--																		--
-- Description															--
-- ===========															--
--																		--
-- This stored procedure is part of the AuditWizard Product				--
-- It is used to update a document										--
--																		--
--------------------------------------------------------------------------
--																		--
-- History																--
-- =======																--
--																		--
--	18-Sep-2008	Chris Drew		Initial Version							--
--																		--
--------------------------------------------------------------------------

CREATE PROCEDURE [dbo].usp_document_update
	@nDocumentID int,
	@cName varchar(255),
	@cPath varchar(255)
	
AS     
BEGIN
	UPDATE DOCUMENTS
		SET 
			_NAME=@cName, _PATH=@cPath
		WHERE
			_DOCUMENTID = @nDocumentID
END
GO





SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--------------------------------------------------------------------------
--																		--
-- usp_operations_enumerate												--
-- ========================												--
--																		--
--------------------------------------------------------------------------
--																		--
-- Description															--
-- ===========															--
--																		--
-- This stored procedure is part of the AuditWizard Product				--
-- It is used to return a list of Operations (of the specified type		--
--																		--
--------------------------------------------------------------------------
--																		--
-- History																--
-- =======																--
--																		--
--	15-Jan-2009	Chris Drew		Initial Version							--
--																		--
--------------------------------------------------------------------------

CREATE PROCEDURE [dbo].[usp_operations_enumerate]
	@nOperation int,
	@nStatus int
AS 
	SET nocount ON  
	
BEGIN
	DECLARE @cSQL varchar(1020)
	DECLARE @cWhere varchar(255)
	SET @cWhere = ''

	SET @cSQL = 'SELECT OPERATIONS.* '
		+		' ,ASSETS._NAME AS ASSETNAME'
		+       ' FROM dbo.OPERATIONS '
		+		'  LEFT JOIN ASSETS ON (ASSETS._ASSETID=OPERATIONS._ASSETID)'
	
	IF (@nOperation <> -1)
		SET @cWhere = ' WHERE OPERATIONS._OPERATION = @nOperation'

	IF (@nStatus <> -1)
	BEGIN
		IF (@cWhere = '')
			SET @cSQL = @cSQL + ' WHERE OPERATIONS._STATUS=' + cast(@nStatus as varchar(2))
		ELSE
			SET @cSQL = @cSQL + ' AND OPERATIONS._STATUS=' + cast(@nStatus as varchar(2))
	END

	SET @cSQL = @cSQL + @cWhere
	SET @cSQL = @cSQL + ' ORDER BY _OPERATIONID'

	EXEC (@cSQL)
END
GO




SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--------------------------------------------------------------------------
--																		--
-- usp_operation_add													--
-- ============															--
--																		--
--------------------------------------------------------------------------
--																		--
-- Description															--
-- ===========															--
--																		--
-- This stored procedure is part of the AuditWizard Product				--
-- It is used to add a new Operation to the database					--
--																		--
--------------------------------------------------------------------------
--																		--
-- History																--
-- =======																--
--																		--
--	15-Jan-2009	Chris Drew		Initial Version							--
--																		--
--------------------------------------------------------------------------

CREATE PROCEDURE [dbo].usp_operation_add
	@nOperation	int,
	@nAssetID int,
	@nReturnID int output
AS

DECLARE @dtToday DateTime

SET nocount ON
BEGIN

	SET @dtToday = GETDATE();

	INSERT INTO OPERATIONS
		(_OPERATION ,_ASSETID ,_START_DATE ,_STATUS)
	VALUES
		(@nOperation ,@nAssetID ,@dtToday ,0)

	SET @nReturnID = @@IDENTITY

END
RETURN
GO





SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--------------------------------------------------------------------------
--																		--
-- usp_operation_delete													--
-- ====================													--
--																		--
--------------------------------------------------------------------------
--																		--
-- Description															--
-- ===========															--
--																		--
-- This stored procedure is part of the AuditWizard Product				--
-- It is used to DELETE an Operation									--
--																		--
--	Dates should be NULL if they are not to be affected					--
--																		--
--------------------------------------------------------------------------
--																		--
-- History																--
-- =======																--
--																		--
--	15-Jan-2009	Chris Drew		Initial Version							--
--																		--
--------------------------------------------------------------------------

CREATE PROCEDURE [dbo].usp_operation_delete
	@nOperationID int
	
AS    
	DELETE FROM OPERATIONS WHERE _OPERATIONID = @nOperationID
RETURN
GO
 
 
 
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--------------------------------------------------------------------------
--																		--
-- usp_operation_update													--
-- ====================													--
--																		--
--------------------------------------------------------------------------
--																		--
-- Description															--
-- ===========															--
--																		--
-- This stored procedure is part of the AuditWizard Product				--
-- It is used to update an Operation									--
--																		--
--------------------------------------------------------------------------
--																		--
-- History																--
-- =======																--
--																		--
--	15-Jan-2009	Chris Drew		Initial Version							--
--																		--
--------------------------------------------------------------------------

CREATE PROCEDURE [dbo].usp_operation_update
	@nOperationID int,
	@dtEndDate DateTime,
	@nStatus int,
	@cErrorText varchar(510)	
AS     
BEGIN

	UPDATE OPERATIONS SET 
		_END_DATE=@dtEndDate,
		_STATUS=@nStatus,
		_ERRORTEXT=@cErrorText
	WHERE _OPERATIONID=@nOperationID
END
GO




SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--------------------------------------------------------------------------
--																		--
-- usp_operation_lastindex												--
-- ====================													--
--																		--
--------------------------------------------------------------------------
--																		--
-- Description															--
-- ===========															--
--																		--
-- This stored procedure is part of the AuditWizard Product				--
-- It is used to return the index of the last operation in the database --
--																		--
--------------------------------------------------------------------------
--																		--
-- History																--
-- =======																--
--																		--
--	15-Jan-2009	Chris Drew		Initial Version							--
--																		--
--------------------------------------------------------------------------

CREATE PROCEDURE [dbo].usp_operation_lastindex
	@nReturnID int output
	
AS    
	SET @nReturnId = isnull((SELECT max(_OPERATIONID) FROM OPERATIONS) ,0) 
RETURN
GO





SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--------------------------------------------------------------------------
--																		--
-- usp_operations_purge													--
-- ====================													--
--																		--
--------------------------------------------------------------------------
--																		--
-- Description															--
-- ===========															--
--																		--
-- This stored procedure is part of the AuditWizard Product				--
-- It is used to purge Operations Records prior to the given date		--
--																		--
--------------------------------------------------------------------------
--																		--
-- History																--
-- =======																--
--																		--
--	15-Jan-2009	Chris Drew		Initial Version							--
--																		--
--------------------------------------------------------------------------

CREATE PROCEDURE [dbo].[usp_operations_purge]
	@dtPurgeDate DateTime
AS  
BEGIN
	DECLARE @nRowCount int

	SET nocount ON  

	DELETE FROM OPERATIONS 
		WHERE (datediff(day, _START_DATE, @dtPurgeDate) > 0)

	return @nRowCount;
END
GO




SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--------------------------------------------------------------------------
--																		--
-- usp_internet_purge													--
-- ==================													--
--																		--
--------------------------------------------------------------------------
--																		--
-- Description															--
-- ===========															--
--																		--
-- This stored procedure is part of the AuditWizard Product				--
-- It is used to purge Internet Records prior to the given date			--
--																		--
--------------------------------------------------------------------------
--																		--
-- History																--
-- =======																--
--																		--
--	9-Jan-2009	Chris Drew		Initial Version							--
--																		--
--------------------------------------------------------------------------

CREATE PROCEDURE [dbo].[usp_internet_purge]
	@dtPurgeDate DateTime
AS  
BEGIN
	DECLARE @nRowCount int

	SET nocount ON  

	-- Internet Cookies are easy enough	
	DELETE FROM AUDITEDITEMS 
		WHERE _CATEGORY LIKE 'Internet|Cookie|%' AND (datediff(day, _VALUE, @dtPurgeDate) > 0)
	SET @nRowCount = @@ROWCOUNT 

	-- Internet History is more complex as the date is held in the _CATEGORY column but
	-- not on its own - the date is held in 10 characters starting at character 18 (1 based)
	DELETE FROM AUDITEDITEMS 
		WHERE _AUDITEDITEMID IN
			(
				SELECT _AUDITEDITEMID 
				FROM AUDITEDITEMS 
				WHERE _CATEGORY LIKE 'Internet|History|%' 
				AND (datediff(day, CONVERT(datetime ,SUBSTRING(_CATEGORY, 18, 10)), @dtPurgeDate) > 0)
			)	

	SET @nRowCount = @nRowCount + @@ROWCOUNT

	return @nRowCount;
END
GO


SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--------------------------------------------------------------------------
--																		--
-- usp_unauditedassets_purge											--
-- =========================											--
--																		--
--------------------------------------------------------------------------
--																		--
-- Description															--
-- ===========															--
--																		--
-- This stored procedure is part of the AuditWizard Product				--
-- It is used to purge Assets which have not been audited since the		--
-- given date															--
--																		--
--------------------------------------------------------------------------
--																		--
-- History																--
-- =======																--
--																		--
--	9-Jan-2009	Chris Drew		Initial Version							--
--																		--
--------------------------------------------------------------------------

CREATE PROCEDURE [dbo].[usp_unauditedassets_purge]
	@dtPurgeDate DateTime
AS  
	DECLARE @nRowCount int
	DECLARE @Proc nvarchar(50)
	DECLARE @MaxRows int
	DECLARE @ExecSql nvarchar(255)

	SET nocount ON  

	SET @nRowCount = 1
	SET @Proc = 'usp_asset_delete'

	-- These next two rows are specific to source table or query
	DECLARE @Tmp TABLE (rownum int IDENTITY (1, 1) Primary key NOT NULL, _ASSETID int)

	-- Add the ID of assets which have been audited but not since the specified date into the table
	INSERT INTO @Tmp (_ASSETID) 
		SELECT _ASSETID FROM ASSETS 
			WHERE (_LASTAUDIT <> NULL) AND (datediff(day, _LASTAUDIT, @dtPurgeDate) > 0)

	-- Now we select all of the records in the temporary table and loop through them deleting each asset
	SELECT @MaxRows = count(*) FROM @Tmp	

	WHILE @nRowCount <= @MaxRows
	BEGIN
	    SELECT @ExecSql = 'exec ' + @Proc + ' ''' + _ASSETID + '''' from @Tmp where rownum = @nRowCount 
		EXECUTE sp_executesql @ExecSql
		SET @nRowCount = @nRowCount + 1
	END

	RETURN @nRowCount;
GO




SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--------------------------------------------------------------------------
--																		--
-- usp_userdata_getvalues												--
-- ===================													--
--																		--
--------------------------------------------------------------------------
--																		--
-- Description															--
-- ===========															--
--																		--
-- This stored procedure is part of the AuditWizard Product				--
-- It is used to return a list of userdata values for the specified 	--
-- user data field and item												--
--																		--
--	@nParentType - Type of parent to return data for					--
--		0 = asset														--
--		1 = application													--
--		2 = application instance										--
--																		--
--	@nParentID - Index of the above typed item							--
--																		--
--	@nCategoryID - ID of the user data category to return values for	--
--																		--
--------------------------------------------------------------------------
--																		--
-- History																--
-- =======																--
--																		--
--	12-Jan-2009	Chris Drew		Initial Version							--
--																		--
--------------------------------------------------------------------------

CREATE PROCEDURE [dbo].usp_userdata_getvalues
	@nParentType	int,
	@nParentID		int,
	@nCategoryID	int
AS 
	DECLARE @cSQL varchar(1024) 
	SET nocount ON  
	
BEGIN

	SELECT _VALUE ,_USERDEFID 
		FROM USERDATA_VALUES
		WHERE _PARENTTYPE=@nParentType
			AND _PARENTID = @nParentID
			AND _USERDEFID IN
				(SELECT _USERDEFID FROM USERDATA_DEFINITIONS WHERE _PARENTID=@nCategoryID)

END
GO



SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--------------------------------------------------------------------------
--																		--
-- usp_userdata_update_value											--
-- =========================											--
--																		--
--------------------------------------------------------------------------
--																		--
-- Description															--
-- ===========															--
--																		--
-- This stored procedure is part of the AuditWizard Product				--
-- It is used to add a new entry to the USERDATA_VALUES table			--
-- or modify an existing entry which matches the criteria. 				--
--																		--
--------------------------------------------------------------------------
--																		--
-- History																--
-- =======																--
--																		--
--	18-Feb-2006	Chris Drew		Initial Version							--
--																		--
--------------------------------------------------------------------------

CREATE PROCEDURE [dbo].usp_userdata_update_value
	@nScope int,
	@nParentID int,
	@nUserDefID int,
	@cValue varchar(1020),
	@nReturnId int output
AS

DECLARE @nRetCode int

SET nocount ON
SET @nRetCode = 0

-- first check the database for an entry that matches the supplied criteria
SET @nReturnId = isnull((SELECT min(_USERVALUEID) FROM USERDATA_VALUES 
							WHERE _PARENTTYPE=@nScope 
							AND   _PARENTID=@nParentID
							AND	  _USERDEFID=@nUserDefID) ,0)

-- did we find a match ?
IF (@nReturnId = 0)
BEGIN
	-- OK We do not have a previous value for this user data field so create one
	INSERT INTO USERDATA_VALUES
		(_PARENTTYPE, _PARENTID, _USERDEFID, _VALUE)
	VALUES
		(@nScope, @nParentID, @nUserDefID, @cValue)

	-- Return the databaae ID for the newly inserted item
	SET @nReturnId = @@IDENTITY

END
ELSE
BEGIN
	-- We found an existing entry so update the value
	UPDATE USERDATA_VALUES SET _VALUE=@cValue WHERE _USERVALUEID=@nReturnId
END

RETURN @nRetCode
GO



set ANSI_NULLS ON
set QUOTED_IDENTIFIER ON
GO
--------------------------------------------------------------------------
--																		--
-- usp_userdatafield_getvalues											--
-- ===========================											--
--																		--
--------------------------------------------------------------------------
--																		--
-- Description															--
-- ===========															--
--																		--
-- This stored procedure is part of the AuditWizard Product				--
-- It returns a list of values for the specified user data field		--
--																		--
--------------------------------------------------------------------------
--																		--
-- History																--
-- =======																--
--																		--
--	06-feb-2009 Chris Drew		Initial Version							--
--																		--
--------------------------------------------------------------------------

CREATE PROCEDURE [dbo].[usp_userdatafield_getvalues]
	@nFieldID	int
AS 
	SET nocount ON  
	
BEGIN

	SELECT _PARENTID, _VALUE
		FROM USERDATA_VALUES
		WHERE _USERDEFID = @nFieldID

END
GO




SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--------------------------------------------------------------------------
--																		--
-- usp_internet_getdata													--
-- ===================													--
--																		--
--------------------------------------------------------------------------
--																		--
-- Description															--
-- ===========															--
--																		--
-- This stored procedure is part of the AuditWizard Product				--
-- It is used to recover Internet Records								--
--																		--
--------------------------------------------------------------------------
--																		--
-- History																--
-- =======																--
--																		--
--	2-Feb-2009	Chris Drew		Initial Version							--
--																		--
--------------------------------------------------------------------------

CREATE PROCEDURE [dbo].[usp_internet_getdata]
	@dtStartDate DateTime,
	@dtEndDate DateTime,
	@strUrl varchar(510)
AS  
BEGIN

	DECLARE @cStartDate varchar(20)
	DECLARE @cEndDate varchar(20)
	DECLARE @cSQL varchar(1024)

	-- Convert any start and/or end date specified
	IF (@dtStartDate is not null) and (@dtEndDate is not null)
	BEGIN
		SET @cStartDate = CONVERT(varchar(20), @dtStartDate ,120)
		SET @cEndDate = CONVERT(varchar(20), @dtEndDate ,120)
	END


	-- First of all build up the basic select statement
	SET @cSQL = 'SELECT AUDITEDITEMS.*'
		+		' ,ASSETS._NAME AS ASSETNAME'
		+		' ,LOCATIONS._FULLNAME AS FULLLOCATIONNAME'

	-- from the AUDITEDITEMS table
		+		' FROM AUDITEDITEMS'

	-- Join ASSETS to get the Asset Name
		+		' LEFT JOIN ASSETS ON (AUDITEDITEMS._ASSETID = ASSETS._ASSETID)'

	--	Join Locations to get the full location name
		+		' LEFT JOIN LOCATIONS ON (ASSETS._LOCATIONID = LOCATIONS._LOCATIONID)'

	-- Exclude records which are not associated with an asset
		+		' WHERE (AUDITEDITEMS._ASSETID <> 0)'
     
	-- Add in the URL filter as this is could rdramatically reduce the records
		IF (@strUrl <> '')
		BEGIN
			SET @cSQL = @cSQL + ' AND (AUDITEDITEMS._CATEGORY LIKE ''Internet|History|%''' 
				+				' AND  (SUBSTRING(AUDITEDITEMS._CATEGORY, 29, 100)) LIKE ''' + @strUrl + ''')'
		END

	-- Now we add on the date/Time filter noting that we must have this entire section as a single AND
	-- Limit to Internet History Records first
		SET @cSQL = @cSQL + ' AND ((AUDITEDITEMS._CATEGORY LIKE ''Internet|History|%'''

	-- Do we have a start and end date?  If so add these as a filter

		IF (@dtStartDate is not null) and (@dtEndDate is not null)
		BEGIN
			SET @cSQL = @cSQL + N' AND CONVERT(datetime ,SUBSTRING(AUDITEDITEMS._CATEGORY, 18, 10) ,120)'
					+	   ' BETWEEN ''' + @cStartDate + '''  AND ''' + @cEndDate + '''' 
		END

	-- Limit Cookies also
		SET @cSQL = @cSQL + ') OR (AUDITEDITEMS._CATEGORY LIKE ''Internet|Cookie|%'''

		IF (@dtStartDate is not null) and (@dtEndDate is not null)
		BEGIN
			SET @cSQL = @cSQL + N' AND AUDITEDITEMS._NAME=''Date'' AND CONVERT(datetime ,SUBSTRING(AUDITEDITEMS._VALUE, 1, 10) ,120)'
					+	   ' BETWEEN ''' + @cStartDate + '''  AND ''' + @cEndDate + '''' 
		END

	-- Close the WHERE
		SET @cSQL = @cSQL + '))'

		EXEC (@cSQL)
END
GO







SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--------------------------------------------------------------------------
--																		--
-- usp_scanner_add														--
-- ===============														--
--																		--
--------------------------------------------------------------------------
--																		--
-- Description															--
-- ===========															--
--																		--
-- This stored procedure is part of the AuditWizard Product				--
-- It is used to add a Scanner Definition to the database				--
--																		--
--------------------------------------------------------------------------
--																		--
-- History																--
-- =======																--
--																		--
--	12-Feb-2009	Chris Drew		Initial Version							--
--																		--
--------------------------------------------------------------------------

CREATE PROCEDURE [dbo].[usp_scanner_add]
	@cName	varchar(255),
	@cXML	Text,
	@nReturnID int output
AS

DECLARE @nRetCode int

SET nocount ON
SET @nRetCode = 0

--
-- first check the database for an entry that matches the supplied scanner name
--
SET @nReturnId = isnull((SELECT _SCANNERID FROM SCANNERS WHERE _NAME= @cName) ,0)

-- did we find a match ?
IF (@nReturnId = 0)
BEGIN

	INSERT INTO SCANNERS
		(_NAME ,_XML)
	VALUES
		(@cName ,@cXML)

	SET @nReturnID = @@IDENTITY
END
RETURN @nRetCode
GO
      
 





SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--------------------------------------------------------------------------
--																		--
-- usp_scanner_delete													--
-- ==================													--
--																		--
--------------------------------------------------------------------------
--																		--
-- Description															--
-- ===========															--
--																		--
-- This stored procedure is part of the AuditWizard Product				--
-- It is used to add a DELETE a Scanner  definititon 					--
--																		--
--------------------------------------------------------------------------
--																		--
-- History																--
-- =======																--
--																		--
--	12-Feb-2008	Chris Drew		Initial Version							--
--																		--
--------------------------------------------------------------------------

CREATE PROCEDURE [dbo].[usp_scanner_delete]
	@nScannerID int
AS     
	SET nocount ON

	DELETE FROM SCANNERS WHERE _SCANNERID=@nScannerID
GO




SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--------------------------------------------------------------------------
--																		--
-- usp_scanner_enumerate												--
-- ======================												--
--																		--
--------------------------------------------------------------------------
--																		--
-- Description															--
-- ===========															--
--																		--
-- This stored procedure is part of the AuditWizard Product				--
-- It is used to enumerate the list of scanners declared				--
--																		--
--------------------------------------------------------------------------
--																		--
-- History																--
-- =======																--
--																		--
--	12-Feb-2009	Chris Drew		Initial Version							--
--																		--
--------------------------------------------------------------------------
CREATE PROCEDURE [dbo].[usp_scanner_enumerate]
	@cName varchar(255)
AS   
    -- Insert statements for procedure here
	IF (@cNAME = '')
		SELECT _SCANNERID, _NAME ,_XML FROM SCANNERS
	ELSE
		SELECT _SCANNERID, _NAME ,_XML FROM SCANNERS WHERE _NAME=@cName
GO






SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--------------------------------------------------------------------------
--																		--
-- usp_scanner_update													--
-- ==================													--
--																		--
--------------------------------------------------------------------------
--																		--
-- Description															--
-- ===========															--
--																		--
-- This stored procedure is part of the AuditWizard Product				--
-- It is used to update a scanner definititon in the database			--
--																		--
--------------------------------------------------------------------------
--																		--
-- History																--
-- =======																--
--																		--
--	12-Feb-2008	Chris Drew		Initial Version							--
--																		--
--------------------------------------------------------------------------

CREATE PROCEDURE [dbo].[usp_scanner_update]
	@nScannerID int,
	@cName	varchar(255),
	@cXML	varchar(255)
	
AS     
	SET nocount ON

	-- Update the user
	UPDATE SCANNERS SET _NAME=@cName
					,_XML=@cXML
					WHERE _SCANNERID=@nScannerID
GO




set ANSI_NULLS ON
set QUOTED_IDENTIFIER ON
GO
--------------------------------------------------------------------------
--																		--
-- usp_fs_folders_enumerate												--
-- ========================												--
--																		--
--------------------------------------------------------------------------
--																		--
-- Description															--
-- ===========															--
--																		--
-- This stored procedure is part of the AuditWizard Product				--
-- It is used to return a list of File System Folders for an asset		--
--																		--
--------------------------------------------------------------------------
--																		--
-- History																--
-- =======																--
--																		--
--	26-Feb-2009	Chris Drew		Initial Version							--
--																		--
--------------------------------------------------------------------------

CREATE PROCEDURE [dbo].[usp_fs_folder_enumerate]
	@nAssetID int
AS 
	SET nocount ON  
	
BEGIN
	SELECT FS_FOLDERS.[_FOLDERID]
		 , FS_FOLDERS.[_NAME] 
		 , FS_FOLDERS.[_PARENTID]
		 , FS_FOLDERS.[_ASSETID] 
	FROM dbo.FS_FOLDERS
	WHERE FS_FOLDERS._ASSETID=@nAssetID
	ORDER BY FS_FOLDERS.[_FOLDERID]
END
GO



set ANSI_NULLS ON
set QUOTED_IDENTIFIER ON
GO
--------------------------------------------------------------------------
--																		--
-- usp_fs_files_enumerate												--
-- ======================												--
--																		--
--------------------------------------------------------------------------
--																		--
-- Description															--
-- ===========															--
--																		--
-- This stored procedure is part of the AuditWizard Product				--
-- It is used to return a list of File System FILES for an asset		--
--																		--
--------------------------------------------------------------------------
--																		--
-- History																--
-- =======																--
--																		--
--	26-Feb-2009	Chris Drew		Initial Version							--
--																		--
--------------------------------------------------------------------------

CREATE PROCEDURE [dbo].[usp_fs_file_enumerate]
	@nAssetID int
AS 
	SET nocount ON  
	
BEGIN
	SELECT FS_FILES.*
			,FS_FOLDERS._NAME AS PARENTNAME
	FROM FS_FILES
	LEFT JOIN FS_FOLDERS ON (FS_FILES._PARENTID = FS_FOLDERS._FOLDERID)
	WHERE FS_FILES._ASSETID=@nAssetID
	ORDER BY FS_FILES._PARENTID
END






set ANSI_NULLS ON
set QUOTED_IDENTIFIER ON
GO
--------------------------------------------------------------------------
--																		--
-- usp_fs_files_enumerate_assignments									--
-- ==================================									--
--																		--
--------------------------------------------------------------------------
--																		--
-- Description															--
-- ===========															--
--																		--
-- This stored procedure is part of the AuditWizard Product				--
-- It is used to return a list of File System FILES for an asset		--
--																		--
--------------------------------------------------------------------------
--																		--
-- History																--
-- =======																--
--																		--
--	26-Feb-2009	Chris Drew		Initial Version							--
--																		--
--------------------------------------------------------------------------

CREATE PROCEDURE [dbo].usp_fs_files_enumerate_assignments
AS 
	SET nocount ON  
	
BEGIN
	SELECT FS_FILES.*
			,FS_FOLDERS._NAME AS PARENTNAME
	FROM FS_FILES
	LEFT JOIN FS_FOLDERS ON (FS_FILES._PARENTID = FS_FOLDERS._FOLDERID)
	WHERE FS_FILES._ASSETID=0
	ORDER BY FS_FILES._PARENTID
END



set ANSI_NULLS ON
set QUOTED_IDENTIFIER ON
GO
--------------------------------------------------------------------------
--																		--
-- usp_fs_clean															--
-- ============															--
--																		--
--------------------------------------------------------------------------
--																		--
-- Description															--
-- ===========															--
--																		--
-- This stored procedure is part of the AuditWizard Product				--
-- It is used to delete (clean) all file system entries for an asset	--
--																		--
--------------------------------------------------------------------------
--																		--
-- History																--
-- =======																--
--																		--
--	26-Feb-2009	Chris Drew		Initial Version							--
--																		--
--------------------------------------------------------------------------

CREATE PROCEDURE [dbo].[usp_fs_clean]
	@nAssetID int
AS 
	SET nocount ON  
	
BEGIN
	DELETE FROM FS_FILES WHERE _ASSETID=@nAssetID
	DELETE FROM FS_FOLDERS WHERE _ASSETID=@nAssetID
END
GO



set ANSI_NULLS ON
set QUOTED_IDENTIFIER ON
GO
--------------------------------------------------------------------------
--																		--
-- usp_fs_folder_add													--
-- =================													--
--																		--
--------------------------------------------------------------------------
--																		--
-- Description															--
-- ===========															--
--																		--
-- This stored procedure is part of the AuditWizard Product				--
-- It is used to a File System Folder for an asset						--
--																		--
--------------------------------------------------------------------------
--																		--
-- History																--
-- =======																--
--																		--
--	26-Feb-2009	Chris Drew		Initial Version							--
--																		--
--------------------------------------------------------------------------

CREATE PROCEDURE [dbo].[usp_fs_folder_add]
	@cName varchar(255),
	@nParentID int,
	@nAssetID int,
	@nReturnID int output
AS

DECLARE @nRetCode int
SET nocount ON
SET @nRetCode = 0 

BEGIN

	INSERT INTO FS_FOLDERS
		(_NAME ,_PARENTID ,_ASSETID)
	VALUES
		(@cName ,@nParentID ,@nAssetID)

	SET @nReturnID = @@IDENTITY

END
RETURN  @nRetCode
GO



set ANSI_NULLS ON
set QUOTED_IDENTIFIER ON
GO
--------------------------------------------------------------------------
--																		--
-- usp_fs_file_add														--
-- ===============														--
--																		--
--------------------------------------------------------------------------
--																		--
-- Description															--
-- ===========															--
--																		--
-- This stored procedure is part of the AuditWizard Product				--
-- It is used to a File System File for an asset						--
--																		--
--------------------------------------------------------------------------
--																		--
-- History																--
-- =======																--
--																		--
--	26-Feb-2009	Chris Drew		Initial Version							--
--																		--
--------------------------------------------------------------------------

CREATE PROCEDURE [dbo].[usp_fs_file_add]
	@cName varchar(255),
	@nParentID int,
	@nAssetID int,
	@nSize int,
	@dtCreated datetime,
	@dtModified datetime,
	@dtLastAccessed datetime,
	@cPublisher varchar(255),
	@cProductName varchar(255),
	@cDescription varchar(255),
	@cPVersion1 varchar(255),
	@cPVersion2 varchar(255),
	@cFVersion1 varchar(255),
	@cFVersion2 varchar(255),
	@cFilename varchar(255),
	@nReturnID int output
AS

DECLARE @nRetCode int
DECLARE @nAssignment int

SET nocount ON
SET @nRetCode = 0 

BEGIN

	INSERT INTO FS_FILES
		(_NAME ,_PARENTID ,_ASSETID ,_SIZE ,_CREATED_DATE ,_MODIFIED_DATE ,_LASTACCESSED_DATE
		,_PUBLISHER ,_PRODUCTNAME ,_DESCRIPTION ,_PRODUCT_VERSION1 ,_PRODUCT_VERSION2 ,_FILE_VERSION1
		,_FILE_VERSION2 ,_ORIGINAL_FILENAME)
	VALUES
		(@cName ,@nParentID ,@nAssetID ,@nSize ,@dtCreated ,@dtModified ,@dtLastAccessed ,@cPublisher 
		,@cProductName ,@cDescription ,@cPVersion1 ,@cPVersion2 ,@cFVersion1 ,@cFVersion2 ,@cFilename )

	SET @nReturnID = @@IDENTITY


	-- See if this file has an application assignment and update it if it has
	SET @nAssignment = isnull((SELECT min(_FILEID) FROM FS_FILES 
				WHERE _NAME=@cName 
					AND _PUBLISHER=@cPublisher 
					AND _PRODUCTNAME=@cProductName 
					AND _FILE_VERSION1=@cFVersion1
					AND _ASSETID=0) ,0)
	IF (@nAssignment <> 0)
		UPDATE FS_FILES 
			SET _ASSIGN_APPLICATIONID = (SELECT _ASSIGN_APPLICATIONID FROM FS_FILES WHERE _FILEID=@nAssignment)
		WHERE _FILEID = @nReturnID

END
RETURN  @nRetCode
GO


set ANSI_NULLS ON
set QUOTED_IDENTIFIER ON
GO
--------------------------------------------------------------------------
--																		--
-- usp_fs_file_report													--
-- ==================													--
--																		--
--------------------------------------------------------------------------
--																		--
-- Description															--
-- ===========															--
--																		--
-- This stored procedure is part of the AuditWizard Product				--
-- It is used to return a list of File System FILES for a report		--
--																		--
--------------------------------------------------------------------------
--																		--
-- History																--
-- =======																--
--																		--
--	26-Feb-2009	Chris Drew		Initial Version							--
--																		--
--------------------------------------------------------------------------

CREATE PROCEDURE [dbo].[usp_fs_file_report]
	@cFilter varchar(255)
AS 
	SET nocount ON  
	
BEGIN
	IF (@cFilter = '')
	BEGIN
		SELECT FS_FILES.*
				,FS_FOLDERS._NAME AS PARENTNAME
				,ASSETS._NAME AS ASSETNAME
				,LOCATIONS._FULLNAME AS FULLLOCATIONNAME
		FROM FS_FILES
		LEFT JOIN FS_FOLDERS ON (FS_FILES._PARENTID = FS_FOLDERS._FOLDERID)
		LEFT JOIN ASSETS ON (FS_FILES._ASSETID = ASSETS._ASSETID)
		LEFT JOIN LOCATIONS ON (ASSETS._LOCATIONID = LOCATIONS._LOCATIONID)
		ORDER BY FS_FILES._PARENTID
	END

	ELSE
	BEGIN
		SELECT FS_FILES.*
				,FS_FOLDERS._NAME AS PARENTNAME
				,ASSETS._NAME AS ASSETNAME
				,LOCATIONS._FULLNAME AS FULLLOCATIONNAME
		FROM FS_FILES
			LEFT JOIN FS_FOLDERS ON (FS_FILES._PARENTID = FS_FOLDERS._FOLDERID)
			LEFT JOIN ASSETS ON (FS_FILES._ASSETID = ASSETS._ASSETID)
			LEFT JOIN LOCATIONS ON (ASSETS._LOCATIONID = LOCATIONS._LOCATIONID)
		WHERE FS_FILES._NAME LIKE @cFilter
			ORDER BY FS_FILES._PARENTID
	END
END
GO





set ANSI_NULLS ON
set QUOTED_IDENTIFIER ON
GO
--------------------------------------------------------------------------
--																		--
-- usp_fs_file_assign													--
-- =================													--
--																		--
--------------------------------------------------------------------------
--																		--
-- Description															--
-- ===========															--
--																		--
-- This stored procedure is part of the AuditWizard Product				--
-- It is used to assign a File System File to an application			--
--																		--
--------------------------------------------------------------------------
--																		--
-- History																--
-- =======																--
--																		--
--	26-Feb-2009	Chris Drew		Initial Version							--
--																		--
--------------------------------------------------------------------------

CREATE PROCEDURE [dbo].[usp_fs_file_assign]
	@cName			varchar(255),
	@cPublisher		varchar(255),
	@cProductName	varchar(255),
	@cFVersion1		varchar(255),
	@nApplicationID int,
	@nReturnID		int output
AS

-- Local declarations
DECLARE @nRetCode int,
		@fileAssignmentID int,
		@nAssignedApplicationID int,
		@nReturnInstanceID int,
		@nAssetID int

SET nocount ON
SET @nRetCode = 0 

BEGIN
	SET @nReturnID = 0

	IF (@nApplicationID = 0)
	BEGIN
		-- recover the ID of the file assignment record
		SET @fileAssignmentID = isnull((SELECT MIN(_FILEID) FROM FS_FILES 
										  WHERE _NAME=@cName 
											AND _PUBLISHER=@cPublisher 
											AND _PRODUCTNAME=@cProductName 
											AND _FILE_VERSION1=@cFVersion1
											AND _ASSETID=0) ,0)

		-- Sanity check to ensure we found the mapping
		IF (@fileAssignmentID = 0)
			RETURN  @nRetCode


		-- Unassign any file records which have this assignment set in their record
		UPDATE FS_FILES SET _ASSIGN_APPLICATIONID = 0
			WHERE _NAME=@cName 
				AND _PUBLISHER=@cPublisher 
				AND _PRODUCTNAME=@cProductName 
				AND _FILE_VERSION1=@cFVersion1

		-- Delete the File Assignment record
		DELETE FROM FS_FILES WHERE _FILEID=@fileAssignmentID

		-- We also need to delete the Application and all application instances for this file
		-- Get the ID of the assigned application
		SET @nAssignedApplicationID = isnull((SELECT MIN(_APPLICATIONID) FROM APPLICATIONS 
												WHERE _ASSIGNED_FILEID = @fileAssignmentID) ,0)

		-- Sanity check to ensure we found the mapping
		IF (@nAssignedApplicationID <> 0)
		BEGIN
			
			-- First delete the application instances for this application
			DELETE FROM APPLICATION_INSTANCES WHERE _APPLICATIONID=@nAssignedApplicationID

			-- Then delete the application itself
			DELETE FROM APPLICATIONS WHERE _APPLICATIONID=@nAssignedApplicationID

		END
	END

	ELSE

	BEGIN
		-- We need to insert a new record into the database which will be the file assignment record 
		INSERT INTO FS_FILES
			(_NAME ,_PUBLISHER ,_PRODUCTNAME ,_FILE_VERSION1)
		VALUES
			(@cName ,@cPublisher ,@cProductName ,@cFVersion1)
		SET @nReturnID = @@IDENTITY

		-- Update the APPLICATIONS table so that the APPLICATION Knows it has been created from an assign
		UPDATE APPLICATIONS SET _ASSIGNED_FILEID=@nReturnID WHERE _APPLICATIONID=@nApplicationID

		-- We now need to update any EXISTING file records so that the assignment is propogated throughout
		-- the FS_FILES table (all assets will not have the assignment in the FS_FILES table)
		UPDATE FS_FILES SET _ASSIGN_APPLICATIONID = @nApplicationID 
			WHERE _NAME=@cName 
				AND _PUBLISHER=@cPublisher 
				AND _PRODUCTNAME=@cProductName 
				AND _FILE_VERSION1=@cFVersion1

		-- Althout the FS_FILES table now reflects the fact that each instanmce of the file has been assigned
		-- to the application we still have an issue in that we have not created the actual APPLICATION_INSTANCE 
		-- record for each asset/file.  We can do this here by using a cursor to iterate through each record in the
		-- FS_FILES table for this application and crteating the instance for each record for an asset
		DECLARE cursorFiles CURSOR FAST_FORWARD FOR 
			SELECT _ASSETID FROM FS_FILES WHERE _ASSIGN_APPLICATIONID = @nApplicationID

		OPEN cursorFiles 
		FETCH NEXT FROM cursorFiles INTO @nAssetID

		WHILE @@FETCH_STATUS = 0  
		BEGIN 
			-- OK - for this asset and application create an application instance
			IF (@nAssetID <> 0)
				EXEC usp_add_application_instance @nAssetID ,@nApplicationID ,'' ,'' ,@nReturnInstanceID output

			-- Fetch the next record
			FETCH NEXT FROM cursorFiles INTO @nAssetID
		END
		CLOSE cursorFiles 
		DEALLOCATE cursorFiles


	END
END
RETURN  @nRetCode
GO




SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Chris Drew
-- Create date: 22/Apr/09
-- Description:	Gets the current license Count
--
-- =============================================
 
CREATE PROCEDURE [dbo].[usp_get_licensecount] 
AS

BEGIN
	DECLARE @nReturnCount int
	set nocount on	
	SET @nReturnCount = (SELECT COUNT(*) FROM ASSETS WHERE _LASTAUDIT<>'' AND _PARENT_ASSETID=0)
	RETURN @nReturnCount
END





SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--------------------------------------------------------------------------
--																		--
-- usp_cleardown_database												--
-- ======================												--
--																		--
--------------------------------------------------------------------------
--																		--
-- Description															--
-- ===========															--
--																		--
-- This stored procedure is part of the AuditWizard Product				--
-- It is used to cleardown all data in the database						--
--																		--
--------------------------------------------------------------------------
--																		--
-- History																--
-- =======																--
--																		--
--	20-Apr-2009	Chris Drew		Initial Version							--
--																		--
--------------------------------------------------------------------------

CREATE PROCEDURE [dbo].[usp_cleardown_database]
AS     
	SET nocount ON

BEGIN
	-- Referential Integrity Checks
	-- ============================ 
	--
	-- We must delete the tables in order to ensure that we do not fail this operation owing to
	-- foreign key conflicts
	--
	--	APPLICATION_INSTANCES
	--	AUDITEDITEMS
	--	AUDITTRAIL
	--	DOCUMENTS
	--	NOTES
	--	OPERATIONS
	--	USERDATA_VALUES
	--	FS_FILES
	--  FS_FOLDER
	--	ALERTS
	--
	-- Delete related entries in the APPLICATION_INSTANCES table
	-- Note that this may leave unresolved references in the Applications table which we will have to handle also
	DELETE FROM APPLICATION_INSTANCES

	-- Delete Applications Table
	DELETE FROM APPLICATIONS

	-- Delete related entries in the AuditedItems table
	DELETE FROM AUDITEDITEMS WHERE _ASSETID<>0

	-- Delete related entries in the AUDITTRAIL table
	DELETE FROM AUDITTRAIL

	-- Delete related entries in the DOCUMENTS table
	DELETE FROM DOCUMENTS 

	-- Delete related entries in the NOTES table
	DELETE FROM NOTES 

	-- Delete related entries in the OPERATIONS table
	DELETE FROM OPERATIONS 
  
	-- Delete related entries in the UserData_values table
	DELETE FROM USERDATA_VALUES

	-- Delete User Data definitions
	DELETE FROM USERDATA_DEFINITIONS

	-- Delete related entries in the FS_FILES table
	DELETE FROM FS_FILES

	-- Delete related entries in the FS_FOLDERS table
	DELETE FROM FS_FOLDERS

	-- Delete related entries in the ACTIONS table
	DELETE FROM ACTIONS 

	-- Delete related entries in the ALERTS table
	DELETE FROM ALERTS 

	-- Now we can delete the asset itself
	DELETE FROM ASSETS 

	-- Delete non-internal asset types
	DELETE FROM ASSET_TYPES WHERE _INTERNAL<>1

	-- Delete Picklists and Pickitems
	DELETE FROM PICKLISTS

	-- Delete locations (other than the first)
	DELETE FROM LOCATIONS WHERE _PARENTID<>null
END
GO


SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--------------------------------------------------------------------------
--																		--
-- usp_auditeditem_getname												--
-- =======================												--
--																		--
--------------------------------------------------------------------------
--																		--
-- Description															--
-- ===========															--
--																		--
-- This stored procedure is part of the AuditWizard Product				--
-- It is used to recover the name of an audited item category			--
--																		--
--------------------------------------------------------------------------
--																		--
-- History																--
-- =======																--
--																		--
--	04-Mar-2008	Chris Drew		Initial Version							--
--																		--
--------------------------------------------------------------------------
CREATE PROCEDURE [dbo].[usp_auditeditem_getname]
	@itemID int,
	@retValue varchar(900) output
AS   
	
BEGIN
	SET @retValue = (SELECT _CATEGORY FROM AUDITEDITEMS WHERE _AUDITEDITEMID=@itemID)
END
GO