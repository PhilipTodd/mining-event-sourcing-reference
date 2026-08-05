IF OBJECT_ID('BlastPlanning.BlastPlanSummary', 'U') IS NULL
        BEGIN
            CREATE TABLE BlastPlanning.BlastPlanSummary
            (
                BlastPlanId UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
                Name NVARCHAR(200) NOT NULL,
                SiteId NVARCHAR(100) NOT NULL,
                Status NVARCHAR(50) NOT NULL,
                CreatedUtc DATETIMEOFFSET NOT NULL,
                ApprovedUtc DATETIMEOFFSET NULL
            );
        END;