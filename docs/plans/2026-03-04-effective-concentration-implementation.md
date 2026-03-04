# 助剂实际有效浓度功能实现计划

> **For Claude:** REQUIRED SUB-SKILL: Use superpowers:executing-plans to implement this plan task-by-task.

**Goal:** 为助剂消耗统计新增"实际有效浓度"字段，使财务能够统计不含水的助剂消耗量。

**Architecture:** 在 AssInfo、MixedDetail、PRCSDataDetail 三个实体中新增 EffectiveConcentration 相关字段，修改 SubSystem 中的消耗计算逻辑，更新 DTO 和 API 服务，最后更新桌面端界面。

**Tech Stack:** .NET 8, SqlSugar ORM, WPF (Prism + MaterialDesign)

---

## Task 1: 实体层 - AssInfo 新增字段

**Files:**
- Modify: `SmallGreen.Entity/Data/AssInfo.cs`

**Step 1: 新增 EffectiveConcentration 属性**

在 `Concentration` 属性后添加：

```csharp
/// <summary>
/// 助剂的浓度，单位是 克每升，用于计算助剂消耗的公斤数
/// </summary>
public double Concentration { get; set; }

/// <summary>
/// 实际有效浓度（g/L），每升母液真正含有的助剂质量，用于财务消耗计算
/// </summary>
public double EffectiveConcentration { get; set; }
```

**Step 2: 验证构建**

Run: `dotnet build SmallGreen.Entity/SmallGreen.Entity.csproj`
Expected: Build succeeded

---

## Task 2: 实体层 - MixedDetail 新增字段

**Files:**
- Modify: `SmallGreen.Entity/Data/MixedDetail.cs`

**Step 1: 新增 EffectiveConcentration 属性**

在 `Concentration` 属性后添加：

```csharp
/// <summary>
/// 助剂的浓度，单位是 克每升，用于计算助剂消耗的公斤数
/// </summary>
public double Concentration { get; set; }

/// <summary>
/// 实际有效浓度（g/L），用于财务消耗计算
/// </summary>
public double EffectiveConcentration { get; set; }
```

**Step 2: 验证构建**

Run: `dotnet build SmallGreen.Entity/SmallGreen.Entity.csproj`
Expected: Build succeeded

---

## Task 3: 实体层 - PRCSDataDetail 新增字段

**Files:**
- Modify: `SmallGreen.Entity/Data/PRCSDataDetail.cs`

**Step 1: 新增两个字段**

在 `AssKg` 属性后添加：

```csharp
/// <summary>
/// 实际公斤数
/// </summary>
public double AssKg { get; set; }

/// <summary>
/// 实际有效浓度（g/L）
/// </summary>
public double EffectiveGramsPerLiter { get; set; }

/// <summary>
/// 财务消耗量（kg），基于实际有效浓度计算
/// </summary>
public double EffectiveAssKg { get; set; }
```

**Step 2: 更新 ToDto 方法**

```csharp
public PRCSDataDetailDto ToDto()
{
    return new PRCSDataDetailDto
    {
        Id = Id,
        AssSequence = AssSequence,
        AssCodeNumber = AssCodeNumber,
        AssGl = AssGl,
        AssId = AssId,
        AssKg = AssKg,
        AssName = AssName,
        ParentId = ParentId,
        PlanKg = PlanKg,
        GramsPerLiter = GramsPerLiter,
        PlanVolumeWithWater = PlanVolumeWithWater,
        RealVolumeWithWater = RealVolumeWithWater,
        EffectiveGramsPerLiter = EffectiveGramsPerLiter,
        EffectiveAssKg = EffectiveAssKg
    };
}
```

**Step 3: 验证构建**

Run: `dotnet build SmallGreen.Entity/SmallGreen.Entity.csproj`
Expected: Build succeeded

---

## Task 4: DTO 层 - AssInfoDto 新增字段

**Files:**
- Modify: `SmallGreen.Dto/Data/AssInfoDto.cs`

**Step 1: 新增 EffectiveConcentration 属性**

在 `Concentration` 属性后添加：

```csharp
/// <summary>
/// 助剂的浓度，单位是 克每升，用于计算助剂消耗的公斤数
/// </summary>
public double Concentration { get; set; }

/// <summary>
/// 实际有效浓度（g/L），用于财务消耗计算
/// </summary>
public double EffectiveConcentration { get; set; }
```

