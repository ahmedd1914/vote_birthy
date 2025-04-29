using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using VoteBirthy.Data;
using VoteBirthy.DTOs;
using VoteBirthy.Models;
using VoteBirthy.ViewModels;

namespace VoteBirthy.Services
{
    public class VoteService : IVoteService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapperService _mapper;
        private readonly ILogger<VoteService> _logger;

        public VoteService(
            IUnitOfWork unitOfWork, 
            IMapperService mapper,
            ILogger<VoteService> logger = null)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Guid> StartVoteAsync(Guid birthdayEmpId, Guid startedById, IEnumerable<Guid> giftOptionIds, DateTime? endDate = null)
        {
            _logger?.LogInformation("StartVoteAsync called with birthdayEmpId={BirthdayEmpId}, startedById={StartedById}, giftOptions={GiftOptionCount}", 
                birthdayEmpId, startedById, giftOptionIds?.Count() ?? 0);
            
            // Check if both IDs are valid
            var birthdayEmp = await _unitOfWork.Employees.GetByIdAsync(birthdayEmpId);
            var startedBy = await _unitOfWork.Employees.GetByIdAsync(startedById);

            if (birthdayEmp == null || startedBy == null)
            {
                _logger?.LogWarning("Invalid employee ID(s) detected");
                throw new ArgumentException("Invalid employee ID(s)");
            }

            // Prevent creating a vote for yourself
            if (birthdayEmpId == startedById)
            {
                _logger?.LogWarning("User attempted to create vote for their own birthday");
                throw new InvalidOperationException("You cannot start a vote for your own birthday");
            }

            // Rule 1: Check if there's already an open vote for this birthday employee in the current year
            var currentYear = DateTime.Now.Year;
            var hasActiveVoteForEmployee = await _unitOfWork.Votes.HasOpenVoteForBirthdayAsync(
                birthdayEmp.DateOfBirth, currentYear, birthdayEmpId);
            
            if (hasActiveVoteForEmployee)
            {
                _logger?.LogWarning("Active vote already exists for employee {EmployeeId} in year {Year}", 
                    birthdayEmpId, currentYear);
                throw new InvalidOperationException($"There's already an active vote for this employee's birthday in {currentYear}");
            }

            // Check if we have enough gift options
            if (giftOptionIds == null || giftOptionIds.Count() < 2)
            {
                _logger?.LogWarning("Not enough gift options: {GiftOptionCount}", giftOptionIds?.Count() ?? 0);
                throw new ArgumentException("At least 2 gift options are required");
            }

            try
            {
                // Start a transaction for the entire operation
                await _unitOfWork.BeginTransactionAsync();

                // Create the vote
                var vote = new Vote
                {
                    Id = Guid.NewGuid(),
                    BirthdayEmpId = birthdayEmpId,
                    StartedById = startedById,
                    StartDate = DateTime.Now,
                    EndDate = endDate,
                    IsClosed = false
                };

                // Add the vote to the database
                var result = await _unitOfWork.Votes.AddAsync(vote);
                if (!result)
                {
                    _logger?.LogError("Failed to create vote");
                    await _unitOfWork.RollbackTransactionAsync();
                    throw new InvalidOperationException("Failed to create vote");
                }

                // Add vote options
                int optionsAdded = 0;
                foreach (var giftId in giftOptionIds)
                {
                    // Verify the gift exists
                    var gift = await _unitOfWork.Gifts.GetByIdAsync(giftId);
                    if (gift == null)
                    {
                        _logger?.LogWarning("Gift not found: {GiftId}", giftId);
                        continue;
                    }

                    // Create the vote option
                    var voteOption = new VoteOption
                    {
                        Id = Guid.NewGuid(),
                        VoteId = vote.Id,
                        GiftId = giftId
                    };

                    // Add the vote option to the database
                    result = await _unitOfWork.VoteOptions.AddAsync(voteOption);
                    if (result)
                    {
                        optionsAdded++;
                        _logger?.LogInformation("Added vote option for gift: {GiftName}", gift.Name);
                    }
                    else
                    {
                        _logger?.LogWarning("Failed to add vote option for gift: {GiftId}", giftId);
                    }
                }

                // Ensure we have at least 2 options
                if (optionsAdded < 2)
                {
                    _logger?.LogError("Failed to add enough vote options");
                    await _unitOfWork.RollbackTransactionAsync();
                    throw new InvalidOperationException("Failed to add enough vote options");
                }

                // Commit the transaction
                await _unitOfWork.CommitTransactionAsync();

                _logger?.LogInformation("Vote creation completed. Total options added: {OptionsAdded}", optionsAdded);
                return vote.Id;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error in StartVoteAsync");
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }
        }

