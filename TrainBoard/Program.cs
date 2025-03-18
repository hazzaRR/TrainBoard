using TrainBoard.Services;
using TrainBoard.Workers;
using OpenLDBWS;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddMemoryCache();
builder.Services.AddSingleton<IRgbMatrixService, RgbMatrixService>(); 
builder.Services.AddSingleton<IPlatformEtdService, PlatformEtdService>(); 
builder.Services.AddSingleton<ILdbwsClient>(provider => new LdbwsClient("<Api-key>"));
builder.Services.AddHostedService<DataFeedWorker>();
builder.Services.AddHostedService<PlatformEtdToggleWorker>();
builder.Services.AddHostedService<DisplayWorker>();

var host = builder.Build();
host.Run();