**Step 2: 验证构建**

Run: `dotnet build SmallGreen.Dto/SmallGreen.Dto.csproj`
Expected: Build succeeded

---

## Task 5: DTO 层 - MixedDetailDto 新增字段

**Files:**
- Modify: `SmallGreen.Dto/Data/MixedDetailDto.cs`

**Step 1: 新增 EffectiveConcentration 属性**

在 `Concentration` 属性后添加：

```csharp
/// <summary>
/// 助剂的浓度，单位是 克每升，用于计算助剂消耗的公斤数
/// </summary>
public double Concentration { get; set; }

/// <summary>
/// 实际有效浓度（g/L），用于财务消耗计算
/// </summary>
public double EffectiveConcentration { get; set; }
```

**Step 2: 验证构建**

Run: `dotnet build SmallGreen.Dto/SmallGreen.Dto.csproj`
Expected: Build succeeded

---

## Task 6: DTO 层 - PRCSDataDetailDto 新增字段

**Files:**
- Modify: `SmallGreen.Dto/Data/PRCSDataDetailDto.cs`

**Step 1: 新增两个字段**

在 `AssKg` 属性后添加：

```csharp
public double AssKg { get; set; }

/// <summary>
/// 实际有效浓度（g/L）
/// </summary>
public double EffectiveGramsPerLiter { get; set; }

/// <summary>
/// 财务消耗量（kg）
/// </summary>
public double EffectiveAssKg { get; set; }
```

**Step 2: 验证构建**

Run: `dotnet build SmallGreen.Dto/SmallGreen.Dto.csproj`
Expected: Build succeeded

---

## Task 7: DTO 层 - AssBucketDto 新增字段

**Files:**
- Modify: `SmallGreen.Dto/Machine/AssBucketDto.cs`

**Step 1: 新增 EffectiveConcentration 属性**

在 `Concentration` 属性后添加：

```csharp
/// <summary>
/// 助剂的浓度，单位是 克每升
/// </summary>
public double Concentration { get; set; }

/// <summary>
/// 实际有效浓度（g/L），用于财务消耗计算
/// </summary>
public double EffectiveConcentration { get; set; }
```

**Step 2: 验证构建**

Run: `dotnet build SmallGreen.Dto/SmallGreen.Dto.csproj`
Expected: Build succeeded

---

## Task 8: DTO 层 - MixedComponentDto 新增字段

**Files:**
- Modify: `SmallGreen.Dto/Machine/MixedComponentDto.cs`

**Step 1: 新增 EffectiveConcentration 属性**

在 `Concentration` 属性后添加：

```csharp
/// <summary>
/// 组分浓度，单位是 克每升
/// </summary>
public double Concentration { get; set; }

/// <summary>
/// 实际有效浓度（g/L），用于财务消耗计算
/// </summary>
public double EffectiveConcentration { get; set; }
```

**Step 2: 验证构建**

Run: `dotnet build SmallGreen.Dto/SmallGreen.Dto.csproj`
Expected: Build succeeded

---

## Task 9: DTO 层 - UpdateAssBucketDto 新增字段

**Files:**
- Modify: `SmallGreen.Dto/Machine/UpdateAssBucketDto.cs`

**Step 1: 新增 EffectiveConcentration 属性**

在 `Concentration` 属性后添加：

```csharp
/// <summary>
/// 助剂的浓度，单位是 克每升
/// </summary>
[Required]
[Range(0, double.MaxValue)]
public double Concentration { get; set; }

/// <summary>
/// 实际有效浓度（g/L），用于财务消耗计算
/// </summary>
[Required]
[Range(0, double.MaxValue)]
public double EffectiveConcentration { get; set; }
```

**Step 2: 验证构建**

Run: `dotnet build SmallGreen.Dto/SmallGreen.Dto.csproj`
Expected: Build succeeded

---

## Task 10: DTO 层 - CreateMixedComponentDto 新增字段

**Files:**
- Modify: `SmallGreen.Dto/Machine/CreateMixedComponentDto.cs`

**Step 1: 新增 EffectiveConcentration 属性**

在 `Concentration` 属性后添加：

```csharp
/// <summary>
/// 组分浓度，单位是 克每升
/// </summary>
[Required]
[Range(0, double.MaxValue)]
public double Concentration { get; set; }

/// <summary>
/// 实际有效浓度（g/L），用于财务消耗计算
/// </summary>
[Required]
[Range(0, double.MaxValue)]
public double EffectiveConcentration { get; set; }
```

