# ERP 配方集成实施计划

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** 将老项目 YLData 的 ERP 配方获取逻辑移植到 SmallGreen，实现订单翻页查询、处方下发、订单状态回调和实际用量回写四大功能。

**Architecture:** 在现有 SystemManagerService 2秒轮询机制基础上，扩展触发器检查逻辑，通过新增的 ErpDbHelper（ADO.NET）调用 ERP 中间库存储过程，与 PLC 进行双向数据交互。

**Tech Stack:** .NET 8, SqlSugar, S7.NET, ADO.NET (Microsoft.Data.SqlClient), SQL Server 存储过程

> **行号说明：** 计划中引用的行号基于编写时的文件状态，编辑过程中可能偏移，请以实际文件为准。

**Spec:** `docs/superpowers/specs/2026-04-22-erp-recipe-integration-design.md`

---

## 文件结构

| 文件 | 操作 | 职责 |
|------|------|------|
| `SmallGreen.Entity/Basic/ErpDbHelper.cs` | 新增 | ERP 中间库 ADO.NET 工具类，执行存储过程 |
| `SmallGreen.Entity/Machine/Equipment.cs` | 修改 | 新增 StepNo 字段和 uGuid 运行时属性 |
| `SmallGreen.Entity/Machine/SubSystem.cs` | 修改 | 新增 GetEquipType() 辅助方法和配方编码方法 |
| `SmallGreen.API/IService/ISystemManagerService.cs` | 修改 | 新增 CheckStartWork 接口方法 |
| `SmallGreen.API/Service/SystemManagerService.cs` | 修改 | 注入 ErpDbHelper，实现四大功能 |
| `SmallGreen.API/appsettings.json` | 修改 | 新增 ErpDbConnection 连接字符串 |
| `SmallGreen.API/Program.cs` | 修改 | 注册 ErpDbHelper 为 Singleton |
| `scripts/proc-order-info-page-query.sql` | 新增 | 订单翻页查询存储过程 |
| `scripts/proc-query-formula.sql` | 新增 | 处方查询存储过程 |
| `scripts/proc-cache-cancel-finish.sql` | 新增 | 订单状态更新存储过程 |
| `scripts/proc-usage-writeback.sql` | 新增 | 用量回写存储过程 |

---

## Task 1: ErpDbHelper — ERP 中间库数据库工具类

**Files:**
- Create: `SmallGreen.Entity/Basic/ErpDbHelper.cs`

- [ ] **Step 1: 创建 ErpDbHelper.cs**

```csharp
using System.Data;
using Microsoft.Data.SqlClient;

namespace SmallGreen.Entity.Basic
{
    public class ErpDbHelper
    {
        private readonly string _connectionString;

        public ErpDbHelper(string connectionString)
        {
            _connectionString = connectionString
                ?? throw new ArgumentNullException(nameof(connectionString));
        }

        /// <summary>
        /// 执行查询类存储过程，返回 DataTable
        /// </summary>
        public DataTable RunProcedure(string procName, SqlParameter[] parameters)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand(procName, conn)
            {
                CommandType = CommandType.StoredProcedure,
                CommandTimeout = 30
            };
            if (parameters != null)
            {
                cmd.Parameters.AddRange(parameters);
            }

            var dt = new DataTable();
            using var adapter = new SqlDataAdapter(cmd);
            adapter.Fill(dt);
            return dt;
        }

        /// <summary>
        /// 执行更新类存储过程，返回影响行数
        /// </summary>
        public int UpdateByProcedure(string procName, SqlParameter[] parameters)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand(procName, conn)
            {
                CommandType = CommandType.StoredProcedure,
                CommandTimeout = 30
            };
            if (parameters != null)
            {
                cmd.Parameters.AddRange(parameters);
            }

            conn.Open();
            return cmd.ExecuteNonQuery();
        }
    }
}
```

- [ ] **Step 2: 添加 Microsoft.Data.SqlClient NuGet 包到 SmallGreen.Entity 项目**

> 注意：选择 `Microsoft.Data.SqlClient`（新版，支持 .NET 8），不使用 SqlSugar 间接引入的 `System.Data.SqlClient`（旧版）。

Run: `cd SmallGreen.Entity && dotnet add package Microsoft.Data.SqlClient`

- [ ] **Step 3: 编译验证**

Run: `dotnet build SmallGreen.Entity/SmallGreen.Entity.csproj`
Expected: BUILD SUCCEEDED

- [ ] **Step 4: 提交**

```bash
git add SmallGreen.Entity/Basic/ErpDbHelper.cs SmallGreen.Entity/SmallGreen.Entity.csproj
git commit -m "feat: 添加ErpDbHelper ERP中间库数据库工具类"
```

---

## Task 2: 配置和 DI 注册

