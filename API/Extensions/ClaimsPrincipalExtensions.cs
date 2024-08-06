using System.Security.Claims;

namespace API.Extensions
{
    public static class ClaimsPrincipalExtensions // pravit cemo extension metode, pa je uredu da i sama klasa bude staticka jer su sve metode u noj staticke
    {
        public static string GetUsername(this ClaimsPrincipal user)
        {
            return user.FindFirst(ClaimTypes.Name)?.Value;
        }        
        public static int GetUserId(this ClaimsPrincipal user)
        {
            return int.Parse(user.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        }
    }
}
