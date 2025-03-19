using TrainBoard.Services;
using TrainBoard.Workers;
using OpenLDBWS;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddMemoryCache();
builder.Services.AddSingleton<IRgbMatrixService, RgbMatrixService>(); 
builder.Services.AddSingleton<IPlatformEtdService, PlatformEtdService>(); 
builder.Services.AddSingleton<ICallingPointService, CallingPointService>();  
builder.Services.AddSingleton<ILdbwsClient>(provider => new LdbwsClient("72a8196c-6306-4f02-8194-6225e3eee456"));
builder.Services.AddHostedService<DataFeedWorker>();
builder.Services.AddHostedService<PlatformEtdToggleWorker>();
builder.Services.AddHostedService<DisplayWorker>();
builder.Services.AddHostedService<ScrollCallingPointsWorker>();

var host = builder.Build();
host.Run();