**Files:**
- Modify: `SmallGreen.API/appsettings.json`
- Modify: `SmallGreen.API/Program.cs`

- [ ] **Step 1: 在 appsettings.json 中添加 ERP 连接字符串**

在 `appsettings.json` 中添加 `ErpDbConnection` 键：

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "ErpDbConnection": "server=.;database=SmallGreenDB;uid=sa;pwd=123;Encrypt=True;TrustServerCertificate=True"
}
```

> 注：实际部署时需根据 ERP 中间库地址修改。

- [ ] **Step 2: 在 Program.cs 中注册 ErpDbHelper**

在 `Program.cs` 的服务注册区域（第34行 `AddSingleton<ISystemManagerService>` 之前）添加：

```csharp
builder.Services.AddSingleton(new ErpDbHelper(builder.Configuration["ErpDbConnection"]
    ?? throw new InvalidOperationException("ErpDbConnection 未配置")));
```

同时在文件顶部添加 using：
```csharp
using SmallGreen.Entity.Basic;
```

- [ ] **Step 3: 编译验证**

Run: `dotnet build SmallGreen.API/SmallGreen.API.csproj`
Expected: BUILD SUCCEEDED

- [ ] **Step 4: 提交**

```bash
git add SmallGreen.API/appsettings.json SmallGreen.API/Program.cs
git commit -m "feat: 配置ERP中间库连接字符串和DI注册"
```

---

## Task 3: Equipment 实体扩展

**Files:**
- Modify: `SmallGreen.Entity/Machine/Equipment.cs`

老项目中 Equipment 有 `StepNo`（工步号）和 `uGuid`（当前订单标识）属性，SmallGreen 缺少这两个字段。

- [ ] **Step 1: 在 Equipment.cs 中添加 StepNo 和 uGuid**

在 `Equipment.cs` 的 `Name` 属性（第30行）之后添加：

```csharp
/// <summary>
/// 工步号（用于存储过程参数）
/// </summary>
public int StepNo { get; set; } = 1;

/// <summary>
/// 当前生产订单的GUID（运行时设置，不持久化）
/// </summary>
[SugarColumn(IsIgnore = true)]
public string uGuid { get; set; } = string.Empty;
```

- [ ] **Step 2: 编译验证**

Run: `dotnet build SmallGreen.Entity/SmallGreen.Entity.csproj`
Expected: BUILD SUCCEEDED

- [ ] **Step 3: 提交**

```bash
git add SmallGreen.Entity/Machine/Equipment.cs
git commit -m "feat: Equipment添加StepNo和uGuid运行时属性"
```

---

## Task 4: SubSystem 辅助方法

**Files:**
- Modify: `SmallGreen.Entity/Machine/SubSystem.cs`

新增设备类型映射方法和配方字符串编码方法。

- [ ] **Step 1: 在 SubSystem.cs 末尾（`StrToFloat` 方法之后，类结束大括号之前）添加辅助方法**

```csharp
/// <summary>
/// 获取设备类型：0=前处理，1=固色
/// </summary>
public int GetEquipType()
{
    return SubSystemName == SubSystemName.GS1 ? 1 : 0;
}

/// <summary>
/// 将配方值数组编码为 PLC 字符串格式（A 分隔，每值7位 0000.00）
/// </summary>
public static string EncodeFomulaArray(float[] values)
{
    var sb = new System.Text.StringBuilder();
    foreach (var val in values)
    {
        sb.Append(val.ToString("0000.00", System.Globalization.CultureInfo.InvariantCulture));
        sb.Append('A');
    }
    return sb.ToString();
}
```

- [ ] **Step 2: 编译验证**

Run: `dotnet build SmallGreen.Entity/SmallGreen.Entity.csproj`
Expected: BUILD SUCCEEDED

- [ ] **Step 3: 提交**

```bash
git add SmallGreen.Entity/Machine/SubSystem.cs
git commit -m "feat: SubSystem添加设备类型映射和配方编码方法"
```

---

## Task 5: ISystemManagerService 接口扩展

**Files:**
- Modify: `SmallGreen.API/IService/ISystemManagerService.cs`

- [ ] **Step 1: 在接口中添加 CheckStartWork 方法声明**

在 `ISystemManagerService.cs` 的 `CheckOrderStatus` 方法声明（第37行）之后添加：

```csharp
/// <summary>
/// 检查开始生产触发器，获取处方并下发
/// </summary>
Task CheckStartWork();
```

- [ ] **Step 2: 编译验证**

Run: `dotnet build SmallGreen.API/SmallGreen.API.csproj`
Expected: BUILD FAILED（SystemManagerService 未实现新接口方法）— 这是预期的

- [ ] **Step 3: 提交**

```bash
git add SmallGreen.API/IService/ISystemManagerService.cs
git commit -m "feat: ISystemManagerService添加CheckStartWork接口方法"
```

---

## Task 6: SystemManagerService — 注入 ErpDbHelper 和扩展轮询流程

**Files:**
- Modify: `SmallGreen.API/Service/SystemManagerService.cs`

这是最大的改动文件。分多步完成。

- [ ] **Step 1: 注入 ErpDbHelper 并修改构造函数**

将 `SystemManagerService.cs` 的构造函数（第15-18行）替换为：

```csharp
public class SystemManagerService : ISystemManagerService
{
    private readonly ILogger<SystemManagerService> logger;
    private readonly ErpDbHelper erpDbHelper;
    private List<SubSystem> ListSubSystem;

