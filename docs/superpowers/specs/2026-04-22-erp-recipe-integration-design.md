# ERP 配方集成设计文档

**日期：** 2026-04-22
**状态：** 已批准

## 概述

将老项目 YLData 的 ERP 配方获取逻辑移植到 SmallGreen 系统，实现操作工人通过车间触摸屏获取 ERP 订单处方信息并下发到 PLC 的完整流程。

## 需求范围

| 功能 | 说明 | 对应老项目方法 |
|------|------|----------------|
| 订单翻页查询 | 工人翻页浏览 ERP 订单列表 | `OperateSQL.EquipPageChange()` |
| 处方下发 | 选择订单后获取处方并写入 PLC | `OperateSQL.EquipStartWork()` |
| 订单状态回调 | 完成/暂停/取消时通知 ERP | `OperateSQL.OrderStatusChange()` |
| 实际用量回写 | 配液完成后将用量回写 ERP | `OperateSQL.BulkComplete()` |

## 架构决策

- **数据接入方式：** 中间数据库 + 存储过程（与老项目一致）
- **ERP 数据源：** 通过链接服务器查询 ERP 视图（vwpsWppDataFT / vwpsWppDataFTPre），视图已存在
- **中间库操作：** ADO.NET 直连（与老项目一致），不通过 SqlSugar ORM
- **SmallGreenDB：** 继续 SqlSugar ORM
- **PLC 通信：** 复用现有 Dom<T> 模式和 S7.NET，PLC 点位与老项目一致

## 整体架构

```
触摸屏(PLC)  <-->  SmallGreen.API (2秒轮询)
                      |
                      +-- SystemManagerService (已有，扩展)
                      |     +-- CheckRuntime()            [已有] 检查触发器
                      |     +-- EquipmentPageChanged()    [补全] 订单翻页查询
                      |     +-- CheckStartWork()          [新增] 处方下发
                      |     +-- CheckOrderStatus()        [补全] 订单状态回调
                      |     +-- CheckBulkCompleted()      [扩展] 用量回写
                      |
                      +-- ErpDbHelper (新增)
                      |     +-- RunProcedure()            执行查询类存储过程
                      |     +-- UpdateByProcedure()       执行更新类存储过程
                      |
                      +-- SmallGreenDB (SqlSugar ORM)
                      +-- ERP 中间库 (ADO.NET)
```

## 新增/修改文件

| 文件 | 操作 | 说明 |
|------|------|------|
| `SmallGreen.Entity/Basic/ErpDbHelper.cs` | 新增 | ERP 中间库 ADO.NET 工具类 |
| `SmallGreen.Entity/Machine/SubSystem.cs` | 修改 | 新增处方解析辅助方法 |
| `SmallGreen.API/Service/SystemManagerService.cs` | 修改 | 补全/新增四大功能方法 |
| `SmallGreen.API/appsettings.json` | 修改 | 新增 ERP 中间库连接字符串 |
| `scripts/` | 新增 | 存储过程 SQL 脚本 |

## 功能详细设计

### 功能1：订单翻页查询

**触发：** PLC 设置 `BtnPageChange = true` + `DataCurrentPage = N`

**流程：**
1. 后台轮询检测到 `BtnPageChange == true`
2. 读取 `DataCurrentPage` 获取页码
3. 调用存储过程 `PROC_OrderInfoPageQueryByEquipName`
   - 参数：`@currentPageNum`, `@equipName`, `@MaxPerPage(5)`, `@equipType`
4. 返回 DataTable（最多5行），字段：`sWorkInfoArray`, `iStatus`, `iColorID`, `iCurrentPageNum`, `iTotalPageCount`
5. 将订单写入 Line1~Line5 的 `Status`/`ColorID`/`WrokInfoArray` PLC 点位
6. 将总页数写入 `DataTotalPage`
7. 重置 `BtnPageChange = false`

### 功能2：处方下发

**触发：** PLC 设置 `BtnStart = true` + `DataWorkOrderInfoArray = 69位字符串`

**69位字符串解析规则：**

| 位置 | 长度 | 含义 |
|------|------|------|
| 0~4 | 5位 | 生产序号 jobGroupOrderNo |
| 5~15 | 11位 | 订单号 orderNo |
| 16~21 | 6位 | 处方号 prescriptionNo |
| 22~32 | 11位 | 成份 component |
| 33~43 | 11位 | 排机数量 unionQty |

**流程：**
1. 解析69位字符串
2. 调用存储过程 `PROC_QueryFomulaByEquipWorkInfo`
   - 参数：`@jobGroupOrderNo`, `@orderNo`, `@component`, `@unionQty`, `@prescriptionNo`, `@equipName`, `@stepNo`, `@equipID`, `@paraEquipType`
3. 返回 DataTable，字段：`BulkID`, `No`, `GL`, `uGUID`
4. 按设备类型调用配方解析：
   - **前处理 (`GetQCLFomular`)**：最多11种助剂，处理组合助剂逻辑（如 8110+8103 按15:1组合为10#桶），验证组合比例误差不超过0.1
   - **固色 (`GetGSFomular`)**：最多3种助剂（12#=元明粉, 13#=纯碱, 14#=其他）
