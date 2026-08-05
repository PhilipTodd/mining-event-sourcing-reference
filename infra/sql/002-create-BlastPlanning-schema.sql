IF SCHEMA_ID('BlastPlanning') IS NULL
BEGIN
    EXEC('CREATE SCHEMA BlastPlanning');
END;
