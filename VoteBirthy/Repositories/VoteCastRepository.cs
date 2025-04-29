using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VoteBirthy.Data;
using VoteBirthy.DTOs;
using VoteBirthy.Models;

namespace VoteBirthy.Repositories
{
    public class VoteCastRepository : EfRepository<VoteCast>, IVoteCastRepository
    {
        private readonly AppDbContext _context;
        private readonly ILogger<VoteCastRepository> _logger;

        public VoteCastRepository(AppDbContext context, ILogger<VoteCastRepository> logger = null) : base(context)
        {
            _context = context;
            _logger = logger;
        }

        public override async Task<VoteCast> GetByIdAsync(Guid id)
        {
            _logger?.LogInformation("Getting vote cast by ID: {VoteCastId}", id);
            return await _context.VoteCasts
                .AsNoTracking()
                .Include(vc => vc.VoteOption)
                .Include(vc => vc.Voter)
                .FirstOrDefaultAsync(vc => vc.VoteCastId == id);
        }

        public async Task<IEnumerable<VoteCast>> GetByVoteIdAsync(Guid voteId)
        {
            _logger?.LogInformation("Getting vote casts for vote ID: {VoteId}", voteId);
            return await _context.VoteCasts
                .AsNoTracking()
                .Include(vc => vc.VoteOption)
                .Include(vc => vc.Voter)
                .Where(vc => vc.VoteOption.VoteId == voteId)
                .ToListAsync();
        }

        public async Task<IEnumerable<VoterDto>> GetVotersByVoteIdAsync(Guid voteId)
        {
            _logger?.LogInformation("Getting voters for vote ID: {VoteId}", voteId);
            return await _context.VoteCasts
                .AsNoTracking()
                .Where(vc => vc.VoteOption.VoteId == voteId)
                .Select(vc => new VoterDto
                {
                    Id = vc.VoterId,
                    FullName = vc.Voter.FullName,
                    VoteDate = vc.CastDate
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<VoteCast>> GetByVoteOptionIdAsync(Guid voteOptionId)
        {
            _logger?.LogInformation("Getting vote casts for vote option ID: {VoteOptionId}", voteOptionId);
            return await _context.VoteCasts
                .AsNoTracking()
                .Include(vc => vc.Voter)
                .Where(vc => vc.VoteOptionId == voteOptionId)
                .ToListAsync();
        }

        public async Task<IEnumerable<VoteCast>> GetByVoterIdAsync(Guid voterId)
        {
            _logger?.LogInformation("Getting vote casts for voter ID: {VoterId}", voterId);
            return await _context.VoteCasts
                .AsNoTracking()
                .Include(vc => vc.VoteOption)
                    .ThenInclude(vo => vo.Vote)
                .Include(vc => vc.VoteOption)
                    .ThenInclude(vo => vo.Gift)
                .Where(vc => vc.VoterId == voterId)
                .ToListAsync();
        }

        public async Task<bool> HasVotedAsync(Guid voteId, Guid voterId)
        {
            _logger?.LogInformation("Checking if user {VoterId} has voted in vote {VoteId}", voterId, voteId);
            return await _context.VoteCasts
                .AsNoTracking()
                .AnyAsync(vc => vc.VoteOption.VoteId == voteId && vc.VoterId == voterId);
        }

        public override async Task<bool> AddAsync(VoteCast voteCast)
        {
            _logger?.LogInformation("Adding vote cast for voter {VoterId} on option {VoteOptionId}", 
                voteCast.VoterId, voteCast.VoteOptionId);
                
            _context.VoteCasts.Add(voteCast);
            return await _context.SaveChangesAsync() > 0;
        }

        public override async Task<bool> UpdateAsync(VoteCast voteCast)
        {
            _logger?.LogInformation("Updating vote cast {VoteCastId}", voteCast.VoteCastId);
            
            _context.VoteCasts.Update(voteCast);
            return await _context.SaveChangesAsync() > 0;
        }

        public override async Task<bool> DeleteAsync(Guid id)
        {
            _logger?.LogInformation("Deleting vote cast {VoteCastId}", id);
            
            var voteCast = await GetByIdAsync(id);
            if (voteCast == null)
                return false;

            _context.VoteCasts.Remove(voteCast);
            return await _context.SaveChangesAsync() > 0;
        }
    }
} 