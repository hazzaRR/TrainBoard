using TrainBoard.Services;
using TrainBoard.Workers;
using OpenLDBWS;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddMemoryCache();
builder.Services.AddSingleton<IRgbMatrixService, RgbMatrixService>(); 
builder.Services.AddSingleton<IPlatformEtdService, PlatformEtdService>();
builder.Services.AddSingleton<ICallingPointService, CallingPointService>();
builder.Services.AddSingleton<IDestinationService, DestinationService>();
builder.Services.AddSingleton<ILdbwsClient>(provider => new LdbwsClient("api-key"));
builder.Services.AddHostedService<DataFeedWorker>();
builder.Services.AddHostedService<PlatformEtdToggleWorker>();
builder.Services.AddHostedService<DisplayWorker>();
builder.Services.AddHostedService<ScrollCallingPointsWorker>();
builder.Services.AddHostedService<ScrollDestinationWorker>();

var host = builder.Build();
host.Run();
