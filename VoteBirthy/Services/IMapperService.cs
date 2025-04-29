using System.Collections.Generic;
using VoteBirthy.DTOs;
using VoteBirthy.Models;

namespace VoteBirthy.Services
{
    public interface IMapperService
    {
        EmployeeDto MapToEmployeeDto(Employee employee);
        GiftDto MapToGiftDto(Gift gift);
        VoteDto MapToVoteDto(Vote vote);
        VoteOptionDto MapToVoteOptionDto(VoteOption option);
        VoterDto MapToVoterDto(Employee employee, System.DateTime voteDate);
        IEnumerable<VoteDto> MapToVoteDtoList(IEnumerable<Vote> votes);
    }
} 