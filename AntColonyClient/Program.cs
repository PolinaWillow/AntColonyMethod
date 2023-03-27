using AntColonyClient.Service;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
// получаем строку подключения из файла конфигурации
string connection = builder.Configuration.GetConnectionString("UserDBConnection");

// добавляем контекст AppDbContext в качестве сервиса в приложение
builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(connection));

//builder.Services.AddScoped<IUserTaskRepository, MockUserTask>();
builder.Services.AddScoped<IUserTaskRepository, SQLUserTask>();
builder.Services.AddScoped<ITaskParamsRepository, SQLTaskParam>();
//builder.Services.AddScoped<IUserTaskRepository, SQLUserTask>();

builder.Services.Configure<RouteOptions>(options => {
    //Корректное отображение URL (/ строчные буквы и т.д)
    options.LowercaseUrls = true;
    options.LowercaseQueryStrings = true;
    options.AppendTrailingSlash = true;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();
