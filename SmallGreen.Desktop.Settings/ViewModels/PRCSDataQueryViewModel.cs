using Microsoft.Win32;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.XSSF.UserModel;
using SmallGreen.Desktop.Settings.IServices;
using SmallGreen.Desktop.Settings.Views;
using SmallGreen.Dto.Data;
using System.IO;

namespace SmallGreen.Desktop.Settings.ViewModels
{
    public class PRCSDataQueryViewModel : NavigationViewModel
    {
        private readonly IPRCSDataService prcsDataService;

        private DateTime startDateTime;
        private DateTime endDateTime;
        private PageInfo<PRCSDataDto> pageInfo;

        public PageInfo<PRCSDataDto> PageInfo
        {
            get { return pageInfo; }
            set { SetProperty(ref pageInfo, value); }
        }
        public DateTime StartDateTime
        {
            get { return startDateTime; }
            set { SetProperty(ref startDateTime, value); }
        }
        public DateTime EndDateTime
        {
            get { return endDateTime; }
            set { SetProperty(ref endDateTime, value); }
        }

        public DelegateCommand QueryCommand { get; }
        public DelegateCommand PageUpCommand { get; }
        public DelegateCommand PageDownCommand { get; }
        public DelegateCommand ExportToExcelCommand { get; }
        public DelegateCommand<PRCSDataDto?> ShowDetailCommand { get; }

        public PRCSDataQueryViewModel(IContainerProvider container) : base(container)
        {
            prcsDataService = container.Resolve<IPRCSDataService>();

            QueryCommand = new DelegateCommand(Query);
            PageUpCommand = new DelegateCommand(PageUp);
            PageDownCommand = new DelegateCommand(PageDown);
            ShowDetailCommand = new DelegateCommand<PRCSDataDto?>(ShowDetail);
            ExportToExcelCommand = new DelegateCommand(ExportToExcel);

            pageInfo = new PageInfo<PRCSDataDto>
            {
                PageNumber = 1,
                PageSize = 20,
                TotalItemCount = 0
            };

            startDateTime = DateTime.Now.AddDays(-1);
            endDateTime = DateTime.Now;
        }

        private async void ShowDetail(PRCSDataDto? dto)
        {
            if (dto == null) return;
            DialogParameters param = new DialogParameters
            {
                { "Title", dto.CardNo ?? "手动配料"},
                { "PRCSData", dto}
            };

            await ShowDialog(nameof(PRCSDataDetailView), param);
        }

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            base.OnNavigatedTo(navigationContext);

            InitPage(1);
        }

        private void Query()
        {
            InitPage(pageInfo.PageNumber);
        }

        private void PageDown()
        {
            InitPage(++pageInfo.PageNumber);
        }

        private void PageUp()
        {
            InitPage(--pageInfo.PageNumber);
        }

        private async void InitPage(int pageNumber)
        {
            var start = Convert.ToDateTime(startDateTime.ToString("D").ToString());
            var end = Convert.ToDateTime(endDateTime.AddDays(1).ToString("D").ToString()).AddSeconds(-1);

            var days = (endDateTime - startDateTime).TotalDays;
            if (days < 0)
            {
                await ShowErrorMessage("起始日期必须小于结束日期");
                return;
            }
            if (days > 31)
            {
                await ShowErrorMessage("查询日期范围不允许超过31天");
                return;
            }


            Loading(true);
            var result = await prcsDataService.GetListByPage(pageNumber, pageInfo.PageSize, start, end);
            Loading(false);


            if (!result.IsSuccess)
            {
                await ShowErrorMessage(result.Message);
                return;
            }

            if (result.Content == null)
            {
                await ShowErrorMessage("数据库返回正确的空结果");
                return;
            }

            PageInfo = result.Content;
        }

