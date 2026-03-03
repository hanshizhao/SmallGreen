# Chemical Bucket Monitor Implementation Plan

> **For Claude:** REQUIRED SUB-SKILL: Use superpowers:executing-plans to implement this plan task-by-task.

**Goal:** Create a WPF page for monitoring chemical bucket liquid levels with real-time updates via SignalR.

**Architecture:** Enhance existing ChemicalBucket custom control, create new View/ViewModel following Prism MVVM pattern, reuse bottom drawer mechanism for detail panel, implement SignalR subscription for real-time data.

**Tech Stack:** WPF, Prism.DryIoc, MaterialDesignThemes, SignalR, SqlSugar ORM

---

## Task 1: Create DTO Classes

**Files:**
- Create: `SmallGreen.Dto/Machine/ChemicalBucketDto.cs`
- Create: `SmallGreen.Dto/Machine/FormulaItemDto.cs`
- Create: `SmallGreen.Dto/Machine/UpdateConcentrationDto.cs`

**Step 1: Create ChemicalBucketDto**

```csharp
namespace SmallGreen.Dto.Machine
{
    public class ChemicalBucketDto
    {
        public long Id { get; set; }
        public string CodeNumber { get; set; } = null!;
        public float Level { get; set; }
        public float MaxCapacity { get; set; }
        public float ConcentrationRatio { get; set; }
        public long SubSystemId { get; set; }
        public string SubSystemName { get; set; } = null!;
        public DateTime LastCompleteTime { get; set; }
        public List<FormulaItemDto>? FormulaItems { get; set; }
    }
}
```

**Step 2: Create FormulaItemDto**

```csharp
namespace SmallGreen.Dto.Machine
{
    public class FormulaItemDto
    {
        public string AssName { get; set; } = null!;
        public float PlanVolume { get; set; }
        public float RealVolume { get; set; }
    }
}
```

**Step 3: Create UpdateConcentrationDto**

```csharp
namespace SmallGreen.Dto.Machine
{
    public class UpdateConcentrationDto
    {
        public long BucketId { get; set; }
        public float ConcentrationRatio { get; set; }
    }
}
```

**Step 4: Build to verify**

Run: `dotnet build SmallGreen.Dto/SmallGreen.Dto.csproj`
Expected: Build succeeded

---

## Task 2: Create SubSystemItem Model

**Files:**
- Create: `SmallGreen.Desktop.Settings/Models/SubSystemItem.cs`

**Step 1: Create SubSystemItem model**

```csharp
namespace SmallGreen.Desktop.Settings.Models
{
    public class SubSystemItem
    {
        public string DisplayName { get; set; } = null!;
        public string[] SubSystemNames { get; set; } = Array.Empty<string>();
        public bool IsShared { get; set; }
    }
}
```

**Step 2: Build to verify**

Run: `dotnet build SmallGreen.Desktop.Settings/SmallGreen.Desktop.Settings.csproj`
Expected: Build succeeded

---

## Task 3: Enhance ChemicalBucket Control

**Files:**
- Modify: `SmallGreen.Desktop.Settings/BlackControl/ChemicalBucket.cs`
- Modify: `SmallGreen.Desktop.Settings/Themes/Generic.xaml`

**Step 1: Add dependency properties to ChemicalBucket.cs**

Replace the entire file content with:

```csharp
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace SmallGreen.Desktop.Settings.BlackControl
{
    public class ChemicalBucket : Button
    {
        static ChemicalBucket()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ChemicalBucket), new FrameworkPropertyMetadata(typeof(ChemicalBucket)));
        }

        public static readonly DependencyProperty BucketCodeProperty =
            DependencyProperty.Register(nameof(BucketCode), typeof(string), typeof(ChemicalBucket), new PropertyMetadata(string.Empty));

        public static readonly DependencyProperty LevelValueProperty =
            DependencyProperty.Register(nameof(LevelValue), typeof(float), typeof(ChemicalBucket), new PropertyMetadata(0f, OnLevelChanged));

        public static readonly DependencyProperty MaxLevelProperty =
            DependencyProperty.Register(nameof(MaxLevel), typeof(float), typeof(ChemicalBucket), new PropertyMetadata(1000f, OnLevelChanged));

        public static readonly DependencyProperty IsSelectedProperty =
            DependencyProperty.Register(nameof(IsSelected), typeof(bool), typeof(ChemicalBucket), new PropertyMetadata(false));

        public static readonly DependencyProperty LevelColorProperty =
            DependencyProperty.Register(nameof(LevelColor), typeof(Brush), typeof(ChemicalBucket), new PropertyMetadata(Brushes.Blue));

        public string BucketCode
        {
            get => (string)GetValue(BucketCodeProperty);
            set => SetValue(BucketCodeProperty, value);
        }

        public float LevelValue
        {
            get => (float)GetValue(LevelValueProperty);
            set => SetValue(LevelValueProperty, value);
        }

        public float MaxLevel
        {
            get => (float)GetValue(MaxLevelProperty);
            set => SetValue(MaxLevelProperty, value);
        }

        public bool IsSelected
        {
            get => (bool)GetValue(IsSelectedProperty);
            set => SetValue(IsSelectedProperty, value);
        }

        public Brush LevelColor
        {
            get => (Brush)GetValue(LevelColorProperty);
            set => SetValue(LevelColorProperty, value);
        }

        private static void OnLevelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var bucket = (ChemicalBucket)d;
            bucket.UpdateLevelColor();
        }

        private void UpdateLevelColor()
        {
            var percentage = MaxLevel > 0 ? LevelValue / MaxLevel : 0;

            if (percentage < 0.2f)
                LevelColor = new SolidColorBrush(Color.FromRgb(244, 67, 54)); // Red
            else if (percentage < 0.5f)
                LevelColor = new SolidColorBrush(Color.FromRgb(255, 152, 0)); // Orange
            else if (percentage < 0.8f)
                LevelColor = new SolidColorBrush(Color.FromRgb(33, 150, 243)); // Blue
            else
                LevelColor = new SolidColorBrush(Color.FromRgb(76, 175, 80)); // Green
        }
    }
}
```

