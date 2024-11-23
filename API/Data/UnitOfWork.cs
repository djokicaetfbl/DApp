using API.Interfaces;
using AutoMapper;

namespace API.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;

        public UnitOfWork(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        IUserRepository IUnitOfWork.UserRepository => new UserRepository(_context, _mapper);

        IMessageRepository IUnitOfWork.MessageRepository => new MessageRepository(_context, _mapper);

        ILikesRepository IUnitOfWork.LikesRepository => new LikesRepository(_context);

        async Task<bool> IUnitOfWork.Complete()
        {
            return await _context.SaveChangesAsync() > 0; // ako je bilo izmjena
        }

        bool IUnitOfWork.HasChanges()
        {
            return _context.ChangeTracker.HasChanges();
        }
    }
}