**Step 2: 验证构建**

Run: `dotnet build SmallGreen.Dto/SmallGreen.Dto.csproj`
Expected: Build succeeded

---

## Task 11: DTO 层 - UpdateMixedComponentDto 新增字段

**Files:**
- Modify: `SmallGreen.Dto/Machine/UpdateMixedComponentDto.cs`

**Step 1: 新增 EffectiveConcentration 属性**

在 `Concentration` 属性后添加：

```csharp
/// <summary>
/// 组分浓度，单位是 克每升
/// </summary>
[Required]
[Range(0, double.MaxValue)]
public double Concentration { get; set; }

/// <summary>
/// 实际有效浓度（g/L），用于财务消耗计算
/// </summary>
[Required]
[Range(0, double.MaxValue)]
public double EffectiveConcentration { get; set; }
```

**Step 2: 验证构建**

Run: `dotnet build SmallGreen.Dto/SmallGreen.Dto.csproj`
Expected: Build succeeded

---

## Task 12: API 服务 - AssInfoService 更新

**Files:**
- Modify: `SmallGreen.API/Service/AssInfoService.cs`

**Step 1: 更新 GetBuckets 方法中的映射**

在第 48 行后添加：

```csharp
Concentration = ass.Concentration,
EffectiveConcentration = ass.EffectiveConcentration,
```

在第 57 行后添加：

```csharp
Concentration = m.Concentration,
EffectiveConcentration = m.EffectiveConcentration,
```

**Step 2: 更新 GetBucketById 方法中的映射**

在第 108 行后添加：

```csharp
Concentration = ass.Concentration,
EffectiveConcentration = ass.EffectiveConcentration,
```

在第 117 行后添加：

```csharp
Concentration = m.Concentration,
EffectiveConcentration = m.EffectiveConcentration,
```

**Step 3: 更新 UpdateBucket 方法**

在第 154 行后添加：

```csharp
ass.Concentration = dto.Concentration;
ass.EffectiveConcentration = dto.EffectiveConcentration;
```

在第 177 行后添加返回值：

```csharp
Concentration = ass.Concentration,
EffectiveConcentration = ass.EffectiveConcentration,
```

**Step 4: 更新 CreateMixedComponent 方法**

在第 226 行后添加：

```csharp
Concentration = dto.Concentration,
EffectiveConcentration = dto.EffectiveConcentration,
```

在第 243 行后添加返回值：

```csharp
Concentration = dto.Concentration,
EffectiveConcentration = dto.EffectiveConcentration,
```

**Step 5: 更新 UpdateMixedComponent 方法**

在第 278 行后添加：

```csharp
detail.Concentration = dto.Concentration;
detail.EffectiveConcentration = dto.EffectiveConcentration;
```

在第 300 行后添加返回值：

```csharp
Concentration = detail.Concentration,
EffectiveConcentration = detail.EffectiveConcentration,
```

**Step 6: 验证构建**

Run: `dotnet build SmallGreen.API/SmallGreen.API.csproj`
Expected: Build succeeded

---

## Task 13: 业务逻辑 - SubSystem 消耗计算更新

**Files:**
- Modify: `SmallGreen.Entity/Machine/SubSystem.cs`

**Step 1: 更新混合助剂计算（第 247-262 行）**

将原来的代码：

```csharp
var assKg = (realLitre * it.Concentration * curRatio) / 1000d;
var planKg = (prcsData.PlanVolume * formular * curRatio) / 1000d;
var planVolumeWithWater = (prcsData.PlanVolume * formular * curRatio) / it.Concentration;
var p = new PRCSDataDetail
{
    AssId = it.Id,
    AssCodeNumber = it.CodeNumber,
    AssName = it.Name,
    AssGl = formular,
    AssSequence = sequence,
    AssKg = assKg,
    PlanKg = planKg,
    GramsPerLiter = it.Concentration,
    PlanVolumeWithWater = planVolumeWithWater,
    RealVolumeWithWater = realLitre
};
```

改为：