**Step 2: Update Generic.xaml with bucket style**

Replace the entire file content with:

```xml
<ResourceDictionary xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
                    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
                    xmlns:local="clr-namespace:SmallGreen.Desktop.Settings.BlackControl"
                    xmlns:md="http://materialdesigninxaml.net/winfx/xaml/themes">

    <Style x:Key="BucketBaseStyle" TargetType="{x:Type local:ChemicalBucket}">
        <Setter Property="Width" Value="100"/>
        <Setter Property="Height" Value="150"/>
        <Setter Property="Margin" Value="8"/>
        <Setter Property="Cursor" Value="Hand"/>
        <Setter Property="Template">
            <Setter.Value>
                <ControlTemplate TargetType="{x:Type local:ChemicalBucket}">
                    <Grid>
                        <VisualStateManager.VisualStateGroups>
                            <VisualStateGroup x:Name="SelectionStates">
                                <VisualState x:Name="Unselected"/>
                                <VisualState x:Name="Selected">
                                    <Storyboard>
                                        <DoubleAnimation Storyboard.TargetName="SelectionBorder"
                                                         Storyboard.TargetProperty="Opacity"
                                                         To="1" Duration="0:0:0.2"/>
                                    </Storyboard>
                                </VisualState>
                            </VisualStateGroup>
                        </VisualStateManager.VisualStateGroups>

                        <!-- Main container -->
                        <Border x:Name="MainBorder"
                                Background="{DynamicResource MaterialDesignPaper}"
                                BorderBrush="{DynamicResource MaterialDesignDivider}"
                                BorderThickness="1"
                                CornerRadius="8">
                            <Grid Margin="4">
                                <Grid.RowDefinitions>
                                    <RowDefinition Height="Auto"/>
                                    <RowDefinition Height="Auto"/>
                                    <RowDefinition Height="*"/>
                                    <RowDefinition Height="Auto"/>
                                </Grid.RowDefinitions>

                                <!-- Bucket top cap -->
                                <Border Grid.Row="0"
                                        Height="8"
                                        Background="{DynamicResource MaterialDesignDivider}"
                                        CornerRadius="4,4,0,0"
                                        Width="40"
                                        Margin="0,0,0,4"/>

                                <!-- Level value display -->
                                <TextBlock Grid.Row="1"
                                           Text="{Binding LevelValue, RelativeSource={RelativeSource TemplatedParent}, StringFormat={}{0:F0} L}"
                                           HorizontalAlignment="Center"
                                           FontWeight="Bold"
                                           FontSize="14"
                                           Foreground="{DynamicResource MaterialDesignBody}"
                                           Margin="0,4"/>

                                <!-- Liquid container -->
                                <Grid Grid.Row="2" Margin="4,0">
                                    <Border Background="{DynamicResource MaterialDesignDivider}"
                                            Opacity="0.3"
                                            CornerRadius="4"/>

                                    <!-- Liquid fill -->
                                    <Grid x:Name="LiquidContainer" VerticalAlignment="Bottom">
                                        <Grid.Height>
                                            <MultiBinding Converter="{StaticResource LevelToHeightConverter}">
                                                <Binding Path="LevelValue" RelativeSource="{RelativeSource TemplatedParent}"/>
                                                <Binding Path="MaxLevel" RelativeSource="{RelativeSource TemplatedParent}"/>
                                                <Binding Path="ActualHeight" RelativeSource="{RelativeSource TemplatedParent}"/>
                                            </MultiBinding>
                                        </Grid.Height>
                                        <Border Background="{Binding LevelColor, RelativeSource={RelativeSource TemplatedParent}}"
                                                CornerRadius="4"
                                                Opacity="0.7"/>
                                    </Grid>
                                </Grid>

                                <!-- Bucket code -->
                                <TextBlock Grid.Row="3"
                                           Text="{Binding BucketCode, RelativeSource={RelativeSource TemplatedParent}}"
                                           HorizontalAlignment="Center"
                                           FontSize="12"
                                           Foreground="{DynamicResource MaterialDesignBody}"
                                           Margin="0,4,0,2"/>
                            </Grid>
                        </Border>

                        <!-- Selection border -->
                        <Border x:Name="SelectionBorder"
                                BorderBrush="{DynamicResource PrimaryHueMidBrush}"
                                BorderThickness="3"
                                CornerRadius="10"
                                Opacity="0"
                                IsHitTestVisible="False"/>
                    </Grid>

                    <ControlTemplate.Triggers>
                        <Trigger Property="IsSelected" Value="True">
                            <Setter TargetName="SelectionBorder" Property="Opacity" Value="1"/>
                        </Trigger>
                        <Trigger Property="IsMouseOver" Value="True">
                            <Setter TargetName="MainBorder" Property="Effect">
                                <Setter.Value>
                                    <DropShadowEffect BlurRadius="8" ShadowDepth="2" Opacity="0.3"/>
                                </Setter.Value>
                            </Setter>
                        </Trigger>
                    </ControlTemplate.Triggers>
                </ControlTemplate>
            </Setter.Value>
        </Setter>
    </Style>

    <Style TargetType="{x:Type local:ChemicalBucket}" BasedOn="{StaticResource BucketBaseStyle}"/>
</ResourceDictionary>
```

