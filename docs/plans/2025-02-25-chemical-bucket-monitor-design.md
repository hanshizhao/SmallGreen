# 助剂桶液位监控页面设计文档

**日期：** 2025-02-25
**状态：** 已确认

## 1. 功能概述

在 WPF 桌面应用中新增助剂桶液位监控页面，支持：
- 图形化桶状图展示各助剂桶液位
- 按子系统分组（QCL1/QCL2 共用、GS1 等）
- SignalR 实时更新液位数据
- 点击桶后底部抽屉显示详情
- 在详情面板中修改浓度比例值，保存到数据库后同步 PLC

## 2. 整体架构

```
┌─────────────────────────────────────────────────────────────┐
│                    ChemicalBucketView                        │
│  ┌─────────────────────────────────────────────────────┐    │
│  │  子系统切换 Tab (QCL1/QCL2 共用 | GS1 | ...)         │    │
│  └─────────────────────────────────────────────────────┘    │
│  ┌─────────────────────────────────────────────────────┐    │
│  │                                                     │    │
│  │     ┌───┐  ┌───┐  ┌───┐  ┌───┐  ┌───┐             │    │
│  │     │ 1 │  │ 2 │  │ 3 │  │ 4 │  │ 5 │  ...        │    │
│  │     │███│  │██ │  │███│  │█  │  │███│             │    │
│  │     │███│  │   │  │██ │  │   │  │██ │             │    │
│  │     └───┘  └───┘  └───┘  └───┘  └───┘             │    │
│  │     ChemicalBucket 控件 (WrapPanel 布局)            │    │
│  │                                                     │    │
│  └─────────────────────────────────────────────────────┘    │
│                                                              │
│  ┌─────────────────────────────────────────────────────┐    │
│  │  底部抽屉 (DrawerNavigationView)                     │    │
│  │  显示选中桶的详情 + 浓度比例修改                      │    │
│  └─────────────────────────────────────────────────────┘    │
└─────────────────────────────────────────────────────────────┘
```

## 3. 文件结构

```
SmallGreen.Desktop.Settings/
├── Views/
│   ├── ChemicalBucketView.xaml          # 新建 - 主页面视图
│   ├── ChemicalBucketView.xaml.cs       # 新建
│   ├── ChemicalBucketDetailView.xaml    # 新建 - 底部抽屉详情
│   └── ChemicalBucketDetailView.xaml.cs # 新建
├── ViewModels/
│   ├── ChemicalBucketViewModel.cs       # 新建 - 主页面 ViewModel
│   └── ChemicalBucketDetailViewModel.cs # 新建 - 详情 ViewModel
├── BlackControl/
│   └── ChemicalBucket.cs                # 修改 - 增强依赖属性
├── Themes/
│   └── Generic.xaml                     # 修改 - 添加桶控件样式
├── IServices/
│   └── IChemicalBucketService.cs        # 新建 - 服务接口
├── Services/
│   └── ChemicalBucketService.cs         # 新建 - 服务实现
└── Models/
    └── SubSystemItem.cs                 # 新建 - 子系统项模型

SmallGreen.Dto/Machine/
├── ChemicalBucketDto.cs                 # 新建
├── FormulaItemDto.cs                    # 新建
└── UpdateConcentrationDto.cs            # 新建

SmallGreen.API/Controllers/
└── ChemicalBucketController.cs          # 新建（如需要）
```

## 4. ChemicalBucket 控件设计

### 4.1 依赖属性

| 属性名 | 类型 | 说明 |
|--------|------|------|
| `BucketCode` | string | 桶编号/名称 |
| `LevelValue` | float | 当前液位值（升） |
| `MaxLevel` | float | 桶容量（升），默认 1000 |
| `IsSelected` | bool | 是否选中 |
| `LevelColor` | Brush | 液体颜色（可绑定，根据液位自动计算） |

### 4.2 视觉设计

```
    ┌────────────┐
    │   ┌────┐   │  ← 桶盖区域
    │   └────┘   │
    ├────────────┤
    │  500 L     │  ← 数值显示区
    │  ┌──────┐  │
    │  │██████│  │  ← 液体填充区（动态高度）
    │  │██████│  │
    │  │██████│  │
    │  └──────┘  │
    ├────────────┤
    │   1#桶     │  ← 桶编号
    └────────────┘
```

### 4.3 液位颜色规则

- 液位 < 20%：红色（低液位警告）
- 液位 20%-50%：橙色
- 液位 50%-80%：蓝色
- 液位 > 80%：绿色

## 5. ChemicalBucketViewModel 设计

### 5.1 属性

| 属性名 | 类型 | 说明 |
|--------|------|------|
| `SubSystems` | `ObservableCollection<SubSystemItem>` | 子系统列表 |
| `SelectedSubSystem` | `SubSystemItem` | 当前选中的子系统 |
| `Buckets` | `ObservableCollection<ChemicalBucketDto>` | 助剂桶列表 |
| `SelectedBucket` | `ChemicalBucketDto` | 当前选中的助剂桶 |
| `IsDrawerOpen` | `bool` | 底部抽屉是否打开 |

### 5.2 命令

| 命令名 | 说明 |
|--------|------|
| `LoadedCommand` | 页面加载时获取初始数据 |
| `SubSystemChangedCommand` | 切换子系统时重新加载数据 |
| `BucketClickCommand` | 点击桶时打开底部抽屉 |

### 5.3 SignalR 订阅逻辑