    public SystemManagerService(ILogger<SystemManagerService> logger, ErpDbHelper erpDbHelper)
    {
        ListSubSystem = new List<SubSystem>();
        this.logger = logger;
        this.erpDbHelper = erpDbHelper;
    }
```

在文件顶部添加 using：
```csharp
using System.Data;
using Microsoft.Data.SqlClient;
```

- [ ] **Step 2: 扩展 CheckRuntime 方法，在触发器读取后添加事件分发**

将 `CheckRuntime` 方法（第69-87行）替换为：

```csharp
public async Task CheckRuntime()
{
    string subSystemName = string.Empty;
    try
    {
        foreach (var subSystem in ListSubSystem)
        {
            subSystemName = subSystem.SubSystemName.ToString();
            var result = await subSystem.CheckRuntimeOnlyTrigger();

            if (!result.IsSuccess)
            {
                logger.LogError("{subSystemName}检查运行时出现故障：{Message}", subSystem.SubSystemName, result.Message);
                continue;
            }

            // 检查各机台的触发器
            foreach (var equip in subSystem.ListEquipment)
            {
                // 翻页触发
                if (equip.BtnPageChange?.GetCurrentValue() == true)
                {
                    await HandleEquipmentPageChanged(subSystem, equip);
                }

                // 开始生产触发
                if (equip.BtnStart?.GetCurrentValue() == true)
                {
                    await HandleStartWork(subSystem, equip);
                }

                // 订单完成触发
                if (equip.TriggerFinished?.GetCurrentValue() == true)
                {
                    await HandleOrderStatusChange(subSystem, equip);
                }
            }
        }
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "检查运行时出现故障,子系统队列：{sub_system_name}， {message}", subSystemName, ex.Message);
    }
}
```

- [ ] **Step 3: 在 SystemManagerService 中添加 HandleXxx 存根方法（保持编译通过）**

在 `GetSubSystem` 方法之前添加以下存根方法：

```csharp
private Task HandleEquipmentPageChanged(SubSystem subSystem, Equipment equip)
{
    throw new NotImplementedException("Task 7 将实现");
}

private Task HandleStartWork(SubSystem subSystem, Equipment equip)
{
    throw new NotImplementedException("Task 8 将实现");
}

