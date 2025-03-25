//using Microsoft.EntityFrameworkCore;
//using zenra_finance_back.Data;
//using zenra_finance_back.Services;
//using zenra_finance_back.Services.IServices;

//var builder = WebApplication.CreateBuilder(args);

//// Add services to the container.
//builder.Services.AddControllers();
//builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();

//// Add DbContext for SQL Server
//builder.Services.AddDbContext<AppDbContext>(options =>
//    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

//// Register the IYourEntityService and its implementation
//builder.Services.AddScoped<IYourEntityService, YourEntityService>();
//builder.Services.AddScoped<IUserService, UserService>();
//builder.Services.AddScoped<ICommonService, CommonService>();

//// Add CORS services and define a default policy
//builder.Services.AddCors(options =>
//{
//    options.AddDefaultPolicy(
//        builder =>
//        {
//            builder.WithOrigins("http://localhost:5173/", // Replace with the actual origin you want to allow
//                               "http://www.contoso.com")
//                   .AllowAnyHeader()
//                   .AllowAnyMethod();
//        });
//});

//var app = builder.Build();

//// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI();
//}

//app.UseCors();

//app.UseAuthorization();

//app.MapControllers();

//app.Run();

using Microsoft.EntityFrameworkCore;
using zenra_finance_back.Data;
using zenra_finance_back.Services;
using zenra_finance_back.Services.IServices;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add DbContext for SQL Server
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register the IYourEntityService and its implementation
builder.Services.AddScoped<IYourEntityService, YourEntityService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ICommonService, CommonService>();

// Add CORS services and configure to allow all origins, headers, and methods
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAll"); // Apply the CORS middleware using the "AllowAll" policy

app.UseAuthorization();

app.MapControllers();

app.Run();
