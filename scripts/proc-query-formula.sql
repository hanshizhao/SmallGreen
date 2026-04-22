-- 处方查询存储过程
-- 根据订单信息查询配方/处方数据
CREATE OR ALTER PROCEDURE [dbo].[PROC_QueryFomulaByEquipWorkInfo]
    @jobGroupOrderNo NVARCHAR(10),
    @orderNo NVARCHAR(20),
    @component NVARCHAR(20),
    @unionQty NVARCHAR(15),
    @prescriptionNo NVARCHAR(10),
    @equipName NVARCHAR(100),
    @stepNo INT = 1,
    @equipID BIGINT = 0,
    @paraEquipType INT = 0
AS
BEGIN
    SET NOCOUNT ON;

    -- 从ERP视图查询处方数据
    -- 返回字段：BulkID, No, GL, uGUID
    SELECT
        f.BulkID,
        f.No,
        f.GL,
        f.uGUID
    FROM [LinkedServerName].[hsdyeingerp].[dbo].[vwpsWppDataFTPre] f
    INNER JOIN [LinkedServerName].[hsdyeingerp].[dbo].[vwpsWppDataFT] m
        ON f.uGUID = m.uGUID
    WHERE m.iJobGroupOrderNo = @jobGroupOrderNo
      AND m.sOrderNo = @orderNo
      AND m.sPrescriptionNo = @prescriptionNo
      AND m.sEquipmentName LIKE '%' + @equipName + '%';
END
