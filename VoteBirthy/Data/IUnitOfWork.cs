using System;
using System.Threading.Tasks;
using VoteBirthy.Repositories;

namespace VoteBirthy.Data
{
    public interface IUnitOfWork : IDisposable
    {
        IEmployeeRepository Employees { get; }
        IGiftRepository Gifts { get; }
        IVoteRepository Votes { get; }
        IVoteOptionRepository VoteOptions { get; }
        IVoteCastRepository VoteCasts { get; }
        
        Task<int> SaveChangesAsync();
        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
    }
} 