using TrainBoard.Services;
using TrainBoard.Workers;
using OpenLDBWS;
using OpenLDBWS.Options;

var builder = Host.CreateApplicationBuilder(args);

builder.Configuration.AddJsonFile("api-secrets.json", optional: true, reloadOnChange: true);
builder.Services.Configure<LdbwsOptions>(builder.Configuration.GetSection("LdbwsClient"));

builder.Services.AddMemoryCache();
builder.Services.AddSingleton<IRgbMatrixService, RgbMatrixService>(); 
builder.Services.AddSingleton<IPlatformEtdService, PlatformEtdService>();
builder.Services.AddSingleton<ICallingPointService, CallingPointService>();
builder.Services.AddSingleton<IDestinationService, DestinationService>();
builder.Services.AddSingleton<INetworkConnectivityService, NetworkConnectivityService>();
builder.Services.AddSingleton<ILdbwsClient, LdbwsClient>();
builder.Services.AddHostedService<DataFeedWorker>();
builder.Services.AddHostedService<PlatformEtdToggleWorker>();
builder.Services.AddHostedService<DisplayWorker>();
builder.Services.AddHostedService<ScrollCallingPointsWorker>();
builder.Services.AddHostedService<ScrollDestinationWorker>();

var host = builder.Build();
host.Run();
