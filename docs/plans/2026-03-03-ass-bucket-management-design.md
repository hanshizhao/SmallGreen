# 助剂桶管理功能设计

> 设计日期: 2026-03-03

---

## 概述

助剂桶管理是一个独立的 WPF 页面，用于维护助剂桶中的助剂信息，确保配液完成时能正确匹配到对应的助剂。

---

## 需求分析

### 业务背景
- 助剂桶数量固定（由设备决定）
- 每个助剂桶存储一种助剂（单一助剂或混合助剂）
- 混合助剂由多个组分按比例混合而成
- 配液完成时通过 `AssInfo.Sequence` 匹配管道对应的助剂

### 功能范围

| 功能 | 支持 | 说明 |
|-----|------|------|
| 新增助剂桶 | ❌ | 桶数量固定，不支持新增 |
| 删除助剂桶 | ❌ | 不支持删除 |
| 编辑助剂桶 | ✓ | 修改助剂信息 |
| 新增混合组分 | ✓ | 为混合助剂添加组分 |
| 编辑混合组分 | ✓ | 修改组分信息 |
| 删除混合组分 | ✓ | 删除单个组分 |

---

## 数据模型

### 实体关系

```
AssInfo (助剂桶)
├── Id
├── Sequence (管道顺序，固定)
├── BulkSequence
├── CodeNumber (助剂编号)
├── Name (助剂名称)
├── MaxV (最大容量，固定1000L)
├── Concentration (浓度)
├── IsMixed (是否混合)
└── ListMixedDetail (1:N) ── 混合助剂才有

MixedDetail (混合组分)
├── Id
├── ParentId → AssInfo.Id
├── CodeNumber (组分编号)
├── Name (组分名称)
├── Concentration (组分浓度)
└── Ratio (混合比例，```

### 混合比例计算逻辑

系统自动归一化比例，用户输入原始比例值即可：

```csharp
// 例：助剂A比例=10，助剂B比例=5
var sumRatio = 10 + 5;  // = 15
var curRatio_A = 10 / 15;  // = 0.667
var curRatio_B = 5 / 15;   // = 0.333
```

---

## API 接口设计

### 接口列表

| 方法 | 路由 | 说明 |
|-----|------|------|
| GET | `/AssInfo/Buckets` | 获取所有助剂桶列表（包含混合组分） |
| PUT | `/AssInfo/Buckets/{id}` | 更新助剂桶基本信息 |
| POST | `/AssInfo/MixedComponent` | 新增混合组分 |
| PUT | `/AssInfo/MixedComponent/{id}` | 更新混合组分 |
| DELETE | `/AssInfo/MixedComponent/{id}` | 删除混合组分 |

### DTO 定义

```csharp
// 查询响应
public class AssBucketDto
{
    public long Id { get; set; }
    public string SubSystemName { get; set; }  // 所属系统
    public int Sequence { get; set; }           // 管道顺序（只读）
    public string CodeNumber { get; set; }       // 助剂编号
    public string Name { get; set; }             // 助剂名称
    public double Concentration { get; set; }    // 浓度
    public float MaxV { get; set; }            // 最大容量（只读，1000L）
    public bool IsMixed { get; set; }          // 是否混合助剂
    public List<MixedComponentDto> MixedComponents { get; set; }
}

public class MixedComponentDto
{
    public long Id { get; set; }
    public long ParentId { get; set; }
    public string CodeNumber { get; set; }
    public string Name { get; set; }
    public double Concentration { get; set; }
    public double Ratio { get; set; }
}

// 请求
public class UpdateAssBucketDto
{
    public long Id { get; set; }
    public string CodeNumber { get; set; }
    public string Name { get; set; }
    public double Concentration { get; set; }
    public bool IsMixed { get; set; }
}

public class CreateMixedComponentDto
{
    public long ParentId { get; set; }
    public string CodeNumber { get; set; }
    public string Name { get; set; }
    public double Concentration { get; set; }
    public double Ratio { get; set; }
}

public class UpdateMixedComponentDto
{
    public long Id { get; set; }
    public string CodeNumber { get; set; }
    public string Name { get; set; }
    public double Concentration { get; set; }
    public double Ratio { get; set; }
}
```

---

## 页面设计

### 主页面 - AssBucketManagementView

**布局结构**
```
┌─────────────────────────────────────────────────────────┐
│  [工具栏]                                                │
│  [刷新按钮]                                               │
├─────────────────────────────────────────────────────────┤
│                                                         │
│  [DataGrid - 助剂桶表格]                                │
│  ┌───────────────────────────────────────────────────┐  │
│  │ 所属系统 │ 管道顺序 │ 编号 │ 名称 │ 浓度 │ ... │  │
│  ├───────────────────────────────────────────────────┤  │
│  │ [展开行 - 混合组分详情]                            │  │
│  │  组分编号 │ 组分名称 │ 组分浓度 │ 混合比例 │ 操作 │  │
│  └───────────────────────────────────────────────────┘  │
│                                                         │
└─────────────────────────────────────────────────────────┘
```

