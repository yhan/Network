using System.Configuration;
using System.Text.Json.Serialization;
using ClientAPI;
using Common;
using Quartz;

var builder = WebApplication.CreateBuilder(args);

var config = builder.Configuration.GetSection("Config").Get<Config>();
//builder.Services.AddSingleton<Config>(config);
builder.Services.Configure<Config>(builder.Configuration.GetSection("Config"));

var isServer = config.Server != null;
if(isServer)
{
    // As server
    builder.Services.AddHostedService<ServerService>();
    builder.Services.AddSingleton<Perf>();
    builder.Services.AddControllers().AddJsonOptions(x => {
        // serialize enums as strings in api responses (e.g. Role)
        x.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });
}

var isClient = config.Client != null;
if (isClient)
{
    // As client
    builder.Services
        .AddQuartz(q => {
            q.UseMicrosoftDependencyInjectionJobFactory();
            // Just use the name of your job that you created in the Jobs folder.
            var jobKey = new JobKey("SendToServerJob");
            q.AddJob<SendToServerJob>(opts => opts.WithIdentity(jobKey));

            q.AddTrigger(opts => {
                var cron = config.Client.SendFreq;
                opts
                    .ForJob(jobKey)
                    .WithIdentity("SendToServerJob-trigger")
                    //This Cron interval can be described as "run every minute" (when second is zero)
                    .WithCronSchedule(cron);
            });
        });
    builder.Services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);
}

var app = builder.Build();
app.MapGet("/", () => "Hello World!");
if(isServer)
    app.MapControllers();
app.Run();