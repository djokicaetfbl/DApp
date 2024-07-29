using API.Data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace API.Controllers
{
    public class AccountController : BaseApiController
    {
        private readonly DataContext _context;
        private readonly ITokenService _tokenService;
        private readonly IMapper _mapper;

        public AccountController(DataContext context, ITokenService tokenService, IMapper mapper)
        {
            _context = context;
            _tokenService = tokenService;
            _mapper = mapper;
        }
        //Use query strings for optional or filtering/sorting parameters and when flexibility is required. Use standard parameters in the route path for essential parameters that are part of the 
        //route's structure and when cleaner, more readable URLs are desired. Choose the approach that best fits the specific requirements and design considerations of your application.

        [HttpPost("register")] // api/account/register?username=dave&password=pwd // npr kao query string umjesto da stavimo parametre
        public async Task<ActionResult<AppUser>> Register(RegisterDto registerDto)
        {
            if (await UserExists(registerDto.Username))
                return BadRequest("Username is taken");

            var user = _mapper.Map<AppUser>(registerDto);

            using var hmac = new HMACSHA512(); // password salt, with using, we tell, we do not need this method again, please dispose

            user.UserName = registerDto.Username.ToLower();
            user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password));
            user.PasswordSalt = hmac.Key;

            //var user = new AppUser
            //{
            //    UserName = registerDto.Username.ToLower(),
            //    PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password)),
            //    PasswordSalt = hmac.Key
            //}; // ovo je bez mapper-a


            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok(new UserDto
            {
                Username = user.UserName,
                Token = _tokenService.CreateToken(user),
                KnownAs = user.KnowAs,
            });
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
        {
            //Find is good when we know primary (composiste) key, in another case FirstOrDefault is better
            // ovde cemo koristiti signe or degault je ce vratit jedan jer sigurno imamo jedinstven username. FirstOrDefault ako ima vise username sa istim imenom vratit ce prvi, a SingleOrDefault ce baciti exception.
            var user = await _context.Users.Include(x => x.Photos).SingleOrDefaultAsync(x => x.UserName == loginDto.Username);

            if (user is null)
                return Unauthorized("Invalid username!");

            using var hmac = new HMACSHA512(user.PasswordSalt);
            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));

            for(int i = 0; i< computedHash.Length; i++)
            {
                if (computedHash[i] != user.PasswordHash[i])
                    return Unauthorized("Invalid password!");
            }

            return new UserDto
            {
                Username = user.UserName,
                Token = _tokenService.CreateToken(user),
                PhotoUrl = user.Photos.FirstOrDefault(x => x.IsMain)?.Url,
                KnownAs = user.KnowAs
            };
        }

        private async Task<bool> UserExists(string username)
        {
            return await _context.Users.AnyAsync(x => x.UserName == username.ToLower());
        }

    }
}
