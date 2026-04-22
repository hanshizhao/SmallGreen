-- 更新订单状态为"生产中"
CREATE OR ALTER PROCEDURE [dbo].[Cache]
    @uGUID NVARCHAR(50),
    @workStatus NVARCHAR(20),
    @sysID NVARCHAR(50),
    @equipName NVARCHAR(100),
    @equipID BIGINT
AS
BEGIN
    SET NOCOUNT ON;
    -- 通过链接服务器更新ERP订单状态
END
GO

-- 订单取消
CREATE OR ALTER PROCEDURE [dbo].[Cancel]
    @uGUID NVARCHAR(50),
    @index INT,
    @sysID NVARCHAR(50),
    @equipName NVARCHAR(100),
    @equipID BIGINT
AS
BEGIN
    SET NOCOUNT ON;
    -- 通知ERP订单取消
END
GO

-- 订单完成/暂停
CREATE OR ALTER PROCEDURE [dbo].[Finish]
    @uGUID NVARCHAR(50),
    @equipName NVARCHAR(100),
    @status NVARCHAR(20),
    @equipID BIGINT,
    @sysID NVARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;
    -- 通知ERP订单完成或暂停
END
GO

-- 更新机台完成状态（内部表）
CREATE OR ALTER PROCEDURE [dbo].[UpdateEquipFinishStatus]
    @equipID BIGINT,
    @equipName NVARCHAR(100),
    @finishType INT,
    @finishDateTime DATETIME
AS
BEGIN
    SET NOCOUNT ON;
    IF EXISTS (SELECT 1 FROM T_EquipStartFinishStatus WHERE equipID = @equipID)
        UPDATE T_EquipStartFinishStatus SET FinishDateTime = @finishDateTime, FinishType = @finishType WHERE equipID = @equipID;
    ELSE
        INSERT INTO T_EquipStartFinishStatus (equipID, equipName, FinishType, FinishDateTime) VALUES (@equipID, @equipName, @finishType, @finishDateTime);
END
