using Quartz;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddQuartz(q => {
        q.UseMicrosoftDependencyInjectionJobFactory();
        // Just use the name of your job that you created in the Jobs folder.
        var jobKey = new JobKey("SendEmailJob");
        q.AddJob<SendToServerJob>(opts => opts.WithIdentity(jobKey));

        q.AddTrigger(opts => {
            //var every5Min = "0 0/5 * * * ?";
            var cron = "0/10 * * * * ?";// every sec "* * * ? * *";
            opts
                .ForJob(jobKey)
                .WithIdentity("SendToServerJob-trigger")
                //This Cron interval can be described as "run every minute" (when second is zero)
                .WithCronSchedule(cron);
        });
    });

builder.Services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);

var app = builder.Build();
//app.MapGet("/", () => "Hello World!");
app.Run();