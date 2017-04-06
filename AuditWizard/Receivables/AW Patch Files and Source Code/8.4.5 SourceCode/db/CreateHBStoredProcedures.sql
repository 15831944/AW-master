/****** Object:  StoredProcedure [dbo].[hb_asset_applications_enumerate]    Script Date: 04/29/2010 14:47:08 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


--------------------------------------------------------------------------
--																		--
-- hb_asset_applications_enumerate									--
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
--	01/19/2010	Lei Zi			Initial Version							--
--																		--
--------------------------------------------------------------------------

IF EXISTS(SELECT * FROM dbo.sysobjects WHERE type = 'P' AND name = 'hb_asset_applications_enumerate')
     DROP PROCEDURE hb_asset_applications_enumerate
GO

Create PROCEDURE [dbo].[hb_asset_applications_enumerate]
	@assetID int,
	@forPublisher varchar(5000),
	@showIncluded bit,
	@showIgnored bit
AS     
	--SET nocount ON

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






/****** Object:  StoredProcedure [dbo].[hb_asset_enumerate]    Script Date: 04/29/2010 14:47:35 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO



-- =============================================
--	01/20/2010		Lei Zi		Initial Version							--
-- Description:	Returns a table of assets
-- =============================================

IF EXISTS(SELECT * FROM dbo.sysobjects WHERE type = 'P' AND name = 'hb_asset_enumerate')
     DROP PROCEDURE hb_asset_enumerate
GO

Create PROCEDURE [dbo].[hb_asset_enumerate]
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

	-- --SET nocount ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	--SET nocount ON;

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
	IF @cWhere <> ''
		EXEC (@cSql)
END


GO




/****** Object:  StoredProcedure [dbo].[hb_asset_getbyid]    Script Date: 04/29/2010 14:47:43 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO



-- =============================================
-- Author:		Lei Zi
-- Create date: 01/19/09
-- Description:	Returns the asset by id
-- =============================================

IF EXISTS(SELECT * FROM dbo.sysobjects WHERE type = 'P' AND name = 'hb_asset_getbyid')
     DROP PROCEDURE hb_asset_getbyid
GO

Create PROCEDURE [dbo].[hb_asset_getbyid]
	@assetID	int
AS
BEGIN
	DECLARE @cSql varchar(1024)
	DECLARE @cWhere varchar(1024)

	-- --SET nocount ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	--SET nocount ON;

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
				WHERE ASSETS._PARENT_ASSETID=0 AND _ASSETID=' + cast(@assetID as varchar(16))
				
	-- Now execute this statement
	EXEC (@cSql)
END


GO




/****** Object:  StoredProcedure [dbo].[hb_asset_getbylikeipaddr]    Script Date: 04/29/2010 14:47:51 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO



-- =============================================
-- Author:		Lei Zi
-- Create date: 01/20/2010
-- Description:	Get Asset By Name
-- =============================================

IF EXISTS(SELECT * FROM dbo.sysobjects WHERE type = 'P' AND name = 'hb_asset_getbylikeipaddr')
     DROP PROCEDURE hb_asset_getbylikeipaddr
GO

create PROCEDURE [dbo].[hb_asset_getbylikeipaddr]
	@ipaddr	varchar(50)
AS
BEGIN
	DECLARE @cSql varchar(1024)
	DECLARE @cWhere varchar(1024)
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	--SET NOCOUNT ON;

    -- Insert statements for procedure here
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
				WHERE ASSETS._PARENT_ASSETID=0 AND ASSETS._IPADDRESS like +'%' +@ipaddr+'%'
				
END



GO




/****** Object:  StoredProcedure [dbo].[hb_asset_getbylikename]    Script Date: 04/29/2010 14:47:58 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO



-- =============================================
-- Author:		Lei Zi
-- Create date: 01/20/2010
-- Description:	Get Asset By Name
-- =============================================

IF EXISTS(SELECT * FROM dbo.sysobjects WHERE type = 'P' AND name = 'hb_asset_getbylikename')
     DROP PROCEDURE hb_asset_getbylikename
GO

CREATE PROCEDURE [dbo].[hb_asset_getbylikename]
	@assetName	varchar(50)
AS
BEGIN
	DECLARE @cSql varchar(1024)
	DECLARE @cWhere varchar(1024)
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	--SET NOCOUNT ON;

    -- Insert statements for procedure here
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
				WHERE ASSETS._PARENT_ASSETID=0 AND ASSETS._NAME like +'%' +@assetName+'%'
				
END



GO




/****** Object:  StoredProcedure [dbo].[hb_asset_getbylikesite]    Script Date: 04/29/2010 14:48:04 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO



-- =============================================
-- Author:		Lei Zi
-- Create date: 01/20/2010
-- Description:	Get Asset By Name
-- =============================================

IF EXISTS(SELECT * FROM dbo.sysobjects WHERE type = 'P' AND name = 'hb_asset_getbylikesite')
     DROP PROCEDURE hb_asset_getbylikesite
GO

create PROCEDURE [dbo].[hb_asset_getbylikesite]
	@site	varchar(50)
AS
BEGIN
	DECLARE @cSql varchar(1024)
	DECLARE @cWhere varchar(1024)
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	--SET NOCOUNT ON;

    -- Insert statements for procedure here
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
				WHERE ASSETS._PARENT_ASSETID=0 AND [LOCATIONS]._FULLNAME like +'%' +@site+'%'
				
END



GO




/****** Object:  StoredProcedure [dbo].[hb_asset_getbyliketagname]    Script Date: 04/29/2010 14:48:11 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO



-- =============================================
-- Author:		Lei Zi
-- Create date: 01/20/2010
-- Description:	Get Asset By Name
-- =============================================

IF EXISTS(SELECT * FROM dbo.sysobjects WHERE type = 'P' AND name = 'hb_asset_getbyliketagname')
     DROP PROCEDURE hb_asset_getbyliketagname
GO

create PROCEDURE [dbo].[hb_asset_getbyliketagname]
	@assettagName	varchar(50)
AS
BEGIN
	DECLARE @cSql varchar(1024)
	DECLARE @cWhere varchar(1024)
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	--SET NOCOUNT ON;

    -- Insert statements for procedure here
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
				WHERE ASSETS._PARENT_ASSETID=0 AND ASSETS._ASSETTAG like +'%' +@assettagName+'%'
				
END



GO




/****** Object:  StoredProcedure [dbo].[hb_asset_getbylikeuser]    Script Date: 04/29/2010 14:48:17 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO



-- =============================================
-- Author:		Lei Zi
-- Create date: 01/20/2010
-- Description:	Get Asset By Name
-- =============================================

IF EXISTS(SELECT * FROM dbo.sysobjects WHERE type = 'P' AND name = 'hb_asset_getbylikeuser')
     DROP PROCEDURE hb_asset_getbylikeuser
GO

create PROCEDURE [dbo].[hb_asset_getbylikeuser]
	@user	varchar(50)
AS
BEGIN
	DECLARE @cSql varchar(1024)
	DECLARE @cWhere varchar(1024)
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	--SET NOCOUNT ON;

    -- Insert statements for procedure here
		SELECT [ASSETS].*
					, [LOCATIONS]._FULLNAME AS FULLLOCATIONNAME
					, [AUDITEDITEMS]._VALUE AS UserName 
					FROM [AUDITEDITEMS],[ASSETS] 
					Left join [LOCATIONS] ON [ASSETS]._LOCATIONID = [LOCATIONS]._LOCATIONID 

					WHERE [ASSETS]._ASSETID = [AUDITEDITEMS]._ASSETID AND [AUDITEDITEMS]._CATEGORY='Hardware|Network' 
						AND [AUDITEDITEMS]._NAME='Username' AND [AUDITEDITEMS]._VALUE like +'%' +@user+'%'
				
END



GO




/****** Object:  StoredProcedure [dbo].[hb_asset_getbyname]    Script Date: 04/29/2010 14:48:24 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO



-- =============================================
-- Author:		Lei Zi
-- Create date: 01/20/2010
-- Description:	Get Asset By Name
-- =============================================

IF EXISTS(SELECT * FROM dbo.sysobjects WHERE type = 'P' AND name = 'hb_asset_getbyname')
     DROP PROCEDURE hb_asset_getbyname
GO

CREATE PROCEDURE [dbo].[hb_asset_getbyname]
	@assetName	varchar(50)
AS
BEGIN
	DECLARE @cSql varchar(1024)
	DECLARE @cWhere varchar(1024)
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	--SET NOCOUNT ON;

    -- Insert statements for procedure here
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
				WHERE ASSETS._PARENT_ASSETID=0 AND ASSETS._NAME=@assetName
				
END



GO




/****** Object:  StoredProcedure [dbo].[hb_asset_getos]    Script Date: 04/29/2010 14:48:29 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO



--------------------------------------------------------------------------
--																		--
-- hb_asset_getos													--
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
--	01/19/2010	Lei Zi		Initial Version							--
--																		--
--------------------------------------------------------------------------

IF EXISTS(SELECT * FROM dbo.sysobjects WHERE type = 'P' AND name = 'hb_asset_getos')
     DROP PROCEDURE hb_asset_getos
GO

create PROCEDURE [dbo].[hb_asset_getos]
	@assetID int
AS     
	--SET nocount ON

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




/****** Object:  StoredProcedure [dbo].[hb_assettype_enumerate]    Script Date: 04/29/2010 14:48:36 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO



--------------------------------------------------------------------------
--																		--
-- hb_assettypes_enumerate												--
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
--	01/20/2010		Lei Zi		Initial Version							--
--																		--
--------------------------------------------------------------------------

IF EXISTS(SELECT * FROM dbo.sysobjects WHERE type = 'P' AND name = 'hb_assettype_enumerate')
     DROP PROCEDURE hb_assettype_enumerate
GO

Create PROCEDURE [dbo].[hb_assettype_enumerate]
AS 
	--SET nocount ON  
	
BEGIN
	SELECT ASSET_TYPES.*
	FROM dbo.ASSET_TYPES
	ORDER BY _ASSETTYPEID
END


GO





/****** Object:  StoredProcedure [dbo].[hb_auditeditems_enumerate]    Script Date: 04/29/2010 14:48:44 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO



--------------------------------------------------------------------------
--																		--
-- hb_auditeditems_enumerate											--
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
--	01/20/2010		Lei Zi		Initial Version							--
--																		--
--------------------------------------------------------------------------

IF EXISTS(SELECT * FROM dbo.sysobjects WHERE type = 'P' AND name = 'hb_auditeditems_enumerate')
     DROP PROCEDURE hb_auditeditems_enumerate
GO

Create PROCEDURE [dbo].[hb_auditeditems_enumerate]
	@assetID int,
	@parentCategory varchar(1024),
	@bAllChildren bit
AS   
	
BEGIN

DECLARE @cSql varchar(1024)
DECLARE @cWhere varchar(1024)

	-- --SET nocount ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	--SET nocount ON;

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





/****** Object:  StoredProcedure [dbo].[hb_audittrail_getassethistory]    Script Date: 04/29/2010 14:48:51 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO



--------------------------------------------------------------------------
--																		--
-- hb_audittrail_getassethistory										--
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
--	01/19/2010	Lei Zi			Initial Version							--
--																		--
--------------------------------------------------------------------------

IF EXISTS(SELECT * FROM dbo.sysobjects WHERE type = 'P' AND name = 'hb_audittrail_getassethistory')
     DROP PROCEDURE hb_audittrail_getassethistory
GO

CREATE PROCEDURE [dbo].[hb_audittrail_getassethistory]
	@nAssetID int,
	@dtStartDate DateTime,
	@dtEndDate DateTime

AS 
	--SET nocount ON  
	
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
	SET @cSQL = @cSQL + ' ORDER BY _AUDITTRAILID desc'

	-- Execute

	print @cSQL
	EXEC (@cSQL)
END


GO




/****** Object:  StoredProcedure [dbo].[hb_childasset_enumerate]    Script Date: 04/29/2010 14:48:57 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO



-- =============================================
--	01/20/2010		Lei Zi		Initial Version							--
-- Description:	Returns a table of child assets
-- =============================================

IF EXISTS(SELECT * FROM dbo.sysobjects WHERE type = 'P' AND name = 'hb_childasset_enumerate')
     DROP PROCEDURE hb_childasset_enumerate
GO

Create PROCEDURE [dbo].[hb_childasset_enumerate]
	@nAssetID	int
AS
BEGIN

	-- --SET nocount ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	--SET nocount ON;

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




/****** Object:  StoredProcedure [dbo].[hb_fs_file_enumerate]    Script Date: 04/29/2010 14:49:05 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO



--------------------------------------------------------------------------
--																		--
-- hb_fs_files_enumerate												--
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
--	01/20/2010		Lei Zi		Initial Version							--
--																		--
--------------------------------------------------------------------------

IF EXISTS(SELECT * FROM dbo.sysobjects WHERE type = 'P' AND name = 'hb_fs_file_enumerate')
     DROP PROCEDURE hb_fs_file_enumerate
GO

Create PROCEDURE [dbo].[hb_fs_file_enumerate]
	@nAssetID int
AS 
	--SET nocount ON  
	
BEGIN
	SELECT FS_FILES.*
			,FS_FOLDERS._NAME AS PARENTNAME
	FROM FS_FILES
	LEFT JOIN FS_FOLDERS ON (FS_FILES._PARENTID = FS_FOLDERS._FOLDERID)
	WHERE FS_FILES._ASSETID=@nAssetID
	ORDER BY FS_FILES._PARENTID
END






--SET ANSI_NULLS ON
--SET QUOTED_IDENTIFIER ON


GO





/****** Object:  StoredProcedure [dbo].[hb_fs_fileinfolder]    Script Date: 04/29/2010 14:49:11 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO



-- =============================================
-- Author:		Lei Zi
-- Create date: 01/20/2010
-- Description:	Get all the files in specified folder
-- =============================================

IF EXISTS(SELECT * FROM dbo.sysobjects WHERE type = 'P' AND name = 'hb_fs_fileinfolder')
     DROP PROCEDURE hb_fs_fileinfolder
GO

Create PROCEDURE [dbo].[hb_fs_fileinfolder]
	@nFolderId int
AS 
	--SET nocount ON  
	
BEGIN
	SELECT FS_FILES.*
	FROM FS_FILES
	WHERE FS_FILES._PARENTID=@nFolderId
	ORDER BY FS_FILES._Name
END




GO





/****** Object:  StoredProcedure [dbo].[hb_fs_folder_enumerate]    Script Date: 04/29/2010 14:49:19 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO



--------------------------------------------------------------------------
--																		--
-- hb_fs_folders_enumerate												--
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
--	01/20/2010		Lei Zi		Initial Version							--
--																		--
--------------------------------------------------------------------------

IF EXISTS(SELECT * FROM dbo.sysobjects WHERE type = 'P' AND name = 'hb_fs_folder_enumerate')
     DROP PROCEDURE hb_fs_folder_enumerate
GO

create PROCEDURE [dbo].[hb_fs_folder_enumerate]
	@nAssetID int
AS 
	--SET nocount ON  
	
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





/****** Object:  StoredProcedure [dbo].[hb_locations_enumerate]    Script Date: 04/29/2010 14:49:25 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO



--------------------------------------------------------------------------
--																		--
-- hb_locations_enumerate												--
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
--	01/20/2010		Lei Zi		Initial Version							--
--																		--
--------------------------------------------------------------------------

IF EXISTS(SELECT * FROM dbo.sysobjects WHERE type = 'P' AND name = 'hb_locations_enumerate')
     DROP PROCEDURE hb_locations_enumerate
GO

Create PROCEDURE [dbo].[hb_locations_enumerate]  
	@nParentID int,
	@bShowAll bit
AS
	DECLARE @cSql varchar(1024) 
	DECLARE @cWhere varchar(1024) 
	--SET nocount ON  
	
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





/****** Object:  StoredProcedure [dbo].[hb_udd_enumerate]    Script Date: 04/29/2010 14:49:31 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


--------------------------------------------------------------------------
--																		--
-- hb_udd_enumerate													--
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
--	01/20/2010		Lei Zi		Initial Version							--
--																		--
--------------------------------------------------------------------------

IF EXISTS(SELECT * FROM dbo.sysobjects WHERE type = 'P' AND name = 'hb_udd_enumerate')
     DROP PROCEDURE hb_udd_enumerate
GO

Create PROCEDURE [dbo].[hb_udd_enumerate]
	@nScope int
AS 
	--SET nocount ON  
	
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




/****** Object:  StoredProcedure [dbo].[hb_userdata_getvalues]    Script Date: 04/29/2010 14:49:38 ******/
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
--	20/01/2010	Lei Zi			Initial Version							--
--																		--
--------------------------------------------------------------------------