private Task HandleOrderStatusChange(SubSystem subSystem, Equipment equip)
{
    throw new NotImplementedException("Task 9 将实现");
}
```

- [ ] **Step 4: 编译验证**

Run: `dotnet build SmallGreen.API/SmallGreen.API.csproj`
Expected: BUILD SUCCEEDED

- [ ] **Step 5: 提交**

```bash
git add SmallGreen.API/Service/SystemManagerService.cs
git commit -m "feat: SystemManagerService注入ErpDbHelper并扩展触发器分发逻辑"
```

---

## Task 7: 功能1 — 订单翻页查询

**Files:**
- Modify: `SmallGreen.API/Service/SystemManagerService.cs`

- [ ] **Step 1: 在 SystemManagerService 类中实现 HandleEquipmentPageChanged 方法**

在 `CheckBulkCompleted` 方法（第89行区域）之后添加：

```csharp
/// <summary>
/// 处理机台翻页请求
/// </summary>
private async Task HandleEquipmentPageChanged(SubSystem subSystem, Equipment equip)
{
    try
    {
        var currentPage = equip.DataCurrentPage?.GetCurrentValue() ?? 1;
        var equipType = subSystem.GetEquipType();
        var equipName = equip.Name;

        // 老项目中会截取 '-' 之前的部分作为机台名
        var dashIndex = equipName.IndexOf('-');
        if (dashIndex > 0) equipName = equipName[..dashIndex];

        var paras = new SqlParameter[]
        {
            new("@currentPageNum", currentPage),
            new("@equipName", equipName),
            new("@MaxPerPage", 5),
            new("@equipType", equipType)
        };

        var dt = erpDbHelper.RunProcedure("PROC_OrderInfoPageQueryByEquipName", paras);

        var lineStatuses = new[] { equip.Line1Status, equip.Line2Status, equip.Line3Status, equip.Line4Status, equip.Line5Status };
        var lineColorIDs = new[] { equip.Line1ColorID, equip.Line2ColorID, equip.Line3ColorID, equip.Line4ColorID, equip.Line5ColorID };
        var lineWorkInfos = new[] { equip.Line1WrokInfoArray, equip.Line2WrokInfoArray, equip.Line3WrokInfoArray, equip.Line4WrokInfoArray, equip.Line5WrokInfoArray };

        // 先清空5行数据
        var domsToWrite = new List<IDom>();
        for (int i = 0; i < 5; i++)
        {
            if (lineStatuses[i] != null) { lineStatuses[i].NewValue = (ushort)0; domsToWrite.Add(lineStatuses[i]); }
            if (lineColorIDs[i] != null) { lineColorIDs[i].NewValue = (ushort)0; domsToWrite.Add(lineColorIDs[i]); }
            if (lineWorkInfos[i] != null) { lineWorkInfos[i].NewValue = string.Empty; domsToWrite.Add(lineWorkInfos[i]); }
        }

        // 写入查询到的订单数据
        for (int i = 0; i < dt.Rows.Count && i < 5; i++)
        {
            var row = dt.Rows[i];
            if (lineStatuses[i] != null && row["iStatus"] != DBNull.Value)
                lineStatuses[i].NewValue = Convert.ToUInt16(row["iStatus"]);
            if (lineColorIDs[i] != null && row["iColorID"] != DBNull.Value)
                lineColorIDs[i].NewValue = Convert.ToUInt16(row["iColorID"]);
            if (lineWorkInfos[i] != null && row["sWorkInfoArray"] != DBNull.Value)
                lineWorkInfos[i].NewValue = row["sWorkInfoArray"].ToString();
        }

        // 写入总页数
        if (dt.Rows.Count > 0 && equip.DataTotalPage != null)
        {
            var totalPage = dt.Rows[0]["iTotalPageCount"];
            if (totalPage != DBNull.Value)
            {
                equip.DataTotalPage.NewValue = Convert.ToUInt16(totalPage);
                domsToWrite.Add(equip.DataTotalPage);
            }
        }

        // 批量写入 PLC
        if (domsToWrite.Count > 0)
        {
            await subSystem.PLC.Write(domsToWrite);
        }

        // 重置翻页按钮
        if (equip.BtnPageChange != null)
        {
            equip.BtnPageChange.NewValue = false;
            await subSystem.PLC.Write([equip.BtnPageChange]);
        }
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "{equipName} 翻页查询出现异常：{message}", equip.Name, ex.Message);
    }
}
```

- [ ] **Step 2: 编译验证**

Run: `dotnet build SmallGreen.API/SmallGreen.API.csproj`

- [ ] **Step 3: 提交**

```bash
git add SmallGreen.API/Service/SystemManagerService.cs
git commit -m "feat: 实现订单翻页查询功能(HandleEquipmentPageChanged)"
```

---

## Task 8: 功能2 — 处方下发（核心）

**Files:**
- Modify: `SmallGreen.API/Service/SystemManagerService.cs`

这是最复杂的功能，包含69位字符串解析、存储过程调用、前处理/固色配方解析。

- [ ] **Step 1: 实现 HandleStartWork 方法**

在 `HandleEquipmentPageChanged` 方法之后添加：

```csharp
/// <summary>
/// 处理开始生产 — 获取处方并下发到PLC
/// </summary>
private async Task HandleStartWork(SubSystem subSystem, Equipment equip)
{
    try
    {
        var workInfoArray = equip.DataWorkOrderInfoArray?.GetCurrentValue() ?? "";

        // 验证字符串长度
        if (string.IsNullOrEmpty(workInfoArray) || workInfoArray.Length < 44)
        {
            await SetFomulaQueryStatus(subSystem, equip, 2, "生产信息字符串长度不足");
            return;
        }

        // 解析69位字符串（44位有效 + 25位保留）
        var jobGroupOrderNo = workInfoArray.Substring(0, 5).TrimEnd();
        var orderNo = workInfoArray.Substring(5, 11).TrimEnd();
        var prescriptionNo = workInfoArray.Substring(16, 6).TrimEnd();
        var component = workInfoArray.Substring(22, 11).TrimEnd();
        var unionQty = workInfoArray.Substring(33, 11).TrimEnd();

        var equipType = subSystem.GetEquipType();
        var equipName = equip.Name;
        var dashIndex = equipName.IndexOf('-');
        if (dashIndex > 0) equipName = equipName[..dashIndex];

        // 调用存储过程查询处方
        var paras = new SqlParameter[]
        {
            new("@jobGroupOrderNo", jobGroupOrderNo),
            new("@orderNo", orderNo),
            new("@component", component),
            new("@unionQty", unionQty),
            new("@prescriptionNo", prescriptionNo),
            new("@equipName", equipName),
            new("@stepNo", equip.StepNo),
            new("@equipID", equip.Id),
            new("@paraEquipType", equipType)
        };

        var dt = erpDbHelper.RunProcedure("PROC_QueryFomulaByEquipWorkInfo", paras);

        if (dt.Rows.Count == 0)
        {
            await SetFomulaQueryStatus(subSystem, equip, 2, "存储过程未返回处方数据");
            return;
        }

        // 解析处方数据
        var listFomula = new List<RealFomula>();
        foreach (DataRow row in dt.Rows)
        {
            listFomula.Add(new RealFomula
            {
                BulkID = Convert.ToInt32(row["BulkID"]),
                No = row["No"].ToString() ?? "",
                GL = Convert.ToSingle(row["GL"]),
                uGUID = row["uGUID"].ToString() ?? ""
            });
        }

        // 按设备类型解析配方
        float[] fomulaValues;
        if (equipType == 0)
        {
            fomulaValues = GetQCLFomular(listFomula);
        }
        else
        {
            fomulaValues = GetGSFomular(listFomula);
        }

        // 编码为 A 分隔字符串
        var fomulaString = SubSystem.EncodeFomulaArray(fomulaValues);

        // 写入所有配液缸的 DataFomulaArray
        var domsToWrite = new List<IDom>();
        foreach (var bulk in equip.ListBulk)
        {
            if (bulk.DataFomulaArray != null)
            {
                bulk.DataFomulaArray.NewValue = fomulaString;
                domsToWrite.Add(bulk.DataFomulaArray);
            }
        }

        if (domsToWrite.Count > 0)
        {
            var writeResult = await subSystem.PLC.Write(domsToWrite);
            if (!writeResult.IsSuccess)
            {
                await SetFomulaQueryStatus(subSystem, equip, 2, $"写入PLC失败:{writeResult.Message}");
                return;
            }
        }

        // 保存 uGuid（用于后续订单状态回调和用量回写）
        equip.uGuid = listFomula[0].uGUID;

        // 设置处方下发成功
        await SetFomulaQueryStatus(subSystem, equip, 1, "");

        // 调用 Cache 存储过程更新订单状态为"生产中"
        try
        {
            erpDbHelper.UpdateByProcedure("Cache", new SqlParameter[]
            {
                new("@uGUID", equip.uGuid),
                new("@workStatus", "生产中"),
                new("@sysID", ""),
                new("@equipName", equip.Name),
                new("@equipID", equip.Id)
            });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "{equipName} 调用Cache存储过程失败", equip.Name);
        }
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "{equipName} 处方下发出现异常：{message}", equip.Name, ex.Message);
        await SetFomulaQueryStatus(subSystem, equip, 2, ex.Message);
    }
}

