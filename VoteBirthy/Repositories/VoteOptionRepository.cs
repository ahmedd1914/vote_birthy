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
    public class VoteOptionRepository : EfRepository<VoteOption>, IVoteOptionRepository
    {
        private readonly AppDbContext _context;
        private readonly ILogger<VoteOptionRepository> _logger;

        public VoteOptionRepository(AppDbContext context, ILogger<VoteOptionRepository> logger = null) : base(context)
        {
            _context = context;
            _logger = logger;
        }

        public override async Task<VoteOption> GetByIdAsync(Guid id)
        {
            _logger?.LogInformation("Getting vote option by ID: {VoteOptionId}", id);
            return await _context.VoteOptions
                .AsNoTracking()
                .Include(vo => vo.Vote)
                .Include(vo => vo.Gift)
                .FirstOrDefaultAsync(vo => vo.Id == id);
        }

        public async Task<VoteOptionDto> GetByIdDtoAsync(Guid id)
        {
            _logger?.LogInformation("Getting vote option DTO by ID: {VoteOptionId}", id);
            return await _context.VoteOptions
                .AsNoTracking()
                .Where(vo => vo.Id == id)
                .Select(vo => new VoteOptionDto
                {
                    Id = vo.Id,
                    VoteId = vo.VoteId,
                    GiftId = vo.GiftId,
                    GiftName = vo.Gift.Name,
                    GiftDescription = vo.Gift.Description,
                    VoteCount = vo.Casts.Count,
                    Voters = vo.Casts.Select(c => new VoterDto
                    {
                        Id = c.VoterId,
                        FullName = c.Voter.FullName,
                        VoteDate = c.CastDate
                    }).ToList()
                })
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<VoteOption>> GetByVoteIdAsync(Guid voteId)
        {
            _logger?.LogInformation("Getting vote options for vote ID: {VoteId}", voteId);
            return await _context.VoteOptions
                .AsNoTracking()
                .Include(vo => vo.Gift)
                .Where(vo => vo.VoteId == voteId)
                .ToListAsync();
        }

        public async Task<IEnumerable<VoteOptionDto>> GetByVoteIdDtoAsync(Guid voteId)
        {
            _logger?.LogInformation("Getting vote option DTOs for vote ID: {VoteId}", voteId);
            return await _context.VoteOptions
                .AsNoTracking()
                .Where(vo => vo.VoteId == voteId)
                .Select(vo => new VoteOptionDto
                {
                    Id = vo.Id,
                    VoteId = vo.VoteId,
                    GiftId = vo.GiftId,
                    GiftName = vo.Gift.Name,
                    GiftDescription = vo.Gift.Description,
                    VoteCount = vo.Casts.Count
                })
                .ToListAsync();
        }

        public async Task<VoteOption> GetByIdWithDetailsAsync(Guid id)
        {
            _logger?.LogInformation("Getting vote option with details by ID: {VoteOptionId}", id);
            return await _context.VoteOptions
                .Include(vo => vo.Gift)
                .Include(vo => vo.Vote)
                .Include(vo => vo.Casts)
                    .ThenInclude(c => c.Voter)
                .FirstOrDefaultAsync(vo => vo.Id == id);
        }

        public override async Task<bool> AddAsync(VoteOption voteOption)
        {
            _logger?.LogInformation("Adding vote option for vote ID: {VoteId}, gift ID: {GiftId}", 
                voteOption.VoteId, voteOption.GiftId);
            
            try
            {
                _context.VoteOptions.Add(voteOption);
                var result = await _context.SaveChangesAsync() > 0;
                _logger?.LogInformation("Vote option add result: {Result}", result);
                return result;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error adding vote option {VoteOptionId}", voteOption.Id);
                throw;
            }
        }

        public override async Task<bool> UpdateAsync(VoteOption voteOption)
        {
            _logger?.LogInformation("Updating vote option: {VoteOptionId}", voteOption.Id);
            
            try
            {
                _context.VoteOptions.Update(voteOption);
                var result = await _context.SaveChangesAsync() > 0;
                _logger?.LogInformation("Vote option update result: {Result}", result);
                return result;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error updating vote option {VoteOptionId}", voteOption.Id);
                throw;
            }
        }

        public override async Task<bool> DeleteAsync(Guid id)
        {
            _logger?.LogInformation("Deleting vote option: {VoteOptionId}", id);
            
            var voteOption = await GetByIdAsync(id);
            if (voteOption == null)
            {
                _logger?.LogWarning("Vote option not found for deletion: {VoteOptionId}", id);
                return false;
            }

            try
            {
                _context.VoteOptions.Remove(voteOption);
                var result = await _context.SaveChangesAsync() > 0;
                _logger?.LogInformation("Vote option delete result: {Result}", result);
                return result;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error deleting vote option {VoteOptionId}", id);
                throw;
            }
        }
    }
} 