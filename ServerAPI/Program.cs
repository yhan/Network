using System.Text.Json.Serialization;
using Common;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddHostedService<ServerService>();
builder.Services.AddSingleton<Perf>();
builder.Services.AddControllers().AddJsonOptions(x => {
    // serialize enums as strings in api responses (e.g. Role)
    x.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});
var app = builder.Build();

app.MapGet("/", () => "Hello World!");
app.MapControllers();

app.Run();