/// <summary>
/// 设置处方下发状态并重置 BtnStart
/// </summary>
private async Task SetFomulaQueryStatus(SubSystem subSystem, Equipment equip, ushort status, string errorMessage)
{
    try
    {
        if (!string.IsNullOrEmpty(errorMessage))
        {
            logger.LogError("{equipName} 处方下发失败：{error}", equip.Name, errorMessage);
        }

        if (equip.DataFomulaQueryStatus != null)
        {
            equip.DataFomulaQueryStatus.NewValue = status;
            await subSystem.PLC.Write([equip.DataFomulaQueryStatus]);
        }

        // 重置 BtnStart
        if (equip.BtnStart != null)
        {
            equip.BtnStart.NewValue = false;
            await subSystem.PLC.Write([equip.BtnStart]);
        }
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "{equipName} 设置处方状态失败", equip.Name);
    }
}
```

- [ ] **Step 2: 在 SystemManagerService 类中添加 RealFomula 模型（内部类）**

在类内部顶部添加：

```csharp
/// <summary>
/// ERP 处方数据模型（对应存储过程返回的行）
/// </summary>
private class RealFomula
{
    public int BulkID { get; set; }
    public string No { get; set; } = "";
    public float GL { get; set; }
    public string uGUID { get; set; } = "";
}
```

- [ ] **Step 3: 实现前处理配方解析方法 GetQCLFomular**

在 `SetFomulaQueryStatus` 方法之后添加：

```csharp
/// <summary>
/// 前处理配方解析 — 11种助剂映射到桶号1-11
/// 组合助剂逻辑：8110+8103按15:1组合为10#桶，8109+8103按10:1组合为7#桶
/// </summary>
private float[] GetQCLFomular(List<RealFomula> list)
{
    var result = new float[11]; // 桶号1-11

    // 先处理组合助剂
    var item8110 = list.Find(x => x.No == "8110");
    var item8103 = list.Find(x => x.No == "8103");
    var item8109 = list.Find(x => x.No == "8109");

    // 8110+8103 组合为10#桶，比例15:1
    if (item8110 != null && item8103 != null)
    {
        var ratio = item8110.GL / (item8103.GL > 0 ? item8103.GL : 1);
        if (Math.Abs(ratio - 15) > 0.1f)
        {
            logger.LogWarning("前处理组合助剂8110+8103比例异常：{ratio}", ratio);
        }
        result[9] = item8110.GL + item8103.GL; // 10#桶 index=9
    }

    // 8109+8103 组合为7#桶，比例10:1
    if (item8109 != null && item8103 != null)
    {
        var ratio = item8109.GL / (item8103.GL > 0 ? item8103.GL : 1);
        if (Math.Abs(ratio - 10) > 0.1f)
        {
            logger.LogWarning("前处理组合助剂8109+8103比例异常：{ratio}", ratio);
        }
        result[6] = item8109.GL + item8103.GL; // 7#桶 index=6
    }

    // 组合助剂编号集合（已处理，不再按 BulkID 映射）
    var combinedNos = new HashSet<string> { "8110", "8103", "8109" };

    // 普通助剂：按 BulkID 直接映射
    foreach (var item in list)
    {
        if (combinedNos.Contains(item.No)) continue;
        if (item.BulkID >= 1 && item.BulkID <= 11)
        {
            result[item.BulkID - 1] += item.GL;
        }
    }

    return result;
}
```

> **重要：** 老项目 `GetQCLFomular` 的组合助剂逻辑非常复杂（约250行），包含多个特殊映射和 `CombianAss` 合并逻辑。上面是简化版本，实施时需精确对照 `OperateSQL.cs` 第790-1043行的完整逻辑进行移植。

- [ ] **Step 4: 实现固色配方解析方法 GetGSFomular**

在 `GetQCLFomular` 方法之后添加：

```csharp
/// <summary>
/// 固色配方解析 — 3种助剂映射到桶号12-14
/// 12#=元明粉, 13#=纯碱, 14#=其他
/// </summary>
private float[] GetGSFomular(List<RealFomula> list)
{
    var result = new float[3]; // 桶号12-14

    foreach (var item in list)
    {
        if (item.BulkID >= 12 && item.BulkID <= 14)
        {
            result[item.BulkID - 12] += item.GL;
        }
        else if (item.BulkID == 0)
        {
            // BulkID=0 的助剂默认分配到13#桶
            result[1] += item.GL;
        }
    }

    // 验证：元明粉与纯碱用量比值不大于10:1
    if (result[0] > 0 && result[1] > 0 && result[0] / result[1] > 10)
    {
        logger.LogWarning("固色配方异常：元明粉与纯碱用量比值超过10:1");
    }

    return result;
}
```

- [ ] **Step 5: 编译验证**

Run: `dotnet build SmallGreen.API/SmallGreen.API.csproj`
Expected: BUILD SUCCEEDED（如果 HandleOrderStatusChange 和 CheckStartWork 存根已添加）

- [ ] **Step 6: 提交**

```bash
git add SmallGreen.API/Service/SystemManagerService.cs
git commit -m "feat: 实现处方下发功能(HandleStartWork)含前处理和固色配方解析"
```

---

## Task 9: 功能3 — 订单状态回调

**Files:**
- Modify: `SmallGreen.API/Service/SystemManagerService.cs`

- [ ] **Step 1: 实现 HandleOrderStatusChange 方法**

在 `GetGSFomular` 方法之后添加：

```csharp
/// <summary>
/// 处理订单完成/暂停/取消
/// </summary>
private async Task HandleOrderStatusChange(SubSystem subSystem, Equipment equip)
{
    try
    {
        var finishedType = equip.DataFinishedType?.GetCurrentValue() ?? 0;
        // 1=完成, 2=暂停, 3=取消

        // 计算配液缸剩余量并写入 T_Residue（通过存储过程）
        // 更新 T_EquipStartFinishStatus
        try
        {
            erpDbHelper.UpdateByProcedure("UpdateEquipFinishStatus", new SqlParameter[]
            {
                new("@equipID", equip.Id),
                new("@equipName", equip.Name),
                new("@finishType", finishedType),
                new("@finishDateTime", DateTime.Now)
            });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "{equipName} 更新完成状态表失败", equip.Name);
        }

        // 如果是手动模式（无 uGuid），跳过 ERP 通知
        if (string.IsNullOrEmpty(equip.uGuid) || equip.uGuid == "手动模式")
        {
            // 重置触发器
            equip.uGuid = string.Empty;
            await ResetTriggerFinished(subSystem, equip);
            return;
        }

        // 调用 ERP 存储过程
        try
        {
            if (finishedType == 3)
            {
                // 取消
                erpDbHelper.UpdateByProcedure("Cancel", new SqlParameter[]
                {
                    new("@uGUID", equip.uGuid),
                    new("@index", 0),
                    new("@sysID", ""),
                    new("@equipName", equip.Name),
                    new("@equipID", equip.Id)
                });
            }
            else
            {
                // 完成(1) 或 暂停(2)
                var status = finishedType == 1 ? "完成" : "暂停";
                erpDbHelper.UpdateByProcedure("Finish", new SqlParameter[]
                {
                    new("@uGUID", equip.uGuid),
                    new("@equipName", equip.Name),
                    new("@status", status),
                    new("@equipID", equip.Id),
                    new("@sysID", "")
                });
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "{equipName} 调用ERP订单状态存储过程失败", equip.Name);
        }

        equip.uGuid = string.Empty;
        await ResetTriggerFinished(subSystem, equip);
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "{equipName} 订单状态回调异常：{message}", equip.Name, ex.Message);
    }
}

