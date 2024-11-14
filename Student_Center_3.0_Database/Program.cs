using Microsoft.EntityFrameworkCore;
using Student_Center_3._0_Database.Models;
using Student_Center_3._0_Database.Utils;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<StudentCenterContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DevConnection")));

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new CustomDateTimeConverterUtils());
    });

/*
//temp.GenerateKeyAndIV();

// Allows services to be registered in controllers
builder.Services.AddControllersWithViews();

// Register a Singleton service (only one instance for the application lifetime),
// service is an instance of IConiguration, which holds all key-value pairs from appsettings.json
builder.Services.AddSingleton(builder.Configuration);
*/

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