        public async Task<bool> CastVoteAsync(Guid voteId, Guid voterId, Guid voteOptionId)
        {
            _logger?.LogInformation("CastVoteAsync called with voteId={VoteId}, voterId={VoterId}, optionId={OptionId}", 
                voteId, voterId, voteOptionId);
            
            // Check if the vote exists and is not closed
            var vote = await _unitOfWork.Votes.GetByIdAsync(voteId);
            
            if (vote == null || vote.IsClosed)
            {
                _logger?.LogWarning("Vote not found or already closed: {VoteId}", voteId);
                throw new InvalidOperationException("Vote not found or already closed");
            }

            // Rule: The birthday person cannot vote
            if (vote.BirthdayEmpId == voterId)
            {
                _logger?.LogWarning("Birthday person attempted to vote in their own birthday vote");
                throw new InvalidOperationException("The birthday person cannot participate in their own vote");
            }

            // Check if the voter has already voted in this vote
            var hasVoted = await _unitOfWork.VoteCasts.HasVotedAsync(voteId, voterId);
            
            if (hasVoted)
            {
                _logger?.LogWarning("User {UserId} has already voted in poll {VoteId}", voterId, voteId);
                throw new InvalidOperationException("You can vote only once in this poll");
            }

            // Check if the vote option is valid for this vote
            var option = await _unitOfWork.VoteOptions.GetByIdAsync(voteOptionId);
            
            if (option == null || option.VoteId != voteId)
            {
                _logger?.LogWarning("Invalid vote option {OptionId} for vote {VoteId}", voteOptionId, voteId);
                throw new ArgumentException("Invalid vote option");
            }

            try
            {
                // Start a transaction
                await _unitOfWork.BeginTransactionAsync();

                // Record the vote
                var voteCast = new VoteCast
                {
                    Id = Guid.NewGuid(),
                    VoteOptionId = option.Id,
                    VoterId = voterId,
                    CastDate = DateTime.Now
                };

                var result = await _unitOfWork.VoteCasts.AddAsync(voteCast);

                if (!result)
                {
                    _logger?.LogError("Failed to record vote for user {UserId} in vote {VoteId}", voterId, voteId);
                    await _unitOfWork.RollbackTransactionAsync();
                    return false;
                }

                // Commit the transaction
                await _unitOfWork.CommitTransactionAsync();
                _logger?.LogInformation("Vote cast recorded successfully for user {UserId} in vote {VoteId}", voterId, voteId);
                return true;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error in CastVoteAsync for user {UserId} in vote {VoteId}", voterId, voteId);
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }
        }