**Step 3: Create LevelToHeightConverter**

Create file: `SmallGreen.Desktop.Settings/Converters/LevelToHeightConverter.cs`

```csharp
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace SmallGreen.Desktop.Settings.Converters
{
    public class LevelToHeightConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length >= 3 &&
                values[0] is float level &&
                values[1] is float maxLevel &&
                values[2] is double actualHeight)
            {
                if (maxLevel <= 0) return 0d;
                var percentage = Math.Min(level / maxLevel, 1.0);
                // Subtract some padding for the container
                var availableHeight = actualHeight - 60; // Account for header and footer
                return Math.Max(0, availableHeight * percentage);
            }
            return 0d;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
```

**Step 4: Register converter in App.xaml**

Add to App.xaml's ResourceDictionary:

```xml
<Application.Resources>
    <ResourceDictionary>
        <ResourceDictionary.MergedDictionaries>
            <!-- existing dictionaries -->
        </ResourceDictionary.MergedDictionaries>
        <converters:LevelToHeightConverter x:Key="LevelToHeightConverter"/>
    </ResourceDictionary>
</Application.Resources>
```

**Step 5: Build to verify**

Run: `dotnet build SmallGreen.Desktop.Settings/SmallGreen.Desktop.Settings.csproj`
Expected: Build succeeded

---

## Task 4: Create Service Interface and Implementation

**Files:**
- Create: `SmallGreen.Desktop.Settings/IServices/IChemicalBucketService.cs`
- Create: `SmallGreen.Desktop.Settings/Services/ChemicalBucketService.cs`

**Step 1: Create IChemicalBucketService interface**

```csharp
using SmallGreen.Dto.Base;
using SmallGreen.Dto.Machine;

namespace SmallGreen.Desktop.Settings.IServices
{
    public interface IChemicalBucketService
    {
        /// <summary>
        /// Get chemical buckets by subsystem names
        /// </summary>
        Task<ApiResponse<List<ChemicalBucketDto>>> GetBySubSystems(string[] subSystemNames);

        /// <summary>
        /// Update concentration ratio (save to DB and sync to PLC)
        /// </summary>
        Task<ApiResponse<bool>> UpdateConcentration(UpdateConcentrationDto dto);
    }
}
```

**Step 2: Create ChemicalBucketService implementation**

```csharp
using System.Net.Http.Json;
using SmallGreen.Desktop.Settings.Common;
using SmallGreen.Desktop.Settings.IServices;
using SmallGreen.Dto.Base;
using SmallGreen.Dto.Machine;

namespace SmallGreen.Desktop.Settings.Services
{
    public class ChemicalBucketService : IChemicalBucketService
    {
        private readonly HttpClient httpClient;

        public ChemicalBucketService()
        {
            httpClient = new HttpClient
            {
                BaseAddress = new Uri(GlobalParam.GetInstance().ApiBaseUrl)
            };
        }

        public async Task<ApiResponse<List<ChemicalBucketDto>>> GetBySubSystems(string[] subSystemNames)
        {
            var response = await httpClient.PostAsJsonAsync("api/ChemicalBucket/query", subSystemNames);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<ApiResponse<List<ChemicalBucketDto>>>()
                ?? new ApiResponse<List<ChemicalBucketDto>> { IsSuccess = false, Message = "Failed to parse response" };
        }

        public async Task<ApiResponse<bool>> UpdateConcentration(UpdateConcentrationDto dto)
        {
            var response = await httpClient.PutAsJsonAsync("api/ChemicalBucket/concentration", dto);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<ApiResponse<bool>>()
                ?? new ApiResponse<bool> { IsSuccess = false, Message = "Failed to parse response" };
        }
    }
}
```

**Step 3: Register service in App.xaml.cs**

Add to `RegisterTypes` method in App.xaml.cs:

```csharp
containerRegistry.Register<IChemicalBucketService, ChemicalBucketService>();
```

**Step 4: Build to verify**

