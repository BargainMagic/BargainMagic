using BargainMagic.Api.Service;
using BargainMagic.Api.Service.Channels;
using BargainMagic.Api.Service.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContextFactory<DataContext>();

builder.Services.AddSingleton<CardFetcherChannel>();

builder.Services.AddHostedService<CardFetcherService>();

builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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

app.Run();
