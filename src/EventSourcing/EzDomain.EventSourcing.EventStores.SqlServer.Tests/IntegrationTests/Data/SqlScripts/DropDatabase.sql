IF EXISTS (SELECT 1
           FROM sys.databases [db]
           WHERE [db].[name] = N'EventStoreTests')
ALTER DATABASE [EventStoreTests]
SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
DROP DATABASE [EventStoreTests]