        public async Task<bool> EndVoteAsync(Guid voteId, Guid endedById)
        {
            _logger?.LogInformation("EndVoteAsync called with voteId={VoteId}, endedById={EndedById}", voteId, endedById);
            
            var vote = await _unitOfWork.Votes.GetByIdAsync(voteId);
            
            if (vote == null)
            {
                _logger?.LogWarning("Vote not found: {VoteId}", voteId);
                throw new InvalidOperationException("Vote not found");
            }
            
            if (vote.IsClosed)
            {
                _logger?.LogWarning("Vote {VoteId} is already closed", voteId);
                throw new InvalidOperationException("This vote is already closed");
            }
            
            // Rule: Only the person who started the vote can end it
            if (vote.StartedById != endedById)
            {
                _logger?.LogWarning("User {UserId} attempting to close vote {VoteId} is not the creator", endedById, voteId);
                throw new InvalidOperationException("Only the person who started the vote can end it");
            }
            
            try
            {
                // Start a transaction
                await _unitOfWork.BeginTransactionAsync();
                
                // Update the vote
                vote.IsClosed = true;
                vote.EndDate = DateTime.Now;
                
                var result = await _unitOfWork.Votes.UpdateAsync(vote);
                
                if (!result)
                {
                    _logger?.LogError("Failed to update vote {VoteId}", voteId);
                    await _unitOfWork.RollbackTransactionAsync();
                    return false;
                }
                
                // Commit the transaction
                await _unitOfWork.CommitTransactionAsync();
                _logger?.LogInformation("Vote {VoteId} closed successfully", voteId);
                return true;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error in EndVoteAsync for vote {VoteId}", voteId);
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }
        }

        public async Task<VoteOptionDto> GetWinningOptionAsync(Guid voteId)
        {
            _logger?.LogInformation("GetWinningOptionAsync called for vote {VoteId}", voteId);
            
            var vote = await _unitOfWork.Votes.GetByIdAsync(voteId);
            if (vote == null)
            {
                _logger?.LogWarning("Vote not found: {VoteId}", voteId);
                throw new ArgumentException("Vote not found");
            }

            if (!vote.IsClosed)
            {
                _logger?.LogWarning("Cannot determine winner for open vote {VoteId}", voteId);
                throw new InvalidOperationException("Cannot determine winner for open vote");
            }

            // Use optimized repository methods to get vote options with vote counts
            var options = await _unitOfWork.VoteOptions.GetByVoteIdDtoAsync(voteId);
            
            if (options == null || !options.Any())
            {
                _logger?.LogWarning("No options found for vote {VoteId}", voteId);
                return null;
            }
            
            // Find the option with the most votes
            var winningOption = options.OrderByDescending(o => o.VoteCount).FirstOrDefault();
            
            _logger?.LogInformation("Found winning option for vote {VoteId}", voteId);
            return winningOption;
        }

        public async Task<VoteResultDto> GetResultsAsync(Guid voteId)
        {
            _logger?.LogInformation("GetResultsAsync called for vote {VoteId}", voteId);
            
            // Get vote with optimized DTO query
            var voteDto = await _unitOfWork.Votes.GetByIdWithOptionsDtoAsync(voteId);
            if (voteDto == null)
            {
                _logger?.LogWarning("Vote not found: {VoteId}", voteId);
                throw new ArgumentException("Vote not found");
            }

            // Get all voters for this vote with optimized DTO query
            var voters = await _unitOfWork.VoteCasts.GetVotersByVoteIdAsync(voteId);
            
            // Get all employees for reference (excluding the birthday person)
            var allEmployees = await _unitOfWork.Employees.GetAllAsync();
            var eligibleEmployeeDtos = allEmployees
                .Where(e => e.Id != voteDto.BirthdayEmpId)
                .Select(e => _mapper.MapToEmployeeDto(e))
                .ToList();
            
            // Get employees who haven't voted (excluding the birthday person)
            var voterIds = voters.Select(v => v.Id).ToList();
            var notVotedEmployees = eligibleEmployeeDtos
                .Where(e => !voterIds.Contains(e.Id))
                .ToList();

            // Create result options
            var options = new List<GiftResultDto>();
            foreach (var option in voteDto.Options)
            {
                options.Add(new GiftResultDto
                {
                    GiftId = option.GiftId,
                    GiftName = option.GiftName,
                    VoteCount = option.VoteCount,
                    Voters = option.Voters
                });
            }

            return new VoteResultDto
            {
                VoteId = voteDto.Id,
                IsClosed = voteDto.IsClosed,
                StartDate = voteDto.StartDate,
                EndDate = voteDto.EndDate,
                Options = options.OrderByDescending(o => o.VoteCount),
                Voters = voters,
                NotVoted = notVotedEmployees
            };
        }

