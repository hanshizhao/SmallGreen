# CLAUDE.md

本文件为 Claude Code (claude.ai/code) 在此代码仓库中工作时提供指导。

## 项目概述

SmallGreen 是一个基于 .NET 8 的工业自动化控制系统，用于纺织印染设备。系统通过与西门子 PLC 通信来监控和控制染色机、退煮漂联合机、修色机等设备。

## 构建命令

```bash
# 构建整个解决方案
dotnet build SmallGreen.sln

# Release 模式构建
dotnet build SmallGreen.sln -c Release

# 构建指定项目
dotnet build SmallGreen.API/SmallGreen.API.csproj

# 运行 API 服务
dotnet run --project SmallGreen.API/SmallGreen.API.csproj

# 运行数据库初始化程序
dotnet run --project SmallGreen.Init/SmallGreen.Init.csproj

# 运行桌面设置应用 (WPF)
dotnet run --project SmallGreen.Desktop.Settings/SmallGreen.Desktop.Settings.csproj
```

## 架构说明

### 项目结构

```
SmallGreen/
├── SmallGreen.API/              # ASP.NET Core Web API（后端服务）
├── SmallGreen.Entity/           # 领域实体和业务逻辑
├── SmallGreen.Dto/              # 数据传输对象
├── SmallGreen.Common/           # 公共工具和枚举
├── SmallGreen.Init/             # 数据库初始化和种子数据
├── SmallGreen.Desktop.Settings/ # WPF 桌面客户端（Prism + MaterialDesign）
└── SmallGreen.Desktop.Test/     # WinForms 测试应用程序
```

### 层级依赖关系

```
SmallGreen.API → SmallGreen.Entity → SmallGreen.Common
                → SmallGreen.Dto    → SmallGreen.Common

SmallGreen.Desktop.Settings → SmallGreen.Entity
SmallGreen.Init → SmallGreen.Entity
```

### 核心架构模式

**Dom 模式**：PLC 数据点的核心抽象。`Dom<T>` 封装了 S7.NET 的 `DataItem` 对象，提供类型安全的访问：
- 位于 `SmallGreen.Entity/Basic/Dom.cs`
- 支持类型：bool、short、ushort、float、byte、string
- 存储 PLC 地址信息（DB号、起始字节地址、位地址）

**子系统层级结构**：
```
ISubSystem（如 QCL1 - 1#前处理系统）
├── SiemensPLC（PLC 连接配置）
├── DataPool[]（从 PLC 读取的内存块）
└── Equipment[]（机台列表）
    └── Bulk[]（配液缸列表）
        └── Dom<T>[]（具体数据点）
```

**后台服务模式**：`SmallGreenBackgroundService` 运行 2 秒轮询循环：
1. `SystemManagerService.CheckRuntime()` - 刷新 PLC 数据，检查触发器
2. `SystemManagerService.CheckBulkCompleted()` - 处理已完成的配液操作

## 主要技术栈

- **SqlSugar** - 支持 CodeFirst 的 ORM
- **S7.Net** - 西门子 PLC 通信库
- **Mapster** - 对象映射工具
- **Prism.DryIoc** - WPF 桌面端 MVVM 框架
- **MaterialDesignThemes** - UI 组件库
- **Serilog + LogDashboard** - 日志基础设施

## 领域模型

### 子系统（SubSystemName 枚举）
- `QCL1` (0) - 1#前处理系统
- `QCL2` (1) - 2#前处理系统
- `GS1` (10) - 固色系统

### 实体类型

**机台实体**（`SmallGreen.Entity/Machine/`）：
- `SubSystem` - 由一个 PLC 控制的子系统
- `Equipment` - 机台（如染色机、退煮漂联合机）
- `Bulk` - 配液缸，包含液位传感器和配方数据

**数据实体**（`SmallGreen.Entity/Data/`）：
- `User` - 系统用户
- `PRCSData` / `PRCSDataDetail` - 生产过程数据记录
- `AssInfo` - 助剂信息

### API 响应格式

```csharp
ApiResponse<T>
{
    bool IsSuccess,    // 是否成功
    string Message,    // 消息
    T? Content         // 返回内容
}
```

## 数据库配置

- **SQL Server** + SqlSugar ORM
- 连接字符串位于 `Repository<T>` 和 `SmallGreen.Init/Program.cs`
- 数据库名：`SmallGreenDB`
- 运行 `SmallGreen.Init` 项目可创建数据库并初始化种子数据

## API 接口

- `POST /User/Login` - 用户登录
- `GET /PRCSData/*` - 生产数据查询
- `GET /AssInfo/*` - 助剂信息查询
- 运行后可在根目录访问 Swagger UI
- 日志面板地址：`/LogDashboard`

## PLC 通信

PLC 地址配置在 `SmallGreen.Init/Program.cs` 的种子数据中。每个 `Dom<T>` 指定：
- `DataType` - 数据类型（通常为 DataBlock）
- `VarType` - 变量类型（Bit、Int、Word、Real、String、Byte）
- `DB` - DB 块编号
- `StartByteAdr` - 起始字节地址
- `BitAdr` - 位地址（用于布尔值）
- `Count` - 字节计数（用于字符串/数组）