        private async void ExportToExcel()
        {
            var start = Convert.ToDateTime(startDateTime.ToString("D").ToString());
            var end = Convert.ToDateTime(endDateTime.AddDays(1).ToString("D").ToString()).AddSeconds(-1);

            var days = (endDateTime - startDateTime).TotalDays;
            if (days < 0)
            {
                await ShowErrorMessage("起始日期必须小于结束日期");
                return;
            }
            if (days > 31)
            {
                await ShowErrorMessage("查询日期范围不允许超过31天");
                return;
            }


            SaveFileDialog sfd = new()
            {
                FileName = $"小绿配液系统_配料记录[{start:yyyy-MM-dd}至{end:yyyy-MM-dd}].xlsx",
                DefaultExt = ".xlsx",
                Filter = "Office 2007 File|*.xlsx|Office 2000-2003 File|*.xls|所有文件|*.*"
            };

            var operateResult = sfd.ShowDialog();

            if (operateResult == null) return;
            if (!(bool)operateResult) return;

            var path = sfd.FileName;
            if (string.IsNullOrEmpty(path)) return;

            Loading(true);
            XSSFWorkbook excel = new XSSFWorkbook();
            ISheet sheet = excel.CreateSheet("配料记录");

            // 设置excel标题行
            IRow row = sheet.CreateRow(0);
            row.CreateCell(0).SetCellValue("卡号");
            row.CreateCell(1).SetCellValue("机台");
            row.CreateCell(2).SetCellValue("配液缸");
            row.CreateCell(3).SetCellValue("计划配液量<升>");
            row.CreateCell(4).SetCellValue("实际配液量<升>");
            row.CreateCell(5).SetCellValue("换单标识");
            row.CreateCell(6).SetCellValue("配液完成时间");
            row.CreateCell(7).SetCellValue("管道顺序");
            row.CreateCell(8).SetCellValue("助剂编号");
            row.CreateCell(9).SetCellValue("助剂名称");
            row.CreateCell(10).SetCellValue("配方用量");
            row.CreateCell(11).SetCellValue("计划用量<公斤>");
            row.CreateCell(12).SetCellValue("实际用量<公斤>");

            // 冻结首行
            sheet.CreateFreezePane(0, 1, 0, 1);

            // 设置列宽
            sheet.SetColumnWidth(0, 16 * 256);
            sheet.SetColumnWidth(1, 6 * 256);
            sheet.SetColumnWidth(2, 8 * 256);
            sheet.SetColumnWidth(3, 8 * 256);
            sheet.SetColumnWidth(4, 20 * 256);
            sheet.SetColumnWidth(5, 14 * 256);
            sheet.SetColumnWidth(6, 21 * 256);
            sheet.SetColumnWidth(7, 21 * 256);
            sheet.SetColumnWidth(8, 21 * 256);
            sheet.SetColumnWidth(9, 10 * 256);
            sheet.SetColumnWidth(10, 8 * 256);
            sheet.SetColumnWidth(11, 30 * 256);
            sheet.SetColumnWidth(12, 10 * 256);
            sheet.SetColumnWidth(13, 10 * 256);
            sheet.SetColumnWidth(14, 8 * 256);
            sheet.SetColumnWidth(15, 21 * 256);

            // 设置单元格格式
            var dataFormat = excel.CreateDataFormat();

            var floatStyle = excel.CreateCellStyle();
            floatStyle.DataFormat = dataFormat.GetFormat("0.00");
            floatStyle.VerticalAlignment = VerticalAlignment.Center;
            floatStyle.Alignment = HorizontalAlignment.Left;

            var dateStyle = excel.CreateCellStyle();
            dateStyle.DataFormat = dataFormat.GetFormat("yyyy-MM-dd HH:mm:ss");
            dateStyle.VerticalAlignment = VerticalAlignment.Center;
            dateStyle.Alignment = HorizontalAlignment.Left;

            try
            {
                // 获取数据
                var result = await prcsDataService.GetExcelData(start, end);
                if (!result.IsSuccess)
                {
                    Loading(false);
                    await Task.Delay(100);
                    await ShowErrorMessage("数据导出失败: " + result.Message);
                    return;
                }


                var listData = result.Content;

                if (listData == null)
                {
                    Loading(false);
                    await Task.Delay(100);
                    await ShowErrorMessage("数据导出失败：服务器返回正确的结果，但内容为空");
                    return;
                }

                if (listData.Count < 1)
                {
                    Loading(false);
                    await Task.Delay(100);
                    await ShowErrorMessage("数据导出失败：服务器返回正确的结果，但内容为空");
                    return;
                }

                // 将数据填充到excel对象
                int rowIndex = 1;
                var rangeEndRowIndex = 0;
                foreach (var prcsData in listData)
                {
                    if (prcsData.ListDetail != null && prcsData.ListDetail.Count > 1)
                    {
                        rangeEndRowIndex += prcsData.ListDetail.Count;
                        // 合并单元格
                        sheet.AddMergedRegion(new CellRangeAddress(rowIndex, rangeEndRowIndex, 0, 0));
                        sheet.AddMergedRegion(new CellRangeAddress(rowIndex, rangeEndRowIndex, 1, 1));
                        sheet.AddMergedRegion(new CellRangeAddress(rowIndex, rangeEndRowIndex, 2, 2));
                        sheet.AddMergedRegion(new CellRangeAddress(rowIndex, rangeEndRowIndex, 3, 3));
                        sheet.AddMergedRegion(new CellRangeAddress(rowIndex, rangeEndRowIndex, 4, 4));
                        sheet.AddMergedRegion(new CellRangeAddress(rowIndex, rangeEndRowIndex, 5, 5));
                        sheet.AddMergedRegion(new CellRangeAddress(rowIndex, rangeEndRowIndex, 6, 6));

                        foreach (var detail in prcsData.ListDetail)
                        {
                            IRow r = sheet.CreateRow(rowIndex);

                            r.CreateCell(0).SetCellValue(prcsData.CardNo);
                            r.CreateCell(1).SetCellValue(prcsData.EquipmentName);
                            r.CreateCell(2).SetCellValue(prcsData.BulkCodeNumber);
                            r.CreateCell(3).SetCellValue(prcsData.PlanVolume);
                            r.CreateCell(4).SetCellValue(prcsData.ActualVolume);
                            r.CreateCell(5).SetCellValue(prcsData.IsChange);
                            r.CreateCell(6).SetCellValue((DateTime)prcsData.CompletedDateTime);

                            r.CreateCell(7).SetCellValue(detail.AssSequence);
                            r.CreateCell(8).SetCellValue(detail.AssCodeNumber);
                            r.CreateCell(9).SetCellValue(detail.AssName);
                            r.CreateCell(10).SetCellValue(detail.AssGl);
                            r.CreateCell(11).SetCellValue(detail.PlanKg);
                            r.CreateCell(12).SetCellValue(detail.AssKg);

                            r.GetCell(3).CellStyle = floatStyle;
                            r.GetCell(4).CellStyle = floatStyle;
                            r.GetCell(6).CellStyle = dateStyle;
                            r.GetCell(10).CellStyle = floatStyle;
                            r.GetCell(12).CellStyle = floatStyle;
                            r.GetCell(13).CellStyle = floatStyle;

                            rowIndex++;
                        }
                    }
                    else
                    {
                        IRow r = sheet.CreateRow(rowIndex);
                        r.CreateCell(0).SetCellValue(prcsData.CardNo);
                        r.CreateCell(1).SetCellValue(prcsData.EquipmentName);
                        r.CreateCell(2).SetCellValue(prcsData.BulkCodeNumber);
                        r.CreateCell(3).SetCellValue(prcsData.PlanVolume);
                        r.CreateCell(4).SetCellValue(prcsData.ActualVolume);
                        r.CreateCell(5).SetCellValue(prcsData.IsChange);
                        r.CreateCell(6).SetCellValue((DateTime)prcsData.CompletedDateTime);

                        if (prcsData.ListDetail != null && prcsData.ListDetail.Count == 1)
                        {
                            var detail = prcsData.ListDetail[0];
                            r.CreateCell(7).SetCellValue(detail.AssSequence);
                            r.CreateCell(8).SetCellValue(detail.AssCodeNumber);
                            r.CreateCell(9).SetCellValue(detail.AssName);
                            r.CreateCell(10).SetCellValue(detail.AssGl);
                            r.CreateCell(11).SetCellValue(detail.PlanKg);
                            r.CreateCell(12).SetCellValue(detail.AssKg);
                        }

                        rowIndex++;
                    }
                }


                // 保存
                //Application.Current.Dispatcher.Invoke(() =>
                //{
                FileStream xlsfile = new(sfd.FileName, FileMode.Create);
                excel.Write(xlsfile);
                xlsfile.Close();
                //});
                Loading(false);
                await Task.Delay(100);
                var isOpen = await ShowConfirmOperateView("导出成功,是否立即打开？");
                if (isOpen == null || isOpen.Result != ButtonResult.Yes) return;

                System.Diagnostics.Process process = new System.Diagnostics.Process();
                process.StartInfo.UseShellExecute = true;
                process.StartInfo.FileName = sfd.FileName;
                process.Start();

            }
            catch (Exception ex)
            {
                Loading(false);
                await ShowErrorMessage("导出失败：" + ex.Message);
            }



        }
    }
}
