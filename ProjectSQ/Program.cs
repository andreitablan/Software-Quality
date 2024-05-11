using ProjectSQ.Interfaces.Memory;
using ProjectSQ.Interfaces.Parser;
using ProjectSQ.Interfaces.Processor;
using ProjectSQ.Models;
using ProjectSQ.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers(options =>
{
    // Clear the existing formatters
    options.InputFormatters.Clear();

    // Add the custom plain text input formatter
    options.InputFormatters.Add(new PlainTextInputFormatter());
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
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
        });
});

Task.Run(() =>
{

    var processorService = new ProcessorService();
    processorService.WriteToVideoMemory();
});

Memory.InitMemory();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
