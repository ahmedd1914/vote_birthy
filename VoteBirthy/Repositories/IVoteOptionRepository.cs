using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VoteBirthy.DTOs;
using VoteBirthy.Models;

namespace VoteBirthy.Repositories
{
    public interface IVoteOptionRepository : IRepository<VoteOption>
    {
        Task<IEnumerable<VoteOption>> GetByVoteIdAsync(Guid voteId);
        Task<IEnumerable<VoteOptionDto>> GetByVoteIdDtoAsync(Guid voteId);
        Task<VoteOption> GetByIdWithDetailsAsync(Guid id);
        Task<VoteOptionDto> GetByIdDtoAsync(Guid id);
    }
} 