IF EXISTS(SELECT * FROM dbo.sysobjects WHERE type = 'P' AND name = 'hb_userdata_getvalues')
     DROP PROCEDURE hb_userdata_getvalues
GO

CREATE PROCEDURE [dbo].[hb_userdata_getvalues]
	@nParentType	int,
	@nParentID		int,
	@nCategoryID	int
AS 
	DECLARE @cSQL varchar(1024) 
	--SET nocount ON  
	
BEGIN

		SELECT USERDATA_VALUES._VALUE ,USERDATA_DEFINITIONS._NAME
		FROM USERDATA_VALUES
		Left join USERDATA_DEFINITIONS on USERDATA_DEFINITIONS._USERDEFID=USERDATA_VALUES._USERDEFID
		WHERE _PARENTTYPE=@nParentType
			AND USERDATA_VALUES._PARENTID = @nParentID
			AND USERDATA_VALUES._USERDEFID IN
				(SELECT _USERDEFID FROM USERDATA_DEFINITIONS WHERE _PARENTID=@nCategoryID)

END

GO


-- =============================================
-- Author:		Lei Zi
-- Create date: 01/20/2010
-- Description:	Get Asset By Serial Number
-- =============================================

IF EXISTS(SELECT * FROM dbo.sysobjects WHERE type = 'P' AND name = 'hb_asset_getbylikesn')
     DROP PROCEDURE hb_asset_getbylikesn