5. 编码为 `A` 分隔字符串（每个值7位 `0000.00` 格式）
6. 写入所有配液缸的 `DataFomulaArray`
7. 设置 `DataFomulaQueryStatus = 1(成功)` 或 `2(失败)`
8. 调用存储过程 `Cache` 更新订单状态为"生产中"
9. 重置 `BtnStart = false`

**前处理助剂对照表：**

| 桶号(BulkID) | 助剂 |
|---|---|
| 1 | 碱 |
| 2 | 稳定剂 |
| 3 | 酶堆液 |
| 4 | 冷堆精炼剂 |
| 5 | 亚硫酸氢钠 |
| 6 | 高温精炼剂与分散螯合剂混合液 |
| 7 | 奇力隆 F-OLB（组合助剂：8109+8103 按10:1） |
| 8 | 水玻璃 |
| 9 | 除蜡剂 |
| 10 | 低温精炼剂（组合助剂：8110+8103 按15:1） |
| 11 | 双氧水（实际用量需除以0.275浓度） |

**固色助剂对照表：**

| 桶号(BulkID) | 助剂 |
|---|---|
| 12 | 元明粉 |
| 13 | 纯碱 |
| 14 | 其他助剂 |

### 功能3：订单状态回调

**触发：** PLC 设置 `TriggerFinished = true` + `DataFinishedType = 1/2/3`

**流程：**
1. 读取 `DataFinishedType`（1=完成, 2=暂停, 3=取消）
2. 计算配液缸剩余量，写入 `T_Residue` 表
3. 更新 `T_EquipStartFinishStatus` 表
4. 调用存储过程：
   - 取消：`Cancel`（参数：`@uGUID`, `@index`, `@sysID`, `@equipName`, `@equipID`）
   - 完成/暂停：`Finish`（参数：`@uGUID`, `@equipName`, `@status`, `@equipID`, `@sysID`）
5. 重置 `TriggerFinished = false`

### 功能4：实际用量回写

**触发：** 配液缸 `TriggerComplete == 1`（在现有 `CheckBulkCompleted` 中触发）

**扩展现有 `SavePRCSData` 方法：**
1. 保留现有本地 `PRCSData` + `PRCSDataDetail` 记录逻辑
2. 新增：解析实际用量字符串，计算各助剂实际消耗公斤数
3. 新增：调用存储过程将实际用量回写到 ERP 中间库
4. 计算公式：`AssKg = realLitre * Concentration / 1000`
5. 特殊处理：双氧水(BulkID=11)实际用量需除以0.275浓度
6. 组合助剂按比例分配用量

## ErpDbHelper 设计

```csharp
public class ErpDbHelper
{
    // 从 IConfiguration 读取中间库连接字符串
    static string ConnectionString;

    // 执行查询类存储过程，返回 DataTable
    public static DataTable RunProcedure(string procName, SqlParameter[] parameters);

    // 执行更新类存储过程，返回影响行数
    public static int UpdateByProcedure(string procName, SqlParameter[] parameters);
}
```

## 存储过程清单

需在 ERP 中间库（SmallGreenDB 同实例）创建：

| 存储过程名 | 类型 | 用途 |
|------------|------|------|
| `PROC_OrderInfoPageQueryByEquipName` | 查询 | 按机台名称分页查询 ERP 订单 |
| `PROC_QueryFomulaByEquipWorkInfo` | 查询 | 根据订单信息查询处方/配方 |
| `Cache` | 更新 | 更新订单状态为"生产中" |
| `Cancel` | 更新 | 通知 ERP 订单取消 |
| `Finish` | 更新 | 通知 ERP 订单完成/暂停 |
| 用量回写存储过程 | 更新 | 将实际助剂用量回写 ERP |

## 后台轮询扩展

```
每2秒执行:
  +-- CheckRuntime()              [已有] 检查所有PLC触发器
  |     +-- BtnPageChange?     -> EquipmentPageChanged()    [补全]
  |     +-- BtnStart?          -> CheckStartWork()           [新增]
  |     +-- TriggerFinished?   -> CheckOrderStatus()         [补全]
  +-- CheckBulkCompleted()     -> SavePRCSData() + 用量回写  [扩展]
```

## 配置项

在 `appsettings.json` 中新增：

```json
{
  "ErpDbConnection": "server=.;database=AssSend_Data;uid=sa;pwd=***;Encrypt=True;TrustServerCertificate=True"
}
```

## 风险与注意事项

1. **组合助剂逻辑复杂**：前处理的组合助剂验证（比例误差不超过0.1）需精确移植，建议单元测试覆盖
2. **69位字符串解析**：固定位置截取，需严格对照老项目偏移量
3. **PLC 字符串编码**：`A` 分隔符格式和 `0000.00` 补零格式需与 PLC 端一致
4. **双氧水特殊处理**：除以0.275浓度是物理特性，必须保留
5. **存储过程创建**：需根据实际 ERP 视图结构调整存储过程 SQL
