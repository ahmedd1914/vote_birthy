using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VoteBirthy.DTOs;
using VoteBirthy.ViewModels;

namespace VoteBirthy.Services
{
    public interface IVoteService
    {
        Task<Guid> StartVoteAsync(Guid birthdayEmpId, Guid startedById, IEnumerable<Guid> giftOptionIds, DateTime? endDate = null);
        Task<bool> CastVoteAsync(Guid voteId, Guid voterId, Guid voteOptionId);
        Task<bool> EndVoteAsync(Guid voteId, Guid endedById);
        Task<VoteResultDto> GetResultsAsync(Guid voteId);
        Task<IEnumerable<VoteDto>> GetActiveVotesAsync();
        Task<IEnumerable<VoteDto>> GetCompletedVotesAsync();
        Task<VoteDto> GetVoteByIdAsync(Guid voteId);
        Task<VoteDto> GetVoteWithOptionsAsync(Guid voteId);
        Task<bool> HasUserVotedAsync(Guid voteId, Guid userId);
        Task<bool> CanUserVoteAsync(Guid voteId, Guid userId);
        Task<VoteOptionDto> GetWinningOptionAsync(Guid voteId);
        Task<bool> UpdateVoteEndDateAsync(Guid voteId, DateTime? endDate);
        Task<bool> CanViewVoteAsync(Guid voteId, Guid userId);
        Task<VoteDetailsViewModel> GetVoteDetailsViewModelAsync(Guid voteId, Guid currentUserId);
        Task<VoteCreateViewModel> PrepareVoteCreateViewModelAsync(Guid currentUserId);
    }

    public class VoteResultDto
    {
        public Guid VoteId { get; set; }
        public bool IsClosed { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public IEnumerable<GiftResultDto> Options { get; set; }
        public IEnumerable<VoterDto> Voters { get; set; }
        public IEnumerable<EmployeeDto> NotVoted { get; set; }
    }

    public class GiftResultDto
    {
        public Guid GiftId { get; set; }
        public string GiftName { get; set; }
        public int VoteCount { get; set; }
        public IEnumerable<VoterDto> Voters { get; set; }
    }
} 