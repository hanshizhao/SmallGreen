using Prism.Commands;
using Prism.Events;
using Prism.Navigation;
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
        private readonly IContainerProvider container;
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
            this.container = container;
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

                // TODO: Subscribe to SignalR updates for each subsystem
                // foreach (var name in subSystem.SubSystemNames)
                // {
                //     await SubscribeToHubAsync(name);
                // }
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
                    container,
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
