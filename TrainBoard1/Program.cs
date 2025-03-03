using TrainBoard.Services;
using TrainBoard.Workers;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddSingleton<IRgbMatrixService, RgbMatrixService>(); 
builder.Services.AddHostedService<DataFeedWorker>();
builder.Services.AddHostedService<CallingPointsWorker>();
builder.Services.AddHostedService<CurrentTimeWorker>();

var host = builder.Build();
host.Run();