**DataGrid 列定义**

| 列名 | 绑定字段 | 宽度 | 说明 |
|-----|---------|------|------|
| SubSystemName | 所属系统 | 80 | 前处理/固色，用背景色区分 |
| Sequence | 管道顺序 | 60 | 数字，只读 |
| CodeNumber | 助剂编号 | 100 | |
| Name | 助剂名称 | 150 | |
| Concentration | 浓度 | 80 | |
| MaxV | 最大容量 | 80 | 只读，固定1000L |
| IsMixed | 是否混合 | 80 | ✓/✗ 图标，可展开 |
| Operations | 操作 | 120 | 编辑按钮 |

**行展开模板** - 使用 DataGrid.RowDetailsTemplate 展示混合组分子表格

### 弹窗设计

#### AssBucketEditDialog - 编辑助剂桶

```
┌─────────────────────────────────────────────┐
│  编辑助剂桶                              [X] │
├─────────────────────────────────────────────┤
│                                         │
│  所属系统: [前处理]  (只读)              │
│  管道顺序: [1]       (只读)              │
│                                         │
│  助剂编号: [______]                     │
│  助剂名称: [_____________________]      │
│                                         │
│  浓度: [______]                          │
│  最大容量: 1000 (只读)                   │
│                                         │
│  是否混合助剂: [✓]                      │
│                                         │
│           [取消]        [保存]            │
└─────────────────────────────────────────────┘
```

**可编辑字段**

| 字段 | 是否可编辑 | 说明 |
|-----|-----------|------|
| 所属系统 | 只读 | 显示所属子系统 |
| 管道顺序 | 只读 | 固定值，不可修改 |
| 助剂编号 | ✓ 可编辑 | |
| 助剂名称 | ✓ 可编辑 | |
| 浓度 | ✓ 可编辑 | |
| 最大容量 | 只读 | 固定值1000L |
| 是否混合 | ✓ 可编辑 | 控制展开行显示/隐藏 |

#### MixedComponentEditDialog - 编辑/新增混合组分

```
┌─────────────────────────────────────────────┐
│  编辑混合组分                              [X] │
├─────────────────────────────────────────────┤
│                                         │
│  组分编号: [______]    组分名称: [______] │
│                                         │
│  组分浓度: [______]    混合比例: [______] │
│                                         │
│           [取消]        [保存]            │
└─────────────────────────────────────────────┘
```

---

## 验证规则

### 前端验证

| 字段 | 验证规则 |
|-----|---------|
| 助剂编号 | 必填，最大50字符 |
| 助剂名称 | 必填，最大100字符 |
| 浓度 | 必填，≥ 0 |
| 组分编号 | 必填，最大50字符 |
| 组分名称 | 必填，最大100字符 |
| 组分浓度 | 必填，≥ 0 |
| 混合比例 | 必填，> 0（不要求和等于1） |

### 后端错误处理
- 返回 `ApiResponse<T>` 格式
- `IsSuccess = false` 时显示错误提示
- 使用 IDialogHostService 显示错误消息

---

## 文件结构

```
SmallGreen.Desktop.Settings/
├── Views/
│   └── AssBucketManagementView.xaml          # 主页面
├── ViewModels/
│   └── AssBucketManagementViewModel.cs      # 主页面 ViewModel
├── Dialogs/
│   ├── AssBucketEditDialog.xaml            # 编辑助剂桶弹窗
│   ├── AssBucketEditDialogViewModel.cs
│   ├── MixedComponentEditDialog.xaml       # 编辑混合组分弹窗
│   └── MixedComponentEditDialogViewModel.cs

SmallGreen.Dto/
└── Machine/
    ├── AssBucketDto.cs
    ├── MixedComponentDto.cs
    ├── UpdateAssBucketDto.cs
    ├── CreateMixedComponentDto.cs
    └── UpdateMixedComponentDto.cs

SmallGreen.API/
└── Controllers/
    └── AssInfoController.cs      # 新增 CRUD 接口
```

---

## 菜单配置

在 `MainWindowViewModel.Configure()` 中修改：

```csharp
new SubItem("助剂桶管理", nameof(AssBucketManagementView), false),
```

替换原来的占位符：
```csharp
new SubItem("助剂桶管理", nameof(ArthurView), false),  // 删除这行
new SubItem("助剂桶管理", nameof(AssBucketManagementView), false),  // 新增这行
```
