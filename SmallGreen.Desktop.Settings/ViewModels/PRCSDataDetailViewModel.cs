using SmallGreen.Desktop.Settings.IServices;
using SmallGreen.Dto.Data;

namespace SmallGreen.Desktop.Settings.ViewModels
{

    public class PRCSDataDetailViewModel : DialogBaseViewModel
    {
        private readonly IPRCSDataService prcsDataService;
        private PRCSDataDto prcsDataDto = null!;

        public PRCSDataDto PRCSDataDto
        {
            get { return prcsDataDto; }
            set { SetProperty(ref prcsDataDto, value); }
        }

        private List<PRCSDataDetailDto> listDetail;
        public List<PRCSDataDetailDto> ListDetail
        {
            get { return listDetail; }
            set { SetProperty(ref listDetail, value); }
        }


        public PRCSDataDetailViewModel(IContainer container) : base(container)
        {
            prcsDataService = container.Resolve<IPRCSDataService>();
            listDetail = new List<PRCSDataDetailDto>();
        }

        public override async void OnDialogOpend(IDialogParameters parameters)
        {
            base.OnDialogOpend(parameters);

            if (!parameters.ContainsKey("PRCSData")) throw new Exception("必须传入参数[PRCSData]");

            PRCSDataDto = parameters.GetValue<PRCSDataDto>("PRCSData");

            var result = await prcsDataService.GetDetail(prcsDataDto.Id);

            if (!result.IsSuccess)
            {
                await ShowErrorMessage(result.Message);
                return;
            }

            if (result.Content == null)
            {
                await ShowErrorMessage("服务器返回正确的空结果");
                return;
            }

            ListDetail = result.Content;


        }
    }
}
