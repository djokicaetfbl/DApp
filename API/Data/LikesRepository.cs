using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class LikesRepository : ILikesRepository
    {
        private readonly DataContext _context;

        public LikesRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<UserLike> GetUserLike(int sourceUserId, int targetUserId)
        {
            return await _context.Likes.FindAsync(sourceUserId, targetUserId);
        }

        /*
         * The query is only executed, and the data fetched from the database, when you use a method that enumerates the results, such as:
            ToList(): Converts the result to a list and executes the query.
            ToArray(): Converts the result to an array and executes the query.
            First() / FirstOrDefault(): Retrieves the first element of the result set and executes the query.
            Single() / SingleOrDefault(): Retrieves a single element and executes the query.
            Count(): Counts the number of elements and executes the query.
            Any(): Determines if any elements exist in the result set and executes the query.
            Until one of these methods or similar ones is called, the query remains unexecuted, allowing for deferred execution and query optimization by the query provider
         */

        public async Task<PagedList<LikeDto>> GetUserLikes(LikesParams likesParams)
        {
            var users = _context.Users.OrderBy(x => x.UserName).AsQueryable(); // ovo je trenutno queryable i sa nasom bazom podataka se trenutno nece desiti nista, nista se nece izvrsiti
            var likes = _context.Likes.AsQueryable();

            if(likesParams.Predicate == "liked") 
            {
                likes = likes.Where(like => like.SourceUserId == likesParams.UserId);
                users = likes.Select(like => like.TargetUser);
            }            

            if(likesParams.Predicate == "likedBy") 
            {
                likes = likes.Where(like => like.TargetUserId == likesParams.UserId);
                users = likes.Select(like => like.SourceUser);
            }

            var likedUser = users.Select(user => new LikeDto
            {
                UserName = user.UserName,
                KnownAs = user.KnowAs,
                Age = user.DateOfBirth.CalculateAge(),
                PhotoUrl = user.Photos.FirstOrDefault(x => x.IsMain).Url,
                City = user.City,
                Id = user.Id,
            }); // select se ne izvrsava direkt na bazu!

            return await PagedList<LikeDto>.CreateAsync(likedUser, likesParams.PageNumber, likesParams.PageSize);
        }

        public async Task<AppUser> GetUserWithLikes(int userId)
        {
            return await _context.Users.Include(x => x.LikedUsers)
                .FirstOrDefaultAsync(x => x.Id == userId);
        }
    }
}
