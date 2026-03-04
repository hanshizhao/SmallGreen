using MaterialDesignThemes.Wpf;
using Prism.Commands;
using SmallGreen.Desktop.Settings.Common;
using SmallGreen.Desktop.Settings.IServices;
using SmallGreen.Desktop.Settings.ViewModels;
using SmallGreen.Desktop.Settings.Views;
using SmallGreen.Dto.Machine;
using System.Collections.ObjectModel;
using System.Windows;

namespace SmallGreen.Desktop.Settings.Dialogs
{
    public class MixedComponentsListDialogViewModel : BindableBase, IDialogHostAware
    {
        private readonly IDialogHostService dialogHostService;
        private readonly IAssBucketService assBucketService;

        private long parentId;
        private string? bucketName;
        private ObservableCollection<MixedComponentDto> components = new();
        private MixedComponentDto? selectedComponent;
        private string? errorMessage;

        public string? BucketName
        {
            get => bucketName;
            set => SetProperty(ref bucketName, value);
        }

        public ObservableCollection<MixedComponentDto> Components
        {
            get => components;
            private set
            {
                components = value ?? new ObservableCollection<MixedComponentDto>();
                RaisePropertyChanged(nameof(Components));
                RaisePropertyChanged(nameof(IsEmpty));
            }
        }

        public MixedComponentDto? SelectedComponent
        {
            get => selectedComponent;
            set => SetProperty(ref selectedComponent, value);
        }

        public string? ErrorMessage
        {
            get => errorMessage;
            set
            {
                SetProperty(ref errorMessage, value);
                RaisePropertyChanged(nameof(HasError));
            }
        }

        public bool HasError => !string.IsNullOrEmpty(ErrorMessage);

        public bool IsEmpty => Components.Count == 0 && !HasError;

        public string HostName { get; set; } = "Root";

        public DelegateCommand CloseCommand { get; }
        public DelegateCommand CloseWindowCommand { get; }
        public DelegateCommand AddComponentCommand { get; }
        public DelegateCommand<MixedComponentDto?> EditComponentCommand { get; }
        public DelegateCommand<MixedComponentDto?> DeleteComponentCommand { get; }

        public MixedComponentsListDialogViewModel(
            IDialogHostService dialogHostService,
            IAssBucketService assBucketService)
        {
            this.dialogHostService = dialogHostService;
            this.assBucketService = assBucketService;

            CloseCommand = new DelegateCommand(Close);
            AddComponentCommand = new DelegateCommand(async () => await AddComponent());
            EditComponentCommand = new DelegateCommand<MixedComponentDto?>(async c => await EditComponent(c));
            DeleteComponentCommand = new DelegateCommand<MixedComponentDto?>(async c => await DeleteComponent(c));
        }

        public void OnDialogOpend(IDialogParameters parameters)
        {
            if (parameters == null) return;

            ErrorMessage = null;

            if (parameters.TryGetValue("Bucket", out AssBucketDto bucket) && bucket != null)
            {
                parentId = bucket.Id;
                BucketName = bucket.Name;

                if (bucket.MixedComponents != null)
                {
                    Components = new ObservableCollection<MixedComponentDto>(bucket.MixedComponents);
                }
                else
                {
                    Components = new ObservableCollection<MixedComponentDto>();
                }

                RaisePropertyChanged(nameof(IsEmpty));
            }
        }

        private async Task AddComponent()
        {
            var param = new DialogParameters
            {
                { "ParentId", parentId },
                { "IsNew", true }
            };

            var dialogResult = await dialogHostService.ShowDialog(nameof(MixedComponentEditDialog), param, "MixedComponentsListDialogHost");

            if (dialogResult.Result == ButtonResult.OK)
            {
                await RefreshComponents();
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

            var dialogResult = await dialogHostService.ShowDialog(nameof(MixedComponentEditDialog), param, "MixedComponentsListDialogHost");

            if (dialogResult.Result == ButtonResult.OK)
            {
                await RefreshComponents();
            }
        }

        private async Task DeleteComponent(MixedComponentDto? component)
        {
            if (component == null) return;

            var result = await dialogHostService.ShowDialog(nameof(ConfirmOperateView), null, "MixedComponentsListDialogHost");

            if (result.Result != ButtonResult.Yes) return;

            var deleteResult = await assBucketService.DeleteMixedComponent(component.Id);

            if (!deleteResult.IsSuccess)
            {
                ErrorMessage = deleteResult.Message;
                return;
            }

            await RefreshComponents();
        }

        private async Task RefreshComponents()
        {
            var result = await assBucketService.GetBucketById(parentId);

            if (result.IsSuccess && result.Content != null)
            {
                if (result.Content.MixedComponents != null)
                {
                    Components = new ObservableCollection<MixedComponentDto>(result.Content.MixedComponents);
                }
                else
                {
                    Components.Clear();
                }

                ErrorMessage = null;
                RaisePropertyChanged(nameof(IsEmpty));
            }
            else
            {
                ErrorMessage = result.Message ?? "刷新数据失败";
            }
        }

        private void Close()
        {
            DialogHost.Close(HostName, new DialogResult(ButtonResult.OK));
        }
    }
}
