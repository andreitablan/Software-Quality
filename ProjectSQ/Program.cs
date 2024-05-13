using ProjectSQ.Interfaces.Memory;
using ProjectSQ.Interfaces.Parser;
using ProjectSQ.Interfaces.Processor;
using ProjectSQ.Models;
using ProjectSQ.Services;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers(options =>
{
    options.InputFormatters.Clear();
    options.InputFormatters.Add(new PlainTextInputFormatter());
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IParseService, ParseService>();
builder.Services.AddScoped<IProcessorService, ProcessorService>();
builder.Services.AddScoped<IMemoryService, MemoryService>();

var allowedCorsHosts = builder.Configuration["AllowedCorsHosts"]?.Split(",");

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
        policyBuilder =>
        {
            policyBuilder.WithOrigins(allowedCorsHosts ?? new[] { "*" });
            policyBuilder.AllowAnyHeader();
            policyBuilder.AllowAnyMethod();
            policyBuilder.AllowCredentials();
        });
});

builder.Services.AddSignalR();

Memory.InitMemory();
Processor.InitProcessor();

Task.Run(action: ProcessorService.WriteToVideoMemory);


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.MapHub<RealTimeHub>("hub");

app.Run();
