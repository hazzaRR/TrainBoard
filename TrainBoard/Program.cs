using TrainBoard.Services;
using TrainBoard.Workers;
using OpenLDBWS;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddMemoryCache();
builder.Services.AddSingleton<IRgbMatrixService, RgbMatrixService>(); 
builder.Services.AddSingleton<IPlatformStdService, PlatformStdService>(); 
builder.Services.AddSingleton<ILdbwsClient>(provider => new LdbwsClient("<Api-key>"));
builder.Services.AddHostedService<DataFeedWorker>();
builder.Services.AddHostedService<PlatformStdToggleWorker>();
builder.Services.AddHostedService<DisplayWorker>();
builder.Services.AddHostedService<ScrollCallingPointsWorker>();

var host = builder.Build();
host.Run();
