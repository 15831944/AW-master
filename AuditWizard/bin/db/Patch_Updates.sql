--------------------------------------------------------------------------
--																		--
-- AuditWizard Patch Updates											--
-- ===========================											--
--																		--
--------------------------------------------------------------------------
--																		--
-- Description															--
-- ===========															--
--																		--
-- Update detail for the Patch form 8.4.5								--
--																		--
--------------------------------------------------------------------------
--																		--
-- History																--
-- =======																--
--																		--
--	18-April-2017			Madhav		Initial Version					--
--																		--
--------------------------------------------------------------------------

--This will drop the primary key temporarily
ALTER TABLE [dbo].[AUDITEDITEMS]
drop CONSTRAINT [PK_AUDITEDITEMS]


--change data type
ALTER TABLE [dbo].[AUDITEDITEMS]
ALTER COLUMN [_AUDITEDITEMID] BigInt


--add primary key
ALTER TABLE [dbo].[AUDITEDITEMS]
ADD CONSTRAINT [PK_AUDITEDITEMS] PRIMARY KEY ([_AUDITEDITEMID])




