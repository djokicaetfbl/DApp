﻿using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class LikesController : BaseApiController
    {
        private readonly IUserRepository _userRepository;
        private readonly ILikesRepository _likesRepository;

        public LikesController(IUserRepository userRepository, ILikesRepository likesRepository)
        {
            _userRepository = userRepository;
            _likesRepository = likesRepository;
        }

        [HttpPost("{username}")]
        public async Task<IActionResult> AddLike(string username)
        {
            var sourceUserId = User.GetUserId(); // implemtirana kao extenstion metoda za ClaimsPrincipal kako bi imali info o logovanom korisniku
            var likedUser = await _userRepository.GetUserByUsernameAsync(username);
            var sourceUser = await _likesRepository.GetUserWithLikes(sourceUserId);

            if (likedUser == null) return NotFound();

            if (sourceUser.UserName == username) return BadRequest("You cannot like yourself!"); // ovo su super stvari jer se na ovakavnacin branimo od napadaca kroz postman npr.!

            var userLike = await _likesRepository.GetUserLike(sourceUserId, likedUser.Id);

            if (userLike != null) return BadRequest("You already like this user!");

            userLike = new UserLike
            {
                SourceUserId = sourceUserId,
                TargetUserId = likedUser.Id,
            };

            sourceUser.LikedUsers.Add(userLike); // uspjesno si lajkovao korisnika hehe

            if (await _userRepository.SaveAllAsync()) return Ok(); // ovo je privremeno like repo ce imati svoj save

            return BadRequest("Failed to like user");
        }

        [HttpGet]
        public async Task<ActionResult<PagedList<LikeDto>>> GetUserlikes([FromQuery] LikesParams likesParams) // like user or liked by user, (one kome se ja svidjam pa su mi ostavili lajk, ili one koje se meni svidjaju pa sam ja njima ostavio lajk)
        {
            // [FromQuery] - iz query ce doci podaci, sa fronta
            likesParams.UserId = User.GetUserId();


            var users = await _likesRepository.GetUserLikes(likesParams);

            Response.AddPaginationHeader(new PaginationHeader(users.CurrentPage, users.PageSize, users.TotalCount, users.TotalPages));

            return Ok(users);
        }

    }
}