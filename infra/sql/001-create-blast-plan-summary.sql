IF OBJECT_ID('dbo.BlastPlanSummary', 'U') IS NULL
        BEGIN
            CREATE TABLE dbo.BlastPlanSummary
            (
                BlastPlanId UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
                Name NVARCHAR(200) NOT NULL,
                SiteId NVARCHAR(100) NOT NULL,
                Status NVARCHAR(50) NOT NULL,
                CreatedUtc DATETIMEOFFSET NOT NULL,
                ApprovedUtc DATETIMEOFFSET NULL
            );
        END;