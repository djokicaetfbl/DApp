namespace API.DTOs
{
    public class MemberUpdateDto
    {
        public string Intodruction { get; set; }
        public string LookingFor { get; set; }
        public string Interests { get; set; }
        public string City { get; set; }
        public string Country { get; set; }

        //  U AutoMapperProfiles , definisat cemo mapper za MemberUpdateDto
    }
}
