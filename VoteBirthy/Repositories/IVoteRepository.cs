using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VoteBirthy.DTOs;
using VoteBirthy.Models;

namespace VoteBirthy.Repositories
{
    public interface IVoteRepository : IRepository<Vote>
    {
        Task<IEnumerable<Vote>> GetActiveVotesAsync();
        Task<IEnumerable<VoteDto>> GetActiveVotesDtoAsync();
        Task<IEnumerable<Vote>> GetCompletedVotesAsync();
        Task<IEnumerable<VoteDto>> GetCompletedVotesDtoAsync();
        Task<Vote> GetByIdWithOptionsAsync(Guid id);
        Task<VoteDto> GetByIdWithOptionsDtoAsync(Guid id);
        Task<Vote> GetVoteForBirthdayEmployeeAsync(Guid employeeId);
        Task<IEnumerable<Vote>> GetVotesByBirthDateAsync(DateTime birthDate);
        Task<bool> HasOpenVoteForBirthdayAsync(DateTime birthDate, int year, Guid employeeId);
        Task<bool> CloseVoteAsync(Guid id);
    }
} 