```csharp
var assKg = (realLitre * it.Concentration * curRatio) / 1000d;
var effectiveAssKg = (realLitre * it.EffectiveConcentration * curRatio) / 1000d;
var planKg = (prcsData.PlanVolume * formular * curRatio) / 1000d;
var planVolumeWithWater = (prcsData.PlanVolume * formular * curRatio) / it.Concentration;
var p = new PRCSDataDetail
{
    AssId = it.Id,
    AssCodeNumber = it.CodeNumber,
    AssName = it.Name,
    AssGl = formular,
    AssSequence = sequence,
    AssKg = assKg,
    EffectiveAssKg = effectiveAssKg,
    PlanKg = planKg,
    GramsPerLiter = it.Concentration,
    EffectiveGramsPerLiter = it.EffectiveConcentration,
    PlanVolumeWithWater = planVolumeWithWater,
    RealVolumeWithWater = realLitre
};
```

**Step 2: 更新非混合助剂计算（第 278-290 行）**

将原来的代码：

```csharp
var p = new PRCSDataDetail
{
    AssGl = formular,
    AssId = ass.Id,
    AssSequence = sequence,
    AssKg = (realLitre * ass.Concentration) / 1000d,
    AssName = ass.Name,
    AssCodeNumber = ass.CodeNumber,
    PlanKg = (prcsData.PlanVolume * formular) / 1000d,
    GramsPerLiter = ass.Concentration,
    PlanVolumeWithWater = (prcsData.PlanVolume * formular) / ass.Concentration,
    RealVolumeWithWater = realLitre
};
```

改为：

```csharp
var p = new PRCSDataDetail
{
    AssGl = formular,
    AssId = ass.Id,
    AssSequence = sequence,
    AssKg = (realLitre * ass.Concentration) / 1000d,
    EffectiveAssKg = (realLitre * ass.EffectiveConcentration) / 1000d,
    AssName = ass.Name,
    AssCodeNumber = ass.CodeNumber,
    PlanKg = (prcsData.PlanVolume * formular) / 1000d,
    GramsPerLiter = ass.Concentration,
    EffectiveGramsPerLiter = ass.EffectiveConcentration,
    PlanVolumeWithWater = (prcsData.PlanVolume * formular) / ass.Concentration,
    RealVolumeWithWater = realLitre
};
```

**Step 3: 验证构建**

Run: `dotnet build SmallGreen.Entity/SmallGreen.Entity.csproj`
Expected: Build succeeded

---

## Task 14: 桌面端 - AssBucketEditDialogViewModel 更新

**Files:**
- Modify: `SmallGreen.Desktop.Settings/Dialogs/AssBucketEditDialogViewModel.cs`

**Step 1: 新增属性**

在第 18 行后添加：

```csharp
private double concentration;
private double effectiveConcentration;
```

在 `Concentration` 属性后添加：

```csharp
public double Concentration
{
    get => concentration;
    set => SetProperty(ref concentration, value);
}

public double EffectiveConcentration
{
    get => effectiveConcentration;
    set => SetProperty(ref effectiveConcentration, value);
}
```

**Step 2: 更新 OnDialogOpend 方法**

在第 100 行后添加：

```csharp
Concentration = bucket.Concentration;
EffectiveConcentration = bucket.EffectiveConcentration;
```

**Step 3: 更新 Save 方法中的 DTO**

在第 125 行后添加：

```csharp
var dto = new UpdateAssBucketDto
{
    Id = id,
    CodeNumber = CodeNumber,
    Name = Name,
    Concentration = Concentration,
    EffectiveConcentration = EffectiveConcentration,
    IsMixed = IsMixed
};
```

**Step 4: 验证构建**

Run: `dotnet build SmallGreen.Desktop.Settings/SmallGreen.Desktop.Settings.csproj`
Expected: Build succeeded

---

## Task 15: 桌面端 - AssBucketEditDialog.xaml 更新

**Files:**
- Modify: `SmallGreen.Desktop.Settings/Dialogs/AssBucketEditDialog.xaml`

**Step 1: 新增输入框**

在浓度输入框（第 84-93 行）后添加：

```xml
<Grid Margin="0,0,0,12">
    <Grid.ColumnDefinitions>
        <ColumnDefinition Width="100"/>
        <ColumnDefinition Width="*"/>
    </Grid.ColumnDefinitions>
    <TextBlock Grid.Column="0" Text="有效浓度(g/L):" VerticalAlignment="Center"/>
    <TextBox Grid.Column="1"
             Text="{Binding EffectiveConcentration, UpdateSourceTrigger=PropertyChanged}"
             md:HintAssist.Hint="每升母液实际含有的助剂质量"/>
</Grid>
```

