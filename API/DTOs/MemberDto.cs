using API.Entities;
using API.Extensions;

namespace API.DTOs
{
    public class MemberDto // Data Transfer Object -> ono sto vracamo kao response nekog request-a npr
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public int Age { get; set; } // AutoMapper ce znati GetAge metod i lijepo ce mapirati
        public string KnowAs { get; set; }
        public DateTime Created { get; set; } = DateTime.UtcNow;
        public DateTime LastActive { get; set; } = DateTime.UtcNow;
        public string Gender { get; set; }
        public string Intodruction { get; set; }
        public string LookingFor { get; set; }
        public string Interests { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public List<PhotoDto> Photos { get; set; } = []; // new () --> isto je
        public string PhotoUrl { get; set; }
    }
}