GO

Create PROCEDURE [dbo].[hb_asset_getbylikesn]
	@assetSN	varchar(50)
AS
BEGIN
	DECLARE @cSql varchar(1024)
	DECLARE @cWhere varchar(1024)
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	--SET NOCOUNT ON;

    -- Insert statements for procedure here
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
				WHERE ASSETS._PARENT_ASSETID=0 AND ASSETS._SERIAL_NUMBER like +'%' +@assetSN+'%'
				
END

GO


-- =============================================
-- Author:		Lei Zi
-- Create date: 01/20/2010
-- Description:	Get Asset By MAC
-- =============================================

IF EXISTS(SELECT * FROM dbo.sysobjects WHERE type = 'P' AND name = 'hb_asset_getbylikemac')
     DROP PROCEDURE hb_asset_getbylikemac
GO

Create PROCEDURE [dbo].[hb_asset_getbylikemac]
	@macaddr	varchar(50)
AS
BEGIN
	DECLARE @cSql varchar(1024)
	DECLARE @cWhere varchar(1024)
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	--SET NOCOUNT ON;

    -- Insert statements for procedure here
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
				WHERE ASSETS._PARENT_ASSETID=0 AND ASSETS._MACADDRESS like +'%' +@macaddr+'%'
				
END

GO


