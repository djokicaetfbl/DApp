using API.Data;
using API.Entities;
using API.Extensions;
using API.Middleware;
using API.SignalR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddApplicationService(builder.Configuration); // extension method for ApplicationServices


builder.Services.AddIdentityServices(builder.Configuration); // middleware // extension method for Identity// token

var app = builder.Build();



// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI();
//}

app.UseHttpsRedirection();

app.UseMiddleware<ExceptionMiddleware>(); // dodana klasa ExceptionMiddleware

app.UseCors(builder => builder.AllowAnyHeader()
.AllowAnyMethod()
.AllowCredentials() // za potrebe signalR-a i komunikaciju-autentikaciju servera i klijenta
.WithOrigins("https://localhost:4200")); //Cross-Origin Resource Sharing - komunikacija izmedju fronend-a i bekenda 

app.UseAuthentication(); // bitan redosljed isppod UseCors...WithOrigins !
app.UseAuthorization(); // bitno da ide ispod UseAuthentication !

app.MapControllers();
app.MapHub<PresenceHub>("hubs/presence"); // PresenceHub koji smo napravili za potrebe SignalR-a
app.MapHub<MessageHub>("hubs/message"); // MessageHub koji smo napravili za potrebe SignalR-a

///---------- Seed -----------------///
using var scope = app.Services.CreateScope();
var services = scope.ServiceProvider;

try
{
    var context = services.GetRequiredService<DataContext>();
    var userManager = services.GetRequiredService<UserManager<AppUser>>();
    var roleManager = services.GetRequiredService<RoleManager<AppRole>>();
    await context.Database.MigrateAsync();
    //context.Connections.RemoveRange(context.Connections); // prilikom ponovnog pokretanja servera da se obrisu postojece konekcije izmedju ljudi koji cetuju, ovaj nacin je jako spor ako imamo nekeliko hiljada redova u tabeli
    //await context.Database.ExecuteSqlRawAsync("TRUNCATE TABLE [Connections]"); // ovo radi daleko brze neko RemoveRange pa ce brzo ocisti sve konekcije iz tabele Connections, ovo ne radi na SQL Lite :D
    await context.Database.ExecuteSqlRawAsync("DELETE FROM [Connections]"); // ovo radi na SQL LITE
    await Seed.SeedUsers(userManager, roleManager);
}
catch(Exception ex)
{
    var logger = services.GetService<ILogger<Program>>();
    logger.LogError(ex, "An error occured during migration");
}

app.Run();
