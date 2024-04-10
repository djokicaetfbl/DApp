using API.Extensions;
using API.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddApplicationService(builder.Configuration); // extension method for ApplicationServices


builder.Services.AddIdentityServices(builder.Configuration); // middleware // extension method for Identity

var app = builder.Build();



// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI();
//}

app.UseHttpsRedirection();

app.UseMiddleware<ExceptionMiddleware>(); // dodana klasa ExceptionMiddleware

app.UseCors(builder => builder.AllowAnyHeader().AllowAnyMethod().WithOrigins("https://localhost:4200")); //Cross-Origin Resource Sharing - komunikacija izmedju fronend-a i bekenda 

app.UseAuthentication(); // bitan redosljed isppod UseCors...WithOrigins !
app.UseAuthorization(); // bitno da ide ispod UseAuthentication !

app.MapControllers();

app.Run();