Run: `dotnet build SmallGreen.Desktop.Settings/SmallGreen.Desktop.Settings.csproj`
Expected: Build succeeded

---

## Task 5: Create ChemicalBucketViewModel

**Files:**
- Create: `SmallGreen.Desktop.Settings/ViewModels/ChemicalBucketViewModel.cs`

**Step 1: Create ChemicalBucketViewModel**

```csharp
using MaterialDesignThemes.Wpf;
using Microsoft.AspNetCore.SignalR.Client;
using Prism.Commands;
using Prism.Events;
using Prism.Regions;
using SmallGreen.Desktop.Settings.Common;
using SmallGreen.Desktop.Settings.Common.Events;
using SmallGreen.Desktop.Settings.Extensions;
using SmallGreen.Desktop.Settings.IServices;
using SmallGreen.Desktop.Settings.Models;
using SmallGreen.Desktop.Settings.Views;
using SmallGreen.Dto.Machine;
using System.Collections.ObjectModel;

namespace SmallGreen.Desktop.Settings.ViewModels
{
    public class ChemicalBucketViewModel : NavigationViewModel
    {
        private readonly IChemicalBucketService chemicalBucketService;
        private readonly IEventAggregator eventAggregator;
        private readonly IDialogHostService dialogHostService;
        private readonly List<IDisposable> hubSubscriptions = new();

        public DelegateCommand LoadedCommand { get; }
        public DelegateCommand<SubSystemItem> SubSystemChangedCommand { get; }
        public DelegateCommand<ChemicalBucketDto> BucketClickCommand { get; }

        private ObservableCollection<SubSystemItem> subSystems = null!;
        public ObservableCollection<SubSystemItem> SubSystems
        {
            get => subSystems;
            set => SetProperty(ref subSystems, value);
        }

        private SubSystemItem? selectedSubSystem;
        public SubSystemItem? SelectedSubSystem
        {
            get => selectedSubSystem;
            set
            {
                SetProperty(ref selectedSubSystem, value);
                if (value != null)
                    _ = LoadBucketsAsync(value);
            }
        }

        private ObservableCollection<ChemicalBucketDto> buckets = new();
        public ObservableCollection<ChemicalBucketDto> Buckets
        {
            get => buckets;
            set => SetProperty(ref buckets, value);
        }

        private ChemicalBucketDto? selectedBucket;
        public ChemicalBucketDto? SelectedBucket
        {
            get => selectedBucket;
            set => SetProperty(ref selectedBucket, value);
        }

        public ChemicalBucketViewModel(IContainerProvider container) : base(container)
        {
            chemicalBucketService = container.Resolve<IChemicalBucketService>();
            eventAggregator = container.Resolve<IEventAggregator>();
            dialogHostService = container.Resolve<IDialogHostService>();

            LoadedCommand = new DelegateCommand(async () => await LoadedAsync());
            SubSystemChangedCommand = new DelegateCommand<SubSystemItem>(async s => await OnSubSystemChanged(s));
            BucketClickCommand = new DelegateCommand<ChemicalBucketDto>(OnBucketClick);

            InitializeSubSystems();
        }

        private void InitializeSubSystems()
        {
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
        }

        private async Task LoadedAsync()
        {
            Loading(true);
            if (SubSystems.Any())
            {
                SelectedSubSystem = SubSystems.First();
            }
            await Task.Delay(500);
            Loading(false);
        }

        private async Task OnSubSystemChanged(SubSystemItem? subSystem)
        {
            if (subSystem == null) return;
            await LoadBucketsAsync(subSystem);
        }

        private async Task LoadBucketsAsync(SubSystemItem subSystem)
        {
            Loading(true);

            // Dispose previous subscriptions
            foreach (var sub in hubSubscriptions)
                sub.Dispose();
            hubSubscriptions.Clear();

            try
            {
                var result = await chemicalBucketService.GetBySubSystems(subSystem.SubSystemNames);

                if (!result.IsSuccess || result.Content == null)
                {
                    await ShowErrorMessage(result.Message ?? "查询助剂桶信息失败");
                    Buckets = new ObservableCollection<ChemicalBucketDto>();
                    return;
                }

                Buckets = new ObservableCollection<ChemicalBucketDto>(result.Content);

                // Subscribe to SignalR updates for each subsystem
                foreach (var name in subSystem.SubSystemNames)
                {
                    await SubscribeToHubAsync(name);
                }
            }
            catch (Exception ex)
            {
                await ShowErrorMessage($"加载失败: {ex.Message}");
            }
            finally
            {
                Loading(false);
            }
        }

        private async Task SubscribeToHubAsync(string subSystemName)
        {
            // TODO: Implement SignalR subscription based on existing HubConnect pattern
            // This should follow the pattern used in other ViewModels
            await Task.CompletedTask;
        }

        private void OnBucketClick(ChemicalBucketDto bucket)
        {
            SelectedBucket = bucket;
            ShowBottomDrawer(bucket);
        }

        private void ShowBottomDrawer(ChemicalBucketDto bucket)
        {
            var view = new ChemicalBucketDetailView
            {
                DataContext = new ChemicalBucketDetailViewModel(
                    Container,
                    bucket,
                    async () => await RefreshBucketAsync(bucket.Id))
            };
            eventAggregator.ShowBottomDrawer(view);
        }

        private async Task RefreshBucketAsync(long bucketId)
        {
            if (SelectedSubSystem == null) return;

            var result = await chemicalBucketService.GetBySubSystems(SelectedSubSystem.SubSystemNames);
            if (result.IsSuccess && result.Content != null)
            {
                Buckets = new ObservableCollection<ChemicalBucketDto>(result.Content);
            }
        }

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            base.OnNavigatedTo(navigationContext);
            LoadedCommand.Execute();
        }

        public override void OnNavigatedFrom(NavigationContext navigationContext)
        {
            base.OnNavigatedFrom(navigationContext);
            foreach (var sub in hubSubscriptions)
                sub.Dispose();
            hubSubscriptions.Clear();
        }
    }
}
```

