-- 用量回写存储过程
-- 将实际助剂消耗量回写到ERP中间库
CREATE OR ALTER PROCEDURE [dbo].[WriteBackUsage]
    @uGUID NVARCHAR(50),
    @equipName NVARCHAR(100),
    @equipID BIGINT,
    @bulkID NVARCHAR(10),
    @assNo NVARCHAR(50),
    @assName NVARCHAR(100),
    @assGL FLOAT,
    @planKG FLOAT,
    @assKG FLOAT,
    @planVolume FLOAT,
    @actualVolume FLOAT
AS
BEGIN
    SET NOCOUNT ON;
    -- 写入配液记录到中间库
END