**Step 2: 验证构建**

Run: `dotnet build SmallGreen.Desktop.Settings/SmallGreen.Desktop.Settings.csproj`
Expected: Build succeeded

---

## Task 16: 桌面端 - MixedComponentEditDialogViewModel 更新

**Files:**
- Modify: `SmallGreen.Desktop.Settings/Dialogs/MixedComponentEditDialogViewModel.cs`

**Step 1: 新增属性**

在第 20 行后添加：

```csharp
private double concentration;
private double effectiveConcentration;
```

在 `Concentration` 属性后添加：

```csharp
public double Concentration
{
    get => concentration;
    set => SetProperty(ref concentration, value);
}

public double EffectiveConcentration
{
    get => effectiveConcentration;
    set => SetProperty(ref effectiveConcentration, value);
}
```

**Step 2: 更新新建组件初始化（第 100 行附近）**

```csharp
if (isNew && parameters.TryGetValue("ParentId", out long parentIdValue))
{
    parentId = parentIdValue;
    CodeNumber = string.Empty;
    Name = string.Empty;
    Concentration = 0;
    EffectiveConcentration = 0;
    Ratio = 1;
}
```

**Step 3: 更新编辑组件初始化（第 111 行附近）**

```csharp
if (!isNew && parameters.TryGetValue("Component", out MixedComponentDto component) && component != null)
{
    id = component.Id;
    parentId = component.ParentId;
    CodeNumber = component.CodeNumber;
    Name = component.Name;
    Concentration = component.Concentration;
    EffectiveConcentration = component.EffectiveConcentration;
    Ratio = component.Ratio;
}
```

**Step 4: 更新 Save 方法中的 DTO**

新建时：

```csharp
var dto = new CreateMixedComponentDto
{
    ParentId = parentId,
    CodeNumber = CodeNumber,
    Name = Name,
    Concentration = Concentration,
    EffectiveConcentration = EffectiveConcentration,
    Ratio = Ratio
};
```

更新时：

```csharp
var dto = new UpdateMixedComponentDto
{
    Id = id,
    CodeNumber = CodeNumber,
    Name = Name,
    Concentration = Concentration,
    EffectiveConcentration = EffectiveConcentration,
    Ratio = Ratio
};
```

**Step 5: 验证构建**

Run: `dotnet build SmallGreen.Desktop.Settings/SmallGreen.Desktop.Settings.csproj`
Expected: Build succeeded

---

## Task 17: 桌面端 - MixedComponentEditDialog.xaml 更新

**Files:**
- Modify: `SmallGreen.Desktop.Settings/Dialogs/MixedComponentEditDialog.xaml`

**Step 1: 新增输入框**

在浓度输入框（第 60-69 行）后添加：

```xml
<Grid Margin="0,0,0,12">
    <Grid.ColumnDefinitions>
        <ColumnDefinition Width="100"/>
        <ColumnDefinition Width="*"/>
    </Grid.ColumnDefinitions>
    <TextBlock Grid.Column="0" Text="有效浓度(g/L):" VerticalAlignment="Center"/>
    <TextBox Grid.Column="1"
             Text="{Binding EffectiveConcentration, UpdateSourceTrigger=PropertyChanged}"
             md:HintAssist.Hint="每升母液实际含有的助剂质量"/>
</Grid>
```

**Step 2: 验证构建**

Run: `dotnet build SmallGreen.Desktop.Settings/SmallGreen.Desktop.Settings.csproj`
Expected: Build succeeded

---

## Task 18: 整体构建验证

**Step 1: 构建整个解决方案**

Run: `dotnet build SmallGreen.sln`
Expected: Build succeeded

**Step 2: 提交代码**

```bash
git add .
git commit -m "feat: 新增助剂实际有效浓度字段，支持财务消耗量统计

- AssInfo/MixedDetail/PRCSDataDetail 新增 EffectiveConcentration 相关字段
- 更新 SubSystem 消耗计算逻辑，新增 EffectiveAssKg 计算
- 更新 DTO 和 API 服务
- 更新桌面端编辑界面"
```

---

## 数据迁移说明

SqlSugar CodeFirst 会自动在数据库中新增字段。现有数据的 `EffectiveConcentration` 默认值为 0，需要手动配置。

建议运行系统后，在桌面端为每种助剂配置实际有效浓度值。