**Step 2: Build to verify**

Run: `dotnet build SmallGreen.Desktop.Settings/SmallGreen.Desktop.Settings.csproj`
Expected: Build succeeded (may have warnings about missing view)

---

## Task 6: Create ChemicalBucketDetailViewModel

**Files:**
- Create: `SmallGreen.Desktop.Settings/ViewModels/ChemicalBucketDetailViewModel.cs`

**Step 1: Create ChemicalBucketDetailViewModel**

```csharp
using Prism.Commands;
using Prism.Mvvm;
using SmallGreen.Desktop.Settings.Common;
using SmallGreen.Desktop.Settings.IServices;
using SmallGreen.Dto.Machine;
using System.Collections.ObjectModel;

namespace SmallGreen.Desktop.Settings.ViewModels
{
    public class ChemicalBucketDetailViewModel : BindableBase
    {
        private readonly IChemicalBucketService chemicalBucketService;
        private readonly Action? onSaveCallback;

        public DelegateCommand SaveCommand { get; }
        public DelegateCommand CancelCommand { get; }

        private ChemicalBucketDto bucket = null!;
        public ChemicalBucketDto Bucket
        {
            get => bucket;
            set => SetProperty(ref bucket, value);
        }

        private float concentrationRatio;
        public float ConcentrationRatio
        {
            get => concentrationRatio;
            set => SetProperty(ref concentrationRatio, value);
        }

        private ObservableCollection<FormulaItemDto> formulaList = new();
        public ObservableCollection<FormulaItemDto> FormulaList
        {
            get => formulaList;
            set => SetProperty(ref formulaList, value);
        }

        private bool isSaving;
        public bool IsSaving
        {
            get => isSaving;
            set => SetProperty(ref isSaving, value);
        }

        private float levelPercentage;
        public float LevelPercentage
        {
            get => levelPercentage;
            set => SetProperty(ref levelPercentage, value);
        }

        public ChemicalBucketDetailViewModel(IContainerProvider container, ChemicalBucketDto bucket, Action? onSaveCallback = null)
        {
            chemicalBucketService = container.Resolve<IChemicalBucketService>();
            this.onSaveCallback = onSaveCallback;

            SaveCommand = new DelegateCommand(async () => await SaveAsync(), CanSave);
            CancelCommand = new DelegateCommand(Cancel);

            Bucket = bucket;
            ConcentrationRatio = bucket.ConcentrationRatio;
            FormulaList = bucket.FormulaItems != null
                ? new ObservableCollection<FormulaItemDto>(bucket.FormulaItems)
                : new ObservableCollection<FormulaItemDto>();
            LevelPercentage = bucket.MaxCapacity > 0
                ? (bucket.Level / bucket.MaxCapacity) * 100
                : 0;
        }

        private bool CanSave()
        {
            return !IsSaving && ConcentrationRatio >= 0 && ConcentrationRatio <= 1;
        }

        private async Task SaveAsync()
        {
            IsSaving = true;
            SaveCommand.RaiseCanExecuteChanged();

            try
            {
                var dto = new UpdateConcentrationDto
                {
                    BucketId = Bucket.Id,
                    ConcentrationRatio = ConcentrationRatio
                };

                var result = await chemicalBucketService.UpdateConcentration(dto);

                if (result.IsSuccess)
                {
                    Bucket.ConcentrationRatio = ConcentrationRatio;
                    onSaveCallback?.Invoke();
                    // Close drawer - this would typically be done via event
                }
                else
                {
                    // Show error message
                }
            }
            finally
            {
                IsSaving = false;
                SaveCommand.RaiseCanExecuteChanged();
            }
        }

        private void Cancel()
        {
            // Reset values and close drawer
            ConcentrationRatio = Bucket.ConcentrationRatio;
        }
    }
}
```

**Step 2: Build to verify**

Run: `dotnet build SmallGreen.Desktop.Settings/SmallGreen.Desktop.Settings.csproj`
Expected: Build succeeded (may have warnings about missing view)

---

## Task 7: Create ChemicalBucketView

**Files:**
- Create: `SmallGreen.Desktop.Settings/Views/ChemicalBucketView.xaml`
- Create: `SmallGreen.Desktop.Settings/Views/ChemicalBucketView.xaml.cs`