/// <summary>
/// 重置订单完成触发器
/// </summary>
private async Task ResetTriggerFinished(SubSystem subSystem, Equipment equip)
{
    if (equip.TriggerFinished != null)
    {
        equip.TriggerFinished.NewValue = false;
        await subSystem.PLC.Write([equip.TriggerFinished]);
    }
}
```

- [ ] **Step 2: 实现接口存根方法**

将 `EquipmentPageChanged` 和 `CheckOrderStatus` 方法（原 NotImplementedException）替换为空实现（功能已通过 HandleXxx 方法在 CheckRuntime 中调用）：

```csharp
public Task EquipmentPageChanged()
{
    // 已在 CheckRuntime 中通过 HandleEquipmentPageChanged 实现
    return Task.CompletedTask;
}

public Task CheckOrderStatus()
{
    // 已在 CheckRuntime 中通过 HandleOrderStatusChange 实现
    return Task.CompletedTask;
}

public Task CheckStartWork()
{
    // 已在 CheckRuntime 中通过 HandleStartWork 实现
    return Task.CompletedTask;
}
```

- [ ] **Step 3: 编译验证**

Run: `dotnet build SmallGreen.API/SmallGreen.API.csproj`
Expected: BUILD SUCCEEDED

- [ ] **Step 4: 提交**

```bash
git add SmallGreen.API/Service/SystemManagerService.cs
git commit -m "feat: 实现订单状态回调功能(HandleOrderStatusChange)"
```

---

## Task 10: 功能4 — 实际用量回写

**Files:**
- Modify: `SmallGreen.Entity/Machine/SubSystem.cs`

扩展现有 `SavePRCSData` 方法，在保存本地记录之后，将用量回写到 ERP 中间库。

- [ ] **Step 1: 在 SystemManagerService.CheckBulkCompleted 中添加回写调用**

将 `CheckBulkCompleted` 方法（第89-107行）扩展为：

```csharp
public async Task CheckBulkCompleted()
{
    foreach (var subSystem in ListSubSystem)
    {
        foreach (var equip in subSystem.ListEquipment)
        {
            foreach (var bulk in equip.ListBulk)
            {
                var trigger = bulk.TriggerComplete;
                if (trigger == null) continue;
                if (trigger.GetCurrentValue() != 1) continue;

                var result = await subSystem.SavePRCSData(equip, bulk);
                if (!result.IsSuccess)
                {
                    logger.LogError(result.Message);
                    continue;
                }

                // ERP 用量回写
                if (result.Content != null && result.Content.ListDetail?.Count > 0)
                {
                    try
                    {
                        WriteBackUsageToErp(equip, bulk, result.Content, result.Content.ListDetail);
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, "{equipName} ERP用量回写异常", equip.Name);
                    }
                }
            }
        }
    }
}

