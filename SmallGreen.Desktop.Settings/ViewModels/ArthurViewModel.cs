namespace SmallGreen.Desktop.Settings.ViewModels
{
    public class ArthurViewModel : NavigationViewModel
    {
        //private readonly IChemicalBucketService chemicalBucketService;
        //public DelegateCommand LoadedCommand { get; }
        //private ObservableCollection<ChemicalBucketDto> listBuckets = null!;
        //public ObservableCollection<ChemicalBucketDto> ListBuckets
        //{
        //    get { return listBuckets; }
        //    set { SetProperty(ref listBuckets, value); }
        //}

        public ArthurViewModel(IContainerProvider container) : base(container)
        {
            //chemicalBucketService = container.Resolve<IChemicalBucketService>();
            //LoadedCommand = new DelegateCommand(Loaded);
        }

        ///// <summary>
        ///// 界面加载完毕事件
        ///// </summary>
        //private async void Loaded()
        //{
        //    Loading(true);

        //    var result = await chemicalBucketService.GetList();

        //    if (!result.IsSuccess)
        //    {
        //        await ShowErrorMessage(result.Message);
        //        return;
        //    }

        //    if (result.Content == null)
        //    {
        //        await Application.Current.Dispatcher.Invoke(async () =>
        //        {
        //            await ShowErrorMessage("查询助剂储罐信息失败：服务器返回正确的空结果");
        //        });

        //        return;
        //    }

        //    ListBuckets = new ObservableCollection<ChemicalBucketDto>(result.Content);


        //    await HubConnect<List<ChemicalBucketDto>>(SubSystemName.Arthur, SubSystemName.Arthur + "_ChemicalBucket", listDto =>
        //    {
        //        ListBuckets = new ObservableCollection<ChemicalBucketDto>(listDto);
        //    });
        //    await Task.Delay(500);
        //    Loading(false);
        //}


    }
}
