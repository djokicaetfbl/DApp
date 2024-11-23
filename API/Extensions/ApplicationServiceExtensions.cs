using API.Data;
using API.Helpers;
using API.Interfaces;
using API.Services;
using API.SignalR;
using Microsoft.EntityFrameworkCore;

namespace API.Extensions
{
    public static class ApplicationServiceExtensions
    {
        public static IServiceCollection AddApplicationService(this IServiceCollection services, IConfiguration config)
        {
            services.AddDbContext<DataContext>(opt =>
            {
                opt.UseSqlite(config.GetConnectionString("DefaultConnection"));
            });
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            //builder.Services.AddEndpointsApiExplorer();
            //builder.Services.AddSwaggerGen();

            /*
             * Transient objects are always different; a new instance is provided to every controller and every service.

                Scoped objects are the same within a request, but different across different requests
            ////////////////SCOPED//////////////////
            ///
            Ova rečenica se odnosi na životni ciklus objekata u kontekstu web aplikacija, posebno u vezi s upravljanjem ovisnostima i kontejnerima za ubrizgavanje ovisnosti (dependency injection containers). Kada se kaže da su objekti "scoped" ili u okviru, to znači da se njihov životni ciklus ograničava na određeni kontekst ili opseg.

            Pojasnimo detaljnije:

            Unutar istog zahtjeva (requesta): Scoped objekti su jednaki. To znači da će svaki zahtjev prema serveru (npr. HTTP zahtjev) unutar iste sesije ili upita koristiti isti instancirani objekt. Bez obzira koliko puta zatražimo tu ovisnost unutar jednog zahtjeva, uvijek ćemo dobiti istu instancu.

            Između različitih zahtjeva: Scoped objekti su različiti. Svaki novi zahtjev dobiva novu instancu objekta. To osigurava da podaci iz jednog zahtjeva ne prelaze u drugi, što je važno za održavanje stanja i sigurnosti.

            Praktičan primjer:

            Ako imate web aplikaciju i koristite scoped objekt za upravljanje podacima korisnika, svaki korisnik koji pošalje zahtjev prema serveru dobit će svoj zaseban objekt s vlastitim podacima. Na taj način se sprječava miješanje podataka između korisnika.

            Ovo je posebno korisno u kontekstima gdje se podaci mogu razlikovati između različitih zahtjeva, ali je poželjno da budu konzistentni unutar jednog zahtjeva.

                Singleton objects are the same for every object and every request (regardless of whether an instance is provided in ConfigureServices)
             */

            services.AddCors(); //Cross-Origin Resource Sharing - komunikacija izmedju frontend-a i bekenda (razliciti serveri, razliciti domeni)
            services.AddScoped<ITokenService, TokenService>();
            /*services.AddScoped<IUserRepository, UserRepository>();*/ // registrovanje  servisa - repozitorija, prebaceno u UnitOfWork
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            services.Configure<CloudinarySettings>(config.GetSection("CloudinarySettings"));
            services.AddScoped<IPhotoService, PhotoService>();
            services.AddScoped<LogUserActivity>();
            //services.AddScoped<ILikesRepository, LikesRepository>(); //prebaceno u UnitOfWork
            //services.AddScoped<IMessageRepository, MessageRepository>(); // prebaceno u UnitOfWork
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddSignalR();
            services.AddSingleton<PresenceTracker>(); // dodan kao Singleton (kako bi vidjeli koji su korisnici online) i dostupan je svim korisnicima kroz citavu aplikaciju

            return services;
        }
    }
}