**Step 1: Create ChemicalBucketView.xaml**

```xml
<UserControl x:Class="SmallGreen.Desktop.Settings.Views.ChemicalBucketView"
             xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             xmlns:d="http://schemas.microsoft.com/expression/blend/2008"
             xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006"
             xmlns:local="clr-namespace:SmallGreen.Desktop.Settings.Views"
             xmlns:blackControl="clr-namespace:SmallGreen.Desktop.Settings.BlackControl"
             xmlns:md="http://materialdesigninxaml.net/winfx/xaml/themes"
             mc:Ignorable="d"
             d:DesignHeight="600" d:DesignWidth="900">

    <Grid>
        <Grid.RowDefinitions>
            <RowDefinition Height="Auto"/>
            <RowDefinition Height="*"/>
        </Grid.RowDefinitions>

        <!-- Subsystem Tabs -->
        <ListBox Grid.Row="0"
                 ItemsSource="{Binding SubSystems}"
                 SelectedItem="{Binding SelectedSubSystem}"
                 Style="{StaticResource MaterialDesignToolToggleListBox}"
                 Margin="16,8">
            <ListBox.ItemsPanel>
                <ItemsPanelTemplate>
                    <StackPanel Orientation="Horizontal"/>
                </ItemsPanelTemplate>
            </ListBox.ItemsPanel>
            <ListBox.ItemTemplate>
                <DataTemplate>
                    <TextBlock Text="{Binding DisplayName}" Padding="16,8"/>
                </DataTemplate>
            </ListBox.ItemTemplate>
        </ListBox>

        <!-- Buckets Container -->
        <ScrollViewer Grid.Row="1" VerticalScrollBarVisibility="Auto">
            <ItemsControl ItemsSource="{Binding Buckets}" Margin="8">
                <ItemsControl.ItemsPanel>
                    <ItemsPanelTemplate>
                        <WrapPanel Orientation="Horizontal"/>
                    </ItemsPanelTemplate>
                </ItemsControl.ItemsPanel>
                <ItemsControl.ItemTemplate>
                    <DataTemplate>
                        <blackControl:ChemicalBucket
                            BucketCode="{Binding CodeNumber}"
                            LevelValue="{Binding Level}"
                            MaxLevel="{Binding MaxCapacity}"
                            IsSelected="{Binding IsSelected, Mode=TwoWay}"
                            Command="{Binding DataContext.BucketClickCommand, RelativeSource={RelativeSource AncestorType=UserControl}}"
                            CommandParameter="{Binding}"/>
                    </DataTemplate>
                </ItemsControl.ItemTemplate>
            </ItemsControl>
        </ScrollViewer>
    </Grid>
</UserControl>
```

**Step 2: Create ChemicalBucketView.xaml.cs**

```csharp
using System.Windows.Controls;

namespace SmallGreen.Desktop.Settings.Views
{
    public partial class ChemicalBucketView : UserControl
    {
        public ChemicalBucketView()
        {
            InitializeComponent();
        }
    }
}
```

**Step 3: Build to verify**

Run: `dotnet build SmallGreen.Desktop.Settings/SmallGreen.Desktop.Settings.csproj`
Expected: Build succeeded

---

## Task 8: Create ChemicalBucketDetailView

**Files:**
- Create: `SmallGreen.Desktop.Settings/Views/ChemicalBucketDetailView.xaml`
- Create: `SmallGreen.Desktop.Settings/Views/ChemicalBucketDetailView.xaml.cs`

**Step 1: Create ChemicalBucketDetailView.xaml**

