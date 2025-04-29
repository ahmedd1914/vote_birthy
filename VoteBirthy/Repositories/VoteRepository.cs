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
    public class VoteRepository : EfRepository<Vote>, IVoteRepository
    {
        private readonly AppDbContext _context;
        private readonly ILogger<VoteRepository> _logger;

        public VoteRepository(AppDbContext context, ILogger<VoteRepository> logger = null) : base(context)
        {
            _context = context;
            _logger = logger;
        }

        public override async Task<IEnumerable<Vote>> GetAllAsync()
        {
            _logger?.LogInformation("Getting all votes");
            return await _context.Votes
                .AsNoTracking()
                .Include(v => v.BirthdayEmp)
                .Include(v => v.StartedBy)
                .ToListAsync();
        }

        public override async Task<Vote> GetByIdAsync(Guid id)
        {
            _logger?.LogInformation("Getting vote by ID: {VoteId}", id);
            return await _context.Votes
                .AsNoTracking()
                .Include(v => v.BirthdayEmp)
                .Include(v => v.StartedBy)
                .FirstOrDefaultAsync(v => v.Id == id);
        }

        public async Task<Vote> GetByIdWithOptionsAsync(Guid id)
        {
            _logger?.LogInformation("Getting vote with options by ID: {VoteId}", id);
            return await _context.Votes
                .Include(v => v.BirthdayEmp)
                .Include(v => v.StartedBy)
                .Include(v => v.Options)
                    .ThenInclude(o => o.Gift)
                .Include(v => v.Options)
                    .ThenInclude(o => o.Casts)
                        .ThenInclude(c => c.Voter)
                .FirstOrDefaultAsync(v => v.Id == id);
        }

        public async Task<VoteDto> GetByIdWithOptionsDtoAsync(Guid id)
        {
            _logger?.LogInformation("Getting vote DTO with options by ID: {VoteId}", id);
            
            // Project directly to a VoteDTO to avoid loading unnecessary data
            return await _context.Votes
                .AsNoTracking()
                .Where(v => v.Id == id)
                .Select(v => new VoteDto
                {
                    Id = v.Id,
                    BirthdayEmpId = v.BirthdayEmpId,
                    BirthdayEmpName = v.BirthdayEmp.FullName,
                    StartedById = v.StartedById,
                    StartedByName = v.StartedBy.FullName,
                    StartDate = v.StartDate,
                    EndDate = v.EndDate,
                    IsClosed = v.IsClosed,
                    Options = v.Options.Select(o => new VoteOptionDto
                    {
                        Id = o.Id,
                        VoteId = o.VoteId,
                        GiftId = o.GiftId,
                        GiftName = o.Gift.Name,
                        GiftDescription = o.Gift.Description,
                        VoteCount = o.Casts.Count,
                        Voters = o.Casts.Select(c => new VoterDto
                        {
                            Id = c.VoterId,
                            FullName = c.Voter.FullName,
                            VoteDate = c.CastDate
                        }).ToList()
                    }).ToList()
                })
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Vote>> GetActiveVotesAsync()
        {
            _logger?.LogInformation("Getting active votes");
            return await _context.Votes
                .AsNoTracking()
                .Include(v => v.BirthdayEmp)
                .Include(v => v.StartedBy)
                .Where(v => !v.IsClosed)
                .OrderBy(v => v.StartDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<VoteDto>> GetActiveVotesDtoAsync()
        {
            _logger?.LogInformation("Getting active vote DTOs");
            return await _context.Votes
                .AsNoTracking()
                .Where(v => !v.IsClosed)
                .OrderBy(v => v.StartDate)
                .Select(v => new VoteDto
                {
                    Id = v.Id,
                    BirthdayEmpId = v.BirthdayEmpId,
                    BirthdayEmpName = v.BirthdayEmp.FullName,
                    StartedById = v.StartedById,
                    StartedByName = v.StartedBy.FullName,
                    StartDate = v.StartDate,
                    EndDate = v.EndDate,
                    IsClosed = v.IsClosed,
                    Options = v.Options.Select(o => new VoteOptionDto
                    {
                        Id = o.Id,
                        GiftId = o.GiftId,
                        GiftName = o.Gift.Name,
                        VoteCount = o.Casts.Count
                    }).ToList()
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<Vote>> GetCompletedVotesAsync()
        {
            _logger?.LogInformation("Getting completed votes");
            return await _context.Votes
                .AsNoTracking()
                .Include(v => v.BirthdayEmp)
                .Include(v => v.StartedBy)
                .Include(v => v.Options)
                    .ThenInclude(o => o.Gift)
                .Where(v => v.IsClosed)
                .OrderByDescending(v => v.EndDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<VoteDto>> GetCompletedVotesDtoAsync()
        {
            _logger?.LogInformation("Getting completed vote DTOs");
            return await _context.Votes
                .AsNoTracking()
                .Where(v => v.IsClosed)
                .OrderByDescending(v => v.EndDate)
                .Select(v => new VoteDto
                {
                    Id = v.Id,
                    BirthdayEmpId = v.BirthdayEmpId,
                    BirthdayEmpName = v.BirthdayEmp.FullName,
                    StartedById = v.StartedById,
                    StartedByName = v.StartedBy.FullName,
                    StartDate = v.StartDate,
                    EndDate = v.EndDate,
                    IsClosed = v.IsClosed,
                    Options = v.Options.Select(o => new VoteOptionDto
                    {
                        Id = o.Id,
                        GiftId = o.GiftId,
                        GiftName = o.Gift.Name,
                        VoteCount = o.Casts.Count
                    }).ToList()
                })
                .ToListAsync();
        }

        public async Task<Vote> GetVoteForBirthdayEmployeeAsync(Guid employeeId)
        {
            var currentYear = DateTime.Now.Year;
            
            _logger?.LogInformation("Getting vote for birthday employee ID: {EmployeeId} in year {Year}", employeeId, currentYear);
            
            return await _context.Votes
                .AsNoTracking()
                .Include(v => v.BirthdayEmp)
                .Include(v => v.Options)
                    .ThenInclude(o => o.Gift)
                .FirstOrDefaultAsync(v => 
                    v.BirthdayEmpId == employeeId && 
                    v.StartDate.Year == currentYear);
        }

        public async Task<IEnumerable<Vote>> GetVotesByBirthDateAsync(DateTime birthDate)
        {
            _logger?.LogInformation("Getting votes by birth date: {BirthDate}", birthDate);
            return await _context.Votes
                .AsNoTracking()
                .Include(v => v.BirthdayEmp)
                .Where(v => v.BirthdayEmp.DateOfBirth.Month == birthDate.Month && 
                            v.BirthdayEmp.DateOfBirth.Day == birthDate.Day)
                .OrderByDescending(v => v.StartDate.Year)
                .ToListAsync();
        }

        public async Task<bool> HasOpenVoteForBirthdayAsync(DateTime birthDate, int year, Guid employeeId)
        {
            _logger?.LogInformation("Checking for open votes for employee {EmployeeId} with birth date {BirthDate} in year {Year}", 
                employeeId, birthDate, year);
            
            return await _context.Votes
                .AsNoTracking()
                .AnyAsync(v => v.BirthdayEmpId == employeeId &&
                               v.StartDate.Year == year &&
                               !v.IsClosed);
        }

        public override async Task<bool> AddAsync(Vote vote)
        {
            _logger?.LogInformation("Adding new vote: {VoteId} for employee {EmployeeId}", vote.VoteId, vote.BirthdayEmpId);
            Console.WriteLine($"VoteRepository.AddAsync: Adding vote with ID {vote.VoteId}");
            
            // Make sure the Id field is set to the same as VoteId
            vote.Id = vote.VoteId;
            
            try
            {
                // Debug information about the vote entity
                Console.WriteLine($"Vote details before saving:");
                Console.WriteLine($"- VoteId/Id: {vote.VoteId}/{vote.Id}");
                Console.WriteLine($"- BirthdayEmpId: {vote.BirthdayEmpId}");
                Console.WriteLine($"- StartedById: {vote.StartedById}");
                Console.WriteLine($"- StartDate: {vote.StartDate}");
                Console.WriteLine($"- IsClosed: {vote.IsClosed}");
                
                // Check for related entity existence
                var birthdayEmployee = await _context.Employees.FindAsync(vote.BirthdayEmpId);
                var startedByEmployee = await _context.Employees.FindAsync(vote.StartedById);
                
                Console.WriteLine($"BirthdayEmployee exists: {birthdayEmployee != null}");
                Console.WriteLine($"StartedByEmployee exists: {startedByEmployee != null}");
                
                if (birthdayEmployee == null || startedByEmployee == null)
                {
                    Console.WriteLine("ERROR: Referenced employees do not exist!");
                    return false;
                }
                
                // Check for existing OPEN vote for this employee in this year
                var existingVote = await _context.Votes
                    .FirstOrDefaultAsync(v => 
                        v.BirthdayEmpId == vote.BirthdayEmpId && 
                        v.StartDate.Year == vote.StartDate.Year && 
                        !v.IsClosed);
                
                if (existingVote != null)
                {
                    Console.WriteLine($"WARNING: Found existing OPEN vote for same employee this year (ID: {existingVote.VoteId})");
                    _logger?.LogWarning("Open vote already exists for employee {EmployeeId} in year {Year}. Existing vote: {ExistingVoteId}", 
                        vote.BirthdayEmpId, vote.StartDate.Year, existingVote.VoteId);
                    return false;
                }
                
                Console.WriteLine("Adding vote to DbSet");
                _context.Votes.Add(vote);
                
                Console.WriteLine("Calling SaveChangesAsync");
                var changes = await _context.SaveChangesAsync();
                Console.WriteLine($"SaveChangesAsync returned: {changes} changes");
                
                var result = changes > 0;
                _logger?.LogInformation("Vote save result: {Result}", result);
                Console.WriteLine($"Vote save result: {result}");
                return result;
            }
            catch (DbUpdateConcurrencyException ex)
            {
                Console.WriteLine($"DbUpdateConcurrencyException: {ex.Message}");
                _logger?.LogError(ex, "Concurrency exception adding vote {VoteId}", vote.VoteId);
                throw;
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine($"DbUpdateException: {ex.Message}");
                Console.WriteLine($"Inner exception: {ex.InnerException?.Message}");
                _logger?.LogError(ex, "Database update error adding vote {VoteId}", vote.VoteId);
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception adding vote: {ex.GetType().Name}: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                _logger?.LogError(ex, "Error adding vote {VoteId}", vote.VoteId);
                throw;
            }
        }

        public override async Task<bool> UpdateAsync(Vote vote)
        {
            _logger?.LogInformation("Updating vote: {VoteId}", vote.VoteId);
            
            // Make sure the Id field is in sync with VoteId
            vote.Id = vote.VoteId;
            
            try 
            {
                _context.Votes.Update(vote);
                var result = await _context.SaveChangesAsync() > 0;
                _logger?.LogInformation("Vote update result: {Result}", result);
                return result;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error updating vote {VoteId}", vote.VoteId);
                throw;
            }
        }

        public async Task<bool> CloseVoteAsync(Guid id)
        {
            _logger?.LogInformation("Closing vote: {VoteId}", id);
            
            try
            {
                var vote = await _context.Votes.FindAsync(id);
                if (vote == null)
                {
                    _logger?.LogWarning("Vote not found for closing: {VoteId}", id);
                    return false;
                }
                
                vote.IsClosed = true;
                vote.EndDate = DateTime.Now;
                
                _context.Votes.Update(vote);
                var result = await _context.SaveChangesAsync() > 0;
                _logger?.LogInformation("Vote close result: {Result}", result);
                return result;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error closing vote {VoteId}", id);
                throw;
            }
        }
    }
} 