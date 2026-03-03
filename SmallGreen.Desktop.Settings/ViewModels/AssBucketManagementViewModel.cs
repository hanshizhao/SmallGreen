using Prism.Commands;
using SmallGreen.Desktop.Settings.Dialogs;
using SmallGreen.Desktop.Settings.IServices;
using SmallGreen.Dto.Machine;
using System.Collections.ObjectModel;

namespace SmallGreen.Desktop.Settings.ViewModels
{
    public class AssBucketManagementViewModel : NavigationViewModel
    {
        private readonly IAssBucketService assBucketService;

        private ObservableCollection<AssBucketDto> buckets = new();
        private AssBucketDto? selectedBucket;

        public ObservableCollection<AssBucketDto> Buckets
        {
            get => buckets;
            set => SetProperty(ref buckets, value);
        }

        public AssBucketDto? SelectedBucket
        {
            get => selectedBucket;
            set => SetProperty(ref selectedBucket, value);
        }

        public DelegateCommand RefreshCommand { get; }
        public DelegateCommand<AssBucketDto?> EditBucketCommand { get; }
        public DelegateCommand<AssBucketDto?> AddComponentCommand { get; }
        public DelegateCommand<MixedComponentDto?> EditComponentCommand { get; }
        public DelegateCommand<MixedComponentDto?> DeleteComponentCommand { get; }

        public AssBucketManagementViewModel(IContainerProvider container) : base(container)
        {
            assBucketService = container.Resolve<IAssBucketService>();

            RefreshCommand = new DelegateCommand(async () => await LoadData());
            EditBucketCommand = new DelegateCommand<AssBucketDto?>(async b => await EditBucket(b));
            AddComponentCommand = new DelegateCommand<AssBucketDto?>(async b => await AddComponent(b));
            EditComponentCommand = new DelegateCommand<MixedComponentDto?>(async c => await EditComponent(c));
            DeleteComponentCommand = new DelegateCommand<MixedComponentDto?>(async c => await DeleteComponent(c));
        }

        public override async void OnNavigatedTo(NavigationContext navigationContext)
        {
            base.OnNavigatedTo(navigationContext);
            await LoadData();
        }

        private async Task LoadData()
        {
            Loading(true);
            var result = await assBucketService.GetBuckets();
            Loading(false);

            if (!result.IsSuccess)
            {
                await ShowErrorMessage(result.Message);
                return;
            }

            if (result.Content == null)
            {
                await ShowErrorMessage("获取数据失败：服务器返回空结果");
                return;
            }

            Buckets = new ObservableCollection<AssBucketDto>(result.Content);
        }

        private async Task EditBucket(AssBucketDto? bucket)
        {
            if (bucket == null) return;

            var param = new DialogParameters
            {
                { "Bucket", bucket }
            };

            var dialogResult = await GetDialogHostService().ShowDialog(nameof(AssBucketEditDialog), param);

            if (dialogResult.Result == ButtonResult.OK)
            {
                await LoadData();
            }
        }

        private async Task AddComponent(AssBucketDto? bucket)
        {
            if (bucket == null) return;
            if (!bucket.IsMixed)
            {
                await ShowErrorMessage("该助剂桶不是混合助剂，无法添加组分");
                return;
            }

            var param = new DialogParameters
            {
                { "ParentId", bucket.Id },
                { "IsNew", true }
            };

            var dialogResult = await GetDialogHostService().ShowDialog(nameof(MixedComponentEditDialog), param);

            if (dialogResult.Result == ButtonResult.OK)
            {
                await LoadData();
            }
        }

        private async Task EditComponent(MixedComponentDto? component)
        {
            if (component == null) return;

            var param = new DialogParameters
            {
                { "Component", component },
                { "IsNew", false }
            };

            var dialogResult = await GetDialogHostService().ShowDialog(nameof(MixedComponentEditDialog), param);

            if (dialogResult.Result == ButtonResult.OK)
            {
                await LoadData();
            }
        }

        private async Task DeleteComponent(MixedComponentDto? component)
        {
            if (component == null) return;

            var confirm = await ShowConfirmOperateView("确定要删除该混合组分吗？");
            if (confirm == null || confirm.Result != ButtonResult.Yes) return;

            Loading(true);
            var result = await assBucketService.DeleteMixedComponent(component.Id);
            Loading(false);

            if (!result.IsSuccess)
            {
                await ShowErrorMessage(result.Message);
                return;
            }

            await ShowSuccessMessage("删除成功");
            await LoadData();
        }
    }
}
