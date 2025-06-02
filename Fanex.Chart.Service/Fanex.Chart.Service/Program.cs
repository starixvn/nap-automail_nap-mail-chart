using Fanex.Data.SqlServer;
using Fanex.Chart.Service;
using Fanex.Logging.Sentry.RavenClient;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
var configuration = builder.Configuration;

services.AddControllers();

services.AddEndpointsApiExplorer();
services.AddSwaggerGen();
services.AddConfiguration();
services.AddFanexSql(configuration);
services.AddFanexLogSentry(configuration);
services.AddInternalService();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();

await app.RunAsync();