using SmallGreen.API.IService;

namespace SmallGreen.API.Service
{
    public class SmallGreenBackgroundService : BackgroundService
    {
        private readonly ISystemManagerService SystemManagerService;
        private readonly ILogger<SmallGreenBackgroundService> Logger;

        public SmallGreenBackgroundService(ISystemManagerService systemManagerService, ILogger<SmallGreenBackgroundService> logger)
        {
            SystemManagerService = systemManagerService;
            Logger = logger;
        }

        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            //return base.StartAsync(cancellationToken);

            var dateStart = DateTime.Now;
            var result = await SystemManagerService.Init();
            if (!result.IsSuccess)
            {
                Logger.LogError("初始化 [SmallGreenBackgroundService] 模块出现故障:{Message}", result.Message);
                await StopAsync(cancellationToken);
                return;
            }
            Logger.LogInformation("初始化 [SmallGreenBackgroundService] 模块完成，耗时{span}毫秒", (DateTime.Now - dateStart).TotalMilliseconds);
            await base.StartAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    //var t1 = DateTime.Now;
                    
                    await SystemManagerService.CheckRuntime();  //  入口  核心

                    await SystemManagerService.CheckBulkCompleted(); // 检查是否有配液缸完成工作

                    //var t2 = DateTime.Now;
                    //var tp = (t2 - t1).TotalMilliseconds + " ms";

                    //Logger.LogInformation("刷新耗时:{tp}", tp);
                    await Task.Delay(TimeSpan.FromSeconds(2), stoppingToken);
                }
                catch (Exception ex)
                {
                    //Logger.LogError("CowboyBackground ExecuteAsync Error: {Message} | {stackTrace}", ex.Message, ex.StackTrace);
                    Logger.LogError(ex, "[SmallGreenBackgroundService] 后台出现异常：{message}", ex.Message);
                    await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
                }
            }
        }
    }
}
