using LogDashboard;
using Serilog.Events;
using Serilog;
using SmallGreen.API.Configuration;
using SmallGreen.API.Service;
using SmallGreen.API.IService;
using SmallGreen.Entity.Basic;

namespace SmallGreen.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // ������־
            string logOutputTemplate = "{Timestamp:yyyy-MM-dd HH:mm:ss} || {Level} || {SourceContext:l} || {Message} || {Exception} ||end {NewLine}";
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .MinimumLevel.Override("Default", LogEventLevel.Information)
                .MinimumLevel.Override("Microsoft", LogEventLevel.Error)
                .MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Information)
                .Enrich.FromLogContext()
                .WriteTo.Console(theme: Serilog.Sinks.SystemConsole.Themes.AnsiConsoleTheme.Code)
                .WriteTo.File($"{AppContext.BaseDirectory}Logs/Log.log", rollingInterval: RollingInterval.Day, outputTemplate: logOutputTemplate)
                .CreateLogger();
            builder.Host.UseSerilog(Log.Logger, dispose: true);
            builder.Services.AddLogDashboard();

            // Mapster����
            MapsterConfiguration.Configure();

            // Add services to the container.
            builder.Services.AddSingleton(new ErpDbHelper(builder.Configuration["ErpDbConnection"]
                ?? throw new InvalidOperationException("ErpDbConnection 未配置")));
            builder.Services.AddSingleton<ISystemManagerService, SystemManagerService>();
            builder.Services.AddHostedService<SmallGreenBackgroundService>();


            builder.Services.AddTransient<IUserService, UserService>();
            builder.Services.AddTransient<IPRCSDataService, PRCSDataService>();
            builder.Services.AddTransient<IAssInfoService, AssInfoService>();

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            //if (app.Environment.IsDevelopment())
            //{
                app.UseSwagger();
                app.UseSwaggerUI();
            //}

            app.UseAuthorization();
            app.UseLogDashboard();

            app.MapControllers();

            app.Run();
        }
    }
}