```xml
<UserControl x:Class="SmallGreen.Desktop.Settings.Views.ChemicalBucketDetailView"
             xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             xmlns:d="http://schemas.microsoft.com/expression/blend/2008"
             xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006"
             xmlns:md="http://materialdesigninxaml.net/winfx/xaml/themes"
             mc:Ignorable="d"
             d:DesignHeight="300" d:DesignWidth="600"
             Height="280">

    <Grid Margin="24">
        <Grid.RowDefinitions>
            <RowDefinition Height="Auto"/>
            <RowDefinition Height="*"/>
        </Grid.RowDefinitions>

        <!-- Header -->
        <StackPanel Grid.Row="0" Orientation="Horizontal" Margin="0,0,0,16">
            <md:PackIcon Kind="ChemicalWeapon" Width="24" Height="24" VerticalAlignment="Center"
                         Foreground="{DynamicResource PrimaryHueMidBrush}"/>
            <TextBlock Text="{Binding Bucket.CodeNumber, StringFormat='桶详情 - {0}'}"
                       FontSize="18" FontWeight="Bold" Margin="8,0,0,0" VerticalAlignment="Center"/>
        </StackPanel>

        <!-- Content -->
        <Grid Grid.Row="1">
            <Grid.ColumnDefinitions>
                <ColumnDefinition Width="*"/>
                <ColumnDefinition Width="Auto"/>
                <ColumnDefinition Width="*"/>
            </Grid.ColumnDefinitions>

            <!-- Basic Info -->
            <StackPanel Grid.Column="0" Margin="0,0,16,0">
                <TextBlock Text="基本信息" FontWeight="Bold" Margin="0,0,0,8"/>

                <Grid>
                    <Grid.ColumnDefinitions>
                        <ColumnDefinition Width="Auto"/>
                        <ColumnDefinition Width="*"/>
                    </Grid.ColumnDefinitions>
                    <Grid.RowDefinitions>
                        <RowDefinition Height="Auto"/>
                        <RowDefinition Height="Auto"/>
                        <RowDefinition Height="Auto"/>
                        <RowDefinition Height="Auto"/>
                        <RowDefinition Height="Auto"/>
                    </Grid.RowDefinitions>

                    <TextBlock Grid.Row="0" Grid.Column="0" Text="当前液位:" Margin="0,4"/>
                    <TextBlock Grid.Row="0" Grid.Column="1" Text="{Binding Bucket.Level, StringFormat={}{0:F1} L}" Margin="8,4" FontWeight="Bold"/>

                    <TextBlock Grid.Row="1" Grid.Column="0" Text="桶容量:" Margin="0,4"/>
                    <TextBlock Grid.Row="1" Grid.Column="1" Text="{Binding Bucket.MaxCapacity, StringFormat={}{0:F0} L}" Margin="8,4"/>

                    <TextBlock Grid.Row="2" Grid.Column="0" Text="液位百分比:" Margin="0,4"/>
                    <TextBlock Grid.Row="2" Grid.Column="1" Text="{Binding LevelPercentage, StringFormat={}{0:F1}%}" Margin="8,4"/>

                    <TextBlock Grid.Row="3" Grid.Column="0" Text="上次配液:" Margin="0,4"/>
                    <TextBlock Grid.Row="3" Grid.Column="1" Text="{Binding Bucket.LastCompleteTime, StringFormat=yyyy-MM-dd HH:mm}" Margin="8,4"/>

                    <TextBlock Grid.Row="4" Grid.Column="0" Text="所属系统:" Margin="0,4"/>
                    <TextBlock Grid.Row="4" Grid.Column="1" Text="{Binding Bucket.SubSystemName}" Margin="8,4"/>
                </Grid>
            </StackPanel>

            <Separator Grid.Column="1" Style="{StaticResource MaterialDesignVerticalSeparator}" Margin="8,0"/>

            <!-- Concentration Settings -->
            <StackPanel Grid.Column="2" Margin="16,0,0,0">
                <TextBlock Text="浓度比例设置" FontWeight="Bold" Margin="0,0,0,8"/>

                <TextBlock Text="当前浓度比例:" Margin="0,8"/>
                <Slider Value="{Binding ConcentrationRatio, Mode=TwoWay}"
                        Minimum="0" Maximum="1" TickFrequency="0.1"
                        IsSnapToTickEnabled="True"
                        Margin="0,4"/>

                <TextBlock Text="{Binding ConcentrationRatio, StringFormat={}{0:P0}}"
                           HorizontalAlignment="Center" FontSize="16" FontWeight="Bold"
                           Foreground="{DynamicResource PrimaryHueMidBrush}"
                           Margin="0,8"/>

                <StackPanel Orientation="Horizontal" HorizontalAlignment="Right" Margin="0,16,0,0">
                    <Button Content="保存"
                            Command="{Binding SaveCommand}"
                            Style="{StaticResource MaterialDesignOutlinedButton}"
                            IsEnabled="{Binding IsSaving, Converter={StaticResource BooleanToInverseConverter}}"
                            Margin="0,0,8,0"/>
                    <Button Content="取消"
                            Command="{Binding CancelCommand}"
                            Style="{StaticResource MaterialDesignFlatButton}"/>
                </StackPanel>
            </StackPanel>
        </Grid>
    </Grid>
</UserControl>
```

**Step 2: Create ChemicalBucketDetailView.xaml.cs**

```csharp
using System.Windows.Controls;

namespace SmallGreen.Desktop.Settings.Views
{
    public partial class ChemicalBucketDetailView : UserControl
    {
        public ChemicalBucketDetailView()
        {
            InitializeComponent();
        }
    }
}
```

**Step 3: Add BooleanToInverseConverter**

Create file: `SmallGreen.Desktop.Settings/Converters/BooleanToInverseConverter.cs`

```csharp
using System;
using System.Globalization;
using System.Windows.Data;

namespace SmallGreen.Desktop.Settings.Converters
{
    public class BooleanToInverseConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is bool b && !b;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is bool b && !b;
        }
    }
}
```

**Step 4: Register view in App.xaml.cs**

Add to `RegisterTypes` method:

```csharp
containerRegistry.RegisterForNavigation<ChemicalBucketView>();
```

**Step 5: Build to verify**

Run: `dotnet build SmallGreen.Desktop.Settings/SmallGreen.Desktop.Settings.csproj`
Expected: Build succeeded

---

## Task 9: Update MainWindowViewModel Menu

**Files:**
- Modify: `SmallGreen.Desktop.Settings/ViewModels/MainWindowViewModel.cs`

