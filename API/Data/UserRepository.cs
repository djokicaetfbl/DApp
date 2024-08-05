using API.DTOs;
using API.Entities;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using System.Linq;

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
                        .Include(x => x.Photos)
                        .Where(x => x.UserName == username)
                        .ProjectTo<MemberDto>(_mapper.ConfigurationProvider) // Sa ProjectTo ne moramo da radimo Eager loading, kao sto je slucaj iznada sa Select i new object MemberDto !
                        .SingleOrDefaultAsync();
        }

        public async Task<PagedList<MemberDto>> GetMembersAsync(UserParams userParams)
        {
            var query = _context.Users.AsQueryable();

            query = query.Include(x => x.Photos).Where(u => u.UserName != userParams.CurrentUsername);
            query = query.Include(x => x.Photos).Where(u => u.Gender != userParams.Gender);

            var minDob = DateOnly.FromDateTime(DateTime.Today.AddYears(-userParams.MaxAge - 1)); //2024 - 100 = 1924
            var maxDob = DateOnly.FromDateTime(DateTime.Today.AddDays(-userParams.MinAge)); // 2024 - 18 = 2006

            query = query.Where(u => u.DateOfBirth >= minDob && u.DateOfBirth <= maxDob);

            query = userParams.OrderBy switch
            {
                "created" => query.OrderByDescending(u => u.Created),
                _ => query.OrderByDescending(u => u.LastActive)
            };

            //var query = _context.Users
            //    .Include(x => x.Photos)
            //    .ProjectTo<MemberDto>(_mapper.ConfigurationProvider)
            //    .AsNoTracking(); // EF Core nece pratiti sta mi vracamo ovom metodom, pa ce ovo sve biti malo brze

            return await PagedList<MemberDto>.CreateAsync(query.AsNoTracking().ProjectTo<MemberDto>(_mapper.ConfigurationProvider), userParams.PageNumber, userParams.PageSize);

            //return await _context.Users
            //                .Include(x => x.Photos)
            //                .ProjectTo<MemberDto>(_mapper.ConfigurationProvider)
            //                .ToListAsync();

        }
    }
}