        public async Task<IEnumerable<VoteDto>> GetActiveVotesAsync()
        {
            _logger?.LogInformation("GetActiveVotesAsync called");
            return await _unitOfWork.Votes.GetActiveVotesDtoAsync();
        }

        public async Task<IEnumerable<VoteDto>> GetCompletedVotesAsync()
        {
            _logger?.LogInformation("GetCompletedVotesAsync called");
            return await _unitOfWork.Votes.GetCompletedVotesDtoAsync();
        }

        public async Task<VoteDto> GetVoteByIdAsync(Guid voteId)
        {
            _logger?.LogInformation("GetVoteByIdAsync called for vote {VoteId}", voteId);
            return await _unitOfWork.Votes.GetByIdWithOptionsDtoAsync(voteId);
        }

        public async Task<VoteDto> GetVoteWithOptionsAsync(Guid voteId)
        {
            _logger?.LogInformation("GetVoteWithOptionsAsync called for vote {VoteId}", voteId);
            return await _unitOfWork.Votes.GetByIdWithOptionsDtoAsync(voteId);
        }

        public async Task<bool> HasUserVotedAsync(Guid voteId, Guid userId)
        {
            _logger?.LogInformation("HasUserVotedAsync called for vote {VoteId}, user {UserId}", voteId, userId);
            return await _unitOfWork.VoteCasts.HasVotedAsync(voteId, userId);
        }

        public async Task<bool> CanUserVoteAsync(Guid voteId, Guid userId)
        {
            _logger?.LogInformation("CanUserVoteAsync called for vote {VoteId}, user {UserId}", voteId, userId);
            var vote = await _unitOfWork.Votes.GetByIdAsync(voteId);
            if (vote == null || vote.IsClosed)
            {
                return false;
            }

            // Rule: Birthday person cannot vote
            if (vote.BirthdayEmpId == userId)
            {
                return false;
            }

            // Check if user has already voted
            return !await HasUserVotedAsync(voteId, userId);
        }

        public async Task<bool> UpdateVoteEndDateAsync(Guid voteId, DateTime? endDate)
        {
            _logger?.LogInformation("UpdateVoteEndDateAsync called with voteId={VoteId}, endDate={EndDate}", 
                voteId, endDate);
            
            var vote = await _unitOfWork.Votes.GetByIdAsync(voteId);
            if (vote == null)
            {
                _logger?.LogWarning("Vote not found: {VoteId}", voteId);
                throw new ArgumentException("Vote not found");
            }
            
            if (vote.IsClosed)
            {
                _logger?.LogWarning("Cannot update end date for closed vote: {VoteId}", voteId);
                throw new InvalidOperationException("Cannot update end date for closed vote");
            }
            
            try
            {
                vote.EndDate = endDate;
                var result = await _unitOfWork.Votes.UpdateAsync(vote);
                
                _logger?.LogInformation("Vote end date updated: {Success}", result);
                return result;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error in UpdateVoteEndDateAsync for vote {VoteId}", voteId);
                throw;
            }
        }
        
        public async Task<bool> CanViewVoteAsync(Guid voteId, Guid userId)
        {
            _logger?.LogInformation("CanViewVoteAsync called with voteId={VoteId}, userId={UserId}", 
                voteId, userId);
            
            var vote = await _unitOfWork.Votes.GetByIdAsync(voteId);
            if (vote == null)
            {
                _logger?.LogWarning("Vote not found: {VoteId}", voteId);
                return false;
            }
            
            // Rule: Birthday person cannot see their own vote
            if (vote.BirthdayEmpId == userId)
            {
                _logger?.LogInformation("User {UserId} is the birthday person and cannot view vote {VoteId}", 
                    userId, voteId);
                return false;
            }
            
            return true;
        }
        