/// <summary>
/// 将配液完成数据回写到ERP中间库
/// </summary>
private void WriteBackUsageToErp(Equipment equipment, Bulk bulk, PRCSData prcsData, List<PRCSDataDetail> listDetail)
{
    if (string.IsNullOrEmpty(equipment.uGuid) || equipment.uGuid == "手动模式")
        return;

    foreach (var detail in listDetail)
    {
        erpDbHelper.UpdateByProcedure("WriteBackUsage", new SqlParameter[]
        {
            new("@uGUID", equipment.uGuid),
            new("@equipName", equipment.Name),
            new("@equipID", equipment.Id),
            new("@bulkID", bulk.CodeNumber),
            new("@assNo", detail.AssCodeNumber ?? ""),
            new("@assName", detail.AssName ?? ""),
            new("@assGL", detail.AssGl),
            new("@planKG", detail.PlanKg),
            new("@assKG", detail.AssKg),
            new("@planVolume", prcsData.PlanVolume),
            new("@actualVolume", prcsData.ActualVolume)
        });
    }
}
```

- [ ] **Step 3: 编译验证**

Run: `dotnet build SmallGreen.API/SmallGreen.API.csproj`
Expected: BUILD SUCCEEDED

- [ ] **Step 4: 提交**

```bash
git add SmallGreen.API/Service/SystemManagerService.cs SmallGreen.Entity/Machine/SubSystem.cs
git commit -m "feat: 实现实际用量ERP回写功能"
```

---

## Task 11: 存储过程 SQL 脚本

**Files:**
- Create: `scripts/proc-order-info-page-query.sql`
- Create: `scripts/proc-query-formula.sql`
- Create: `scripts/proc-cache-cancel-finish.sql`
- Create: `scripts/proc-usage-writeback.sql`

> **注意：** 存储过程需根据实际 ERP 视图结构和链接服务器名称调整。以下为参考模板。

- [ ] **Step 1: 创建 proc-order-info-page-query.sql**

```sql
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
            -- 构造69位订单信息字符串
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
```

- [ ] **Step 2: 创建 proc-query-formula.sql**

```sql
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
```

> **重要：** 以上存储过程中的 `vwpsWppDataFTPre` 视图的字段名（BulkID, No, GL）和 `vwpsWppDataFT` 的关联方式需要根据实际 ERP 数据库结构调整。`f.BulkID` 对应的可能是助剂桶号或管道号，需确认 ERP 端的字段映射。

- [ ] **Step 3: 创建 proc-cache-cancel-finish.sql**

```sql
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
    -- 具体实现取决于ERP系统的接口约定
