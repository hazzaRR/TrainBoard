using TrainBoard.Services;
using TrainBoard.Workers;
using OpenLDBWS;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddMemoryCache();
builder.Services.AddSingleton<IRgbMatrixService, RgbMatrixService>(); 
builder.Services.AddSingleton<IPlatformStdService, PlatformStdService>(); 
builder.Services.AddSingleton<ILdbwsClient, LdbwsClient>();
builder.Services.AddHostedService<DataFeedWorker>();
builder.Services.AddHostedService<PlatformStdToggleWorker>();
builder.Services.AddHostedService<DisplayWorker>();

var host = builder.Build();
host.Run();
