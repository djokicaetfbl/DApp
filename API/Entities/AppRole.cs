using Microsoft.AspNetCore.Identity;

namespace API.Entities
{
    public class AppRole : IdentityRole<int> // sa ovim smo naglasili da ce Id biti tipa int
    {
        public ICollection<AppUserRole> UserRoles { get; set; }
    }
}
