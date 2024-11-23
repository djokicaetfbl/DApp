namespace API.Interfaces
{
    public interface IUnitOfWork
    {
        IUserRepository UserRepository { get; }
        IMessageRepository MessageRepository { get; }

        ILikesRepository LikesRepository { get; }

        Task<bool> Complete(); // UnitOfWork je kao transkacija. Ukoliko imamo vise razlicitih update-a (razlicitih repozirotija) koje zelimo da smjestimo u nasu bazu i npr ako istovremeno koristimo akcije na sva tri Repo, mozemo na ovaj nacin reci ako jedna ne prodje neka sve ne prodju. Tj ako ne prodje Complete sve ide na RollBack. SaveShanges to vec radi :D pa ovde pravimo ekvivalenciju

        bool HasChanges(); // da li EF upratio da se nesto promijenilo unutar nase transakcije
    }
}
