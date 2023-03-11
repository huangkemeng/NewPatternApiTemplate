using Autofac.Extensions.DependencyInjection;
using NewPattern.Api.Engines.Bases;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
EngineHub.StartEngines(builder.Services);
builder.Host.UseServiceProviderFactory(spf => new AutofacServiceProviderFactory(containerBuilder =>
{
    EngineHub.StartEngines(containerBuilder);
}));
var app = builder.Build();
app.UseHttpsRedirection();

app.UseAuthorization();
app.MapControllers();
EngineHub.StartEngines(app);
app.Run();
