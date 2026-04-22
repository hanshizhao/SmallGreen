-- 订单翻页查询存储过程
-- 根据机台名称和页码查询ERP订单列表
CREATE OR ALTER PROCEDURE [dbo].[PROC_OrderInfoPageQueryByEquipName]
    @currentPageNum INT,
    @equipName NVARCHAR(100),
    @MaxPerPage INT = 5,
    @equipType INT = 0
AS
BEGIN
    SET NOCOUNT ON;

    -- 查询总记录数
    DECLARE @totalCount INT;
    SELECT @totalCount = COUNT(*)
    FROM [LinkedServerName].[hsdyeingerp].[dbo].[vwpsWppDataFT]
    WHERE sEquipmentName LIKE '%' + @equipName + '%'
      AND sDyeStatus = '未生产';

    -- 查询当前页数据
    SELECT
        sWorkInfoArray,
        iStatus,
        iColorID,
        iCurrentPageNum = @currentPageNum,
        iTotalPageCount = CEILING(CAST(@totalCount AS FLOAT) / @MaxPerPage)
    FROM (
        SELECT
            RIGHT('     ' + CAST(iJobGroupOrderNo AS VARCHAR(5)), 5)
            + RIGHT('           ' + ISNULL(sOrderNo, ''), 11)
            + RIGHT('      ' + ISNULL(sPrescriptionNo, ''), 6)
            + RIGHT('           ' + ISNULL(sComponent, ''), 11)
            + RIGHT('           ' + ISNULL(CAST(nUnionQty AS VARCHAR(11)), ''), 11)
            + REPLICATE(' ', 25) AS sWorkInfoArray,
            0 AS iStatus,
            ISNULL(iColorID, 0) AS iColorID,
            ROW_NUMBER() OVER (ORDER BY tPlanStartTime) AS RowNum
        FROM [LinkedServerName].[hsdyeingerp].[dbo].[vwpsWppDataFT]
        WHERE sEquipmentName LIKE '%' + @equipName + '%'
          AND sDyeStatus = '未生产'
    ) t
    WHERE RowNum BETWEEN (@currentPageNum - 1) * @MaxPerPage + 1
                     AND @currentPageNum * @MaxPerPage;
END
