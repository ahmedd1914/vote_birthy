using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VoteBirthy.DTOs;
using VoteBirthy.Models;

namespace VoteBirthy.Repositories
{
    public interface IVoteCastRepository : IRepository<VoteCast>
    {
        Task<IEnumerable<VoteCast>> GetByVoteIdAsync(Guid voteId);
        Task<IEnumerable<VoterDto>> GetVotersByVoteIdAsync(Guid voteId);
        Task<IEnumerable<VoteCast>> GetByVoteOptionIdAsync(Guid voteOptionId);
        Task<IEnumerable<VoteCast>> GetByVoterIdAsync(Guid voterId);
        Task<bool> HasVotedAsync(Guid voteId, Guid voterId);
    }
} 