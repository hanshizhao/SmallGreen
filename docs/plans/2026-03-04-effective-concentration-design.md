# 助剂实际有效浓度功能设计

## 背景

在印染过程中，助剂需要加水稀释成母液后使用。流量计采集的是母液体积，财务需要统计的是母液中实际含有的助剂质量（不含水）。

### 问题示例

```
100KG 净棉素 + 400L 水 = 500L 母液
- PLC Concentration = 1000 g/L（用于 PLC 控制流量计）
- 实际有效浓度 = 100KG / 500L = 200 g/L（每升母液真正含有的助剂质量）

财务需要按 200 g/L 计算消耗量，而非 1000 g/L
```

## 解决方案

新增"实际有效浓度"字段，用于财务消耗计算，保留原有字段与 PLC 对应。

## 字段设计

### AssInfo 表（助剂信息）

| 字段 | 类型 | 含义 | 示例值 |
|------|------|------|--------|
| Concentration | double | 与 PLC 对应的浓度（g/L） | 1000 |
| **EffectiveConcentration** | double | 实际有效浓度（g/L），用于财务计算 | 200 |

### MixedDetail 表（混合助剂组分）

| 字段 | 类型 | 含义 | 示例值 |
|------|------|------|--------|
| Concentration | double | 与 PLC 对应的浓度（g/L） | 1000 |
| **EffectiveConcentration** | double | 实际有效浓度（g/L） | 200 |

### PRCSDataDetail 表（消耗记录）

| 字段 | 类型 | 含义 |
|------|------|------|
| GramsPerLiter | double | PLC 浓度（g/L） |
| AssKg | double | 基于 PLC 浓度计算的消耗量（kg） |
| **EffectiveGramsPerLiter** | double | 实际有效浓度（g/L） |
| **EffectiveAssKg** | double | 财务消耗量（kg） |

## 计算公式

```
AssKg = realLitre × Concentration / 1000
EffectiveAssKg = realLitre × EffectiveConcentration / 1000
```

## 影响范围

### 实体层
- `SmallGreen.Entity/Data/AssInfo.cs`
- `SmallGreen.Entity/Data/MixedDetail.cs`
- `SmallGreen.Entity/Data/PRCSDataDetail.cs`

### DTO 层
- `SmallGreen.Dto/Data/AssInfoDto.cs`
- `SmallGreen.Dto/Data/MixedDetailDto.cs`
- `SmallGreen.Dto/Data/PRCSDataDetailDto.cs`

### API 层
- `SmallGreen.API/Service/AssInfoService.cs`

### 业务逻辑层
- `SmallGreen.Entity/Machine/SubSystem.cs`

### 桌面端
- `SmallGreen.Desktop.Settings/Dialogs/AssBucketEditDialog.xaml`
- `SmallGreen.Desktop.Settings/Dialogs/AssBucketEditDialogViewModel.cs`
- `SmallGreen.Desktop.Settings/Dialogs/MixedComponentEditDialog.xaml`
- `SmallGreen.Desktop.Settings/Dialogs/MixedComponentEditDialogViewModel.cs`

## 数据迁移

现有数据的 `EffectiveConcentration` 默认值处理：
- 方案 A：默认等于 `Concentration`（假设现有数据都是 1:1 未稀释）
- 方案 B：默认为 0，需要手动配置

建议采用方案 A，减少数据迁移工作量。
