using Microsoft.EntityFrameworkCore;
using TacoTech.UserSync.Application.Users.Commands;
using TacoTech.UserSync.Application.Users.Interfaces;
using TacoTech.UserSync.Domain.Users.Interfaces.Repositories;
using TacoTech.UserSync.Infrastructure.Users.Emails;
using TacoTech.UserSync.Infrastructure.Users.Persistence.DBContext;
using TacoTech.UserSync.Infrastructure.Users.Persistence.Repositories;
using TacoTech.UserSync.Infrastructure.Users.Remote;
using Serilog;  

var builder = WebApplication.CreateBuilder(args);

// Swagger services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add DbContext with SQLite
builder.Services.AddDbContext<UserSyncDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("Default")
                           ?? "Data Source=users.db";
    options.UseSqlite(connectionString);
});

// Register repositories
builder.Services.AddScoped<IUserRepository, UserRepository>();

// Register remote user client
builder.Services.AddHttpClient<IRemoteUserClient, JsonPlaceholderUserClient>(client =>
{
    var baseUrl = builder.Configuration["JsonPlaceholder:BaseUrl"]
                  ?? "https://jsonplaceholder.typicode.com/";
    client.BaseAddress = new Uri(baseUrl);
});

// Register email notifier
builder.Services.Configure<EmailSettings>(
    builder.Configuration.GetSection("Email"));
builder.Services.AddScoped<IEmailNotifier, SmtpEmailNotifier>();

// Add MediatR (Application project)
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(SyncUsersCommand).Assembly);
});

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console()
    .WriteTo.File("logs/app.log", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

var app = builder.Build();

// Apply EF Core migrations at startup (for demo / local dev)
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<UserSyncDbContext>();
    db.Database.Migrate();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
