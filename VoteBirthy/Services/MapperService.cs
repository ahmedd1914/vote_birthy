using System;
using System.Collections.Generic;
using System.Linq;
using VoteBirthy.DTOs;
using VoteBirthy.Models;

namespace VoteBirthy.Services
{
    public class MapperService : IMapperService
    {
        public EmployeeDto MapToEmployeeDto(Employee employee)
        {
            if (employee == null) return null;
            
            return new EmployeeDto
            {
                Id = employee.Id,
                Username = employee.Username,
                FullName = employee.FullName,
                DateOfBirth = employee.DateOfBirth
            };
        }

        public GiftDto MapToGiftDto(Gift gift)
        {
            if (gift == null) return null;
            
            return new GiftDto
            {
                Id = gift.Id,
                Name = gift.Name,
                Description = gift.Description
            };
        }

        public VoteDto MapToVoteDto(Vote vote)
        {
            if (vote == null) return null;
            
            var voteDto = new VoteDto
            {
                Id = vote.Id,
                BirthdayEmpId = vote.BirthdayEmpId,
                BirthdayEmpName = vote.BirthdayEmp?.FullName,
                StartedById = vote.StartedById,
                StartedByName = vote.StartedBy?.FullName,
                StartDate = vote.StartDate,
                EndDate = vote.EndDate,
                IsClosed = vote.IsClosed,
                Options = new List<VoteOptionDto>()
            };

            if (vote.Options != null)
            {
                foreach (var option in vote.Options)
                {
                    voteDto.Options.Add(MapToVoteOptionDto(option));
                }
            }

            return voteDto;
        }

        public VoteOptionDto MapToVoteOptionDto(VoteOption option)
        {
            if (option == null) return null;
            
            var optionDto = new VoteOptionDto
            {
                Id = option.Id,
                VoteId = option.VoteId,
                GiftId = option.GiftId,
                GiftName = option.Gift?.Name,
                GiftDescription = option.Gift?.Description,
                VoteCount = option.Casts?.Count ?? 0,
                Voters = new List<VoterDto>()
            };

            if (option.Casts != null)
            {
                foreach (var cast in option.Casts)
                {
                    optionDto.Voters.Add(MapToVoterDto(cast.Voter, cast.CastDate));
                }
            }

            return optionDto;
        }

        public VoterDto MapToVoterDto(Employee employee, DateTime voteDate)
        {
            if (employee == null) return null;
            
            return new VoterDto
            {
                Id = employee.Id,
                FullName = employee.FullName,
                VoteDate = voteDate
            };
        }

        public IEnumerable<VoteDto> MapToVoteDtoList(IEnumerable<Vote> votes)
        {
            if (votes == null) return Enumerable.Empty<VoteDto>();
            
            var voteDtos = new List<VoteDto>();
            foreach (var vote in votes)
            {
                voteDtos.Add(MapToVoteDto(vote));
            }
            
            return voteDtos;
        }
    }
} 