**Step 1: Update menu configuration**

In `Configure()` method, change the "助剂桶液位信息" menu item:

```csharp
ListMenu = new List<ItemMenu>
{
    new ItemMenu("小绿系统实时状态", new List<SubItem>
    {
        new SubItem("助剂桶液位信息", nameof(ChemicalBucketView), false),  // Changed from ArthurView
        new SubItem("前处理配液信息", nameof(ArthurView), false),
        new SubItem("固色配液信息", nameof(ArthurView), false),
    }, PackIconKind.TrayArrowUp),
    // ... rest of menu
};
```

**Step 2: Build to verify**

Run: `dotnet build SmallGreen.Desktop.Settings/SmallGreen.Desktop.Settings.csproj`
Expected: Build succeeded

---

## Task 10: Create API Controller (Backend)

**Files:**
- Create: `SmallGreen.API/Controllers/ChemicalBucketController.cs`

**Step 1: Create ChemicalBucketController**

```csharp
using Microsoft.AspNetCore.Mvc;
using SmallGreen.Dto.Base;
using SmallGreen.Dto.Machine;
using SmallGreen.Entity.Machine;
using SqlSugar;

namespace SmallGreen.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChemicalBucketController : ControllerBase
    {
        private readonly ISqlSugarClient db;

        public ChemicalBucketController(ISqlSugarClient db)
        {
            this.db = db;
        }

        [HttpPost("query")]
        public async Task<ApiResponse<List<ChemicalBucketDto>>> Query([FromBody] string[] subSystemNames)
        {
            try
            {
                var result = new List<ChemicalBucketDto>();

                foreach (var name in subSystemNames)
                {
                    var subsystem = await db.Queryable<SubSystem>()
                        .FirstAsync(s => s.Name == name);

                    if (subsystem == null) continue;

                    var equipments = await db.Queryable<Equipment>()
                        .Where(e => e.SubSystemId == subsystem.Id)
                        .ToListAsync();

                    foreach (var equipment in equipments)
                    {
                        var bulks = await db.Queryable<Bulk>()
                            .Where(b => b.EquipmentId == equipment.Id)
                            .ToListAsync();

                        foreach (var bulk in bulks)
                        {
                            result.Add(new ChemicalBucketDto
                            {
                                Id = bulk.Id,
                                CodeNumber = bulk.CodeNumber,
                                Level = bulk.DataLevel?.GetCurrentValue() ?? 0,
                                MaxCapacity = 1000, // TODO: Get from config or equipment
                                ConcentrationRatio = 0.85f, // TODO: Add to entity if needed
                                SubSystemId = subsystem.Id,
                                SubSystemName = subsystem.Name,
                                LastCompleteTime = bulk.LastCompleteTime,
                                FormulaItems = ParseFormulaItems(bulk)
                            });
                        }
                    }
                }

                return new ApiResponse<List<ChemicalBucketDto>>
                {
                    IsSuccess = true,
                    Content = result
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<List<ChemicalBucketDto>>
                {
                    IsSuccess = false,
                    Message = ex.Message
                };
            }
        }

        [HttpPut("concentration")]
        public async Task<ApiResponse<bool>> UpdateConcentration([FromBody] UpdateConcentrationDto dto)
        {
            try
            {
                // TODO: Implement concentration update logic
                // 1. Save to database
                // 2. Sync to PLC via Dom pattern

                return new ApiResponse<bool>
                {
                    IsSuccess = true,
                    Content = true
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<bool>
                {
                    IsSuccess = false,
                    Message = ex.Message
                };
            }
        }

        private List<FormulaItemDto>? ParseFormulaItems(Bulk bulk)
        {
            // TODO: Parse formula array strings into structured data
            return null;
        }
    }
}
```

**Step 2: Build to verify**

Run: `dotnet build SmallGreen.API/SmallGreen.API.csproj`
Expected: Build succeeded

---

## Task 11: Final Build and Test

**Step 1: Build entire solution**

Run: `dotnet build SmallGreen.sln`
Expected: Build succeeded

**Step 2: Run desktop application**

Run: `dotnet run --project SmallGreen.Desktop.Settings/SmallGreen.Desktop.Settings.csproj`
Expected: Application starts, navigate to "助剂桶液位信息" menu

**Step 3: Verify functionality**

- [ ] Subsystem tabs display correctly
- [ ] Bucket controls render with proper styling
- [ ] Clicking a bucket opens bottom drawer
- [ ] Concentration ratio can be edited
- [ ] Save button triggers API call

---

## Notes for Implementation

1. **SignalR Integration**: The SignalR subscription needs to be implemented based on the existing HubConnect pattern in the codebase.

2. **Concentration Ratio Field**: The `ConcentrationRatio` field may need to be added to the `Bulk` entity if it doesn't exist.

3. **MaxCapacity Configuration**: The bucket max capacity should be configurable, possibly stored in the database.

4. **Formula Parsing**: The formula items parsing logic needs to be implemented based on the actual data format in `DataFomulaArray`.

5. **PLC Sync**: The concentration update needs to implement the actual PLC synchronization using the Dom pattern.