-- =============================================
-- Author:		Lei Zi
-- Create date: 01/20/2010
-- Description:	Get Asset By Make
-- =============================================

IF EXISTS(SELECT * FROM dbo.sysobjects WHERE type = 'P' AND name = 'hb_asset_getbylikemake')
     DROP PROCEDURE hb_asset_getbylikemake
GO

Create PROCEDURE [dbo].[hb_asset_getbylikemake]
	@assetMake	varchar(50)
AS
BEGIN
	DECLARE @cSql varchar(1024)
	DECLARE @cWhere varchar(1024)
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	--SET NOCOUNT ON;

    -- Insert statements for procedure here
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
				WHERE ASSETS._PARENT_ASSETID=0 AND ASSETS._MAKE like +'%' +@assetMake+'%'
				
END

GO



-- =============================================
-- Author:		Lei Zi
-- Create date: 01/20/2010
-- Description:	Get Asset By Model
-- =============================================

IF EXISTS(SELECT * FROM dbo.sysobjects WHERE type = 'P' AND name = 'hb_asset_getbylikemodel')
     DROP PROCEDURE hb_asset_getbylikemodel
GO

Create PROCEDURE [dbo].[hb_asset_getbylikemodel]
	@assetModel	varchar(50)
AS
BEGIN
	DECLARE @cSql varchar(1024)
	DECLARE @cWhere varchar(1024)
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	--SET NOCOUNT ON;

    -- Insert statements for procedure here
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
				WHERE ASSETS._PARENT_ASSETID=0 AND ASSETS._MODEL like +'%' +@assetModel+'%'
				
