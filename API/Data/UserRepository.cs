using API.DTOs;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class UserRepository : IUserRepository
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;

        public UserRepository(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<AppUser> GetUserByIdAsync(int id)
        {
            return await _context.Users.FindAsync(id);
        }

        public async Task<AppUser> GetUserByUsernameAsync(string username)
        {
            return await _context.Users
                .Include(x => x.Photos)
                .SingleOrDefaultAsync(x => x.UserName == username);
        }

        public async Task<IEnumerable<AppUser>> GetUsersAsync()
        {
            return await _context.Users
                .Include(x => x.Photos)
                .ToListAsync();
        }

        public async Task<bool> SaveAllAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }

        public void Update(AppUser user)
        {
            _context.Entry(user).State = EntityState.Modified; // informisanje EF trakera da je objekt modifikovan, iako ovo sve EF radi sam u pozadini
        }

        public async Task<MemberDto> GetMemberAsync(string username)
        {

            //return await _context.Users      //Without AutoMapper
            //    .Where(x => x.UserName == username)
            //    .Select(user => new MemberDto
            //    {
            //        Id = user.Id,
            //        UserName = username,
            //        KnowAs = user.KnowAs,
            //    })
            //    .SingleOrDefaultAsync();

            return await _context.Users
                        .Where(x => x.UserName == username)
                        .ProjectTo<MemberDto>(_mapper.ConfigurationProvider) // Sa ProjectTo ne moramo da radimo Eager loading, kao sto je slucaj iznada sa Select i new object MemberDto !
                        .SingleOrDefaultAsync();
        }

        public async Task<IEnumerable<MemberDto>> GetMembersAsync()
        {
            return await _context.Users
                            .ProjectTo<MemberDto>(_mapper.ConfigurationProvider)
                            .ToListAsync();
        }
    }
}
