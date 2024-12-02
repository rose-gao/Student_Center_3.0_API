using Student_Center_3._0_Services.Services;
using System.Net.Http.Headers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//builder.Services.AddScoped<ScheduleService>();
builder.Services.AddScoped<AddCourseService>();
builder.Services.AddScoped<DropCourseService>();
builder.Services.AddScoped<SwapCourseService>();
builder.Services.AddScoped<CourseEnrollmentService>();
builder.Services.AddScoped<HttpClient>();


builder.Services.AddHttpClient<LoginService>(client =>
{
    client.BaseAddress = new Uri("https://localhost:7176");
});

builder.Services.AddHttpClient<UserService>(client =>
{
    client.BaseAddress = new Uri("https://localhost:7176");
});

builder.Services.AddHttpClient<AddCourseService>(client =>
{
    client.BaseAddress = new Uri("https://localhost:7176");
});

builder.Services.AddHttpClient<DropCourseService>(client =>
{
    client.BaseAddress = new Uri("https://localhost:7176");
});

builder.Services.AddHttpClient<SwapCourseService>(client =>
{
    client.BaseAddress = new Uri("https://localhost:7176");
});

builder.Services.AddHttpClient<CourseService>(client =>
{
    client.BaseAddress = new Uri("https://localhost:7176");
});

builder.Services.AddHttpClient<ScheduleService>(client =>
{
    client.BaseAddress = new Uri("https://localhost:7176");
});


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
