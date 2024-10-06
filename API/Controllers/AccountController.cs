using API.DTOs;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    public class AccountController : BaseApiController
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly ITokenService _tokenService;
        private readonly IMapper _mapper;

        public AccountController(UserManager<AppUser> userManager, ITokenService tokenService, IMapper mapper)
        {
            _userManager = userManager;
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
            user.UserName = registerDto.Username.ToLower();
            
            var  result = await _userManager.CreateAsync(user, registerDto.Password);

            if (!result.Succeeded)
                return BadRequest(result.Errors);

            var roleResult = await _userManager.AddToRoleAsync(user, "Member");
            
            if(!roleResult.Succeeded)
                return BadRequest(roleResult.Errors);

            return Ok(new UserDto
            {
                Username = user.UserName,
                Token = await _tokenService.CreateToken(user),
                KnownAs = user.KnowAs,
                Gender = user.Gender
            });
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
        {
            //Find is good when we know primary (composiste) key, in another case FirstOrDefault is better
            // ovde cemo koristiti signe or degault je ce vratit jedan jer sigurno imamo jedinstven username. FirstOrDefault ako ima vise username sa istim imenom vratit ce prvi, a SingleOrDefault ce baciti exception.
            var user = await _userManager.Users.Include(x => x.Photos).SingleOrDefaultAsync(x => x.UserName == loginDto.Username);

            if (user is null)
                return Unauthorized("Invalid username!");

            var result = await _userManager.CheckPasswordAsync(user, loginDto.Password);

            if(!result)
                return Unauthorized("Invalid password!");  

            return new UserDto
            {
                Username = user.UserName,
                Token = await _tokenService.CreateToken(user),
                PhotoUrl = user.Photos.FirstOrDefault(x => x.IsMain)?.Url,
                KnownAs = user.KnowAs,
                Gender = user.Gender
            };
        }

        private async Task<bool> UserExists(string username)
        {
            return await _userManager.Users.AnyAsync(x => x.UserName == username.ToLower());
        }

    }
}