        public async Task<VoteDetailsViewModel> GetVoteDetailsViewModelAsync(Guid voteId, Guid currentUserId)
        {
            _logger?.LogInformation("GetVoteDetailsViewModelAsync called with voteId={VoteId}, currentUserId={UserId}", 
                voteId, currentUserId);
            
            var vote = await _unitOfWork.Votes.GetByIdWithOptionsAsync(voteId);
            if (vote == null)
            {
                _logger?.LogWarning("Vote not found: {VoteId}", voteId);
                throw new ArgumentException("Vote not found");
            }
            
            // Check if user can view this vote
            if (!await CanViewVoteAsync(voteId, currentUserId))
            {
                _logger?.LogWarning("User {UserId} is not allowed to view vote {VoteId}", 
                    currentUserId, voteId);
                throw new InvalidOperationException("You cannot view votes for your own birthday");
            }
            
            var hasVoted = await _unitOfWork.VoteCasts.HasVotedAsync(vote.Id, currentUserId);

            var optionViewModels = new List<VoteOptionViewModel>();
            Guid? votedOptionId = null;

            foreach (var option in vote.Options)
            {
                var voterNames = option.Casts.Select(c => c.Voter.FullName).ToList();
                var voteCount = option.Casts.Count;

                if (hasVoted && option.Casts.Any(c => c.VoterId == currentUserId))
                {
                    votedOptionId = option.Id;
                }

                optionViewModels.Add(new VoteOptionViewModel
                {
                    VoteOptionId = option.Id,
                    GiftName = option.Gift.Name,
                    GiftDescription = option.Gift.Description,
                    VoteCount = voteCount,
                    VoterNames = voterNames
                });
            }

            VoteOptionViewModel winningOption = null;
            List<string> nonVoterNames = null;
            
            if (vote.IsClosed)
            {
                // Get the winning option
                winningOption = optionViewModels.OrderByDescending(o => o.VoteCount).FirstOrDefault();
                
                // Get list of employees who haven't voted
                var allEmployees = await _unitOfWork.Employees.GetAllAsync();
                var eligibleEmployees = allEmployees.Where(e => e.EmployeeId != vote.BirthdayEmpId).ToList();
                
                // Get all who have voted across all options
                var allVoters = optionViewModels
                    .SelectMany(o => o.VoterNames)
                    .Distinct()
                    .ToList();
                
                // Find who didn't vote
                nonVoterNames = eligibleEmployees
                    .Select(e => e.FullName)
                    .Where(name => !allVoters.Contains(name))
                    .ToList();
            }

            var model = new VoteDetailsViewModel
            {
                VoteId = vote.Id,
                BirthdayEmployeeName = vote.BirthdayEmp.FullName,
                StartedByName = vote.StartedBy.FullName,
                StartDate = vote.StartDate,
                EndDate = vote.EndDate,
                IsClosed = vote.IsClosed,
                Options = optionViewModels,
                WinningOption = winningOption,
                HasUserVoted = hasVoted,
                VotedOptionId = votedOptionId.GetValueOrDefault(),
                CanCloseVote = vote.StartedById == currentUserId,
                NonVoterNames = nonVoterNames
            };

            return model;
        }
        
        public async Task<VoteCreateViewModel> PrepareVoteCreateViewModelAsync(Guid currentUserId)
        {
            _logger?.LogInformation("PrepareVoteCreateViewModelAsync called with currentUserId={UserId}", 
                currentUserId);
            
            var employees = await _unitOfWork.Employees.GetAllAsync();
            // Filter out current user from the employee list (can't create a vote for yourself)
            var eligibleEmployees = employees.Where(e => e.EmployeeId != currentUserId).ToList();
            
            var gifts = await _unitOfWork.Gifts.GetAllAsync();

            var model = new VoteCreateViewModel
            {
                EmployeeList = eligibleEmployees.Select(e => new SelectListItem
                {
                    Value = e.EmployeeId.ToString(),
                    Text = $"{e.FullName} ({e.DateOfBirth.ToShortDateString()})"
                }).ToList(),

                GiftList = gifts.Select(g => new SelectListItem
                {
                    Value = g.GiftId.ToString(),
                    Text = $"{g.Name} - {g.Description}"
                }).ToList(),

                SelectedGiftIds = new List<Guid>(),
                
                // Initialize with an empty string to prevent null reference
                BirthdayEmpName = string.Empty
            };

            return model;
        }
    }
} 