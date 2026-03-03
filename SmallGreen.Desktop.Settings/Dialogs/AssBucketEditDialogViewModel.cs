using MaterialDesignThemes.Wpf;
using Prism.Commands;
using SmallGreen.Desktop.Settings.Common;
using SmallGreen.Desktop.Settings.IServices;
using SmallGreen.Dto.Machine;

namespace SmallGreen.Desktop.Settings.Dialogs
{
    public class AssBucketEditDialogViewModel : BindableBase, IDialogHostAware
    {
        private readonly IAssBucketService assBucketService;

        private long id;
        private string? subSystemName;
        private int sequence;
        private string? codeNumber;
        private string? name;
        private double concentration;
        private bool isMixed;
        private string? errorMessage;

        public string? SubSystemName
        {
            get => subSystemName;
            set => SetProperty(ref subSystemName, value);
        }

        public int Sequence
        {
            get => sequence;
            set => SetProperty(ref sequence, value);
        }

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

        public bool IsMixed
        {
            get => isMixed;
            set => SetProperty(ref isMixed, value);
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

        public bool HasChanges { get; private set; }

        public string HostName { get; set; } = "Root";

        public DelegateCommand SaveCommand { get; }
        public DelegateCommand CancelCommand { get; }
        public DelegateCommand CloseWindowCommand { get; }

        public AssBucketEditDialogViewModel(IAssBucketService assBucketService)
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

            if (parameters.TryGetValue("Bucket", out AssBucketDto bucket) && bucket != null)
            {
                id = bucket.Id;
                SubSystemName = bucket.SubSystemName;
                Sequence = bucket.Sequence;
                CodeNumber = bucket.CodeNumber;
                Name = bucket.Name;
                Concentration = bucket.Concentration;
                IsMixed = bucket.IsMixed;
                HasChanges = false;
            }
        }

        private async Task Save()
        {
            if (string.IsNullOrWhiteSpace(CodeNumber))
            {
                await ShowError("编号不能为空");
                return;
            }

            if (string.IsNullOrWhiteSpace(Name))
            {
                await ShowError("名称不能为空");
                return;
            }

            var dto = new UpdateAssBucketDto
            {
                Id = id,
                CodeNumber = CodeNumber,
                Name = Name,
                Concentration = Concentration,
                IsMixed = IsMixed
            };

            var result = await assBucketService.UpdateBucket(dto);

            if (!result.IsSuccess)
            {
                await ShowError(result.Message);
                return;
            }

            HasChanges = true;
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