```csharp
// 订阅当前子系统的助剂桶数据更新
await HubConnect<List<ChemicalBucketDto>>(
    selectedSubSystem,
    "ChemicalBuckets",
    updatedList => {
        Buckets = new ObservableCollection<ChemicalBucketDto>(updatedList);
    }
);
```

## 6. 底部抽屉详情面板

### 6.1 布局

```
┌──────────────────────────────────────────────────────────────────┐
│  桶详情 - 1#助剂桶                                         [×]  │
├──────────────────────────────────────────────────────────────────┤
│  ┌─────────────────────┐  ┌─────────────────────────────────┐   │
│  │   基本信息           │  │   浓度比例设置                   │   │
│  │   桶编号: 1#桶      │  │   当前浓度比例:                  │   │
│  │   当前液位: 500 L   │  │   ┌─────────────────────────┐   │   │
│  │   桶容量: 1000 L    │  │   │  [====●=====] 0.85     │   │   │
│  │   液位百分比: 50%   │  │   └─────────────────────────┘   │   │
│  │   上次配液时间:     │  │                                 │   │
│  │     2025-02-25      │  │   ┌─────────────────────────┐   │   │
│  │     10:30:00        │  │   │  [保存]     [取消]      │   │   │
│  └─────────────────────┘  └─────────────────────────────────┘   │
│  ┌─────────────────────────────────────────────────────────┐    │
│  │   配方数据（最近一次）                                    │    │
│  │   助剂A: 10L  │  助剂B: 5L  │  助剂C: 8L  │  ...        │    │
│  └─────────────────────────────────────────────────────────┘    │
└──────────────────────────────────────────────────────────────────┘
```

### 6.2 ChemicalBucketDetailViewModel

| 属性名 | 类型 | 说明 |
|--------|------|------|
| `Bucket` | `ChemicalBucketDto` | 当前桶信息 |
| `ConcentrationRatio` | `float` | 浓度比例（可编辑） |
| `FormulaList` | `ObservableCollection<FormulaItemDto>` | 配方数据列表 |

| 命令名 | 说明 |
|--------|------|
| `SaveCommand` | 保存浓度比例（先存数据库，再同步 PLC） |
| `CancelCommand` | 取消并关闭抽屉 |

## 7. API 和服务层

### 7.1 DTO 定义

```csharp
public class ChemicalBucketDto
{
    public long Id { get; set; }
    public string CodeNumber { get; set; }      // 桶编号
    public float Level { get; set; }            // 当前液位（升）
    public float MaxCapacity { get; set; }      // 桶容量（升）
    public float ConcentrationRatio { get; set; } // 浓度比例
    public long SubSystemId { get; set; }       // 所属子系统
    public string SubSystemName { get; set; }   // 子系统名称
    public DateTime LastCompleteTime { get; set; } // 上次配液时间
    public List<FormulaItemDto>? FormulaItems { get; set; }
}

public class FormulaItemDto
{
    public string AssName { get; set; }
    public float PlanVolume { get; set; }
    public float RealVolume { get; set; }
}

public class UpdateConcentrationDto
{
    public long BucketId { get; set; }
    public float ConcentrationRatio { get; set; }
}
```

### 7.2 服务接口

```csharp
public interface IChemicalBucketService
{
    Task<ApiResponse<List<ChemicalBucketDto>>> GetBySubSystem(string subSystemName);
    Task<ApiResponse<bool>> UpdateConcentration(UpdateConcentrationDto dto);
}
```

### 7.3 API 端点

| 方法 | 路径 | 说明 |
|------|------|------|
| GET | `/api/ChemicalBucket/{subSystemName}` | 获取子系统助剂桶列表 |
| PUT | `/api/ChemicalBucket/Concentration` | 更新浓度比例 |

## 8. 子系统分组逻辑

### 8.1 SubSystemItem 模型

```csharp
public class SubSystemItem
{
    public string DisplayName { get; set; }      // 显示名称
    public string[] SubSystemNames { get; set; } // 包含的子系统代码
    public bool IsShared { get; set; }           // 是否共用
}
```

### 8.2 初始化

```csharp
SubSystems = new ObservableCollection<SubSystemItem>
{
    new SubSystemItem
    {
        DisplayName = "前处理共用",
        SubSystemNames = new[] { "QCL1", "QCL2" },
        IsShared = true
    },
    new SubSystemItem
    {
        DisplayName = "固色系统",
        SubSystemNames = new[] { "GS1" },
        IsShared = false
    }
};
```

### 8.3 多订阅管理

```csharp
private List<IDisposable> _hubSubscriptions = new();

private async Task SubscribeSubSystems(string[] subSystemNames)
{
    // 取消之前的订阅
    foreach (var sub in _hubSubscriptions)
        sub.Dispose();
    _hubSubscriptions.Clear();

    // 订阅新的子系统
    foreach (var name in subSystemNames)
    {
        var sub = await HubConnect<List<ChemicalBucketDto>>(
            name,
            "ChemicalBuckets",
            list => MergeAndUpdateList(name, list)
        );
        _hubSubscriptions.Add(sub);
    }
}
```

## 9. 技术要点

- 复用现有 Prism + MaterialDesign 架构
- 复用现有底部抽屉机制（`ShowBottomDrawerEvent`）
- 增强 ChemicalBucket 自定义控件
- SignalR 多订阅管理支持共用子系统
- 浓度修改：先保存数据库，再同步 PLC
