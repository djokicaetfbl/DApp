using API.Extensions;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc.Filters;

namespace API.Helpers
{
    public class LogUserActivity : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next) // kako bi smo logovali posljednju aktivnost korisnika
        {
            var resultContext = await next(); // nakon sto se izvrsi akcija radimo nesto ...

            if (!resultContext.HttpContext.User.Identity.IsAuthenticated)
                return; // nije potrebno sto se vec autentifikuje kroz kontroller, ali nije lose ostaviti

            var userId = resultContext.HttpContext.User.GetUserId(); // GetUsername() extension metod

            var repo = resultContext.HttpContext.RequestServices.GetRequiredService<IUserRepository>();
            var user = await repo.GetUserByIdAsync(int.Parse(userId));
            user.LastActive = DateTime.UtcNow;

            await repo.SaveAllAsync();

            //HttpContext sadrzi nas username claim, sadrzi nase servise kao sto je npr: IUserRepository, jer je definisan u services.AddScoped<IUserRepository, UserRepository>(); itd..
        }
    }
}
