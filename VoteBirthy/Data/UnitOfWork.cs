using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using VoteBirthy.Repositories;

namespace VoteBirthy.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _dbContext;
        private readonly ILogger<UnitOfWork> _logger;
        private IDbContextTransaction _transaction;
        private bool _disposed;

        public IEmployeeRepository Employees { get; }
        public IGiftRepository Gifts { get; }
        public IVoteRepository Votes { get; }
        public IVoteOptionRepository VoteOptions { get; }
        public IVoteCastRepository VoteCasts { get; }

        public UnitOfWork(
            AppDbContext dbContext,
            IEmployeeRepository employeeRepository,
            IGiftRepository giftRepository,
            IVoteRepository voteRepository,
            IVoteOptionRepository voteOptionRepository,
            IVoteCastRepository voteCastRepository,
            ILogger<UnitOfWork> logger = null)
        {
            _dbContext = dbContext;
            _logger = logger;
            
            Employees = employeeRepository;
            Gifts = giftRepository;
            Votes = voteRepository;
            VoteOptions = voteOptionRepository;
            VoteCasts = voteCastRepository;
        }

        public async Task BeginTransactionAsync()
        {
            _logger?.LogInformation("Beginning database transaction");
            _transaction = await _dbContext.Database.BeginTransactionAsync();
        }

        public async Task CommitTransactionAsync()
        {
            try
            {
                _logger?.LogInformation("Committing transaction");
                await _transaction?.CommitAsync();
            }
            finally
            {
                if (_transaction != null)
                {
                    _transaction.Dispose();
                    _transaction = null;
                }
            }
        }

        public async Task RollbackTransactionAsync()
        {
            try
            {
                _logger?.LogInformation("Rolling back transaction");
                await _transaction?.RollbackAsync();
            }
            finally
            {
                if (_transaction != null)
                {
                    _transaction.Dispose();
                    _transaction = null;
                }
            }
        }

        public async Task<int> SaveChangesAsync()
        {
            _logger?.LogInformation("Saving changes to database");
            return await _dbContext.SaveChangesAsync();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _transaction?.Dispose();
                    _dbContext.Dispose();
                }
                _disposed = true;
            }
        }
    }
} 