END

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
```

- [ ] **Step 4: 创建 proc-usage-writeback.sql**

```sql
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
    -- 可根据ERP接口约定调整为直接写入 psWppDosage 表
    -- 或通过链接服务器调用ERP存储过程
END
```

- [ ] **Step 5: 提交**

```bash
git add scripts/
git commit -m "feat: 添加ERP配方集成相关存储过程SQL脚本模板"
```

---

## Task 12: 数据库初始化更新

**Files:**
- Modify: `SmallGreen.Init/Program.cs`

需要为新增的 Equipment.StepNo 字段添加种子数据。

- [ ] **Step 1: 检查 SmallGreen.Init 的种子数据**

阅读 `SmallGreen.Init/Program.cs`，找到 Equipment 的种子数据初始化部分，为每台设备添加正确的 `StepNo` 值。参考老项目的设备配置：

| 机台 | StepNo |
|------|--------|
| 3#退煮漂联合机-后段 | 2 |
| 1#退煮漂联合机 | 1 |
| 冷堆机 | 1 |
| 3#退煮漂联合机-前段 | 1 |
| 4#退煮漂联合机 | 1 |
| 2#退煮漂联合机-后段 | 2 |
| 2#退煮漂联合机-前段 | 1 |
| 所有固色设备 | 1 |

- [ ] **Step 2: 更新种子数据并验证**

Run: `dotnet build SmallGreen.Init/SmallGreen.Init.csproj`
Expected: BUILD SUCCEEDED

- [ ] **Step 3: 提交**

```bash
git add SmallGreen.Init/Program.cs
git commit -m "feat: Equipment种子数据添加StepNo字段"
```

---

## Task 13: 全量编译和集成验证

- [ ] **Step 1: 全量编译**

Run: `dotnet build SmallGreen.sln`
Expected: BUILD SUCCEEDED

- [ ] **Step 2: 检查代码质量**

确认所有新增代码：
- 无 `console.log` / `Console.WriteLine`（使用 logger）
- 无硬编码的数据库连接字符串（使用配置）
- 无不必要的注释代码
- 异常处理覆盖所有 ERP 存储过程调用

- [ ] **Step 3: 最终提交**

```bash
git add -A
git commit -m "feat: 完成ERP配方集成功能（订单查询、处方下发、状态回调、用量回写）"
```

---

## 实施注意事项

1. **组合助剂逻辑**：Task 8 中的 `GetQCLFomular` 是简化版本。老项目 `OperateSQL.cs:790-1043` 有约250行的复杂逻辑，包含：
   - 按 `No`（助剂编号）识别组合助剂
   - `CombianAss()` 合并相同桶号
   - 多种组合规则（8110+8103, 8109+8103 等）
   - 实施时必须精确对照老代码逐行移植

2. **存储过程**：SQL 脚本为模板，`[LinkedServerName]` 需替换为实际链接服务器名称。存储过程内部逻辑需与 ERP 管理员确认。

3. **PLC 字符串格式**：`EncodeFomulaArray` 使用 `0000.00A` 格式，必须与 PLC 端的解析逻辑完全匹配。

4. **uGuid 生命周期**：`Equipment.uGuid` 在处方下发时设置，在订单完成/暂停/取消时清空。在配液完成时用于用量回写。

5. **并发安全**：2秒轮询内所有操作是串行的。如果 ERP 存储过程调用超时（>2秒），下个周期不会重复处理（因为触发器已被重置）。