END

GO



-- =============================================
-- Author:		Lei Zi
-- Create date: 01/20/2010
-- Description:	Get Asset By User Data
-- =============================================

IF EXISTS(SELECT * FROM dbo.sysobjects WHERE type = 'P' AND name = 'hb_asset_getbylikeuserdata')
     DROP PROCEDURE hb_asset_getbylikeuserdata
GO

CREATE PROCEDURE [dbo].[hb_asset_getbylikeuserdata]
	@assetUserData	varchar(50)
AS
BEGIN
	DECLARE @cSql varchar(1024)
	DECLARE @cWhere varchar(1024)
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	--SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT ASSETS.*
					 , ASSET_TYPES._NAME AS ASSETTYPENAME 
					 , ASSET_TYPES._ICON AS ICON
					 , ASSET_TYPES._AUDITABLE AS AUDITABLE
					 , LOCATIONS._NAME AS LOCATIONNAME
					 , LOCATIONS._FULLNAME AS FULLLOCATIONNAME
					 , DOMAINS._NAME AS DOMAINNAME				
					 , ISNULL(SUPPLIERS._NAME, '''') SUPPLIER_NAME
					 , uv._VALUE AS ItemValue
				FROM USERDATA_VALUES uv 
					LEFT JOIN ASSETS ON (ASSETS._ASSETID = uv._PARENTID)
					LEFT JOIN ASSET_TYPES ON (ASSETS._ASSETTYPEID=ASSET_TYPES._ASSETTYPEID)
					LEFT JOIN LOCATIONS   ON (ASSETS._LOCATIONID=LOCATIONS._LOCATIONID)
					LEFT JOIN DOMAINS	  ON (ASSETS._DOMAINID=DOMAINS._DOMAINID)
					LEFT JOIN SUPPLIERS   ON (ASSETS._SUPPLIERID = SUPPLIERS._SUPPLIERID)
				WHERE ASSETS._PARENT_ASSETID=0 AND uv._VALUE LIKE +'%' +@assetUserData+'%'
				
END

GO


-- =============================================
-- Author:		Lei Zi
-- Create date: 01/20/2010
-- Description:	Get Asset By Hardware
-- =============================================

IF EXISTS(SELECT * FROM dbo.sysobjects WHERE type = 'P' AND name = 'hb_asset_getbylikehardware')
     DROP PROCEDURE hb_asset_getbylikehardware
GO

CREATE PROCEDURE [dbo].[hb_asset_getbylikehardware]
	@assetHardware	varchar(50)
AS
BEGIN
	DECLARE @cSql varchar(1024)
	DECLARE @cWhere varchar(1024)
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	--SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT ASSETS.*
					 , ASSET_TYPES._NAME AS ASSETTYPENAME 
					 , ASSET_TYPES._ICON AS ICON
					 , ASSET_TYPES._AUDITABLE AS AUDITABLE
					 , LOCATIONS._NAME AS LOCATIONNAME
					 , LOCATIONS._FULLNAME AS FULLLOCATIONNAME
					 , DOMAINS._NAME AS DOMAINNAME				
					 , ISNULL(SUPPLIERS._NAME, '''') SUPPLIER_NAME
					 , ai._VALUE AS ItemValue
				FROM ASSETS
					LEFT JOIN AUDITEDITEMS ai ON (ai._ASSETID = ASSETS._ASSETID) 
					LEFT JOIN ASSET_TYPES ON (ASSETS._ASSETTYPEID=ASSET_TYPES._ASSETTYPEID)
					LEFT JOIN LOCATIONS   ON (ASSETS._LOCATIONID=LOCATIONS._LOCATIONID)
					LEFT JOIN DOMAINS	  ON (ASSETS._DOMAINID=DOMAINS._DOMAINID)
					LEFT JOIN SUPPLIERS   ON (ASSETS._SUPPLIERID = SUPPLIERS._SUPPLIERID)
				WHERE ASSETS._PARENT_ASSETID=0 AND ai._CATEGORY LIKE 'Hardware|%' AND ai._VALUE LIKE +'%' +@assetHardware+'%'
				
END

GO


-- =============================================
-- Author:		Lei Zi
-- Create date: 01/20/2010
-- Description:	Get Asset By Files
-- =============================================

IF EXISTS(SELECT * FROM dbo.sysobjects WHERE type = 'P' AND name = 'hb_asset_getbylikefiles')
     DROP PROCEDURE hb_asset_getbylikefiles
GO

CREATE PROCEDURE [dbo].[hb_asset_getbylikefiles]
	@assetFile	varchar(50)
AS
BEGIN
	DECLARE @cSql varchar(1024)
	DECLARE @cWhere varchar(1024)
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	--SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT ASSETS.*
					 , ASSET_TYPES._NAME AS ASSETTYPENAME 
					 , ASSET_TYPES._ICON AS ICON
					 , ASSET_TYPES._AUDITABLE AS AUDITABLE
					 , LOCATIONS._NAME AS LOCATIONNAME
					 , LOCATIONS._FULLNAME AS FULLLOCATIONNAME
					 , DOMAINS._NAME AS DOMAINNAME				
					 , ISNULL(SUPPLIERS._NAME, '''') SUPPLIER_NAME
					 , f._NAME AS ItemValue
				FROM FS_FILES f
					LEFT JOIN ASSETS ON (ASSETS._ASSETID = f._ASSETID) 
					INNER JOIN FS_FOLDERS fl ON (fl._FOLDERID = f._PARENTID) 
					LEFT JOIN AUDITEDITEMS ai ON (ai._ASSETID = ASSETS._ASSETID) 
					LEFT JOIN ASSET_TYPES ON (ASSETS._ASSETTYPEID=ASSET_TYPES._ASSETTYPEID)
					LEFT JOIN LOCATIONS   ON (ASSETS._LOCATIONID=LOCATIONS._LOCATIONID)
					LEFT JOIN DOMAINS	  ON (ASSETS._DOMAINID=DOMAINS._DOMAINID)
					LEFT JOIN SUPPLIERS   ON (ASSETS._SUPPLIERID = SUPPLIERS._SUPPLIERID)
				WHERE ASSETS._PARENT_ASSETID=0 AND f._NAME LIKE +'%' +@assetFile+'%'
				
END

GO



-- =============================================
-- Author:		Lei Zi
-- Create date: 01/20/2010
-- Description:	Get Asset By Internet cache files
-- =============================================

IF EXISTS(SELECT * FROM dbo.sysobjects WHERE type = 'P' AND name = 'hb_asset_getbylikeinternet')
     DROP PROCEDURE hb_asset_getbylikeinternet
GO

CREATE PROCEDURE [dbo].[hb_asset_getbylikeinternet]
	@assetInet	varchar(50)
AS
BEGIN
	DECLARE @cSql varchar(1024)
	DECLARE @cWhere varchar(1024)
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	--SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT ASSETS.*
					 , ASSET_TYPES._NAME AS ASSETTYPENAME 
					 , ASSET_TYPES._ICON AS ICON
					 , ASSET_TYPES._AUDITABLE AS AUDITABLE
					 , LOCATIONS._NAME AS LOCATIONNAME
					 , LOCATIONS._FULLNAME AS FULLLOCATIONNAME
					 , DOMAINS._NAME AS DOMAINNAME				
					 , ISNULL(SUPPLIERS._NAME, '''') SUPPLIER_NAME
					 , AUDITEDITEMS._CATEGORY AS ItemValue
				FROM AUDITEDITEMS
					LEFT JOIN ASSETS ON (AUDITEDITEMS._ASSETID = ASSETS._ASSETID) 
					LEFT JOIN ASSET_TYPES ON (ASSETS._ASSETTYPEID=ASSET_TYPES._ASSETTYPEID)
					LEFT JOIN LOCATIONS   ON (ASSETS._LOCATIONID=LOCATIONS._LOCATIONID)
					LEFT JOIN DOMAINS	  ON (ASSETS._DOMAINID=DOMAINS._DOMAINID)
					LEFT JOIN SUPPLIERS   ON (ASSETS._SUPPLIERID = SUPPLIERS._SUPPLIERID)
				WHERE ASSETS._PARENT_ASSETID=0 AND AUDITEDITEMS._CATEGORY LIKE 'Internet|History|%' AND AUDITEDITEMS._CATEGORY LIKE +'%' +@assetInet+'%'
				
END

GO



-- =============================================
-- Author:		Lei Zi
-- Create date: 01/20/2010
-- Description:	Get Asset By Application
-- =============================================

IF EXISTS(SELECT * FROM dbo.sysobjects WHERE type = 'P' AND name = 'hb_asset_getbylikeapplication')
     DROP PROCEDURE hb_asset_getbylikeapplication
GO

CREATE PROCEDURE [dbo].[hb_asset_getbylikeapplication]
	@assetAppName	varchar(50)
AS
BEGIN
	DECLARE @cSql varchar(1024)
	DECLARE @cWhere varchar(1024)
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	--SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT ASSETS.*
					 , ASSET_TYPES._NAME AS ASSETTYPENAME 
					 , ASSET_TYPES._ICON AS ICON
					 , ASSET_TYPES._AUDITABLE AS AUDITABLE
					 , LOCATIONS._NAME AS LOCATIONNAME
					 , LOCATIONS._FULLNAME AS FULLLOCATIONNAME
					 , DOMAINS._NAME AS DOMAINNAME				
					 , ISNULL(SUPPLIERS._NAME, '''') SUPPLIER_NAME
					 , ap._NAME AS ItemValue
				FROM APPLICATIONS ap
					LEFT JOIN APPLICATION_INSTANCES ai ON (ai._APPLICATIONID = ap._APPLICATIONID) 
					LEFT JOIN ASSETS ON (ASSETS._ASSETID = ai._ASSETID) 
					LEFT JOIN ASSET_TYPES ON (ASSETS._ASSETTYPEID=ASSET_TYPES._ASSETTYPEID)
					LEFT JOIN LOCATIONS   ON (ASSETS._LOCATIONID=LOCATIONS._LOCATIONID)
					LEFT JOIN DOMAINS	  ON (ASSETS._DOMAINID=DOMAINS._DOMAINID)
					LEFT JOIN SUPPLIERS   ON (ASSETS._SUPPLIERID = SUPPLIERS._SUPPLIERID)
				WHERE ASSETS._PARENT_ASSETID=0 AND ap._NAME LIKE +'%' +@assetAppName+'%'
				
END

GO

