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
        public DelegateCommand<AssBucketDto?> ViewComponentsCommand { get; }

        public AssBucketManagementViewModel(IContainerProvider container) : base(container)
        {
            assBucketService = container.Resolve<IAssBucketService>();

            RefreshCommand = new DelegateCommand(async () => await LoadData());
            EditBucketCommand = new DelegateCommand<AssBucketDto?>(async b => await EditBucket(b));
            ViewComponentsCommand = new DelegateCommand<AssBucketDto?>(async b => await ViewComponents(b));
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

        private async Task ViewComponents(AssBucketDto? bucket)
        {
            if (bucket == null) return;
            if (!bucket.IsMixed)
            {
                await ShowErrorMessage("该助剂桶不是混合助剂");
                return;
            }

            var param = new DialogParameters
            {
                { "Bucket", bucket }
            };

            await GetDialogHostService().ShowDialog(nameof(MixedComponentsListDialog), param);

            // 刷新数据以获取最新的组分信息
            await LoadData();
        }
    }
}
