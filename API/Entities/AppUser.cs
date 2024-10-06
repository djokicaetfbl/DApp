using Microsoft.AspNetCore.Identity;

namespace API.Entities;

public class AppUser : IdentityUser<int> // sa ovim int cemo reci da je Id tipa int
{
    //public int Id { get; set; }
    //public string UserName { get; set; }
    //public byte[] PasswordHash { get; set; }
    //public byte[] PasswordSalt { get; set; } // ova 4 porpertija nam vise nece trebati jer sada koristimo ASP.NET Core Identity
    public DateOnly DateOfBirth { get; set; }
    public string KnowAs { get; set; }
    public DateTime Created { get; set; } = DateTime.UtcNow;
    public DateTime LastActive { get; set; } = DateTime.UtcNow;
    public string Gender { get; set; }
    public string Intodruction { get; set; }
    public string LookingFor { get; set; }
    public string Interests { get; set; }
    public string City { get; set; }
    public string Country { get; set; }
    public List<Photo> Photos { get; set; } = []; // new () --> isto je
    public List<UserLike> LikedByUsers { get; set; }
    public List<UserLike> LikedUsers { get; set; }
    public List<Message> MessagesSent { get; set; }
    public List<Message> MessagesReceived { get; set; }
    public ICollection<AppUserRole> UserRoles { get; set; }

    //public int GetAge()
    //{
    //    return DateOfBirth.CalculateAge();
    //}
}