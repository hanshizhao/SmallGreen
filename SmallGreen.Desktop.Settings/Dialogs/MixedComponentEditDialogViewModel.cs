using MaterialDesignThemes.Wpf;
using Prism.Commands;
using SmallGreen.Desktop.Settings.Common;
using SmallGreen.Desktop.Settings.IServices;
using SmallGreen.Dto.Base;
using SmallGreen.Dto.Machine;

namespace SmallGreen.Desktop.Settings.Dialogs
{
    public class MixedComponentEditDialogViewModel : BindableBase, IDialogHostAware
    {
        private readonly IAssBucketService assBucketService;

        private bool isNew;
        private long id;
        private long parentId;
        private string? codeNumber;
        private string? name;
        private double concentration;
        private double ratio;
        private string title = "新增混合助剂";
        private string? errorMessage;

        public string Title
        {
            get => title;
            set => SetProperty(ref title, value);
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

        public string? CodeNumber
        {
            get => codeNumber;
            set => SetProperty(ref codeNumber, value);
        }

        public string? Name
        {
            get => name;
            set => SetProperty(ref name, value);
        }

        public double Concentration
        {
            get => concentration;
            set => SetProperty(ref concentration, value);
        }

        public double Ratio
        {
            get => ratio;
            set => SetProperty(ref ratio, value);
        }

        public string HostName { get; set; } = "Root";

        public DelegateCommand SaveCommand { get; }
        public DelegateCommand CancelCommand { get; }
        public DelegateCommand CloseWindowCommand { get; }

        public MixedComponentEditDialogViewModel(IAssBucketService assBucketService)
        {
            this.assBucketService = assBucketService;

            SaveCommand = new DelegateCommand(async () => await Save());
            CancelCommand = new DelegateCommand(Cancel);
            CloseWindowCommand = new DelegateCommand(Cancel);
        }

        public void OnDialogOpend(IDialogParameters parameters)
        {
            if (parameters == null) return;

            ErrorMessage = null;

            // Check if new or edit
            if (parameters.TryGetValue("IsNew", out bool isNewValue))
            {
                isNew = isNewValue;
                Title = isNew ? "新增混合助剂" : "编辑混合助剂";
            }

            // For new component
            if (isNew && parameters.TryGetValue("ParentId", out long parentIdValue))
            {
                parentId = parentIdValue;
                CodeNumber = string.Empty;
                Name = string.Empty;
                Concentration = 0;
                Ratio = 1;
            }

            // For edit component
            if (!isNew && parameters.TryGetValue("Component", out MixedComponentDto component) && component != null)
            {
                id = component.Id;
                parentId = component.ParentId;
                CodeNumber = component.CodeNumber;
                Name = component.Name;
                Concentration = component.Concentration;
                Ratio = component.Ratio;
            }
        }

        private async Task Save()
        {
            if (string.IsNullOrWhiteSpace(CodeNumber))
            {
                await ShowError("助剂编号不能为空");
                return;
            }

            if (string.IsNullOrWhiteSpace(Name))
            {
                await ShowError("助剂名称不能为空");
                return;
            }

            if (Ratio <= 0)
            {
                await ShowError("混合比例必须大于0");
                return;
            }

            ApiResponse<MixedComponentDto> result;

            if (isNew)
            {
                var dto = new CreateMixedComponentDto
                {
                    ParentId = parentId,
                    CodeNumber = CodeNumber,
                    Name = Name,
                    Concentration = Concentration,
                    Ratio = Ratio
                };
                result = await assBucketService.CreateMixedComponent(dto);
            }
            else
            {
                var dto = new UpdateMixedComponentDto
                {
                    Id = id,
                    CodeNumber = CodeNumber,
                    Name = Name,
                    Concentration = Concentration,
                    Ratio = Ratio
                };
                result = await assBucketService.UpdateMixedComponent(dto);
            }

            if (!result.IsSuccess)
            {
                await ShowError(result.Message);
                return;
            }

            DialogHost.Close(HostName, new DialogResult(ButtonResult.OK));
        }

        private void Cancel()
        {
            DialogHost.Close(HostName, new DialogResult(ButtonResult.Ignore));
        }

        private Task ShowError(string message)
        {
            ErrorMessage = message;
            return Task.CompletedTask;
        }
    }
}
