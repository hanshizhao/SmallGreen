using Prism.Commands;
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

        private float concentration;
        /// <summary>
        /// 助剂浓度，单位：克/升
        /// </summary>
        public float Concentration
        {
            get => concentration;
            set
            {
                SetProperty(ref concentration, value);
                SaveCommand.RaiseCanExecuteChanged();
            }
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
            set
            {
                SetProperty(ref isSaving, value);
                SaveCommand.RaiseCanExecuteChanged();
            }
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
            Concentration = bucket.Concentration;
            FormulaList = bucket.FormulaItems != null
                ? new ObservableCollection<FormulaItemDto>(bucket.FormulaItems)
                : new ObservableCollection<FormulaItemDto>();
            LevelPercentage = bucket.MaxCapacity > 0
                ? (bucket.Level / bucket.MaxCapacity) * 100
                : 0;
        }

        private bool CanSave()
        {
            return !IsSaving && Concentration >= 0;
        }

        private async Task SaveAsync()
        {
            IsSaving = true;

            try
            {
                var dto = new UpdateConcentrationDto
                {
                    BucketId = Bucket.Id,
                    Concentration = Concentration
                };

                var result = await chemicalBucketService.UpdateConcentration(dto);

                if (result.IsSuccess)
                {
                    Bucket.Concentration = Concentration;
                    onSaveCallback?.Invoke();
                }
                else
                {
                    // TODO: Show error message
                }
            }
            finally
            {
                IsSaving = false;
            }
        }

        private void Cancel()
        {
            Concentration = Bucket.Concentration;
        }
    }
}
