using SKADA;
using SKADA.Models.Alarms.Repository;
using SKADA.Models.Alarms.Service;
using SKADA.Models.Devices.Repository;
using SKADA.Models.Devices.Service;
using SKADA.Models.Inputs.Repository;
using SKADA.Models.Inputs.Service;
using SKADA.Models.Users.Model;
using SKADA.Models.Users.Repository;
using SKADA.Models.Users.Service;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<AppDbContext>();

builder.Services.AddTransient<IUserRepository<User>, UserRepository<User>>();
builder.Services.AddTransient<IUserService, UserService>();
builder.Services.AddTransient<IDeviceRepository, DeviceRepository>();
builder.Services.AddTransient<IDeviceService, DeviceService>();
builder.Services.AddTransient<IAnalogInputService, AnalogInputService>();
builder.Services.AddTransient<IAnalogInputRepository, AnalogInputRepository>();
builder.Services.AddTransient<IDigitalInputRepository, DigitalInputRepository>();
builder.Services.AddTransient<IDigitalInputService, DigitalInputService>();
builder.Services.AddTransient<IDigitalReadInstanceRepository, DigitalReadInstanceRepository>();
builder.Services.AddTransient<IAnalogReadInstanceRepository, AnalogReadInstanceRepository>();
builder.Services.AddTransient<IAnalogReadInstanceService, AnalogReadInstanceService>();
builder.Services.AddTransient<IAlarmInstanceRepository, AlarmInstanceRepository>();
builder.Services.AddTransient<IAlarmRepository, AlarmRepository>();
builder.Services.AddTransient<IAlarmService, AlarmService>();


builder.Services.AddAuthentication(Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Events = new Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationEvents
        {
            OnRedirectToLogin = context =>
            {
                context.Response.StatusCode = (int)System.Net.HttpStatusCode.Unauthorized;
                return Task.CompletedTask;
            },
            OnRedirectToAccessDenied = context =>
            {
                context.Response.StatusCode = (int)System.Net.HttpStatusCode.Forbidden;
                return Task.CompletedTask;
            }
        };
        options.Cookie.SameSite = SameSiteMode.None;
        options.SlidingExpiration = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;

        options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
    });
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy =>
    {
        policy.RequireRole(UserType.ADMIN.ToString(),UserType.CLIENT.ToString());
    });
    options.AddPolicy("ClientOnly", policy =>
    {
        policy.RequireRole(UserType.CLIENT.ToString());
    });

});
builder.Services.AddHostedService<BackgroundStartupService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    dbContext.InitDataBase();
    Console.WriteLine(dbContext.Users.First().Name);
    Console.WriteLine(dbContext.Users.Last().Name);
}
app.Run();

